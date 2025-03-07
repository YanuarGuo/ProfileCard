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
        }

        private void SplitData()
        {
            string hexData = EncodeProfileData();

            hexData = hexData.Replace(" ", "");

            byte[] byteArray = Enumerable
                .Range(0, hexData.Length / 2)
                .Select(i => Convert.ToByte(hexData.Substring(i * 2, 2), 16))
                .ToArray();

            int chunkSize = 16;
            for (int i = 0; i < byteArray.Length; i += chunkSize)
            {
                byte[] splitDataReturn = byteArray.Skip(i).Take(chunkSize).ToArray();
                Debug.WriteLine(BitConverter.ToString(splitDataReturn).Replace("-", " "));
            }
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
                EncodeProfileData();
                SplitData();
                //MessageBox.Show(
                //    "Uploaded successfully!",
                //    "Success",
                //    MessageBoxButtons.OK,
                //    MessageBoxIcon.Information
                //);
            }
            else { }
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

        // READER BELUM BISA
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
    }
}
