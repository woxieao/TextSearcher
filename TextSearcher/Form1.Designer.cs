namespace TextSearcher
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.keywordsBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.searchBtn = new System.Windows.Forms.Button();
            this.msgLabel = new System.Windows.Forms.Label();
            this.caseSensitiveChk = new System.Windows.Forms.CheckBox();
            this.fileListBox = new System.Windows.Forms.ListBox();
            this.dirPathBox = new System.Windows.Forms.TextBox();
            this.fileTypeBox = new System.Windows.Forms.TextBox();
            this.ignoreDirBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textOrFileNameBtn = new System.Windows.Forms.Button();
            this.chooseDirBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // keywordsBox
            // 
            this.keywordsBox.Location = new System.Drawing.Point(86, 24);
            this.keywordsBox.Name = "keywordsBox";
            this.keywordsBox.Size = new System.Drawing.Size(356, 21);
            this.keywordsBox.TabIndex = 0;
            this.keywordsBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keywordsBox_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Keywords:";
            // 
            // searchBtn
            // 
            this.searchBtn.Location = new System.Drawing.Point(305, 426);
            this.searchBtn.Name = "searchBtn";
            this.searchBtn.Size = new System.Drawing.Size(75, 23);
            this.searchBtn.TabIndex = 2;
            this.searchBtn.Text = "&Search";
            this.searchBtn.UseVisualStyleBackColor = true;
            this.searchBtn.Click += new System.EventHandler(this.searchBtn_Click);
            // 
            // msgLabel
            // 
            this.msgLabel.AutoSize = true;
            this.msgLabel.Location = new System.Drawing.Point(16, 422);
            this.msgLabel.Name = "msgLabel";
            this.msgLabel.Size = new System.Drawing.Size(0, 12);
            this.msgLabel.TabIndex = 4;
            // 
            // caseSensitiveChk
            // 
            this.caseSensitiveChk.AutoSize = true;
            this.caseSensitiveChk.Location = new System.Drawing.Point(448, 26);
            this.caseSensitiveChk.Name = "caseSensitiveChk";
            this.caseSensitiveChk.Size = new System.Drawing.Size(108, 16);
            this.caseSensitiveChk.TabIndex = 5;
            this.caseSensitiveChk.Text = "Case Sensitive";
            this.caseSensitiveChk.UseVisualStyleBackColor = true;
            this.caseSensitiveChk.CheckedChanged += new System.EventHandler(this.caseSensitiveChk_CheckedChanged);
            // 
            // fileListBox
            // 
            this.fileListBox.FormattingEnabled = true;
            this.fileListBox.ItemHeight = 12;
            this.fileListBox.Location = new System.Drawing.Point(18, 138);
            this.fileListBox.Name = "fileListBox";
            this.fileListBox.Size = new System.Drawing.Size(647, 280);
            this.fileListBox.TabIndex = 6;
            this.fileListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fileListBox_KeyDown);
            this.fileListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.fileListBox_MouseDoubleClick);
            // 
            // dirPathBox
            // 
            this.dirPathBox.Location = new System.Drawing.Point(86, 51);
            this.dirPathBox.Name = "dirPathBox";
            this.dirPathBox.Size = new System.Drawing.Size(470, 21);
            this.dirPathBox.TabIndex = 7;
            this.dirPathBox.Text = "C:\\";
            // 
            // fileTypeBox
            // 
            this.fileTypeBox.Location = new System.Drawing.Point(86, 78);
            this.fileTypeBox.Name = "fileTypeBox";
            this.fileTypeBox.Size = new System.Drawing.Size(579, 21);
            this.fileTypeBox.TabIndex = 8;
            this.fileTypeBox.Text = "[\"js\",\"cs\",\"txt\",\"config\"]";
            // 
            // ignoreDirBox
            // 
            this.ignoreDirBox.Location = new System.Drawing.Point(86, 105);
            this.ignoreDirBox.Name = "ignoreDirBox";
            this.ignoreDirBox.Size = new System.Drawing.Size(579, 21);
            this.ignoreDirBox.TabIndex = 9;
            this.ignoreDirBox.Text = "[\".vs\",\".git\",\"packages\",\"bin\",\"obj\"]";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "DirPath:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "FileType:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 12;
            this.label4.Text = "IgnoreDir:";
            // 
            // textOrFileNameBtn
            // 
            this.textOrFileNameBtn.Location = new System.Drawing.Point(562, 22);
            this.textOrFileNameBtn.Name = "textOrFileNameBtn";
            this.textOrFileNameBtn.Size = new System.Drawing.Size(75, 23);
            this.textOrFileNameBtn.TabIndex = 13;
            this.textOrFileNameBtn.Text = "Text";
            this.textOrFileNameBtn.UseVisualStyleBackColor = true;
            this.textOrFileNameBtn.Click += new System.EventHandler(this.textOrFileNameBtn_Click);
            // 
            // chooseDirBtn
            // 
            this.chooseDirBtn.Location = new System.Drawing.Point(562, 51);
            this.chooseDirBtn.Name = "chooseDirBtn";
            this.chooseDirBtn.Size = new System.Drawing.Size(75, 23);
            this.chooseDirBtn.TabIndex = 14;
            this.chooseDirBtn.Text = "ChooseDir";
            this.chooseDirBtn.UseVisualStyleBackColor = true;
            this.chooseDirBtn.Click += new System.EventHandler(this.chooseDirBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 461);
            this.Controls.Add(this.chooseDirBtn);
            this.Controls.Add(this.textOrFileNameBtn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ignoreDirBox);
            this.Controls.Add(this.fileTypeBox);
            this.Controls.Add(this.dirPathBox);
            this.Controls.Add(this.fileListBox);
            this.Controls.Add(this.caseSensitiveChk);
            this.Controls.Add(this.msgLabel);
            this.Controls.Add(this.searchBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.keywordsBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "TextSearcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox keywordsBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button searchBtn;
        private System.Windows.Forms.Label msgLabel;
        private System.Windows.Forms.CheckBox caseSensitiveChk;
        private System.Windows.Forms.ListBox fileListBox;
        private System.Windows.Forms.TextBox dirPathBox;
        private System.Windows.Forms.TextBox fileTypeBox;
        private System.Windows.Forms.TextBox ignoreDirBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button textOrFileNameBtn;
        private System.Windows.Forms.Button chooseDirBtn;
    }
}

