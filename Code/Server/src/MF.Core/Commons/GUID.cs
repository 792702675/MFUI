using Abp.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Commons
{
    public static class GUID
    {
        private static string CodeSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        /// <summary>
        /// 获得Int64的随机ID
        /// </summary>
        /// <returns></returns>
        public static long GuidToLongID()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// 获得不重复的ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="IDSet">已有的ID集合</param>
        /// <param name="getId">取得ID的Fun</param>
        /// <returns></returns>
        public static T GetID<T>(IEnumerable<T> IDSet, Func<T> getId)
        {
            return GetID(x => IDSet.Contains(x), getId);
        }
        /// <summary>
        /// 获得不重复的ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="IsExist">ID是否存在的判断</param>
        /// <param name="getId">取得ID的Fun</param>
        /// <returns></returns>
        public static T GetID<T>(Func<T, bool> IsExist, Func<T> getId)
        {
            T id = default(T);
            do
            {
                id = getId();
            } while ( IsExist(id));
            return id;
        }
        /// <summary>
        /// 获得不重复的ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="IsExist">ID是否存在的判断</param>
        /// <param name="getId">取得ID的Fun</param>
        /// <returns></returns>
        public static async Task<T> GetIDAsync<T>(Func<T, Task<bool>> IsExist, Func<T> getId)
        {
            T id = default(T);
            do
            {
                id = getId();
            } while (await IsExist(id));
            return id;
        }

        /// <summary>
        /// 将long类型的值编码为字符型
        /// </summary>
        public static string To64(long value)
        {
            int jinzhi = CodeSet.Length; //64;
            long _value = value;
            int yushu = 0;
            var cl64 = new List<int>();
            do
            {
                yushu = (int)(_value % jinzhi);
                _value = _value / jinzhi;
                cl64.Add(yushu);
            }
            while (_value > 0);
            cl64.Reverse();
            var r = cl64.Select(x => CodeSet[x]);
            var rs = "";
            foreach (var item in r)
            {
                rs += item;
            }
            return rs;
        }
    }
}
