using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SECRON.Utils
{
    internal static class Cls_EmailEncryption
    {
        private static string ObtenerRutaLlave()
        {
            try
            {
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    string dataDir = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.DataDirectory;
                    return Path.Combine(dataDir, "secron.key");
                }
            }
            catch { /* No corre bajo ClickOnce, usar respaldo */ }

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "secron.key");
        }

        public static string DiagnosticoRutaLlave()
        {
            string ruta = ObtenerRutaLlave();
            bool existe = File.Exists(ruta);
            bool esClickOnce = System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed;
            return $"IsNetworkDeployed: {esClickOnce}\nRuta calculada: {ruta}\n¿Existe el archivo?: {existe}";
        }

        private static byte[] ObtenerLlave()
        {
            string rutaLlave = ObtenerRutaLlave();

            if (!File.Exists(rutaLlave))
                throw new FileNotFoundException($"No se encontró el archivo de llave de cifrado en '{rutaLlave}'. Verifique la instalación.");

            string llaveBase64 = File.ReadAllText(rutaLlave).Trim();
            byte[] llave = Convert.FromBase64String(llaveBase64);

            if (llave.Length != 32) // 256 bits
                throw new InvalidOperationException("La llave de cifrado debe ser de 256 bits (32 bytes).");

            return llave;
        }

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