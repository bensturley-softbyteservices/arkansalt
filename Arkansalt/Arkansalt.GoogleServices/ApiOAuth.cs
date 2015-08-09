using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;

namespace Arkansalt.GoogleServices
{
    public class ApiOAuth
    {

        //public readonly string P12_Filename = @"auth_files\Arkansalt-ff9bdcb0a6c7.p12";
        public readonly string P12_Filename = @"auth_files\Arkansalt-d1b37430808d.p12";
        public readonly string P12_Password = @"notasecret";

        private readonly string ClientSecret_Filename = @"auth_files\arkansalt_client_secret_1.json";
        private readonly string Credentials_OutputPath = @".credentials";

        public ApiOAuth()
        {
        }


        public X509Certificate2 GetCertificate()
        {
            try
            {
                X509Certificate2 cert = new X509Certificate2(
                    this.GetP12FilePath(), this.P12_Password,
                    X509KeyStorageFlags.Exportable);

                return cert;
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error creating X509 certificate: {0}", ex.Message);
                throw new Exception(errorMsg, ex);
            }
        }

        public UserCredential GetUserCredential(string userEmail, string[] scopes)
        {
            try
            {
                UserCredential credential;

                using (FileStream stream = 
                    new FileStream(this.GetClientSecretJsonPath(), FileMode.Open, FileAccess.Read))
                {
                    string credPath = this.GetCredentialsOutputPath();

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        scopes,
                        userEmail,
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }

                return credential;
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error creating user credential: {0}", ex.Message);
                throw new Exception(errorMsg, ex);
            }
        }


        private string GetP12FilePath()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string file = this.P12_Filename;
            string filePath = Path.Combine(path, file);

            return filePath;
        }

        private string GetClientSecretJsonPath()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string file = this.ClientSecret_Filename;
            string filePath = Path.Combine(path, file);

            return filePath;
        }

        private string GetCredentialsOutputPath()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string dir = this.Credentials_OutputPath;
            path = Path.Combine(path, dir);

            return path;
        }

    }
}
