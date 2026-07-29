using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SECRON.Utils
{
    // Cifrado/descifrado AES-256 para credenciales sensibles (ej. contraseña SMTP).
    // La llave se lee de un archivo externo al repositorio (ver ObtenerLlave()).
    // NO usar para contraseñas de usuarios (eso sigue siendo BCrypt, irreversible).
    internal static class Cls_EmailEncryption
    {
        // Ruta del archivo de llave, distribuido junto al instalador (no versionado en Git)
        private static readonly string RutaLlave = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "secron.key");

        private static byte[] ObtenerLlave()
        {
            if (!File.Exists(RutaLlave))
                throw new FileNotFoundException("No se encontró el archivo de llave de cifrado (secron.key). Verifique la instalación.");

            string llaveBase64 = File.ReadAllText(RutaLlave).Trim();
            byte[] llave = Convert.FromBase64String(llaveBase64);

            if (llave.Length != 32) // 256 bits
                throw new InvalidOperationException("La llave de cifrado debe ser de 256 bits (32 bytes).");

            return llave;
        }

        // Cifra un texto plano y devuelve Base64 (IV + datos cifrados concatenados)
        public static string Encrypt(string plainText)
        {
            byte[] key = ObtenerLlave();

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                {
                    // Prefijar el IV (16 bytes) para poder descifrar después
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs, Encoding.UTF8))
                    {
                        sw.Write(plainText);
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        // Descifra un valor generado por Encrypt()
        public static string Decrypt(string cipherTextBase64)
        {
            byte[] key = ObtenerLlave();
            byte[] fullCipher = Convert.FromBase64String(cipherTextBase64);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;

                byte[] iv = new byte[16];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs, Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        // Genera una llave AES-256 aleatoria en Base64 (ejecutar UNA VEZ para crear secron.key)
        public static string GenerarNuevaLlave()
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.GenerateKey();
                return Convert.ToBase64String(aes.Key);
            }
        }
    }
}