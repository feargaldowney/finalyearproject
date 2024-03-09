using UnityEngine;
using System;
using System.Text;
using System.Security.Cryptography;

public class CredentialsLoader : MonoBehaviour
{
    private string encryptedAPIKey = "z7TeNGnxkMc3i+uqpaHniNg1M19TvUy6UVIpgMsMFdl2e/Dn91pzQtc5WrU28FVW";
    private string secretKey = "444444444444444";

    // This method is public but it returns the decrypted API key only when called
    // This keeps the decryptedAPIKey variable itself private and secure
    public string GetDecryptedAPIKey()
    {
        return DecryptAPIKey(encryptedAPIKey, secretKey);
    }

    private string DecryptAPIKey(string encryptedText, string key)
    {
        byte[] iv = new byte[16]; // You might need to adjust this based on your encryption
        byte[] buffer = Convert.FromBase64String(encryptedText);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(buffer))
            {
                using (System.Security.Cryptography.CryptoStream cryptoStream = new System.Security.Cryptography.CryptoStream(memoryStream, decryptor, System.Security.Cryptography.CryptoStreamMode.Read))
                {
                    using (System.IO.StreamReader streamReader = new System.IO.StreamReader(cryptoStream))
                    {
                        return streamReader.ReadToEnd(); // Return the decrypted API key
                    }
                }
            }
        }
    }
}
