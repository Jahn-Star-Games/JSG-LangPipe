using System;
using System.Text;
using System.Security.Cryptography;

namespace JahnStarGames.Langpipe
{
    public class SecureToken
    {
        private readonly byte[] token, skey;
        private readonly TimeSpan tokenLifetime, bufferTime;
        private readonly DateTime timestamp;
        public bool TokenExpired => DateTime.UtcNow - this.timestamp > this.tokenLifetime - bufferTime;
        public SecureToken(string token, TimeSpan tokenLifetime, TimeSpan bufferTime = default)
        {
            // set timestamp
            this.timestamp = DateTime.UtcNow;
            this.tokenLifetime = tokenLifetime;
            if (bufferTime == default) this.bufferTime = TimeSpan.FromTicks(this.tokenLifetime.Ticks / 60);
            else this.bufferTime = bufferTime;
            
            // generate secret key
            skey = GenerateSecretKey();
            byte[] GenerateSecretKey()
            {
                using (var aes = new AesManaged())
                {
                    aes.GenerateKey();
                    return aes.Key;
                }
            }

            // encrypt token
            this.token = EncryptToken(token);
        }
        
        ~ SecureToken()
        {
            // Zero out the secret key and token
            Array.Clear(skey, 0, skey.Length);
            Array.Clear(token, 0, token.Length);
        }
                
        public string GetToken()
        {
            if (TokenExpired) throw new TokenExpiredException();
            return DecryptToken(this.token);
        }
        
        private byte[] EncryptToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return Encoding.UTF8.GetBytes("");

            using (var aes = new AesManaged())
            {
                aes.Key = skey;
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    var plainText = Encoding.UTF8.GetBytes(token);
                    var cipherText = encryptor.TransformFinalBlock(plainText, 0, plainText.Length);

                    // Prepend the IV to the cipher text
                    var result = new byte[aes.IV.Length + cipherText.Length];
                    Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
                    Buffer.BlockCopy(cipherText, 0, result, aes.IV.Length, cipherText.Length);

                    return Encoding.UTF8.GetBytes(Convert.ToBase64String(result));
                }
            }
        }

        private string DecryptToken(byte[] encryptedToken)
        {
            string token = Encoding.UTF8.GetString(encryptedToken);
            if (string.IsNullOrEmpty(token)) return "";

            var fullCipher = Convert.FromBase64String(token);

            using (var aes = new AesManaged())
            {
                aes.Key = skey;

                // Extract the IV from the cipher text
                var iv = new byte[aes.IV.Length];
                var cipherText = new byte[fullCipher.Length - aes.IV.Length];
                Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(fullCipher, iv.Length, cipherText, 0, cipherText.Length);

                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    var plainText = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
                    return Encoding.UTF8.GetString(plainText);
                }
            }
        }
        
        public class TokenExpiredException : Exception
        {
            public TokenExpiredException() : base("Token has expired") { }
        }
    }
}