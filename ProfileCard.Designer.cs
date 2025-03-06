namespace ProfileCard
{
    partial class ProfileCard
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            ProfilePict = new PictureBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            TxtName = new TextBox();
            TxtBirthDate = new TextBox();
            TxtGender = new TextBox();
            TxtAddress = new TextBox();
            TxtNumber = new TextBox();
            BtnConfirm = new Button();
            BtnReset = new Button();
            ((System.ComponentModel.ISupportInitialize)ProfilePict).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 161);
            label1.Name = "label1";
            label1.Size = new Size(61, 15);
            label1.TabIndex = 0;
            label1.Text = "Full Name";
            // 
            // ProfilePict
            // 
            ProfilePict.Location = new Point(149, 12);
            ProfilePict.Name = "ProfilePict";
            ProfilePict.Size = new Size(100, 114);
            ProfilePict.TabIndex = 1;
            ProfilePict.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 219);
            label2.Name = "label2";
            label2.Size = new Size(45, 15);
            label2.TabIndex = 2;
            label2.Text = "Gender";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 190);
            label3.Name = "label3";
            label3.Size = new Size(73, 15);
            label3.TabIndex = 3;
            label3.Text = "Date of Birth";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 249);
            label4.Name = "label4";
            label4.Size = new Size(49, 15);
            label4.TabIndex = 4;
            label4.Text = "Address";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 319);
            label5.Name = "label5";
            label5.Size = new Size(96, 15);
            label5.TabIndex = 5;
            label5.Text = "Contact Number";
            // 
            // TxtName
            // 
            TxtName.Location = new Point(125, 158);
            TxtName.Name = "TxtName";
            TxtName.Size = new Size(258, 23);
            TxtName.TabIndex = 6;
            // 
            // TxtBirthDate
            // 
            TxtBirthDate.Location = new Point(125, 187);
            TxtBirthDate.Name = "TxtBirthDate";
            TxtBirthDate.Size = new Size(258, 23);
            TxtBirthDate.TabIndex = 7;
            // 
            // TxtGender
            // 
            TxtGender.Location = new Point(125, 216);
            TxtGender.Name = "TxtGender";
            TxtGender.Size = new Size(258, 23);
            TxtGender.TabIndex = 8;
            // 
            // TxtAddress
            // 
            TxtAddress.Location = new Point(125, 245);
            TxtAddress.Multiline = true;
            TxtAddress.Name = "TxtAddress";
            TxtAddress.Size = new Size(258, 66);
            TxtAddress.TabIndex = 9;
            // 
            // TxtNumber
            // 
            TxtNumber.Location = new Point(125, 317);
            TxtNumber.Name = "TxtNumber";
            TxtNumber.Size = new Size(258, 23);
            TxtNumber.TabIndex = 10;
            // 
            // BtnConfirm
            // 
            BtnConfirm.Location = new Point(290, 385);
            BtnConfirm.Name = "BtnConfirm";
            BtnConfirm.Size = new Size(93, 23);
            BtnConfirm.TabIndex = 11;
            BtnConfirm.Text = "Confirm";
            BtnConfirm.UseVisualStyleBackColor = true;
            // 
            // BtnReset
            // 
            BtnReset.Location = new Point(191, 385);
            BtnReset.Name = "BtnReset";
            BtnReset.Size = new Size(93, 23);
            BtnReset.TabIndex = 12;
            BtnReset.Text = "Reset";
            BtnReset.UseVisualStyleBackColor = true;
            // 
            // ProfileCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(395, 423);
            Controls.Add(BtnReset);
            Controls.Add(BtnConfirm);
            Controls.Add(TxtNumber);
            Controls.Add(TxtAddress);
            Controls.Add(TxtGender);
            Controls.Add(TxtBirthDate);
            Controls.Add(TxtName);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(ProfilePict);
            Controls.Add(label1);
            Name = "ProfileCard";
            Text = "ProfileCard";
            ((System.ComponentModel.ISupportInitialize)ProfilePict).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private PictureBox ProfilePict;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private TextBox TxtName;
        private TextBox TxtBirthDate;
        private TextBox TxtGender;
        private TextBox TxtAddress;
        private TextBox TxtNumber;
        private Button BtnConfirm;
        private Button BtnReset;
    }
}
