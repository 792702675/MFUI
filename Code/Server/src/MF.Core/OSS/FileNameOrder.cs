using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MF.OSS
{
    public class FileNameComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == null || y == null)
            {
                return string.Compare(x, y);
            }

            var arr1 = x.ChineseDigitReplaceNumber().ToCharArray();
            var arr2 = y.ChineseDigitReplaceNumber().ToCharArray();

            var i = 0;
            var j = 0;

            while (i < arr1.Length && j < arr2.Length)
            {
                if (char.IsDigit(arr1[i]) && char.IsDigit(arr2[j]))
                {
                    string s1 = "", s2 = "";
                    while (i < arr1.Length && char.IsDigit(arr1[i]))
                    {
                        s1 += arr1[i];
                        i++;
                    }
                    while (j < arr2.Length && char.IsDigit(arr2[j]))
                    {
                        s2 += arr2[j];
                        j++;
                    }
                    if (int.Parse(s1) > int.Parse(s2))
                    {
                        return 1;
                    }
                    if (int.Parse(s1) < int.Parse(s2))
                    {
                        return -1;
                    }
                }
                else
                {
                    var csa = arr1[i].ToString();
                    var csb = arr2[j].ToString();
                    if (csa == csb)
                    {
                        i++;
                        j++;
                    }
                    else
                    {
                        return Comparer<string>.Default.Compare(csa, csb);
                    }
                }
            }

            if (arr1.Length == arr2.Length)
            {
                return 0;
            }
            else
            {
                return arr1.Length > arr2.Length ? 1 : -1;
            }
        }
    }
}
