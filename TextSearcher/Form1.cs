using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextSearcher
{

    public partial class Form1 : Form
    {

        private static bool _caseSensitive = false;
        private static bool _textOrFileName = true;
        private static bool _inSearch = false;
        private static bool _needReload = false;
        private const string SaveFileName = "SearchInfo.config";
        private Thread _thread;
        private static List<string> _searchResult = new List<string>();

        public Form1()
        {
            InitializeComponent();
            LoadData();
        }

        #region Data handler
        private void LoadData()
        {
            var file = new FileInfo(SaveFileName);
            var dataList = new List<string>();
            if (file.Exists)
            {
                using (var stream = new StreamReader(file.FullName))
                {
                    while (!stream.EndOfStream)
                    {
                        dataList.Add(stream.ReadLine());
                    }
                }
            }
            dirPathBox.OverrideValue(dataList, 0);
            fileTypeBox.OverrideValue(dataList, 1);
            ignoreDirBox.OverrideValue(dataList, 2);
        }

        private void SaveData()
        {
            var dataList = new List<string> { dirPathBox.Text, fileTypeBox.Text, ignoreDirBox.Text };
            using (var file = new FileStream(SaveFileName, FileMode.Create))
            {
                using (var streamWriter = new StreamWriter(file))
                {
                    foreach (var data in dataList)
                    {
                        streamWriter.WriteLine(data);
                    }
                }
            }
        }
        #endregion

        private void Search()
        {
            _searchResult = new List<string>();
            try
            {
                var dirList = Core.GetDirList(dirPathBox.Text, ignoreDirBox.Text.Split(',').Select(i => i.Trim()).ToList());
                var filePathList = new List<FileInfo>();

                var fileTypeList = fileTypeBox.Text.Split(',').Select(i => i.Trim()).ToList();
                foreach (var dir in dirList)
                {
                    foreach (var fileType in fileTypeList)
                    {
                        filePathList.AddRange(dir.GetFiles(string.IsNullOrWhiteSpace(fileType) ? "*" : fileType));
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
                            if (text.IndexOf(keywords, _caseSensitive
                                    ? StringComparison.CurrentCulture
                                    : StringComparison.CurrentCultureIgnoreCase) != -1)
                            {
                                _searchResult.Add(filePath.FullName);
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
                            _searchResult.Add(filePath.FullName);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            _needReload = true;
        }



        private void searchHandler()
        {
            _thread = new Thread(Search);
            msgLabel.Text = @"Searching...";
            fileListBox.Items.Clear();
            _thread.Start();
        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            searchHandler();
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
                searchHandler();
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveData();
        }

        private void abortBtn_Click(object sender, EventArgs e)
        {
            if (_inSearch && _thread.IsAlive)
            {
                _thread.Abort("Search cancelled");
                fileListBox.Items.AddRange(_searchResult.OrderBy(i => i).ToArray());
                _needReload = false;
                _inSearch = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_thread != null && !_thread.IsAlive && _needReload)
            {
                fileListBox.Items.AddRange(_searchResult.OrderBy(i => i).ToArray());
                msgLabel.Text = $"Count:{  fileListBox.Items.Count}";
                _needReload = false;
            }
        }
    }
}

