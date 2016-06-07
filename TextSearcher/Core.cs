using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TextSearcher
{
    public class Core
    {
        public static List<DirectoryInfo> GetDirList(string dirPath, List<string> ignoreDir)
        {
            var dir = new DirectoryInfo(dirPath);
            var dirList = dir.GetDirectories().ToList();
            for (var i = 0; i < dirList.Count; i++)
            {
                if (ignoreDir.Any(x => string.Equals(dirList[i].Name, x, StringComparison.CurrentCultureIgnoreCase)))
                {
                    dirList.Remove(dirList[i]);
                }
            }
            var newDirList = new List<DirectoryInfo>();
            if (dirList.Count == 0)
            {
                return newDirList;
            }
            else
            {
                newDirList.AddRange(dirList);
                foreach (var dirInfo in dirList)
                {
                    newDirList.AddRange(GetDirList(dirInfo.FullName, ignoreDir));
                }
            }
            return newDirList;
        }



    }
}
