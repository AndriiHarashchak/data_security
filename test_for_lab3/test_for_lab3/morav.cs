using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_for_lab3
{
    internal class morav
    {
        UInt32 P32 = 0xB7E15162;
        UInt32 Q32 = 0x9E3779B9;
        private byte[] BuildExpandedKeyTable(Byte[] key)
    {

        var keysWordArrLength = key.Length % 4 > 0
            ? key.Length / 4 + 1
            : key.Length / 4;

        var lArr = new UInt32[keysWordArrLength];

        for (int i = 0; i < lArr.Length; i++)
        {
            lArr[i] = 0;
        }

        for (var i = key.Length - 1; i >= 0; i--)
        {
            lArr[i / 4].ROL(RC5Constants.BitsPerByte).Add(key[i]);
        }

        var sArray = new UInt32[2 * (8 + 1)];
        sArray[0] = P32;
        var q = Q32;

        for (var i = 1; i < sArray.Length; i++)
        {
            sArray[i] = sArray[i - 1] + Q32;
            //sArray[i].Add(q);
        }

        var x = 0;
        var y = 0;

        var n = 3 * Math.Max(sArray.Length, lArr.Length);

        for (Int32 k = 0, i = 0, j = 0; k < n; ++k)
        {
            sArray[i].Add(x).Add(y).ROL(3);
            x = sArray[i];

            lArr[j].Add(x).Add(y).ROL(x.ToInt32() + y.ToInt32());
            y = (int)lArr[j];

            i = (i + 1) % sArray.Length;
            j = (j + 1) % lArr.Length;
        }

        return sArray;
    }
    public Int32 ROR(Int32 value, Int32 offset)
    {
        value = (value << offset) | (value >> (value - offset));

        return value;
    }
    }
     

    
}
