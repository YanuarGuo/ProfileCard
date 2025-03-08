using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProfileCard
{
    public partial class ProfileCard : Form
    {
        private Bitmap originalImage;
        private string loadedFilePath;
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

        public ProfileCard()
        {
            InitializeComponent();
        }

        private void ProfileCard_Load(object sender, EventArgs e)
        {
            InitMenu();
        }

        private void InitMenu()
        {
            connActive = false;
            cbReader.Items.Clear();
            cbReader.Text = "";
            mMsg.Items.Clear();
            displayOut(0, 0, "Program ready");
            bConnect.Enabled = false;
            bInit.Enabled = true;
            bReset.Enabled = false;
            btnGetUID.Enabled = false;
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
                Debug.WriteLine(
                    $"Chunk {i / chunkSize}: {BitConverter.ToString(splitDataReturn).Replace("-", " ")}"
                );
            }

            return splitDataList;
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

                ProfilePict.SizeMode = PictureBoxSizeMode.Zoom;
                ProfilePict.Image = originalImage;

                UpdateLabelVisibility();
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            TxtAddress.Text = "";
            TxtBirthDate.Text = "";
            TxtGender.Text = "";
            TxtName.Text = "";
            TxtNumber.Text = "";
            ProfilePict.Image = null;
        }

        private string EncodeProfileData()
        {
            string name = TxtName.Text.Trim();
            string dob = TxtBirthDate.Text.Trim();
            string gender = TxtGender.Text.Trim();
            string address = TxtAddress.Text.Trim();
            string phone = TxtNumber.Text.Trim();

            byte[]? compressedImage = null;
            if (ProfilePict.Image != null)
            {
                compressedImage = CompressImage(ProfilePict.Image, 40, 60, 80);
            }
            string hexImage =
                compressedImage != null ? ConvertToHexWithHeader("PIC", compressedImage) : "PIC00";
            string hexName = ConvertToHexWithHeader("NME", name);
            string hexDOB = ConvertToHexWithHeader("DTE", dob);
            string hexGender = ConvertToHexWithHeader("GDR", gender);
            string hexAddress = ConvertToHexWithHeader("ADR", address);
            string hexPhone = ConvertToHexWithHeader("NUM", phone);

            return $"{hexImage} {hexName} {hexDOB} {hexGender} {hexAddress} {hexPhone}";
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

        private static string ConvertToHexWithHeader(string header, byte[] data)
        {
            string hexHeader = BitConverter
                .ToString(Encoding.ASCII.GetBytes(header))
                .Replace("-", "");
            string hexData = BitConverter.ToString(data).Replace("-", "");
            return $"{hexHeader} {hexData}";
        }

        private static string ConvertToHexWithHeader(string header, string data)
        {
            string hexHeader = BitConverter
                .ToString(Encoding.ASCII.GetBytes(header))
                .Replace("-", "");
            string hexData = BitConverter.ToString(Encoding.ASCII.GetBytes(data)).Replace("-", "");
            return $"{hexHeader} {hexData}";
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
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(
                System.Drawing.Imaging.Encoder.Quality,
                quality
            );
            resizedImage.Save(ms, jpgEncoder, encoderParameters);
            return ms.ToArray();
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
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
            bInit.Enabled = false;
            bConnect.Enabled = true;
            bReset.Enabled = true;
            bClear.Enabled = true;
            btnGetUID.Enabled = true;
        }

        private void displayOut(int errType, int retVal, string PrintText)
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
                displayOut(1, retCode, "");
                return;
            }

            retCode = ModWinsCard.SCardListReaders(this.hContext, null, null, ref pcchReaders);

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                displayOut(1, retCode, "");
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
                displayOut(0, 0, " ");
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
                displayOut(0, 0, "Successful connection to " + cbReader.Text);
            }
            else
            {
                displayOut(
                    0,
                    0,
                    "The smart card has been removed, so that further communication is not possible."
                );
            }

            connActive = true;
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

            displayOut(2, 0, tmpStr);
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
                displayOut(1, retCode, "");
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
                            displayOut(4, 0, "Return bytes are not acceptable.");
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

                displayOut(3, 0, tmpStr.Trim());
            }

            return retCode;
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
                Debug.WriteLine($"Authentication successful for block {block}");
                return true;
            }
            else
            {
                Debug.WriteLine($"Authentication failed for block {block}");
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

            Debug.WriteLine("All data written successfully!");
        }

        private byte[] ReadBlock(int block)
        {
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

        private void ParseProfileData(byte[] data)
        {
            Dictionary<string, string> profileData = new Dictionary<string, string>();

            int index = 0;
            while (index < data.Length - 3)
            {
                string header = Encoding.ASCII.GetString(data, index, 3);
                index += 3;

                List<byte> content = new List<byte>();
                while (
                    index < data.Length
                    && data[index] != 0x50
                    && data[index] != 0x4E
                    && data[index] != 0x44
                    && data[index] != 0x47
                    && data[index] != 0x41
                    && data[index] != 0x4E
                )
                {
                    content.Add(data[index]);
                    index++;
                }

                profileData[header] = Encoding.ASCII.GetString(content.ToArray()).TrimEnd('\0');
            }

            if (profileData.ContainsKey("NME"))
                TxtName.Text = profileData["NME"];
            if (profileData.ContainsKey("DTE"))
                TxtBirthDate.Text = profileData["DTE"];
            if (profileData.ContainsKey("GDR"))
                TxtGender.Text = profileData["GDR"];
            if (profileData.ContainsKey("ADR"))
                TxtAddress.Text = profileData["ADR"];
            if (profileData.ContainsKey("NUM"))
                TxtNumber.Text = profileData["NUM"];

            if (profileData.ContainsKey("PIC"))
            {
                string imageData = profileData["PIC"];
                MessageBox.Show(
                    $"Data Gambar (PIC): {imageData}",
                    "Debug",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                try
                {
                    byte[] imageBytes = Encoding.ASCII.GetBytes(profileData["PIC"]);
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        ProfilePict.Image = System.Drawing.Image.FromStream(ms);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Gagal menampilkan gambar: {ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
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
            int blockIndex = 4;

            try
            {
                for (int sector = 1; sector < sectorSizes.Length; sector++)
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

                    for (int i = 0; i < blocksInSector - 1; i++)
                    {
                        int currentBlock = blockIndex + i;
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
    }
}
