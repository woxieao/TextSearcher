using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace TextSearcher
{
    public class SearchInfo
    {
        private const string SaveFileName = "SearchInfo.config";
        private static List<string> Deserialize(string str)
        {
            try
            {
                return JsonConvert.DeserializeObject<List<string>>(str);
            }
            catch
            {
                return new List<string>();
            }
        }
        public SearchInfo()
        {
       
        }
        public void SaveInfo()
        {
            var file = new FileStream(SaveFileName, FileMode.Create);
            using (var stream = new StreamWriter(file))
            {
                ; stream.Write(JsonConvert.SerializeObject(this));
            }
        }
        public SearchInfo LoadInfo()
        {
            var file = new FileInfo(SaveFileName);
            if (file.Exists)
            {
                using (var stream = new StreamReader(file.FullName))
                {
                    var info = stream.ReadToEnd();
                    return JsonConvert.DeserializeObject<SearchInfo>(info);
                }
            }
            else
            {
                return new SearchInfo
                {
                    FileTypeList = { "*.js", "*.cs", "*.config" },
                    IgnoreDirList = { ".vs", ".git", "packages", "bin", "obj" },
                    DirPath = @"D:\"
                };
            }

        }

        public string DirPath { get; private set; }

        public List<string> FileTypeList { get; private set; }

        public List<string> IgnoreDirList { get; private set; }
    }
}
