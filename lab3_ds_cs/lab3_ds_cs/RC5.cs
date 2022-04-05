using System;
using System.Collections.Generic;
using System.Linq;

namespace lab3_ds_cs
{
    enum OperationType
    {
        Encrypt, Decrypt
    }
    public class RC5
    {
        public int rounds { get; private set; }
        public int wordSize { get; private set; }

        private UInt16 pw = 0xB7E1;
        public UInt16 Pw { get => pw; }

        private UInt16 qw = 0x9E37;
        public UInt16 Qw { get => qw; }
        
        List<Int32> s;
        public RC5(int roundsCount)
        {
            wordSize = 16;
            if(roundsCount> 0 && roundsCount < 256)
            {
                rounds = roundsCount;
            }
            else
            {
                throw new ArgumentOutOfRangeException(" rounds count must be greater than 0 and less that 256");
            }
        }

        public UInt16[] generateExtendedKeys(byte[] key)
        {
            int u = wordSize / 8;
            int b = key.Length;
            int c = b % u > 0 ? b / u +1 : b / u;
            int[] l = new int[c];
            
            for (int r = b - 1; r >= 0; r--) {
                l[r / u] = (int)ROL((ushort)l[r / u], 8) + key[r];
            }
            int size = 2 * rounds + 2;
            UInt16[] s = new UInt16[size];
            s[0] = Pw;
            for(int r = 1; r < size; r++)
            {
                s[r] = (ushort)(s[r - 1] + Qw);
            }
            UInt16 A =0, B = 0;
            int i=0, j = 0;
            int n = 3 * Math.Max(size, c);
            for (int k = 0; k< n; k++)
            {
                UInt16 sum = (ushort)(s[i] + A + B);
                s[i] = (ushort)ROL(sum, 3);
                A = s[i];
                l[j] = (int)ROL((ushort)(l[j] + A + B), (int)(A + B));
                B = (UInt16)l[j];
                i = (i + 1) % size;
                j = (j + 1) % c;

            }
            return s;
        }

        public byte[] encryptCBCPAD(byte[] data, byte[] key)
        {
            var padding = getPadding(data);
            var s = generateExtendedKeys(key);
            var extendedArray = new byte[data.Length + padding.Length];
            Array.Copy(data, 0, extendedArray,0, data.Length);
            Array.Copy(padding, 0, extendedArray, data.Length, padding.Length);
            var generator = new PseudoGenerator();
            generator.X = 31;
            generator.A = (ulong)Math.Pow(7, 5);
            generator.M = (ulong)Math.Pow(2, 31) - 1;
            generator.C = 17711;
            var Pprev = generator.getRandomNumbers();

            var resultArray = new Byte[extendedArray.Length+ Pprev.Length];
            var block = encryptECB(Pprev, s);
            
            Array.Copy(block, 0, resultArray, 0, block.Length);
            int bytesPerBlock = wordSize * 2 / 8;
            for (int i = 0; i < extendedArray.Length; i += bytesPerBlock) // 4 - bytes per block
            {
                var cn = new Byte[bytesPerBlock];
                Array.Copy(extendedArray, i, cn, 0, cn.Length);

                cn = XOR(cn, Pprev);

                var result = encryptECB(cn, s);

                Array.Copy(result, 0, resultArray, i + bytesPerBlock, result.Length);

                Array.Copy(resultArray, i + bytesPerBlock, Pprev, 0, cn.Length);
            }

            return resultArray;
        }
       
