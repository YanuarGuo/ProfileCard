using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace ProfileCard
{
    public partial class ProfileCard : Form
    {
        private Bitmap originalImage;
        private string loadedFilePath;

        public ProfileCard()
        {
            InitializeComponent();
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

            byte[] compressedImage = CompressImage(ProfilePict.Image, 40, 60, 80);
            string hexImage = ConvertToHexWithHeader("PIC", compressedImage);
            string hexName = ConvertToHexWithHeader("NME", name);
            string hexDOB = ConvertToHexWithHeader("DTE", dob);
            string hexGender = ConvertToHexWithHeader("GDR", gender);
            string hexAddress = ConvertToHexWithHeader("ADR", address);
            string hexPhone = ConvertToHexWithHeader("NUM", phone);

            Debug.WriteLine($"{hexImage} {hexName} {hexDOB} {hexGender} {hexAddress} {hexPhone}");
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
                //MessageBox.Show(
                //    "Confirmed successfully!",
                //    "Success",
                //    MessageBoxButtons.OK,
                //    MessageBoxIcon.Information
                //);
            }
            else { }
        }

        private string ConvertToHexWithHeader(string header, byte[] data)
        {
            string hexHeader = BitConverter
                .ToString(Encoding.ASCII.GetBytes(header))
                .Replace("-", "");
            string hexData = BitConverter.ToString(data).Replace("-", "");
            return $"{hexHeader} {hexData}";
        }

        private string ConvertToHexWithHeader(string header, string data)
        {
            string hexHeader = BitConverter
                .ToString(Encoding.ASCII.GetBytes(header))
                .Replace("-", "");
            string hexData = BitConverter.ToString(Encoding.ASCII.GetBytes(data)).Replace("-", "");
            return $"{hexHeader} {hexData}";
        }

        private byte[] CompressImage(
            System.Drawing.Image image,
            int width,
            int height,
            long quality
        )
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image), "Image cannot be null.");

            using (Bitmap resizedImage = new Bitmap(image, new Size(width, height)))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                    EncoderParameters encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(
                        System.Drawing.Imaging.Encoder.Quality,
                        quality
                    );
                    resizedImage.Save(ms, jpgEncoder, encoderParameters);
                    return ms.ToArray();
                }
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
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
    }
}
