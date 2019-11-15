using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Abp.Extensions;

namespace MF
{
    public static class ChineseDigit
    {
        /// <summary>
        /// 1万以内中文转数字
        /// </summary>
        /// <param name="src">源文本如：四千三百二十一</param>
        /// <returns></returns>
        public static int Convert2Number(this string src)
        {
            // 定义包含所有数字的字符串，用以判断字符是否为数字。
            string numberString = "零一二三四五六七八九";
            // 定义单位字符串，用以判断字符是否为单位。
            string unitString = "零十百千";
            // 把数字字符串转换为char数组，方便截取。
            char[] charArr = src.Replace(" ", "").ToCharArray();
            // 返回结果
            int result = 0;
            // 如果源为空指针、空字符串、空白字符串 则返回0
            if (string.IsNullOrEmpty(src) || string.IsNullOrWhiteSpace(src))
            {
                return 0;
            }
            // 如果源的第一个字符不是数字 则返回0
            if (numberString.IndexOf(charArr[0]) == -1)
            {
                return 0;
            }
            // 遍历字符数组
            for (int i = 0; i < charArr.Length; i++)
            {
                // 遍历单位字符串
                for (int j = 0; j < unitString.Length; j++)
                {
                    // 如果字符为单位则进行计算
                    if (charArr[i] == unitString[j])
                    {
                        // 如果字符为非'零'字符，则计算出十位以上到万位以下数字的和
                        if (charArr[i] != '零')
                        {
                            result += Convert.ToInt32(int.Parse(numberString.IndexOf(charArr[i - 1]).ToString()) * Math.Pow(10, j));
                        }
                    }
                }
            }
            // 如果源文本末尾字符为'零'-'九'其中之一，则计算结果和个位数相加。
            if (numberString.IndexOf(charArr[charArr.Length - 1]) != -1)
            {
                result += numberString.IndexOf(charArr[charArr.Length - 1]);
            }
            // 返回计算结果。
            return result;
        }



        /// <summary>
        /// 1万以内中文数字替换为阿拉伯数字
        /// </summary>
        /// <param name="src">源文本如：第四千三百二十一章</param>
        /// <returns>第4321章</returns>
        public static string ChineseDigitReplaceNumber(this string str)
        {
            var chars = str.ToCharArray();
            var i = 0;
            while (i < chars.Length)
            {
                if (IsChineseDigit(chars[i]))
                {
                    var chineseDigit = "";
                    var start = i;
                    while (i < chars.Length && IsChineseDigit(chars[i]))
                    {
                        chineseDigit += chars[i];
                        i++;
                    }
                    var end = i;
                    var digit = Convert2Number(chineseDigit);
                    if (digit != 0 || digit == 0 && chineseDigit == "零") // 正确转换
                    {
                        var s = chars.Aggregate("", (x, y) => x.ToString() + y.ToString());
                        chars = (ReplaceNumber(s, start, end, "" + digit)).ToCharArray();
                        i = start;
                    }
                }
                else
                {
                    i++;
                }
            }
            return chars.Aggregate("", (x, y) => x.ToString() + y.ToString());
        }
        public static bool IsChineseDigit(char c)
        {
            var numberString = "零一二三四五六七八九十百千";
            return numberString.Contains(c.ToString());
        }

        public static string ReplaceNumber(string str, int start, int end, string replaceStr)
        {
            var s1 = str.Substring(0, start);
            var s2 = str.Substring(end);
            return s1 + replaceStr + s2;
        }
    }
}
