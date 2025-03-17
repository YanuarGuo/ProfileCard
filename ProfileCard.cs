using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Linq;
using FizzWare.NBuilder.Dates;
using Npgsql;
using NpgsqlTypes;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using static ModWinsCard;

namespace ProfileCard
{
    public partial class ProfileCard : Form
    {
        private NpgsqlConnection? conn;
        private Bitmap? originalImage;
        private string? loadedFilePath;
        public int retCode,
            hContext,
            hCard,
            Protocol;
        public bool connActive = false;
        public bool autoDet;
        public byte[] SendBuff = new byte[263];
        public byte[] RecvBuff = new byte[263];
        public int SendLen,
            RecvLen,
            nBytesRet,
            reqType,
            Aprotocol,
            dwProtocol,
            cbPciLength;
        public ModWinsCard.SCARD_IO_REQUEST pioSendRequest;
        private static readonly int[] SECTOR_TRAILERS =
        {
            3,
            7,
            11,
            15,
            19,
            23,
            27,
            31,
            35,
            39,
            43,
            47,
            51,
            55,
            59,
            63,
            67,
            71,
            75,
            79,
            83,
            87,
            91,
            95,
            99,
            103,
            107,
            111,
            115,
            119,
            123,
            127,
            143,
            159,
            175,
            191,
            207,
            223,
            239,
            255,
        };
        private string selectedReader = string.Empty;
        private bool isCardPresent = false;
        private CancellationTokenSource? cts;
        private bool isMonitoring = false;

        public ProfileCard()
        {
            InitializeComponent();
            DatabaseConnection();
        }

        private void ProfileCard_Load(object sender, EventArgs e)
        {
            InitMenu();
            InitCard();
            // !! Uncomment a line below if you want to start card monitoring automatically !!
            //StartCardMonitoringAsync();
            DtBirth.Format = DateTimePickerFormat.Custom;
            DtBirth.CustomFormat = "dd-MM-yyyy";
            System.Windows.Forms.Timer cardTimer;
            cardTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            cardTimer.Tick += CheckCardStatus;
        }

        private void InitMenu()
        {
            connActive = false;
            cbReader.Items.Clear();
            cbReader.Text = "";
            mMsg.Items.Clear();
            BtnConnect.Enabled = false;
            BtnResetReader.Enabled = false;
            BtnGetUID.Enabled = false;
        }

        private void ProfilePict_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Image Files (*.jpg; *.jpeg)|*.jpg;*.jpeg",
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                loadedFilePath = ofd.FileName;
                originalImage = new Bitmap(loadedFilePath);

                ProfilePict.SizeMode = PictureBoxSizeMode.StretchImage;
                ProfilePict.Image = originalImage;

                UpdateLabelVisibility();
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            TxtID.Text = "";
            TxtAddress.Text = "";
            DtBirth.Value = DateTime.Today;
            CbGender.SelectedIndex = -1;
            TxtName.Text = "";
            TxtNumber.Text = "";
            ProfilePict.Image = null;
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Please make sure your information is correct!",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    string encodedData = EncodeProfileData();
                    if (string.IsNullOrEmpty(encodedData))
                    {
                        MessageBox.Show(
                            "Error: Data encoding failed!",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }

                    List<byte[]> splitDataList = SplitData();
                    if (splitDataList == null || splitDataList.Count == 0)
                    {
                        MessageBox.Show(
                            "Error: Data split failed!",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }

                    WriteBlock(splitDataList);

                    MessageBox.Show(
                        "Uploaded successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"An error occurred: {ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            if (connActive)
            {
                retCode = ModWinsCard.SCardDisconnect(hCard, ModWinsCard.SCARD_UNPOWER_CARD);
            }

            retCode = ModWinsCard.SCardConnect(
                hContext,
                cbReader.Text,
                ModWinsCard.SCARD_SHARE_SHARED,
                1 | 2,
                ref hCard,
                ref Protocol
            );

            if (retCode == ModWinsCard.SCARD_S_SUCCESS)
            {
                DisplayOutput(0, 0, "Successful connection to " + cbReader.Text);
            }
            else
            {
                DisplayOutput(
                    0,
                    0,
                    "The smart card has been removed, so that further communication is not possible."
                );
            }

            connActive = true;
        }

        private void BtnInitialize_Click(object sender, EventArgs e)
        {
            string ReaderList = "" + Convert.ToChar(0);
            int indx;
            int pcchReaders = 0;
            string rName = "";
            hContext = 0;
            retCode = ModWinsCard.SCardEstablishContext(
                ModWinsCard.SCARD_SCOPE_USER,
                0,
                0,
                ref hContext
            );

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                DisplayOutput(1, retCode, "");
                return;
            }

            retCode = ModWinsCard.SCardListReaders(this.hContext, null, null, ref pcchReaders);

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                DisplayOutput(1, retCode, "");
                return;
            }

            EnableButtons();

            byte[] ReadersList = new byte[pcchReaders];

            retCode = ModWinsCard.SCardListReaders(
                this.hContext,
                null,
                ReadersList,
                ref pcchReaders
            );

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                mMsg.Items.Add("SCardListReaders Error: " + ModWinsCard.GetScardErrMsg(retCode));
                mMsg.SelectedIndex = mMsg.Items.Count - 1;
                return;
            }
            else
            {
                DisplayOutput(0, 0, " ");
            }

            rName = "";
            indx = 0;

            while (ReadersList[indx] != 0)
            {
                while (ReadersList[indx] != 0)
                {
                    rName = rName + (char)ReadersList[indx];
                    indx = indx + 1;
                }

                cbReader.Items.Add(rName);
                rName = "";
                indx = indx + 1;
            }

            if (cbReader.Items.Count > 0)
            {
                cbReader.SelectedIndex = 0;
            }

            indx = 1;

            for (indx = 1; indx <= cbReader.Items.Count - 1; indx++)
            {
                cbReader.SelectedIndex = indx;

                if (cbReader.Text == "ACS ACR128U PICC Interface 0")
                {
                    cbReader.SelectedIndex = 1;
                    return;
                }
            }
            return;
        }

