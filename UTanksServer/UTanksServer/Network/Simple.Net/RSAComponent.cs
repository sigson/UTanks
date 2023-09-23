using System;
using System.Text;
using System.Security.Cryptography;

namespace UTanksServer.Network.Simple.Net
{
    // Dividing into two classes to make it easier later on
    public class RSADecryptComponent
    {
        RSACryptoServiceProvider provider;
        public string publicKey { get; private set; }

        public RSADecryptComponent(int keyLength = 512)
        {
            provider = new RSACryptoServiceProvider(keyLength);

            RSAParameters parameters = provider.ExportParameters(false);

            publicKey = $"{Convert.ToBase64String(parameters.Modulus)}:{Convert.ToBase64String(parameters.Exponent)}";
        }

        public byte[] DecryptToBytes(string text)
            => provider.Decrypt(Convert.FromBase64String(text), false);

        public string DecryptToString(string text)
            => Encoding.UTF8.GetString(DecryptToBytes(text));
    }

    public class RSAEncryptCompoenent
    {
        RSACryptoServiceProvider provider;

        public RSAEncryptCompoenent(string publicKey)
        {
            provider = new RSACryptoServiceProvider();
            string[] publicKeyParams = publicKey.Split(':');
            provider.FromXmlString(string.Concat(new string[] {
                "<RSAKeyValue><Modulus>",
                publicKeyParams[0],
                "</Modulus><Exponent>",
                publicKeyParams[1],
                "</Exponent></RSAKeyValue>"
            }));
        }

        public string Encrypt(byte[] input)
            => Convert.ToBase64String(provider.Encrypt(input, false));

        public string Encrypt(string input)
            => Encrypt(Encoding.UTF8.GetBytes(input));
    }
}
