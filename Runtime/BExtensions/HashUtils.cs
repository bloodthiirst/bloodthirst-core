using System;
using System.Security.Cryptography;
using System.Text;

namespace Bloodthirst.Utils
{
    public static class HashUtils
    {
        public static uint StringToHash(this string value)
        {
            MD5 md5Hasher = MD5.Create();

            byte[] hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(value));

            md5Hasher.Dispose();

            return BitConverter.ToUInt32(hashed, 0);
        }

        public static string IntToString(uint number)
        {
            string val = number.ToString();

            for (int i = 0; i < 10 - val.Length; i++)
            {
                val = "0" + val;
            }

            return val;
        }
    }
}
