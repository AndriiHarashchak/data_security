using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace lab4_ds_cs
{
    internal class Encryptor
    {
        public void EncryptFileRSA(string filename, string? RsaParamsFilePath, string? exportPublicParamsFilePath, string? exportAllParamsFilePath)
        {
            if (File.Exists(filename))
            {
                byte [] data = File.ReadAllBytes(filename);
                RSAParameters? parameters = null;
                if (RsaParamsFilePath != null)
                {
                    //RSAParameters parameters = new RSAParameters();
                    bool ok = DeserializeFromFile(RsaParamsFilePath, out parameters);
                    if (!ok) { throw new Exception("Can`t import params"); }
                    
                }
                //byte [] encryptedData = EncryptRSA(data, parameters);
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                if(parameters != null)
                {
                    RSA.ImportParameters((RSAParameters)parameters);
                }


                //var encryptedData = RSA.Encrypt(data, false);
                var encryptedData = encrypt(data, RSA);


                string path = GetFilePath(filename, "_enc_rsa");
                File.WriteAllBytes(path, encryptedData);
                if(exportAllParamsFilePath != null)
                {
                    var par = RSA.ExportParameters(true);
                    DateTime date = DateTime.Now;
                    SerializeToFile(par, exportAllParamsFilePath + "/exportPrivate.txt");
                }
                if (exportPublicParamsFilePath != null)
                {
                    var par = RSA.ExportParameters(false);
                    DateTime date = DateTime.Now;
                    SerializeToFile(par, exportAllParamsFilePath + "/exportPublic.txt");
                }
            }
            else
            {
                throw new Exception("Invalid filemame");
            }
        }

        
        public void DecryptFileRSA(string filename, string? importAllParamsFilePath)
        {
            if (File.Exists(filename))
            {
                RSAParameters? parameters = null;
                bool ok = true; ;
                if(importAllParamsFilePath!= null)
                {
                    ok = DeserializeFromFile(importAllParamsFilePath, out parameters);
                }
                byte[] data = File.ReadAllBytes(filename);

                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                if(parameters != null)
                {
                    RSA.ImportParameters((RSAParameters)parameters);
                }


                //var decryptedData = RSA.Decrypt(data, false);
                var decryptedData = decrypt(data, RSA);


                //byte[] DecryptedData = DecryptRSA(data);
                string path = GetFilePath(filename, "_dec");
                File.WriteAllBytes(path, decryptedData);
            }
            else
            {
                throw new Exception("Invalid filemame");
            }
        }
        
        public bool SerializeToFile(RSAParameters parameters, string filename)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(parameters);
            File.WriteAllText(filename, json);
           return true;
        }
        public bool DeserializeFromFile(string filename, out RSAParameters? parameters)
        {
            //var json = JsonSerializer.Serialize(parameters);
            if (File.Exists(filename))
            {
                string text = File.ReadAllText(filename);
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<RSAParameters>(text);
                parameters = result;
                return true;
            }
            parameters = null;
            return false;
        }

        private byte[] encrypt(byte[] data, RSACryptoServiceProvider RSA)
        {
            int encryptBlockSize = 64;
            var encryptedBytes = new List<byte>
            {
                Capacity = Math.Max(data.Length * 2, encryptBlockSize)
            };
            for (int i = 0; i < data.Length; i += encryptBlockSize)
            {
                var inputBlock = data
                    .Skip(i)
                    .Take(encryptBlockSize)
                    .ToArray();

                encryptedBytes.AddRange(RSA.Encrypt(
                    inputBlock,
                    fOAEP: false));
            }
            return encryptedBytes.ToArray();
        }
        private byte[] decrypt(byte[] data, RSACryptoServiceProvider RSA)
        {
            var decryptedBytes = new List<byte>
            {
                Capacity = data.Length / 2
            };
            for (int i = 0; i < data.Length; i += 128)
            {
                var inputBlock = data
                    .Skip(i)
                    .Take(128)
                    .ToArray();

                decryptedBytes.AddRange(RSA.Decrypt(
                    inputBlock,
                    fOAEP: false));
            }
            return decryptedBytes.ToArray();
        }
        //private byte[] EncryptRSA(byte[] data)
        //{
        //    RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
        //    //RSAParameters RSAKeyInfo = new RSAParameters();
        //    //RSAKeyInfo.Exponent =  new byte []{ 0,2,2};
        //    //RSAKeyInfo.Modulus = key;
        //    //RSA.ImportParameters(RSAKeyInfo);

        //    var encryptedData = RSA.Encrypt(data, false);
        //    return encryptedData;

        //}
        //private byte[] DecryptRSA(byte[] data)
        //{
        //    RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
        //    //RSAParameters RSAKeyInfo = new RSAParameters();
        //    //RSAKeyInfo.Exponent = new byte[] { 0, 2, 2 };
        //    //RSAKeyInfo.Modulus = key;
        //    //RSA.ImportParameters(RSAKeyInfo);
        //    var decryptedData = RSA.Decrypt(data, false);
        //    return decryptedData;
        //}
        private string GetFilePath(string filename, string expanding)
        {
            string? dir = Path.GetDirectoryName(filename);
            string ext = Path.GetExtension(filename);
            string filename2 = Path.GetFileNameWithoutExtension(filename);
            if (dir == null)
            {
                dir = "./";
            }
            string path = Path.Combine(dir, filename2 + expanding + ext);
            return path;
        }
    }
}
