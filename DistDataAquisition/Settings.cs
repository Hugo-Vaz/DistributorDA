using Prion.Tools;

namespace DistDataAcquisition
{
    public class Settings
    {
        private static string GetValue(string chave)
        {
            System.Configuration.AppSettingsReader appReader = new System.Configuration.AppSettingsReader();
            return appReader.GetValue(chave, typeof(string)).ToString();
        }

        public static string ConnectionString
        {
            get
            {
                Criptografia criptografia = new Criptografia();
                return criptografia.Decriptografar(GetValue("ConnectionString"));
            }
        }
        public static string ConnectionStringLocal
        {
            get
            {
                Criptografia criptografia = new Criptografia();
                return criptografia.Decriptografar(GetValue("ConnectionStringLocal"));
            }
        }
    }
}
