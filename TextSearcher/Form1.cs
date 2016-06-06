using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TextSearcher
{
    public partial class Form1 : Form
    {

        private static bool _caseSensitive = false;
        private static bool _textOrFileName = true;
        public Form1()
        {
            InitializeComponent();
        }
        private void Search()
        {
            try
            {
                msgLabel.Text = @"Searching...";
                var showBox = fileListBox.Items;
                showBox.Clear();
                var dirList = Core.GetDirList(dirPathBox.Text, ignoreDirBox.Text.Split(',').Select(i => i.Trim()).ToList());
                var filePathList = new List<FileInfo>();
                var result = new List<string>();
                foreach (var dir in dirList)
                {
                    foreach (var fileType in fileTypeBox.Text.Split(',').Select(i => i.Trim()).ToList())
                    {
                        filePathList.AddRange(dir.GetFiles(fileType));
                    }
                }

                if (_textOrFileName)
                {
                    foreach (var filePath in filePathList)
                    {
                        using (var stream = new StreamReader(filePath.FullName))
                        {
                            var text = stream.ReadToEnd();
                            var keywords = keywordsBox.Text ?? string.Empty;
                            if (!string.IsNullOrWhiteSpace(keywords)
                                && text.IndexOf(keywords, _caseSensitive
                                    ? StringComparison.CurrentCulture
                                    : StringComparison.CurrentCultureIgnoreCase) != -1)
                            {
                                result.Add(filePath.FullName);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var filePath in filePathList)
                    {
                        var fileName = filePath.FullName;
                        var keywords = keywordsBox.Text ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(keywords)
                            && fileName.IndexOf(keywords, _caseSensitive
                                ? StringComparison.CurrentCulture
                                : StringComparison.CurrentCultureIgnoreCase) != -1)
                            result.Add(filePath.FullName);
                    }
                }
                showBox.AddRange(result.OrderByDescending(i => i).ToArray());
                msgLabel.Text = $"Count:{showBox.Count}";
            }
            catch (Exception ex)
            {
                msgLabel.Text = ex.ToString();
            }
        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void caseSensitiveChk_CheckedChanged(object sender, EventArgs e)
        {
            _caseSensitive = caseSensitiveChk.Checked;
        }

        private void fileListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var filePath = new FileInfo(fileListBox.Text).DirectoryName; ;
            ProcessStartInfo info = new ProcessStartInfo(filePath);
            Process.Start(info);
        }

        private void keywordsBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Search();
            }
        }

        private void fileListBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    {
                        ProcessStartInfo info = new ProcessStartInfo(fileListBox.Text);
                        Process.Start(info);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void textOrFileNameBtn_Click(object sender, EventArgs e)
        {
            _textOrFileName = !_textOrFileName;
            textOrFileNameBtn.Text = _textOrFileName ? "Text" : "FileName";
        }
    }
}
