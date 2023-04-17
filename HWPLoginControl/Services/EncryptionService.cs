using System.Security.Cryptography;
using System.Text;

namespace HWPLoginControl.Services
{
    public class EncryptionService
    {
        private readonly IConfiguration _config;

        public EncryptionService(IConfiguration config)
        {
            _config = config;
        }

        public string Encrypt(string clearText)
        {
            string EncryptionKey = _config.GetValue<string>("Keys:Forgotten");

            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    var stringBytes = ms.ToArray();

                    StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
                    foreach (byte b in stringBytes)
                    {
                        sbBytes.AppendFormat("{0:X2}", b);
                    }
                    clearText = sbBytes.ToString();
                }
            }
            return clearText;
        }
        public string Decrypt(string cipherText)
        {
            try
            {            
                string EncryptionKey = _config.GetValue<string>("Keys:Forgotten");

                cipherText = cipherText.Replace(" ", "+");

                int numberChars = cipherText.Length;
                byte[] cipherBytes = new byte[numberChars / 2];
                for (int i = 0; i < numberChars; i += 2)
                {
                    cipherBytes[i / 2] = Convert.ToByte(cipherText.Substring(i, 2), 16);
                }

                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }
            catch
            {
                return "";
            }
        }
    }
    
}
