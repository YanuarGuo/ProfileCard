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
            label6 = new Label();
            BtnRead = new Button();
            GBoxConnectReader = new GroupBox();
            bInit = new Button();
            btnGetUID = new Button();
            bConnect = new Button();
            GBoxTapReader = new GroupBox();
            label8 = new Label();
            BtnStartThreading = new Button();
            BtnStopThreading = new Button();
            BtnStartTimer = new Button();
            BtnStopTimer = new Button();
            cbReader = new ComboBox();
            label7 = new Label();
            mMsg = new ListBox();
            bReset = new Button();
            bClear = new Button();
            dReadAll = new DataGridView();
            BtnReadProfile = new Button();
            BtnResetDataBlock = new Button();
            cardTimer = new System.Windows.Forms.Timer(components);
            label9 = new Label();
            label10 = new Label();
            ((System.ComponentModel.ISupportInitialize)ProfilePict).BeginInit();
            GBoxConnectReader.SuspendLayout();
            GBoxTapReader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dReadAll).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 181);
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
            label2.Location = new Point(12, 239);
            label2.Name = "label2";
            label2.Size = new Size(45, 15);
            label2.TabIndex = 2;
            label2.Text = "Gender";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 210);
            label3.Name = "label3";
            label3.Size = new Size(73, 15);
            label3.TabIndex = 3;
            label3.Text = "Date of Birth";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 269);
            label4.Name = "label4";
            label4.Size = new Size(49, 15);
            label4.TabIndex = 4;
            label4.Text = "Address";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 339);
            label5.Name = "label5";
            label5.Size = new Size(96, 15);
            label5.TabIndex = 5;
            label5.Text = "Contact Number";
            // 
            // TxtName
            // 
            TxtName.Location = new Point(125, 178);
            TxtName.MaxLength = 255;
            TxtName.Name = "TxtName";
            TxtName.Size = new Size(258, 23);
            TxtName.TabIndex = 6;
            // 
            // TxtBirthDate
            // 
            TxtBirthDate.Location = new Point(125, 207);
            TxtBirthDate.MaxLength = 50;
            TxtBirthDate.Name = "TxtBirthDate";
            TxtBirthDate.Size = new Size(258, 23);
            TxtBirthDate.TabIndex = 7;
            // 
            // TxtGender
            // 
            TxtGender.Location = new Point(125, 236);
            TxtGender.MaxLength = 10;
            TxtGender.Name = "TxtGender";
            TxtGender.Size = new Size(258, 23);
            TxtGender.TabIndex = 8;
            // 
            // TxtAddress
            // 
            TxtAddress.Location = new Point(125, 265);
            TxtAddress.MaxLength = 255;
            TxtAddress.Multiline = true;
            TxtAddress.Name = "TxtAddress";
            TxtAddress.Size = new Size(258, 66);
            TxtAddress.TabIndex = 9;
            // 
            // TxtNumber
            // 
            TxtNumber.Location = new Point(125, 337);
            TxtNumber.MaxLength = 15;
            TxtNumber.Name = "TxtNumber";
            TxtNumber.Size = new Size(258, 23);
            TxtNumber.TabIndex = 10;
            // 
            // BtnConfirm
            // 
            BtnConfirm.BackColor = SystemColors.ButtonHighlight;
            BtnConfirm.Location = new Point(298, 382);
            BtnConfirm.Name = "BtnConfirm";
            BtnConfirm.Size = new Size(85, 23);
            BtnConfirm.TabIndex = 11;
            BtnConfirm.Text = "Confirm";
            BtnConfirm.UseVisualStyleBackColor = false;
            BtnConfirm.Click += BtnConfirm_Click;
            // 
            // BtnReset
            // 
            BtnReset.BackColor = SystemColors.ButtonHighlight;
            BtnReset.Location = new Point(116, 382);
            BtnReset.Name = "BtnReset";
            BtnReset.Size = new Size(85, 23);
            BtnReset.TabIndex = 12;
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
            BtnRead.Location = new Point(1072, 382);
            BtnRead.Name = "BtnRead";
            BtnRead.Size = new Size(107, 23);
            BtnRead.TabIndex = 14;
            BtnRead.Text = "Read All Block";
            BtnRead.UseVisualStyleBackColor = false;
            BtnRead.Click += BtnRead_Click;
            // 
            // GBoxConnectReader
            // 
            GBoxConnectReader.Controls.Add(bInit);
            GBoxConnectReader.Controls.Add(btnGetUID);
            GBoxConnectReader.Controls.Add(bConnect);
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
            // bInit
            // 
            bInit.BackColor = SystemColors.ButtonHighlight;
            bInit.Location = new Point(92, 63);
            bInit.Name = "bInit";
            bInit.Size = new Size(85, 23);
            bInit.TabIndex = 27;
            bInit.Text = "Initialize";
            bInit.UseVisualStyleBackColor = false;
            bInit.Click += bInit_Click;
            // 
            // btnGetUID
            // 
            btnGetUID.BackColor = SystemColors.ButtonHighlight;
            btnGetUID.Location = new Point(183, 63);
            btnGetUID.Name = "btnGetUID";
            btnGetUID.Size = new Size(85, 23);
            btnGetUID.TabIndex = 20;
            btnGetUID.Text = "Get UID";
            btnGetUID.UseVisualStyleBackColor = false;
            btnGetUID.Click += btnGetUID_Click;
            // 
            // bConnect
            // 
            bConnect.BackColor = SystemColors.ButtonHighlight;
            bConnect.Location = new Point(274, 63);
            bConnect.Name = "bConnect";
            bConnect.Size = new Size(85, 23);
            bConnect.TabIndex = 19;
            bConnect.Text = "Connect";
            bConnect.UseVisualStyleBackColor = false;
            bConnect.Click += bConnect_Click;
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
            BtnStartThreading.TabIndex = 21;
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
            BtnStopThreading.TabIndex = 22;
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
            BtnStartTimer.TabIndex = 21;
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
            BtnStopTimer.TabIndex = 22;
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
            mMsg.Size = new Size(304, 229);
            mMsg.TabIndex = 18;
            // 
            // bReset
            // 
            bReset.BackColor = SystemColors.ButtonHighlight;
            bReset.Location = new Point(622, 381);
            bReset.Name = "bReset";
            bReset.Size = new Size(85, 23);
            bReset.TabIndex = 21;
            bReset.Text = "Reset";
            bReset.UseVisualStyleBackColor = false;
            bReset.Click += bReset_Click;
            // 
            // bClear
            // 
            bClear.BackColor = SystemColors.ButtonHighlight;
            bClear.Location = new Point(531, 381);
            bClear.Name = "bClear";
            bClear.Size = new Size(85, 23);
            bClear.TabIndex = 22;
            bClear.Text = "Clear";
            bClear.UseVisualStyleBackColor = false;
            bClear.Click += bClear_Click;
            // 
            // dReadAll
            // 
            dReadAll.BackgroundColor = SystemColors.ButtonHighlight;
            dReadAll.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dReadAll.Location = new Point(719, 131);
            dReadAll.Name = "dReadAll";
            dReadAll.Size = new Size(460, 229);
            dReadAll.TabIndex = 23;
            // 
            // BtnReadProfile
            // 
            BtnReadProfile.BackColor = SystemColors.ButtonHighlight;
            BtnReadProfile.Location = new Point(207, 382);
            BtnReadProfile.Name = "BtnReadProfile";
            BtnReadProfile.Size = new Size(85, 23);
            BtnReadProfile.TabIndex = 24;
            BtnReadProfile.Text = "Read Profile";
            BtnReadProfile.UseVisualStyleBackColor = false;
            BtnReadProfile.Click += BtnReadProfile_Click;
            // 
            // BtnResetDataBlock
            // 
            BtnResetDataBlock.BackColor = SystemColors.ButtonHighlight;
            BtnResetDataBlock.Location = new Point(959, 382);
            BtnResetDataBlock.Name = "BtnResetDataBlock";
            BtnResetDataBlock.Size = new Size(107, 23);
            BtnResetDataBlock.TabIndex = 25;
            BtnResetDataBlock.Text = "Reset Data Block";
            BtnResetDataBlock.UseVisualStyleBackColor = false;
            BtnResetDataBlock.Click += BtnResetDataBlock_Click;
            // 
            // cardTimer
            // 
            cardTimer.Tick += cardTimer_Tick;
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
            // ProfileCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1196, 425);
            Controls.Add(BtnResetDataBlock);
            Controls.Add(BtnReadProfile);
            Controls.Add(dReadAll);
            Controls.Add(bClear);
            Controls.Add(bReset);
            Controls.Add(mMsg);
            Controls.Add(BtnRead);
            Controls.Add(GBoxConnectReader);
            Controls.Add(label6);
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
        private TextBox TxtBirthDate;
        private TextBox TxtGender;
        private TextBox TxtAddress;
        private TextBox TxtNumber;
        private Button BtnConfirm;
        private Button BtnReset;
        private Label label6;
        private Button BtnRead;
        private GroupBox GBoxConnectReader;
        private ListBox mMsg;
        private Button bConnect;
        private ComboBox cbReader;
        private Label label7;
        private Button btnGetUID;
        private Button bReset;
        private Button bClear;
        private DataGridView dReadAll;
        private Button BtnReadProfile;
        private Button BtnResetDataBlock;
        private System.Windows.Forms.Timer cardTimer;
        private Button BtnStopTimer;
        private Button BtnStartTimer;
        private GroupBox GBoxTapReader;
        private Label label8;
        private Button bInit;
        private Button BtnStartThreading;
        private Button BtnStopThreading;
        private Label label10;
        private Label label9;
    }
}
