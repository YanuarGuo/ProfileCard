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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfileCard));
            label1 = new Label();
            ProfilePict = new PictureBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            TxtName = new TextBox();
            TxtAddress = new TextBox();
            TxtNumber = new TextBox();
            BtnConfirm = new Button();
            BtnReset = new Button();
            label6 = new Label();
            BtnRead = new Button();
            GBoxConnectReader = new GroupBox();
            BtnInitialize = new Button();
            BtnGetUID = new Button();
            BtnConnect = new Button();
            GBoxTapReader = new GroupBox();
            label10 = new Label();
            label9 = new Label();
            label8 = new Label();
            BtnStartThreading = new Button();
            BtnStopThreading = new Button();
            BtnStartTimer = new Button();
            BtnStopTimer = new Button();
            cbReader = new ComboBox();
            label7 = new Label();
            mMsg = new ListBox();
            BtnResetReader = new Button();
            BtnClear = new Button();
            dReadAll = new DataGridView();
            BtnReadProfile = new Button();
            BtnResetDataBlock = new Button();
            CardTimer = new System.Windows.Forms.Timer(components);
            label11 = new Label();
            TxtID = new TextBox();
            DtBirth = new DateTimePicker();
            CbGender = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)ProfilePict).BeginInit();
            GBoxConnectReader.SuspendLayout();
            GBoxTapReader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dReadAll).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 191);
            label1.Name = "label1";
            label1.Size = new Size(61, 15);
            label1.TabIndex = 0;
            label1.Text = "Full Name";
            // 
            // ProfilePict
            // 
            ProfilePict.BorderStyle = BorderStyle.FixedSingle;
            ProfilePict.Location = new Point(152, 12);
            ProfilePict.Name = "ProfilePict";
            ProfilePict.Size = new Size(100, 140);
            ProfilePict.TabIndex = 1;
            ProfilePict.TabStop = false;
            ProfilePict.Click += ProfilePict_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 278);
            label2.Name = "label2";
            label2.Size = new Size(45, 15);
            label2.TabIndex = 2;
            label2.Text = "Gender";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 249);
            label3.Name = "label3";
            label3.Size = new Size(73, 15);
            label3.TabIndex = 3;
            label3.Text = "Date of Birth";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 308);
            label4.Name = "label4";
            label4.Size = new Size(49, 15);
            label4.TabIndex = 4;
            label4.Text = "Address";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 378);
            label5.Name = "label5";
            label5.Size = new Size(96, 15);
            label5.TabIndex = 5;
            label5.Text = "Contact Number";
            // 
            // TxtName
            // 
            TxtName.Location = new Point(125, 188);
            TxtName.MaxLength = 255;
            TxtName.Name = "TxtName";
            TxtName.Size = new Size(258, 23);
            TxtName.TabIndex = 1;
            // 
            // TxtAddress
            // 
            TxtAddress.Location = new Point(125, 304);
            TxtAddress.MaxLength = 255;
            TxtAddress.Multiline = true;
            TxtAddress.Name = "TxtAddress";
            TxtAddress.Size = new Size(258, 66);
            TxtAddress.TabIndex = 5;
            // 
            // TxtNumber
            // 
            TxtNumber.Location = new Point(125, 376);
            TxtNumber.MaxLength = 20;
            TxtNumber.Name = "TxtNumber";
            TxtNumber.Size = new Size(258, 23);
            TxtNumber.TabIndex = 6;
            // 
            // BtnConfirm
            // 
            BtnConfirm.BackColor = SystemColors.ButtonHighlight;
            BtnConfirm.Location = new Point(298, 421);
            BtnConfirm.Name = "BtnConfirm";
            BtnConfirm.Size = new Size(85, 23);
            BtnConfirm.TabIndex = 7;
            BtnConfirm.Text = "Confirm";
            BtnConfirm.UseVisualStyleBackColor = false;
            BtnConfirm.Click += BtnConfirm_Click;
            // 
            // BtnReset
            // 
            BtnReset.BackColor = SystemColors.ButtonHighlight;
            BtnReset.Location = new Point(116, 421);
            BtnReset.Name = "BtnReset";
            BtnReset.Size = new Size(85, 23);
            BtnReset.TabIndex = 9;
            BtnReset.Text = "Reset";
            BtnReset.UseVisualStyleBackColor = false;
            BtnReset.Click += BtnReset_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 7F);
            label6.Location = new Point(176, 75);
            label6.Name = "label6";
            label6.Size = new Size(54, 12);
            label6.TabIndex = 13;
            label6.Text = "Add Image";
            // 
            // BtnRead
            // 
            BtnRead.BackColor = SystemColors.ButtonHighlight;
            BtnRead.Location = new Point(1072, 421);
            BtnRead.Name = "BtnRead";
            BtnRead.Size = new Size(107, 23);
            BtnRead.TabIndex = 13;
            BtnRead.Text = "Read All Block";
            BtnRead.UseVisualStyleBackColor = false;
            BtnRead.Click += BtnRead_Click;
            // 
            // GBoxConnectReader
            // 
            GBoxConnectReader.Controls.Add(BtnInitialize);
            GBoxConnectReader.Controls.Add(BtnGetUID);
            GBoxConnectReader.Controls.Add(BtnConnect);
            GBoxConnectReader.Controls.Add(GBoxTapReader);
            GBoxConnectReader.Controls.Add(cbReader);
            GBoxConnectReader.Controls.Add(label7);
            GBoxConnectReader.Location = new Point(403, 12);
            GBoxConnectReader.Name = "GBoxConnectReader";
            GBoxConnectReader.Size = new Size(776, 110);
            GBoxConnectReader.TabIndex = 17;
            GBoxConnectReader.TabStop = false;
            GBoxConnectReader.Text = "Reader's Connection";
            // 
            // BtnInitialize
            // 
            BtnInitialize.BackColor = SystemColors.ButtonHighlight;
            BtnInitialize.Location = new Point(92, 63);
            BtnInitialize.Name = "BtnInitialize";
            BtnInitialize.Size = new Size(85, 23);
            BtnInitialize.TabIndex = 12;
            BtnInitialize.Text = "Initialize";
            BtnInitialize.UseVisualStyleBackColor = false;
            BtnInitialize.Click += BtnInitialize_Click;
            // 
            // BtnGetUID
            // 
            BtnGetUID.BackColor = SystemColors.ButtonHighlight;
            BtnGetUID.Location = new Point(183, 63);
            BtnGetUID.Name = "BtnGetUID";
            BtnGetUID.Size = new Size(85, 23);
            BtnGetUID.TabIndex = 11;
            BtnGetUID.Text = "Get UID";
            BtnGetUID.UseVisualStyleBackColor = false;
            BtnGetUID.Click += BtnGetUID_Click;
            // 
            // BtnConnect
            // 
            BtnConnect.BackColor = SystemColors.ButtonHighlight;
            BtnConnect.Location = new Point(274, 63);
            BtnConnect.Name = "BtnConnect";
            BtnConnect.Size = new Size(85, 23);
            BtnConnect.TabIndex = 10;
            BtnConnect.Text = "Connect";
            BtnConnect.UseVisualStyleBackColor = false;
            BtnConnect.Click += BtnConnect_Click;
            // 
            // GBoxTapReader
            // 
            GBoxTapReader.Controls.Add(label10);
            GBoxTapReader.Controls.Add(label9);
            GBoxTapReader.Controls.Add(label8);
            GBoxTapReader.Controls.Add(BtnStartThreading);
            GBoxTapReader.Controls.Add(BtnStopThreading);
            GBoxTapReader.Controls.Add(BtnStartTimer);
            GBoxTapReader.Controls.Add(BtnStopTimer);
            GBoxTapReader.Location = new Point(374, 15);
            GBoxTapReader.Name = "GBoxTapReader";
            GBoxTapReader.Size = new Size(396, 89);
            GBoxTapReader.TabIndex = 26;
            GBoxTapReader.TabStop = false;
            GBoxTapReader.Text = "Tap Reader";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 8.25F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label10.Location = new Point(279, 73);
            label10.Name = "label10";
            label10.Size = new Size(40, 13);
            label10.TabIndex = 25;
            label10.Text = "Thread";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 8.25F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label9.Location = new Point(78, 73);
            label9.Name = "label9";
            label9.Size = new Size(34, 13);
            label9.TabIndex = 24;
            label9.Text = "Timer";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 8.25F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label8.Location = new Point(62, 24);
            label8.Name = "label8";
            label8.Size = new Size(272, 13);
            label8.TabIndex = 23;
            label8.Text = "Tap your card on the reader once you have pressed Start";
            // 
            // BtnStartThreading
            // 
            BtnStartThreading.BackColor = SystemColors.ButtonHighlight;
            BtnStartThreading.Location = new Point(302, 48);
            BtnStartThreading.Name = "BtnStartThreading";
            BtnStartThreading.Size = new Size(85, 23);
            BtnStartThreading.TabIndex = 17;
            BtnStartThreading.Text = "Start";
            BtnStartThreading.UseVisualStyleBackColor = false;
            BtnStartThreading.Click += BtnStartThreading_Click;
            // 
            // BtnStopThreading
            // 
            BtnStopThreading.BackColor = SystemColors.ButtonHighlight;
            BtnStopThreading.Location = new Point(211, 48);
            BtnStopThreading.Name = "BtnStopThreading";
            BtnStopThreading.Size = new Size(85, 23);
            BtnStopThreading.TabIndex = 18;
            BtnStopThreading.Text = "Stop";
            BtnStopThreading.UseVisualStyleBackColor = false;
            BtnStopThreading.Click += BtnStopThreading_Click;
            // 
            // BtnStartTimer
            // 
            BtnStartTimer.BackColor = SystemColors.ButtonHighlight;
            BtnStartTimer.Location = new Point(99, 48);
            BtnStartTimer.Name = "BtnStartTimer";
            BtnStartTimer.Size = new Size(85, 23);
            BtnStartTimer.TabIndex = 19;
            BtnStartTimer.Text = "Start";
            BtnStartTimer.UseVisualStyleBackColor = false;
            BtnStartTimer.Click += BtnStartTimer_Click;
            // 
            // BtnStopTimer
            // 
            BtnStopTimer.BackColor = SystemColors.ButtonHighlight;
            BtnStopTimer.Location = new Point(8, 48);
            BtnStopTimer.Name = "BtnStopTimer";
            BtnStopTimer.Size = new Size(85, 23);
            BtnStopTimer.TabIndex = 20;
            BtnStopTimer.Text = "Stop";
            BtnStopTimer.UseVisualStyleBackColor = false;
            BtnStopTimer.Click += BtnStopTimer_Click;
            // 
            // cbReader
            // 
            cbReader.FormattingEnabled = true;
            cbReader.Location = new Point(87, 25);
            cbReader.Name = "cbReader";
            cbReader.Size = new Size(272, 23);
            cbReader.TabIndex = 15;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(6, 28);
            label7.Name = "label7";
            label7.Size = new Size(77, 15);
            label7.TabIndex = 16;
            label7.Text = "Select Reader";
            // 
            // mMsg
            // 
            mMsg.FormattingEnabled = true;
            mMsg.ItemHeight = 15;
            mMsg.Location = new Point(403, 131);
            mMsg.Name = "mMsg";
            mMsg.Size = new Size(304, 274);
            mMsg.TabIndex = 18;
            // 
            // BtnResetReader
            // 
            BtnResetReader.BackColor = SystemColors.ButtonHighlight;
            BtnResetReader.Location = new Point(622, 421);
            BtnResetReader.Name = "BtnResetReader";
            BtnResetReader.Size = new Size(85, 23);
            BtnResetReader.TabIndex = 15;
            BtnResetReader.Text = "Reset";
            BtnResetReader.UseVisualStyleBackColor = false;
            BtnResetReader.Click += BtnResetReader_Click;
            // 
            // BtnClear
            // 
            BtnClear.BackColor = SystemColors.ButtonHighlight;
            BtnClear.Location = new Point(531, 421);
            BtnClear.Name = "BtnClear";
            BtnClear.Size = new Size(85, 23);
            BtnClear.TabIndex = 16;
            BtnClear.Text = "Clear";
            BtnClear.UseVisualStyleBackColor = false;
            BtnClear.Click += BtnClear_Click;
            // 
            // dReadAll
            // 
            dReadAll.BackgroundColor = SystemColors.ButtonHighlight;
            dReadAll.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dReadAll.Location = new Point(719, 131);
            dReadAll.Name = "dReadAll";
            dReadAll.Size = new Size(460, 274);
            dReadAll.TabIndex = 23;
            // 
            // BtnReadProfile
            // 
            BtnReadProfile.BackColor = SystemColors.ButtonHighlight;
            BtnReadProfile.Location = new Point(207, 421);
            BtnReadProfile.Name = "BtnReadProfile";
            BtnReadProfile.Size = new Size(85, 23);
            BtnReadProfile.TabIndex = 8;
            BtnReadProfile.Text = "Read Profile";
            BtnReadProfile.UseVisualStyleBackColor = false;
            BtnReadProfile.Click += BtnReadProfile_Click;
            // 
            // BtnResetDataBlock
            // 
            BtnResetDataBlock.BackColor = SystemColors.ButtonHighlight;
            BtnResetDataBlock.Location = new Point(959, 421);
            BtnResetDataBlock.Name = "BtnResetDataBlock";
            BtnResetDataBlock.Size = new Size(107, 23);
            BtnResetDataBlock.TabIndex = 14;
            BtnResetDataBlock.Text = "Reset Data Block";
            BtnResetDataBlock.UseVisualStyleBackColor = false;
            BtnResetDataBlock.Click += BtnResetDataBlock_Click;
            // 
            // CardTimer
            // 
            CardTimer.Tick += CardTimer_Tick;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(12, 220);
            label11.Name = "label11";
            label11.Size = new Size(73, 15);
            label11.TabIndex = 26;
            label11.Text = "Employee ID";
            // 
            // TxtID
            // 
            TxtID.Location = new Point(125, 217);
            TxtID.MaxLength = 255;
            TxtID.Name = "TxtID";
            TxtID.Size = new Size(258, 23);
            TxtID.TabIndex = 2;
            // 
            // DtBirth
            // 
            DtBirth.Format = DateTimePickerFormat.Custom;
            DtBirth.Location = new Point(125, 246);
            DtBirth.Name = "DtBirth";
            DtBirth.Size = new Size(258, 23);
            DtBirth.TabIndex = 3;
            // 
            // CbGender
            // 
            CbGender.DropDownStyle = ComboBoxStyle.DropDownList;
            CbGender.FormattingEnabled = true;
            CbGender.Items.AddRange(new object[] { "Male", "Female" });
            CbGender.Location = new Point(125, 275);
            CbGender.Name = "CbGender";
            CbGender.Size = new Size(258, 23);
            CbGender.TabIndex = 4;
            // 
            // ProfileCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1192, 464);
            Controls.Add(CbGender);
            Controls.Add(DtBirth);
            Controls.Add(TxtID);
            Controls.Add(label11);
            Controls.Add(BtnResetDataBlock);
            Controls.Add(BtnReadProfile);
            Controls.Add(dReadAll);
            Controls.Add(BtnClear);
            Controls.Add(BtnResetReader);
            Controls.Add(mMsg);
            Controls.Add(BtnRead);
            Controls.Add(GBoxConnectReader);
            Controls.Add(label6);
            Controls.Add(BtnReset);
            Controls.Add(BtnConfirm);
            Controls.Add(TxtNumber);
            Controls.Add(TxtAddress);
            Controls.Add(TxtName);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(ProfilePict);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ProfileCard";
            Text = "ProfileCard";
            Load += ProfileCard_Load;
            ((System.ComponentModel.ISupportInitialize)ProfilePict).EndInit();
            GBoxConnectReader.ResumeLayout(false);
            GBoxConnectReader.PerformLayout();
            GBoxTapReader.ResumeLayout(false);
            GBoxTapReader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dReadAll).EndInit();
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
        private TextBox TxtAddress;
        private TextBox TxtNumber;
        private Button BtnConfirm;
        private Button BtnReset;
        private Label label6;
        private Button BtnRead;
        private GroupBox GBoxConnectReader;
        private ListBox mMsg;
        private Button BtnConnect;
        private ComboBox cbReader;
        private Label label7;
        private Button BtnGetUID;
        private Button BtnResetReader;
        private Button BtnClear;
        private DataGridView dReadAll;
        private Button BtnReadProfile;
        private Button BtnResetDataBlock;
        private System.Windows.Forms.Timer CardTimer;
        private Button BtnStopTimer;
        private Button BtnStartTimer;
        private GroupBox GBoxTapReader;
        private Label label8;
        private Button BtnInitialize;
        private Button BtnStartThreading;
        private Button BtnStopThreading;
        private Label label10;
        private Label label9;
        private Label label11;
        private TextBox TxtID;
        private DateTimePicker DtBirth;
        private ComboBox CbGender;
    }
}
