using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3_ds_cs
{
    internal class PseudoGenerator
    {
        public ulong X { get; set; }
        public int C { get; set; }
        public ulong  M {  get; set; }
        public ulong A { get; set;  }
        public byte[] getRandomNumbers()
        {
            var ivParts = new List<Byte[]>();

            /*while (ivParts.Sum(ivp => ivp.Length) < 4) // 4 bytes in block
            {
                ivParts.Add(BitConverter.GetBytes(NextNumber()));
            }
            byte[] result = new byte[0];
            ivParts.ForEach(ivp =>
            {
                result = result.Concat(ivp).ToArray();
            });

            return result;*/
            List<byte> bytes = new List<byte>();
            while(bytes.Count <= 4)
            {
                var byteArr = BitConverter.GetBytes(NextNumber());
                for (int i = 0; i < byteArr.Length; i++)
                {
                    bytes.Add(byteArr[i]);
                }
            }
            bytes = bytes.GetRange(0, 4);
            return bytes.ToArray();
        }

        public ulong NextNumber()
        {
            var next = (A * (ulong)X + (ulong)C) % (ulong)M;
            X = next;
            return next;
        }
    }
}
