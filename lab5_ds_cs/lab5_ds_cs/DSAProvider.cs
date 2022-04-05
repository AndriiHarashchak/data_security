using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace lab5_ds_cs
{
    class DSAProvider
    {
        DSACryptoServiceProvider provider { get; }
        public DSAProvider(DSAParameters? parameters)
        {
            provider = new DSACryptoServiceProvider();
            if (parameters != null)
            {
                provider.ImportParameters((DSAParameters)parameters);
            }
        }
        public byte[] GenerateEDS(byte[] data)
        {
            SHA1 sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(data);
            byte[] signature = provider.CreateSignature(hash);
            return signature;
        }
        public byte[] GenerateEDSForFile(string filepath)
        {
            try
            {
                if (File.Exists(filepath))
                {
                    byte[] data = File.ReadAllBytes(filepath);
                    SHA1 sha1 = SHA1.Create();
                    var hash = sha1.ComputeHash(data);
                    byte[] signature = provider.CreateSignature(hash);
                    return signature;
                }
            }
            catch (Exception e)
            {

                throw e;
            }

            return new byte[] { };
        }

        public bool VerifyEDS(byte[] EDS, byte[] data, DSAParameters publicParameters)
        {
            provider.ImportParameters(publicParameters);
            SHA1 sha1 = SHA1.Create();
            byte[] hash = sha1.ComputeHash(data);
            bool ok = provider.VerifySignature(hash, EDS);
            //var dataEDS = GenerateEDS(data);
            return ok;
        }
        public DSAParameters GetParameters(bool includePrivate)
        {
            return provider.ExportParameters(includePrivate);
        }
        public bool ImportParameters(DSAParameters parameters)
        {
            provider.ImportParameters(parameters);
            return true;
        }
    }
}
