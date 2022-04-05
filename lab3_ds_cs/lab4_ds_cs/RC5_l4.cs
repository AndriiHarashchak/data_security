using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using lab3_ds_cs;
namespace lab4_ds_cs
{
    internal class RC5_l4
    {
        public byte[] encryptFile(string filepath, string key)
        {
            //string key = Key.Text;
            var RC5 = new RC5(8);
            var keyEncoded = UTF8Encoding.UTF8.GetBytes(key);
            var md5 = MD5.Create();
            var hashedKey = md5.ComputeHash(keyEncoded);
            byte[] keyArr = new byte[8];
            Array.Copy(hashedKey, 0, keyArr, 0, 8);
            byte[] data = File.ReadAllBytes(filepath);

            byte[] encryptedData = RC5.encryptCBCPAD(data, keyArr);
            return encryptedData;
        }

        public byte[] decryptFile(string filepath, string key)
        {
            //string key = Key.Text;
            var RC5 = new RC5(8);
            var keyEncoded = UTF8Encoding.UTF8.GetBytes(key);
            var md5 = System.Security.Cryptography.MD5.Create();
            var hashedKey = md5.ComputeHash(keyEncoded);
            byte[] keyArr = new byte[8];
            Array.Copy(hashedKey, 0, keyArr, 0, 8);
            byte[] data = File.ReadAllBytes(filepath);

            byte[] decryptedData = RC5.decryptCBCPAD(data, keyArr);
            return decryptedData;
        }
    }
}
