using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    public static class RandomFactory
    {
        private static RNGCryptoServiceProvider m_rng_provider = new RNGCryptoServiceProvider();

        public static Random Create()
        {
            var buffer = new byte[4];
            m_rng_provider.GetBytes(buffer);
            return new Random(BitConverter.ToInt32(buffer, 0));
        }
    }
}