        private void BtnGetUID_Click(object sender, EventArgs e)
        {
            ClearBuffers();

            SendBuff[0] = 0xFF;
            SendBuff[1] = 0xCA;
            SendBuff[2] = 0x00;
            SendBuff[3] = 0x00;
            SendBuff[4] = 0x00;

            SendLen = 5;
            RecvLen = 10;

            retCode = SendAPDUandDisplay(2);

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                return;
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            mMsg.Items.Clear();
        }

        private void BtnResetReader_Click(object sender, EventArgs e)
        {
            if (connActive)
            {
                retCode = ModWinsCard.SCardDisconnect(hCard, ModWinsCard.SCARD_UNPOWER_CARD);
            }

            retCode = ModWinsCard.SCardReleaseContext(hCard);

            InitMenu();
        }

        private void BtnRead_Click(object sender, EventArgs e)
        {
            int[] sectorSizes =
            {
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                16,
                16,
                16,
                16,
                16,
                16,
                16,
                16,
            };

            int totalSectors = sectorSizes.Length;
            int blockIndex = 0;
            int bytesPerBlock = 16;
            List<byte> fullData = new List<byte>();
            DataTable dt = new DataTable();
            dt.Columns.Add("Sektor", typeof(int));
            dt.Columns.Add("Blok", typeof(int));
            dt.Columns.Add("Data", typeof(string));

            try
            {
                for (int sector = 0; sector < totalSectors; sector++)
                {
                    int blocksInSector = sectorSizes[sector];
                    int trailerBlock = blockIndex + blocksInSector - 1;

                    byte authKey = 0x60;
                    byte keySource = 0x20;
                    if (!Authenticate(trailerBlock))
                    {
                        Debug.WriteLine($"Autentikasi gagal untuk sektor {sector}");
                        return;
                    }
                    ClearBuffers();
                    SendBuff[0] = 0xFF;
                    SendBuff[1] = 0x86;
                    SendBuff[2] = 0x00;
                    SendBuff[3] = 0x00;
                    SendBuff[4] = 0x05;
                    SendBuff[5] = 0x01;
                    SendBuff[6] = 0x00;
                    SendBuff[7] = (byte)trailerBlock;
                    SendBuff[8] = authKey;
                    SendBuff[9] = keySource;

                    SendLen = 10;
                    RecvLen = 2;

                    if (SendAPDUandDisplay(0) != ModWinsCard.SCARD_S_SUCCESS)
                    {
                        MessageBox.Show(
                            $"Autentikasi gagal di sektor {sector}!",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }

                    for (int i = 0; i < blocksInSector; i++)
                    {
                        int currentBlock = blockIndex + i;
                        byte[] readData = new byte[bytesPerBlock];

                        ClearBuffers();
                        SendBuff[0] = 0xFF;
                        SendBuff[1] = 0xB0;
                        SendBuff[2] = 0x00;
                        SendBuff[3] = (byte)currentBlock;
                        SendBuff[4] = (byte)bytesPerBlock;

                        SendLen = 5;
                        RecvLen = bytesPerBlock + 2;

                        if (SendAPDUandDisplay(2) != ModWinsCard.SCARD_S_SUCCESS)
                        {
                            MessageBox.Show(
                                $"Gagal membaca blok {currentBlock}!",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            return;
                        }

                        Array.Copy(RecvBuff, 0, readData, 0, bytesPerBlock);
                        fullData.AddRange(readData);

                        string blockData = BitConverter.ToString(readData).Replace("-", " ");
                        dt.Rows.Add(sector, currentBlock, blockData);
                    }

                    blockIndex += blocksInSector;
                }

                dReadAll.DataSource = dt;
                dReadAll.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Terjadi kesalahan: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void BtnReadProfile_Click(object sender, EventArgs e)
        {
            List<byte> rawData = new List<byte>();
            int[] sectorSizes =
            {
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                16,
                16,
                16,
                16,
                16,
                16,
                16,
                16,
            };
            int startSector = 1;
            int startBlock = 4;
            int blockIndex = 0;

            try
            {
                for (int sector = 0; sector < startSector; sector++)
                {
                    blockIndex += sectorSizes[sector];
                }

                for (int sector = startSector; sector < sectorSizes.Length; sector++)
                {
                    int blocksInSector = sectorSizes[sector];

                    int trailerBlock = blockIndex + blocksInSector - 1;
                    if (!Authenticate(trailerBlock))
                    {
                        MessageBox.Show(
                            $"Autentikasi gagal untuk sektor {sector}!",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }

                    for (
                        int i = (sector == startSector ? startBlock - blockIndex : 0);
                        i < blocksInSector - 1;
                        i++
                    )
                    {
                        int currentBlock = blockIndex + i;

                        if (SECTOR_TRAILERS.Contains(currentBlock))
                        {
                            continue;
                        }

                        byte[]? blockData = ReadBlock(currentBlock);
                        if (blockData != null)
                        {
                            rawData.AddRange(blockData);
                        }
                        else
                        {
                            MessageBox.Show(
                                $"Gagal membaca blok {currentBlock}!",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            return;
                        }
                    }

                    blockIndex += blocksInSector;
                }

                ParseProfileData([.. rawData], "Read");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Terjadi kesalahan: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void BtnResetDataBlock_Click(object sender, EventArgs e)
        {
            int startBlock = 4;
            byte[] emptyBlock = new byte[16];

            try
            {
                int blockIndex = startBlock;
                for (int sector = 1; sector < 40; sector++)
                {
                    int blocksInSector = (sector < 32) ? 4 : 16;
                    int trailerBlock = blockIndex + blocksInSector - 1;

                    if (!Authenticate(trailerBlock))
                    {
                        MessageBox.Show(
                            $"Autentikasi gagal untuk sektor {sector}!",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }

                    for (int i = 0; i < blocksInSector - 1; i++)
                    {
                        int currentBlock = blockIndex + i;

                        if (SECTOR_TRAILERS.Contains(currentBlock))
                        {
                            continue;
                        }

                        if (!ResetDataBlock(currentBlock, emptyBlock))
                        {
                            MessageBox.Show(
                                $"Gagal mereset blok {currentBlock}!",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            return;
                        }
                    }

                    blockIndex += blocksInSector;
                }

                MessageBox.Show(
                    "Reset Data Block berhasil!",
                    "Sukses",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Terjadi kesalahan: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void BtnStartTimer_Click(object sender, EventArgs e)
        {
            if (cbReader.SelectedItem != null)
            {
                MessageBox.Show(
                    "Timer monitoring kartu dimulai!",
                    "Info",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                selectedReader = cbReader.SelectedItem?.ToString() ?? string.Empty;
                CardTimer.Start();
                BtnStartTimer.Enabled = false;
                BtnStopTimer.Enabled = true;
                BtnStartThreading.Enabled = false;
                BtnStopThreading.Enabled = false;
            }
            else
            {
                MessageBox.Show(
                    "Pilih reader terlebih dahulu!",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void BtnStopTimer_Click(object sender, EventArgs e)
        {
            CardTimer.Stop();
            BtnStartTimer.Enabled = true;
            BtnStopTimer.Enabled = false;
            BtnStartThreading.Enabled = true;
            BtnStopThreading.Enabled = false;
            MessageBox.Show(
                "Timer monitoring kartu dihentikan!",
                "Info",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void BtnStartThreading_Click(object sender, EventArgs e)
        {
            if (cts == null || cts.IsCancellationRequested)
            {
                MessageBox.Show(
                    "Thread monitoring kartu dimulai!",
                    "Info",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                StartCardMonitoringAsync();
                BtnStartThreading.Enabled = false;
                BtnStopThreading.Enabled = true;
                BtnStartTimer.Enabled = false;
                BtnStopTimer.Enabled = false;
            }
        }

        private void BtnStopThreading_Click(object sender, EventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
                BtnStartThreading.Enabled = true;
                BtnStopThreading.Enabled = false;
                BtnStartTimer.Enabled = true;
                BtnStopTimer.Enabled = false;

                MessageBox.Show(
                    "Thread monitoring kartu dihentikan!",
                    "Info",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void CardTimer_Tick(object sender, EventArgs e)
        {
            CheckCardStatus(sender, e);
        }

        private void UpdateLabelVisibility()
        {
            if (ProfilePict.Image != null)
            {
                label6.Visible = false;
            }
            else
            {
                label6.Visible = true;
            }
        }

        private string EncodeProfileData()
        {
            string id = TxtID.Text.Trim();
            string name = TxtName.Text.Trim();
            DateTime dob = DtBirth.Value.Date;
            string gender = CbGender.Text.Trim();
            string address = TxtAddress.Text.Trim();
            string phone = TxtNumber.Text.Trim();

            try
            {
                if (conn == null || conn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        "Koneksi database belum dibuka!",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return string.Empty;
                }

                using NpgsqlCommand comm = new(
                    "UPDATE public.\"MsEmployees\" "
                        + "SET \"name\" = @name, \"birth_date\" = @dob, \"gender\" = @gender, "
                        + "\"address\" = @address, \"contact_number\" = @phone "
                        + "WHERE \"id\" = @id AND \"is_active\" = true",
                    conn
                );

                comm.Parameters.AddWithValue("@id", id);
                comm.Parameters.AddWithValue("@name", name);
                comm.Parameters.AddWithValue("@dob", dob);
                comm.Parameters.AddWithValue("@gender", gender);
                comm.Parameters.AddWithValue("@address", address);
                comm.Parameters.AddWithValue("@phone", phone);

                int rowsAffected = comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Terjadi kesalahan: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            byte[]? compressedImage = null;
            if (ProfilePict.Image != null)
            {
                compressedImage = CompressImage(ProfilePict.Image, 40, 60, 95);
            }
            string hexID = ConvertStringToHexWithHeader("*", id);
            string hexName = ConvertStringToHexWithHeader("*", name);
            string hexDOB = ConvertStringToHexWithHeader("*", dob.ToString("dd-MM-yyyy"));
            string hexGender = ConvertStringToHexWithHeader("*", gender);
            string hexAddress = ConvertStringToHexWithHeader("*", address);
            string hexPhone = ConvertStringToHexWithHeader("*", phone);

            string hexTextData = $"{hexID}{hexName}{hexDOB}{hexGender}{hexAddress}{hexPhone}";

            byte[] textBytes = Enumerable
                .Range(0, hexTextData.Length / 2)
                .Select(i => Convert.ToByte(hexTextData.Substring(i * 2, 2), 16))
                .ToArray();

            byte[] separator = new byte[4];
            byte[] fixedTextBytes = textBytes.Concat(separator).ToArray();

            byte[] imageBytes = compressedImage ?? Array.Empty<byte>();
            byte[] finalData = fixedTextBytes.Concat(imageBytes).ToArray();

            return BitConverter.ToString(finalData).Replace("-", " ");
        }

        private void ClearBuffers()
        {
            long indx;

            if (SendBuff.Length < 263 || RecvBuff.Length < 263)
            {
                throw new InvalidOperationException("Buffer sizes are incorrect.");
            }

            for (indx = 0; indx <= 262; indx++)
            {
                RecvBuff[indx] = 0;
                SendBuff[indx] = 0;
            }
        }

        private void EnableButtons()
        {
            BtnConnect.Enabled = true;
            BtnResetReader.Enabled = true;
            BtnClear.Enabled = true;
            BtnGetUID.Enabled = true;
        }

        private void InitCard()
        {
            string ReaderList = "" + Convert.ToChar(0);
            int indx;
            int pcchReaders = 0;
            string rName = "";
            hContext = 0;
            retCode = ModWinsCard.SCardEstablishContext(
                ModWinsCard.SCARD_SCOPE_USER,
                0,
                0,
                ref hContext
            );

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                DisplayOutput(1, retCode, "");
                return;
            }

            retCode = ModWinsCard.SCardListReaders(this.hContext, null, null, ref pcchReaders);

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                DisplayOutput(1, retCode, "");
                return;
            }

            EnableButtons();

            byte[] ReadersList = new byte[pcchReaders];

            retCode = ModWinsCard.SCardListReaders(
                this.hContext,
                null,
                ReadersList,
                ref pcchReaders
            );

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                mMsg.Items.Add("SCardListReaders Error: " + ModWinsCard.GetScardErrMsg(retCode));
                mMsg.SelectedIndex = mMsg.Items.Count - 1;
                return;
            }
            else
            {
                DisplayOutput(0, 0, " ");
            }

            rName = "";
            indx = 0;

            while (ReadersList[indx] != 0)
            {
                while (ReadersList[indx] != 0)
                {
                    rName = rName + (char)ReadersList[indx];
                    indx = indx + 1;
                }

                cbReader.Items.Add(rName);
                rName = "";
                indx = indx + 1;
            }

            if (cbReader.Items.Count > 0)
            {
                cbReader.SelectedIndex = 0;
            }

            indx = 1;

            for (indx = 1; indx <= cbReader.Items.Count - 1; indx++)
            {
                cbReader.SelectedIndex = indx;

                if (cbReader.Text == "ACS ACR128U PICC Interface 0")
                {
                    cbReader.SelectedIndex = 1;
                    return;
                }
            }
            return;
        }

        private List<byte[]> SplitData()
        {
            string hexData = EncodeProfileData();
            hexData = hexData.Replace(" ", "");

            if (hexData.Length % 2 != 0)
            {
                throw new InvalidOperationException("Hex string has an odd length.");
            }

            byte[] byteArray = Enumerable
                .Range(0, hexData.Length / 2)
                .Select(i => Convert.ToByte(hexData.Substring(i * 2, 2), 16))
                .ToArray();

            int chunkSize = 16;
            List<byte[]> splitDataList = new List<byte[]>();

            for (int i = 0; i < byteArray.Length; i += chunkSize)
            {
                byte[] splitDataReturn = byteArray.Skip(i).Take(chunkSize).ToArray();

                if (splitDataReturn.Length < chunkSize)
                {
                    Array.Resize(ref splitDataReturn, chunkSize);
                }

                splitDataList.Add(splitDataReturn);
            }

            return splitDataList;
        }

        private static string ConvertStringToHexWithHeader(string separator, string input)
        {
            string hexSeparator = BitConverter
                .ToString(Encoding.ASCII.GetBytes(separator))
                .Replace("-", "");
            string hexData = BitConverter.ToString(Encoding.ASCII.GetBytes(input)).Replace("-", "");
            return hexSeparator + hexData;
        }

        private static byte[] CompressImage(
            System.Drawing.Image image,
            int width,
            int height,
            long quality
        )
        {
            using Bitmap resizedImage = new Bitmap(image, new Size(width, height));
            using MemoryStream ms = new MemoryStream();
            ImageCodecInfo? jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            if (jpgEncoder == null)
            {
                throw new InvalidOperationException("JPEG encoder not found.");
            }
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(
                System.Drawing.Imaging.Encoder.Quality,
                quality
            );
            resizedImage.Save(ms, jpgEncoder, encoderParameters);
            return ms.ToArray();
        }

        private static ImageCodecInfo? GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private void DisplayOutput(int errType, int retVal, string PrintText)
        {
            switch (errType)
            {
                case 0:
                    break;
                case 1:
                    PrintText = ModWinsCard.GetScardErrMsg(retVal);
                    break;
                case 2:
                    PrintText = "<" + PrintText;
                    break;
                case 3:
                    PrintText = ">" + PrintText;
                    break;
            }
            mMsg.Items.Add(PrintText);
            mMsg.ForeColor = Color.Black;
            mMsg.Focus();
        }

        private int SendAPDUandDisplay(int reqType)
        {
            int indx;
            string tmpStr;

            pioSendRequest.dwProtocol = Aprotocol;
            pioSendRequest.cbPciLength = 8;

            tmpStr = "";
            for (indx = 0; indx <= SendLen - 1; indx++)
            {
                tmpStr = tmpStr + " " + string.Format("{0:X2}", SendBuff[indx]);
            }

            DisplayOutput(2, 0, tmpStr);
            retCode = ModWinsCard.SCardTransmit(
                hCard,
                ref pioSendRequest,
                ref SendBuff[0],
                SendLen,
                ref pioSendRequest,
                ref RecvBuff[0],
                ref RecvLen
            );

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                DisplayOutput(1, retCode, "");
                return retCode;
            }
            else
            {
                tmpStr = "";
                switch (reqType)
                {
                    case 0:
                        for (indx = (RecvLen - 2); indx <= (RecvLen - 1); indx++)
                        {
                            tmpStr = tmpStr + " " + string.Format("{0:X2}", RecvBuff[indx]);
                        }

                        if ((tmpStr).Trim() != "90 00")
                        {
                            DisplayOutput(4, 0, "Return bytes are not acceptable.");
                        }
                        break;

                    case 1:
                        for (indx = (RecvLen - 2); indx <= (RecvLen - 1); indx++)
                        {
                            tmpStr = tmpStr + string.Format("{0:X2}", RecvBuff[indx]);
                        }

                        if (tmpStr.Trim() != "90 00")
                        {
                            tmpStr = tmpStr + " " + string.Format("{0:X2}", RecvBuff[indx]);
                        }
                        else
                        {
                            tmpStr = "ATR : ";
                            for (indx = 0; indx <= (RecvLen - 3); indx++)
                            {
                                tmpStr = tmpStr + " " + string.Format("{0:X2}", RecvBuff[indx]);
                            }
                        }
                        break;

                    case 2:
                        for (indx = 0; indx <= (RecvLen - 1); indx++)
                        {
                            tmpStr = tmpStr + " " + string.Format("{0:X2}", RecvBuff[indx]);
                        }
                        break;
                }

                DisplayOutput(3, 0, tmpStr.Trim());
            }

            return retCode;
        }

        private bool Authenticate(int block)
        {
            SendBuff[0] = 0xFF;
            SendBuff[1] = 0x86;
            SendBuff[2] = 0x00;
            SendBuff[3] = 0x00;
            SendBuff[4] = 0x05;
            SendBuff[5] = 0x01;
            SendBuff[6] = 0x00;
            SendBuff[7] = (byte)block;
            SendBuff[8] = 0x60;
            SendBuff[9] = 0x00;

            SendLen = 10;
            RecvLen = 2;

            retCode = SendAPDUandDisplay(2);

            if (retCode == ModWinsCard.SCARD_S_SUCCESS)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void WriteBlock(List<byte[]> splitDataList)
        {
            int block = 4;
            int dataIndex = 0;

            foreach (byte[] splitData in splitDataList)
            {
                if (SECTOR_TRAILERS.Contains(block))
                {
                    block++;
                }

                if (!Authenticate(block))
                {
                    MessageBox.Show(
                        $"Authentication failed at block {block}. Writing process stopped!",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                if (splitData.Length != 16)
                {
                    Debug.WriteLine("Data harus tepat 16 byte untuk satu dataBlock!");
                    return;
                }

                SendBuff[0] = 0xFF;
                SendBuff[1] = 0xD6;
                SendBuff[2] = 0x00;
                SendBuff[3] = (byte)block;
                SendBuff[4] = 0x10;

                Array.Copy(splitData, 0, SendBuff, 5, splitData.Length);

                SendLen = SendBuff[4] + 5;
                RecvLen = 2;

                retCode = SendAPDUandDisplay(2);

                if (retCode == ModWinsCard.SCARD_S_SUCCESS)
                {
                    Debug.WriteLine($"Berhasil menulis ke block {block}");
                }
                else
                {
                    Debug.WriteLine($"Gagal menulis ke block {block}");
                    MessageBox.Show(
                        $"Write failed at block {block}. Process stopped!",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                block++;
                dataIndex++;
            }
        }

        private byte[]? ReadBlock(int block)
        {
            if (SECTOR_TRAILERS.Contains(block))
            {
                return null;
            }

            ClearBuffers();
            SendBuff[0] = 0xFF;
            SendBuff[1] = 0xB0;
            SendBuff[2] = 0x00;
            SendBuff[3] = (byte)block;
            SendBuff[4] = 0x10;

            SendLen = 5;
            RecvLen = 18;

            if (SendAPDUandDisplay(2) == ModWinsCard.SCARD_S_SUCCESS)
            {
                return RecvBuff.Take(16).ToArray();
            }
            return null;
        }

        // !! Function below is used to name the image file with unique name !!
        static string GetUniqueFilePath(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath) ?? string.Empty;
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);

            int counter = 1;
            string newFilePath = filePath;

            while (File.Exists(newFilePath))
            {
                newFilePath = Path.Combine(directory, $"{fileNameWithoutExt}_{counter}{extension}");
                counter++;
            }

            return newFilePath;
        }

        private void ParseProfileData(byte[] rawData, string caller)
        {
            if (caller == "Read")
            {
                try
                {
                    int imageStartIndex = FindSeparatorIndex(rawData);
                    byte[] textData = rawData.Take(imageStartIndex).ToArray();
                    string profileText = Encoding.UTF8.GetString(textData);

                    string[] parts = profileText.Split('*');
                    if (parts.Length > 0 && string.IsNullOrWhiteSpace(parts[0]))
                    {
                        parts = [.. parts.Skip(1)];
                    }

                    bool isIdFound = false;

                    try
                    {
                        if (conn == null || conn.State != ConnectionState.Open)
                        {
                            MessageBox.Show(
                                "Koneksi database belum dibuka!",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            return;
                        }

                        using NpgsqlCommand comm = new(
                            "SELECT * FROM public.\"MsEmployees\" WHERE \"id\" = @id AND \"is_active\" = true",
                            conn
                        );
                        if (!int.TryParse(parts[0], out int employeeId))
                        {
                            MessageBox.Show(
                                "ID tidak valid!",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            ProfilePict.Image = null;
                            return;
                        }

                        comm.Parameters.AddWithValue("@id", parts[0]);

                        using (NpgsqlDataReader reader = comm.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                TxtID.Text = reader["id"].ToString();
                                TxtName.Text = reader["name"].ToString();

                                if (reader["birth_date"] != DBNull.Value)
                                {
                                    DtBirth.Value = (DateTime)reader["birth_date"];
                                }
                                else
                                {
                                    DtBirth.Value = DateTime.Today;
                                }
                                CbGender.Text = reader["gender"].ToString();
                                TxtAddress.Text = reader["address"].ToString();
                                TxtNumber.Text = reader["contact_number"].ToString();
                                isIdFound = true;
                            }
                            else
                            {
                                TxtID.Text = "";
                                TxtAddress.Text = "";
                                DtBirth.Value = DateTime.Today;
                                CbGender.SelectedIndex = -1;
                                TxtName.Text = "";
                                TxtNumber.Text = "";
                                ProfilePict.Image = null;
                                MessageBox.Show(
                                    "ID tidak ditemukan dalam database!",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error
                                );
                            }

                            reader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Terjadi kesalahan: {ex.Message}",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }

                    if (isIdFound)
                    {
                        int realImageStartIndex = FindImageStartIndex(rawData, imageStartIndex);
                        if (realImageStartIndex == -1)
                        {
                            return;
                        }

                        byte[] imageBytes = rawData.Skip(realImageStartIndex).ToArray();

                        using MemoryStream ms = new(imageBytes);
                        try
                        {
                            using System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                            ProfilePict.Image = (System.Drawing.Image)img.Clone();
                            ProfilePict.SizeMode = PictureBoxSizeMode.StretchImage;
                            UpdateLabelVisibility();

                            // !! Uncomment lines below if you want to save image !!
                            string userInputName = TxtName.Text;
                            string basePath = $"D:\\{userInputName}.jpg";
                            string outputPath = GetUniqueFilePath(basePath);
                            ProfilePict.Image.Save(
                                outputPath,
                                System.Drawing.Imaging.ImageFormat.Jpeg
                            );
                            Debug.WriteLine($"Gambar berhasil disimpan di {outputPath}");
                        }
                        catch (Exception imgEx)
                        {
                            Debug.WriteLine($"Error saat membaca gambar: {imgEx.Message}");
                        }
                    }
                    else
                    {
                        ProfilePict.Image = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Kesalahan saat membaca profil: {ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            else if (caller == "Tap")
            {
                try
                {
                    int imageStartIndex = FindSeparatorIndex(rawData);
                    byte[] textData = rawData.Take(imageStartIndex).ToArray();
                    string profileText = Encoding.UTF8.GetString(textData);

                    string[] parts = profileText.Split('*');
                    if (parts.Length > 0 && string.IsNullOrWhiteSpace(parts[0]))
                    {
                        parts = [.. parts.Skip(1)];
                    }

                    bool isIdFound = false;

                    try
                    {
                        if (conn == null || conn.State != ConnectionState.Open)
                        {
                            MessageBox.Show(
                                "Koneksi database belum dibuka!",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            return;
                        }

                        using NpgsqlCommand comm = new(
                            "SELECT * FROM public.\"MsEmployees\" WHERE \"id\" = @id AND \"is_active\" = true",
                            conn
                        );
                        if (!int.TryParse(parts[0], out int employeeId))
                        {
                            MessageBox.Show(
                                "ID tidak valid!",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            ProfilePict.Image = null;
                            return;
                        }

                        comm.Parameters.AddWithValue("@id", parts[0]);

                        using (NpgsqlDataReader reader = comm.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Debug.WriteLine("ID cocok, menampilkan data...");
                                TxtID.Text = reader["id"].ToString();
                                TxtName.Text = reader["name"].ToString();

                                if (reader["birth_date"] != DBNull.Value)
                                {
                                    DtBirth.Value = (DateTime)reader["birth_date"];
                                }
                                else
                                {
                                    DtBirth.Value = DateTime.Today;
                                }
                                CbGender.Text = reader["gender"].ToString();
                                TxtAddress.Text = reader["address"].ToString();
                                TxtNumber.Text = reader["contact_number"].ToString();
                                isIdFound = true;
                            }
                            else
                            {
                                TxtID.Text = "";
                                TxtAddress.Text = "";
                                DtBirth.Value = DateTime.Today;
                                CbGender.SelectedIndex = -1;
                                TxtName.Text = "";
                                TxtNumber.Text = "";
                                ProfilePict.Image = null;
                                MessageBox.Show(
                                    "ID tidak ditemukan dalam database!",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error
                                );
                            }
                            reader.Close();
                        }

                        Task.Run(() => RecordTap(parts[0]));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Terjadi kesalahan: {ex.Message}",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }

                    if (isIdFound)
                    {
                        int realImageStartIndex = FindImageStartIndex(rawData, imageStartIndex);
                        if (realImageStartIndex == -1)
                        {
                            return;
                        }

                        byte[] imageBytes = rawData.Skip(realImageStartIndex).ToArray();

                        using MemoryStream ms = new(imageBytes);
                        try
                        {
                            using System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                            ProfilePict.Image = (System.Drawing.Image)img.Clone();
                            ProfilePict.SizeMode = PictureBoxSizeMode.StretchImage;
                            UpdateLabelVisibility();
                        }
                        catch (Exception imgEx)
                        {
                            Debug.WriteLine($"Error saat membaca gambar: {imgEx.Message}");
                        }
                        MessageBox.Show(
                            "Absensi berhasil!",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    else
                    {
                        ProfilePict.Image = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Kesalahan saat membaca profil: {ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private static int FindImageStartIndex(byte[] data, int startSearchIndex)
        {
            for (int i = startSearchIndex; i < data.Length - 1; i++)
            {
                if (data[i] == 0xFF && data[i + 1] == 0xD8)
                {
                    return i;
                }
            }
            return -1;
        }

        private static int FindSeparatorIndex(byte[] data)
        {
            int separatorSize = 4;
            if (data.Length < separatorSize)
                return data.Length;

            for (int i = 0; i <= data.Length - separatorSize; i++)
            {
                bool isSeparator = true;

                for (int j = 0; j < separatorSize; j++)
                {
                    if (data[i + j] != 0x00)
                    {
                        isSeparator = false;
                        break;
                    }
                }

                if (isSeparator)
                    return i;
            }

            return data.Length;
        }

        private bool ResetDataBlock(int block, byte[] data)
        {
            if (data.Length != 16)
            {
                throw new ArgumentException("Data harus 16 byte.");
            }

            ClearBuffers();
            SendBuff[0] = 0xFF;
            SendBuff[1] = 0xD6;
            SendBuff[2] = 0x00;
            SendBuff[3] = (byte)block;
            SendBuff[4] = 0x10;
            Array.Copy(data, 0, SendBuff, 5, 16);

            SendLen = 21;
            RecvLen = 2;

            return SendAPDUandDisplay(2) == ModWinsCard.SCARD_S_SUCCESS;
        }

        private void TapReadProfile()
        {
            List<byte> rawData = new List<byte>();
            int[] sectorSizes =
            {
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                16,
                16,
                16,
                16,
                16,
                16,
                16,
                16,
            };
            int startSector = 1;
            int startBlock = 4;
            int blockIndex = 0;

            if (connActive)
            {
                retCode = ModWinsCard.SCardDisconnect(hCard, ModWinsCard.SCARD_UNPOWER_CARD);
            }

            retCode = ModWinsCard.SCardConnect(
                hContext,
                cbReader.Text,
                ModWinsCard.SCARD_SHARE_SHARED,
                1 | 2,
                ref hCard,
                ref Protocol
            );

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                return;
            }

            connActive = true;

            try
            {
                for (int sector = 0; sector < startSector; sector++)
                {
                    blockIndex += sectorSizes[sector];
                }

                for (int sector = startSector; sector < sectorSizes.Length; sector++)
                {
                    int blocksInSector = sectorSizes[sector];

                    int trailerBlock = blockIndex + blocksInSector - 1;
                    if (!Authenticate(trailerBlock))
                    {
                        return;
                    }

                    for (
                        int i = (sector == startSector ? startBlock - blockIndex : 0);
                        i < blocksInSector - 1;
                        i++
                    )
                    {
                        int currentBlock = blockIndex + i;

                        if (SECTOR_TRAILERS.Contains(currentBlock))
                        {
                            continue;
                        }

                        byte[]? blockData = ReadBlock(currentBlock);
                        if (blockData != null)
                        {
                            rawData.AddRange(blockData);
                        }
                        else
                        {
                            MessageBox.Show(
                                $"Gagal membaca blok {currentBlock}!",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            return;
                        }
                    }

                    blockIndex += blocksInSector;
                }

                ParseProfileData([.. rawData], "Tap");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Terjadi kesalahan: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void CheckCardStatus(object? sender, EventArgs e)
        {
            ModWinsCard.SCARD_READERSTATE readerState = new()
            {
                RdrName = selectedReader,
                RdrCurrState = ModWinsCard.SCARD_STATE_EMPTY,
            };

            retCode = ModWinsCard.SCardGetStatusChange(hContext, 0, ref readerState, 1);
            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                Console.WriteLine("[Error] SCardGetStatusChange failed.");
                return;
            }

            bool cardDetected = (readerState.RdrEventState & ModWinsCard.SCARD_STATE_PRESENT) != 0;

            if (cardDetected && !isCardPresent)
            {
                isCardPresent = true;
                TapReadProfile();
            }
            else if (!cardDetected)
            {
                isCardPresent = false;
            }

            readerState.RdrCurrState = readerState.RdrEventState;
        }

        private async void StartCardMonitoringAsync()
        {
            cts = new CancellationTokenSource();
            string readerName = cbReader.Text;

            await Task.Run(
                async () =>
                {
                    SCARD_READERSTATE readerState = new()
                    {
                        RdrName = readerName,
                        RdrCurrState = ModWinsCard.SCARD_STATE_EMPTY,
                    };

                    while (!cts.Token.IsCancellationRequested)
                    {
                        int status = ModWinsCard.SCardGetStatusChange(
                            hContext,
                            1000,
                            ref readerState,
                            1
                        );

                        if (status == ModWinsCard.SCARD_S_SUCCESS)
                        {
                            if (
                                (readerState.RdrEventState & ModWinsCard.SCARD_STATE_PRESENT)
                                    == ModWinsCard.SCARD_STATE_PRESENT
                                && (readerState.RdrCurrState & ModWinsCard.SCARD_STATE_PRESENT) == 0
                            )
                            {
                                readerState.RdrCurrState = ModWinsCard.SCARD_STATE_PRESENT;

                                if (InvokeRequired)
                                {
                                    BeginInvoke(new Action(() => TapReadProfile()));
                                }
                                else
                                {
                                    TapReadProfile();
                                }
                            }

                            if (
                                (readerState.RdrEventState & ModWinsCard.SCARD_STATE_EMPTY)
                                    == ModWinsCard.SCARD_STATE_EMPTY
                                && (readerState.RdrCurrState & ModWinsCard.SCARD_STATE_PRESENT)
                                    == ModWinsCard.SCARD_STATE_PRESENT
                            )
                            {
                                readerState.RdrCurrState = ModWinsCard.SCARD_STATE_EMPTY;
                            }
                        }

                        await Task.Delay(500);
                    }
                },
                cts.Token
            );
        }

        private void RecordTap(string employeeId)
        {
            try
            {
                if (conn == null || conn.State != ConnectionState.Open)
                {
                    MessageBox.Show(
                        "Koneksi database belum dibuka!",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                using NpgsqlCommand cmd = new(
                    "INSERT INTO public.\"EmployeeAttendance\" (id, employee_id, tapped_at) VALUES (gen_random_uuid(), @employee_id, NOW())",
                    conn
                );
                cmd.Parameters.AddWithValue("@employee_id", employeeId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Kesalahan saat mencatat tap: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void DatabaseConnection()
        {
            try
            {
                conn = new NpgsqlConnection(
                    "Server=localhost;Port=5432;User Id=postgres;Password=1sampai8;Database=postgres"
                );
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Database connection failed: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
