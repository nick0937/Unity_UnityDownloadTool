using System;

namespace yoyohan
{
    /// <summary>
    /// 描述：
    /// 功能：
    /// 作者：yoyohan
    /// 创建时间：2019-05-13 10:50:51
    /// </summary>
    public class Util
    {
        /// <summary>
        /// 转换方法
        /// </summary>
        /// <param name="size">字节值</param>
        /// <returns></returns>
        public static string HumanReadableFilesize(double size)
        {
            String[] units = new String[] { "B", "KB", "MB", "GB", "TB", "PB" };
            double mod = 1024.0;
            int i = 0;
            while (size >= mod)
            {
                size /= mod;
                i++;
            }
            int littleNum = 0;//取几位小数
            if (i == 0 || i == 1)
                littleNum = 0;
            else
                littleNum = 2;

            return Math.Round(size, littleNum) + units[i];
        }
    }
}

