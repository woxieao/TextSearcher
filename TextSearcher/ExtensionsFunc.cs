using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TextSearcher
{
    public static class BoxExtensions
    {
        public static void OverrideValue(this TextBox text, List<string> dataList, int index)
        {
            text.Text = dataList[index];
        }
    }
}
