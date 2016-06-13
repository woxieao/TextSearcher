using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TextSearcher
{

    public partial class Form1 : Form
    {

        private static bool _caseSensitive = false;
        private static bool _textOrFileName = true;
        private const string SaveFileName = "SearchInfo.config";
        private static Thread _thread;

        public Form1()
        {
            InitializeComponent();
            LoadData();
        }

        public void CallInMainThread(Action act)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Action>(CallInMainThread), act);
            }
            else
            {
                act();
            }
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
                try
                {
                    dirPathBox.OverrideValue(dataList, 0);
                    fileTypeBox.OverrideValue(dataList, 1);
                    ignoreDirBox.OverrideValue(dataList, 2);
                }
                catch
                {
                    return;
                }
            }
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


        private string[] GetFileTypeList()
        {
            var defaultValue = "[\"txt\",\"cs\",\"defaultFileType\"]";
            try
            {
                var fileTypeList = JsonConvert.DeserializeObject<string[]>(fileTypeBox.Text);
                fileTypeList = fileTypeList.Length == 0 ? new[] { "*" } : fileTypeList;
                for (var i = 0; i < fileTypeList.Length; i++)
                {
                    var type = fileTypeList[i].Trim();
                    type = string.IsNullOrWhiteSpace(type) ? "*" : type;
                    if (type == "*")
                        return new[] { "*" };
                    fileTypeList[i] = $"*.{type}";
                }
                return fileTypeList;
            }
            catch
            {
                fileTypeBox.Text = defaultValue;
                new Thread(() => MessageBox.Show(@"Deserialize fileTypeList Failed!")).Start();
                return JsonConvert.DeserializeObject<string[]>(defaultValue);
            }
        }

        private string[] GetDirList()
        {
            var defaultValue = "[\".vs\",\".git\",\"packages\",\"bin\",\"obj\"]";
            try
            {
                var ignoreDirList = JsonConvert.DeserializeObject<string[]>(ignoreDirBox.Text);
                return ignoreDirList;
            }
            catch
            {
                ignoreDirBox.Text = defaultValue;
                new Thread(() => MessageBox.Show(@"Deserialize ignoreDirList Failed!")).Start();
                return JsonConvert.DeserializeObject<string[]>(defaultValue);
            }
        }



        private void Search()
        {
            CallInMainThread(() => fileListBox.Items.Clear());
            var searchResult = new List<string>();
            try
            {
                var dirStrList = new List<string>();
                CallInMainThread(() => fileListBox.Items.Clear());
                CallInMainThread(() => dirStrList = (GetDirList() ?? new string[] { }).ToList());
                var dirList = Core.GetDirList(dirPathBox.Text, dirStrList);
                var filePathList = new List<FileInfo>();
                var fileTypeList = new List<string>();
                CallInMainThread(() => { fileTypeList = GetFileTypeList().ToList(); });

                if (_textOrFileName)
                {
                    foreach (var dir in dirList)
                    {
                        foreach (var fileType in fileTypeList)
                        {
                            filePathList.AddRange(dir.GetFiles(fileType));
                        }
                    }
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
                                searchResult.Add(filePath.FullName);
                                CallInMainThread(() =>
                                {
                                    fileListBox.Items.Add(filePath.FullName);
                                    msgLabel.Text = $"Searching({searchResult.Count})...";
                                });
                            }
                        }
                    }
                }
                else
                {
                    foreach (var dir in dirList)
                    {
                        filePathList.AddRange(dir.GetFiles());
                    }
                    foreach (var filePath in filePathList)
                    {
                        var fileName = filePath.FullName;
                        var keywords = keywordsBox.Text ?? string.Empty;
                        if (fileName.IndexOf(keywords, _caseSensitive
                            ? StringComparison.CurrentCulture
                            : StringComparison.CurrentCultureIgnoreCase) != -1)
                        {
                            searchResult.Add(filePath.FullName);
                            CallInMainThread(() =>
                            {
                                fileListBox.Items.Add(filePath.FullName);
                                msgLabel.Text = $"Searching({searchResult.Count})...";
                            });
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                CallInMainThread(() =>
                {
                    CallInMainThread(() => fileListBox.Items.Clear());
                    fileListBox.Items.AddRange(searchResult.OrderBy(i => i).ToArray());
                    searchBtn.Text = @"&Search";
                    msgLabel.Text = $"Done!({searchResult.Count})";
                });
            }
        }



        private void searchHandler()
        {
            _thread = _thread ?? new Thread(Search);
            if (_thread.IsAlive)
            {
                searchBtn.Text = @"&Search";
                _thread.Abort("Search Cancelled!");
            }
            else
            {
                _thread = new Thread(Search);
                msgLabel.Text = @"Searching...";
                searchBtn.Text = @"&Abort";
                _thread.Start();
            }
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
            ProcessStartInfo info = new ProcessStartInfo(fileListBox.Text);
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
                        var filePath = new FileInfo(fileListBox.Text).DirectoryName; ;
                        ProcessStartInfo info = new ProcessStartInfo(filePath);
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

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {


            ProcessStartInfo info = new ProcessStartInfo(fileListBox.Text);
            Process.Start(info);
        }



        private void chooseDirBtn_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                dirPathBox.Text = fbd.SelectedPath;
            }
        }
    }
}

