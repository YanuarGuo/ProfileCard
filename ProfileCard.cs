using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Linq;
using Npgsql;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static ModWinsCard;

namespace ProfileCard
{
    public partial class ProfileCard : Form
    {
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
        public ModWinsCard.SCARD_READERSTATE RdrState;
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

        public ProfileCard()
        {
            InitializeComponent();
            TestDatabaseConnection();
        }

        private void ProfileCard_Load(object sender, EventArgs e)
        {
            InitMenu();
            InitCard();
            // !! Uncomment a line below if you want to start card monitoring automatically !!
            StartCardMonitoringAsync();
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
            DisplayOutput(0, 0, "Program ready");
            bConnect.Enabled = false;
            bReset.Enabled = false;
            btnGetUID.Enabled = false;
        }

        private void ProfilePict_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter =
                    "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png",
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
            TxtBirthDate.Text = "";
            TxtGender.Text = "";
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

        private void bConnect_Click(object sender, EventArgs e)
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

        private void bInit_Click(object sender, EventArgs e)
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

        private void btnGetUID_Click(object sender, EventArgs e)
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

        private void bClear_Click(object sender, EventArgs e)
        {
            mMsg.Items.Clear();
        }

        private void bReset_Click(object sender, EventArgs e)
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
            int startSector = 2;
            int startBlock = 8;
            int blockIndex = 4;

            try
            {
                for (int sector = 1; sector < startSector; sector++)
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

                        byte[] blockData = ReadBlock(currentBlock);
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

                ParseProfileData(rawData.ToArray());
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
            int startBlock = 8;
            byte[] emptyBlock = new byte[16];

            try
            {
                int blockIndex = startBlock;
                for (int sector = 2; sector < 40; sector++)
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
                selectedReader = cbReader.SelectedItem?.ToString() ?? string.Empty;
                cardTimer.Start();
                BtnStartTimer.Enabled = false;
                BtnStopTimer.Enabled = true;
                BtnStartThreading.Enabled = false;
                BtnStopThreading.Enabled = false;
                MessageBox.Show(
                    "Monitoring kartu dimulai!",
                    "Info",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
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
            cardTimer.Stop();
            BtnStartTimer.Enabled = true;
            BtnStopTimer.Enabled = false;
            BtnStartThreading.Enabled = true;
            BtnStopThreading.Enabled = false;
            MessageBox.Show(
                "Monitoring kartu dihentikan!",
                "Info",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void BtnStartThreading_Click(object sender, EventArgs e)
        {
            if (cts == null || cts.IsCancellationRequested)
            {
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
            }
        }

        private void cardTimer_Tick(object sender, EventArgs e)
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
            string dob = TxtBirthDate.Text.Trim();
            string gender = TxtGender.Text.Trim();
            string address = TxtAddress.Text.Trim();
            string phone = TxtNumber.Text.Trim();

            byte[]? compressedImage = null;
            if (ProfilePict.Image != null)
            {
                compressedImage = CompressImage(ProfilePict.Image, 40, 60, 95);
            }
            string hexID = ConvertStringToHexWithHeader("*", id);
            string hexName = ConvertStringToHexWithHeader("*", name);
            string hexDOB = ConvertStringToHexWithHeader("*", dob);
            string hexGender = ConvertStringToHexWithHeader("*", gender);
            string hexAddress = ConvertStringToHexWithHeader("*", address);
            string hexPhone = ConvertStringToHexWithHeader("*", phone);

            string hexTextData = $"{hexID}{hexName}{hexDOB}{hexGender}{hexAddress}{hexPhone}";

            byte[] textBytes = Enumerable
                .Range(0, hexTextData.Length / 2)
                .Select(i => Convert.ToByte(hexTextData.Substring(i * 2, 2), 16))
                .ToArray();

            int remainder = textBytes.Length % 16;
            int paddingSize = remainder == 0 ? 0 : 16 - remainder;

            byte[] separator = new byte[16];
            byte[] paddedTextBytes = textBytes
                .Concat(new byte[paddingSize])
                .Concat(separator)
                .ToArray();

            byte[] imageBytes = compressedImage ?? Array.Empty<byte>();
            byte[] finalData = paddedTextBytes.Concat(imageBytes).ToArray();

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
            bConnect.Enabled = true;
            bReset.Enabled = true;
            bClear.Enabled = true;
            btnGetUID.Enabled = true;
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

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable
                .Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
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
            int block = 8;
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

        private void ParseProfileData(byte[] rawData)
        {
            try
            {
                int imageStartIndex = FindSeparatorIndex(rawData);
                if (imageStartIndex == rawData.Length)
                {
                    Debug.WriteLine("Error: No image separator (16 bytes of 0x00) found!");
                    return;
                }

                byte[] textData = rawData.Take(imageStartIndex).ToArray();
                string profileText = Encoding.UTF8.GetString(textData);

                string[] parts = profileText.Split('*');
                if (parts.Length > 0 && string.IsNullOrWhiteSpace(parts[0]))
                {
                    parts = [.. parts.Skip(1)];
                }

                if (parts.Length >= 6)
                {
                    TxtID.Text = parts[0];
                    TxtName.Text = parts[1];
                    TxtBirthDate.Text = parts[2];
                    TxtGender.Text = parts[3];
                    TxtAddress.Text = parts[4];
                    TxtNumber.Text = parts[5];
                }
                else
                {
                    MessageBox.Show(
                        "Data profil tidak lengkap!",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                // IMAGE SECTION
                int realImageStartIndex = FindImageStartIndex(rawData, imageStartIndex);
                if (realImageStartIndex == -1)
                {
                    Debug.WriteLine("Error: No image header (FF D8) found after separator!");
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
                    //string userInputName = TxtName.Text;
                    //string basePath = $"D:\\{userInputName}.jpg";
                    //string outputPath = GetUniqueFilePath(basePath);
                    //ProfilePict.Image.Save(outputPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    //Debug.WriteLine($"Gambar berhasil disimpan di {outputPath}");
                }
                catch (Exception imgEx)
                {
                    Debug.WriteLine($"Error saat membaca gambar: {imgEx.Message}");
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
            for (int i = 0; i <= data.Length - 16; i++)
            {
                if (data.Skip(i).Take(16).All(b => b == 0x00))
                {
                    return i;
                }
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
            int startSector = 2;
            int startBlock = 8;
            int blockIndex = 4;

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
                for (int sector = 1; sector < startSector; sector++)
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

                ParseProfileData(rawData.ToArray());
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
            ModWinsCard.SCARD_READERSTATE readerState = new ModWinsCard.SCARD_READERSTATE();
            readerState.RdrName = selectedReader;
            readerState.RdrCurrState = ModWinsCard.SCARD_STATE_EMPTY;

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
                    SCARD_READERSTATE readerState = new SCARD_READERSTATE
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
                                    BeginInvoke(
                                        new Action(() =>
                                        {
                                            TapReadProfile();
                                            StopCardMonitoring();
                                        })
                                    );
                                }
                                else
                                {
                                    TapReadProfile();
                                    StopCardMonitoring();
                                }
                            }
                        }

                        await Task.Delay(500);
                    }
                },
                cts.Token
            );
        }

        private void StopCardMonitoring()
        {
            cts?.Cancel();
        }

        private void TestDatabaseConnection()
        {
            string connString =
                "Server=localhost;Port=5432;User Id=postgres;Password=1sampai8;Database=postgres";

            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    MessageBox.Show(
                        "Connected to PostgreSQL successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                } // Connection is automatically closed when leaving the 'using' block.
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