        public byte[] decryptCBCPAD(byte[] data, byte[] key)
        {
            var bytesPerBlock = wordSize*2/8;
            var s = generateExtendedKeys(key);
            //var cnPrev = new Byte[bytesPerBlock];
            var decodedFileContent = new Byte[data.Length - bytesPerBlock];
            var cnPrev = decryptECB(data, s);

            for (int i = bytesPerBlock; i < data.Length; i += bytesPerBlock)
            {
                var cn = new Byte[bytesPerBlock];
                Array.Copy(data, i, cn, 0, cn.Length);
                
                var block = decryptECB(cn, s);

                block = XOR(block, cnPrev);
                
                Array.Copy(block, 0, decodedFileContent, i-bytesPerBlock, block.Length);
                Array.Copy(data, i, cnPrev, 0, cnPrev.Length);
            }
            byte[] decodedWithoutPadding = new byte[] { };
            if (decodedFileContent.Last() <  decodedFileContent.Length)
                decodedWithoutPadding = new Byte[decodedFileContent.Length - decodedFileContent.Last()];
            else
            {
                decodedWithoutPadding = new Byte[decodedFileContent.Length];
            }
            Array.Copy(decodedFileContent, decodedWithoutPadding, decodedWithoutPadding.Length);

            return decodedWithoutPadding;
        }
       
        public byte[] XOR(byte[] left, byte[] right)
        {
            for (int i = 0; i < left.Length; ++i)
            {
                left[i] ^= right[i];
            }
            return left;
        }
        public byte[] encryptECB(byte[] data, UInt16[] key)
        {
            var a = BitConverter.ToUInt16(data, 0);
            var b = BitConverter.ToUInt16(data, 2);
            //var _s = generateExtendedKeys(key);
            var _s = key;
            a = (ushort)(a + _s[0]);
            b = (ushort)(b + _s[1]);
            for (int i = 1; i < rounds + 1; i++)
            {
                a = (ushort)(ROL((ushort)(a ^ b), (int)b) + _s[2 * i]);
                b = (ushort)(ROL((ushort)(b ^ a), (int)a) + _s[2 * i + 1]);
            }
            //byte outputData = Uint8List(inputData.length)..buffer.asByteData().setInt16(0, a, Endian.little)..buffer.asByteData().setInt16(2, b, Endian.little);
            byte[] outputData1 = BitConverter.GetBytes(a);
            byte[] outputData2 =  BitConverter.GetBytes(b);
            outputData1 = outputData1.Concat(outputData2).ToArray();
            return outputData1;
        }
        public byte[] decryptECB(byte[] data, UInt16[] key)
        {
            UInt16 a = BitConverter.ToUInt16(data, 0);
            UInt16 b = BitConverter.ToUInt16(data, 2);
            var _s = key;

            for (int i = rounds; i > 0; i--)
            {
                b = (ushort)(ROR((ushort)(b - _s[2 * i + 1]), (Int32)a) ^ a);
                a = (ushort)(ROR((ushort)(a - _s[2 * i]), (Int32)b) ^ b);
            }
            a = (ushort)(a - _s[0]);
            b = (ushort)(b - _s[1]);

            byte[] outputData1 = BitConverter.GetBytes((UInt16)a);
            byte[] outputData2 = BitConverter.GetBytes((UInt16)b);
            outputData1 = outputData1.Concat(outputData2).ToArray();
            return outputData1;
        }
        public byte[] appendBlock(byte[] data)
        {
            return data;
        }
        UInt16 ROL(UInt16 num, Int32 offset)
        {
            offset %= 2; //2 bytes in word
            var res = (UInt16) ((num << offset) | (num >> (wordSize - offset)));
            return res;
        }
        UInt16 ROR(UInt16 num, Int32 offset)
        {
            offset %= 2;// 2 bytes in word
            var res = (UInt16)((num >> offset) | (num << (wordSize - offset)));
            return res;
        }
        private byte[] getPadding(byte[] data)
        {
            var bytesInBlock = wordSize*2 / 8;
            var paddingLength =  bytesInBlock - data.Length % (bytesInBlock);
            if (paddingLength < 0)
            {
                paddingLength = bytesInBlock;
            }
            var padding = new byte[paddingLength];

            for (int i = 0; i < padding.Length; ++i)
            {
                padding[i] = (byte)paddingLength;
            }

            return padding;
        }
    }
}
