using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Entities.Helpers
{
    public static class OkbUuid
    {
        private static RNGCryptoServiceProvider rand  = new RNGCryptoServiceProvider();

        const string Base62Chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string GenerateUserId()
        {
            var bytes = new byte[8];
            rand.GetBytes(bytes);

            //convert bytes to base62 string
            var longId = BitConverter.ToUInt64(bytes, 0);
            var str = "";

            do
            {
                str += Base62Chars[(int)(longId % 62)];
                longId /= 62;

            } while (longId != 0);

            return str;
        }
    }
}
