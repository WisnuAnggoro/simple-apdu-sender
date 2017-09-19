using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleApduSender
{
    public static class Utility
    {
        public static string RemoveNonHexa(
            string str)
        {
            int i = 0;
            string sRet = string.Empty;

            if (str == string.Empty) return sRet;

            str = str.ToUpper();
            for (i = 0; i < str.Length; i++)
            {
                if ("0123456789ABCDEF".Contains(str[i])) sRet += str[i];
            }

            return sRet;
        }
        public static byte StrByteToByte(string strByte)
        {
            return Convert.ToByte(strByte, 16);
        }

        public static UInt32 StrByteArrayToByteArray(string strByteArray, ref byte[] byteArray)
        {
            int i;
            UInt32 j;
            string sTmp;
            strByteArray = RemoveNonHexa(strByteArray);


            for (i = 0, j = 0; i < strByteArray.Length; i += 2, j++)
            {
                sTmp = strByteArray.Substring(i, 2);
                byteArray[j] = StrByteToByte(sTmp);
            }

            return j;
        }

        public static string ByteArrayToStrByteArray(byte[] byteArray, UInt16 Len)
        {
            string sRet = "";
            string sTmp;

            for (int i = 0; i < Len; i++)
            {
                sTmp = byteArray[i].ToString("X");
                if (sTmp.Length == 1) sTmp = "0" + sTmp;
                sRet += sTmp;
            }

            return sRet;
        }
    }
}
