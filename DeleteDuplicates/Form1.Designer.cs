namespace WindowsFormsApplication1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonGetInfo = new System.Windows.Forms.Button();
            this.textBoxForPath = new System.Windows.Forms.TextBox();
            this.textBoxForInfo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonBrowseFolder = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.ResultCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.buttonSelectAll = new System.Windows.Forms.Button();
            this.buttonselectNone = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.textBoxDuplicatesInfo = new System.Windows.Forms.TextBox();
            this.labelForPercentage = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.buttonPrevious = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonGetInfo
            // 
            this.buttonGetInfo.Location = new System.Drawing.Point(449, 18);
            this.buttonGetInfo.Name = "buttonGetInfo";
            this.buttonGetInfo.Size = new System.Drawing.Size(75, 32);
            this.buttonGetInfo.TabIndex = 1;
            this.buttonGetInfo.Text = "Search";
            this.buttonGetInfo.UseVisualStyleBackColor = true;
            this.buttonGetInfo.Click += new System.EventHandler(this.OnGetInfoButtonClick);
            // 
            // textBoxForPath
            // 
            this.textBoxForPath.Location = new System.Drawing.Point(12, 25);
            this.textBoxForPath.Name = "textBoxForPath";
            this.textBoxForPath.Size = new System.Drawing.Size(225, 20);
            this.textBoxForPath.TabIndex = 0;
            // 
            // textBoxForInfo
            // 
            this.textBoxForInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxForInfo.Enabled = false;
            this.textBoxForInfo.Location = new System.Drawing.Point(12, 463);
            this.textBoxForInfo.Multiline = true;
            this.textBoxForInfo.Name = "textBoxForInfo";
            this.textBoxForInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxForInfo.Size = new System.Drawing.Size(349, 34);
            this.textBoxForInfo.TabIndex = 10;
            this.textBoxForInfo.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(272, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Type of files:";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Enabled = false;
            this.buttonCancel.Location = new System.Drawing.Point(532, 18);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 32);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Stop";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.OnCancelButtonClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Please select folder:";
            // 
            // buttonBrowseFolder
            // 
            this.buttonBrowseFolder.Location = new System.Drawing.Point(236, 25);
            this.buttonBrowseFolder.Name = "buttonBrowseFolder";
            this.buttonBrowseFolder.Size = new System.Drawing.Size(30, 20);
            this.buttonBrowseFolder.TabIndex = 8;
            this.buttonBrowseFolder.Text = "...";
            this.buttonBrowseFolder.UseVisualStyleBackColor = true;
            this.buttonBrowseFolder.Click += new System.EventHandler(this.OnBrowseButtonClick);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonDelete.Enabled = false;
            this.buttonDelete.Location = new System.Drawing.Point(224, 309);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 32);
            this.buttonDelete.TabIndex = 9;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.OnDeleteButtonClick);
            // 
            // ResultCheckedListBox
            // 
            this.ResultCheckedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ResultCheckedListBox.CheckOnClick = true;
            this.ResultCheckedListBox.FormattingEnabled = true;
            this.ResultCheckedListBox.HorizontalScrollbar = true;
            this.ResultCheckedListBox.IntegralHeight = false;
            this.ResultCheckedListBox.Location = new System.Drawing.Point(4, 201);
            this.ResultCheckedListBox.Name = "ResultCheckedListBox";
            this.ResultCheckedListBox.Size = new System.Drawing.Size(365, 102);
            this.ResultCheckedListBox.TabIndex = 11;
            this.ResultCheckedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.OnItemCheck);
            this.ResultCheckedListBox.DoubleClick += new System.EventHandler(this.OntemDoubleClick);
            // 
            // buttonSelectAll
            // 
            this.buttonSelectAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonSelectAll.Enabled = false;
            this.buttonSelectAll.Location = new System.Drawing.Point(143, 309);
            this.buttonSelectAll.Name = "buttonSelectAll";
            this.buttonSelectAll.Size = new System.Drawing.Size(75, 32);
            this.buttonSelectAll.TabIndex = 12;
            this.buttonSelectAll.Text = "Select All";
            this.buttonSelectAll.UseVisualStyleBackColor = true;
            this.buttonSelectAll.Click += new System.EventHandler(this.OnSelectAllButtonClick);
            // 
            // buttonselectNone
            // 
            this.buttonselectNone.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonselectNone.Enabled = false;
            this.buttonselectNone.Location = new System.Drawing.Point(62, 309);
            this.buttonselectNone.Name = "buttonselectNone";
            this.buttonselectNone.Size = new System.Drawing.Size(75, 32);
            this.buttonselectNone.TabIndex = 13;
            this.buttonselectNone.Text = "Select None";
            this.buttonselectNone.UseVisualStyleBackColor = true;
            this.buttonselectNone.Click += new System.EventHandler(this.OnSelectNoneButtonClick);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(12, 434);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(715, 23);
            this.progressBar.TabIndex = 14;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "All Files (*)",
            "All Image Files",
            "Bitmap Files (*.bmp)",
            "JPEG (*.jpg, *.jpeg)",
            "GIF (*.gif)",
            "PNG (*.png)",
            "MP3 (*.mp3)",
            "DOC (*.doc, *.docs)"});
            this.comboBox1.Location = new System.Drawing.Point(345, 24);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(96, 21);
            this.comboBox1.TabIndex = 15;
            this.comboBox1.SelectedValueChanged += new System.EventHandler(this.OnComboBoxSelectionCompleted);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.IntegralHeight = false;
            this.listBox1.Location = new System.Drawing.Point(6, 21);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(365, 136);
            this.listBox1.TabIndex = 16;
            this.listBox1.SelectedValueChanged += new System.EventHandler(this.OnSelectedValueChanged);
            // 
            // textBoxDuplicatesInfo
            // 
            this.textBoxDuplicatesInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDuplicatesInfo.Enabled = false;
            this.textBoxDuplicatesInfo.Location = new System.Drawing.Point(378, 463);
            this.textBoxDuplicatesInfo.Multiline = true;
            this.textBoxDuplicatesInfo.Name = "textBoxDuplicatesInfo";
            this.textBoxDuplicatesInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDuplicatesInfo.Size = new System.Drawing.Size(349, 34);
            this.textBoxDuplicatesInfo.TabIndex = 18;
            // 
            // labelForPercentage
            // 
            this.labelForPercentage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelForPercentage.AutoSize = true;
            this.labelForPercentage.Location = new System.Drawing.Point(12, 412);
            this.labelForPercentage.Name = "labelForPercentage";
            this.labelForPercentage.Size = new System.Drawing.Size(15, 13);
            this.labelForPercentage.TabIndex = 20;
            this.labelForPercentage.Text = "%";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(12, 62);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.buttonPrevious);
            this.splitContainer1.Panel1.Controls.Add(this.buttonNext);
            this.splitContainer1.Panel1.Controls.Add(this.buttonDelete);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.listBox1);
            this.splitContainer1.Panel1.Controls.Add(this.ResultCheckedListBox);
            this.splitContainer1.Panel1.Controls.Add(this.buttonselectNone);
            this.splitContainer1.Panel1.Controls.Add(this.buttonSelectAll);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.splitContainer1.Size = new System.Drawing.Size(715, 344);
            this.splitContainer1.SplitterDistance = 378;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 22;
            // 
            // buttonPrevious
            // 
            this.buttonPrevious.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonPrevious.Enabled = false;
            this.buttonPrevious.Location = new System.Drawing.Point(185, 163);
            this.buttonPrevious.Name = "buttonPrevious";
            this.buttonPrevious.Size = new System.Drawing.Size(75, 32);
            this.buttonPrevious.TabIndex = 18;
            this.buttonPrevious.Text = "Previous";
            this.buttonPrevious.UseVisualStyleBackColor = true;
            this.buttonPrevious.Click += new System.EventHandler(this.OnPreviousButtonClick);
            // 
            // buttonNext
            // 
            this.buttonNext.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonNext.Enabled = false;
            this.buttonNext.Location = new System.Drawing.Point(104, 163);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(75, 32);
            this.buttonNext.TabIndex = 17;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.OnNextButtonClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Search results:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(739, 509);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.textBoxDuplicatesInfo);
            this.Controls.Add(this.textBoxForInfo);
            this.Controls.Add(this.labelForPercentage);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonBrowseFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxForPath);
            this.Controls.Add(this.buttonGetInfo);
            this.MinimumSize = new System.Drawing.Size(755, 539);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Delete duplicated files";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonGetInfo;
        private System.Windows.Forms.TextBox textBoxForPath;
        private System.Windows.Forms.TextBox textBoxForInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonBrowseFolder;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.CheckedListBox ResultCheckedListBox;
        private System.Windows.Forms.Button buttonSelectAll;
        private System.Windows.Forms.Button buttonselectNone;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox textBoxDuplicatesInfo;
        private System.Windows.Forms.Label labelForPercentage;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonPrevious;
        private System.Windows.Forms.Button buttonNext;
    }
}

