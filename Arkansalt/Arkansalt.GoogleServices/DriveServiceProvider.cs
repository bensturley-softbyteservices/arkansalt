using System;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Services;

namespace Arkansalt.GoogleServices
{
    public class DriveServiceProvider
    {

        // service account info
        public readonly string ServiceAccount_ClientId =
            //@"830662531701-s63bcgl6ck7tvuajau68vbdk8149ier0.apps.googleusercontent.com";
            @"772831471255-f0hesu2m0qjg3bjh74agfbrvofs3q6ci.apps.googleusercontent.com";

        public readonly string ServiceAccount_Email =
            //@"830662531701-s63bcgl6ck7tvuajau68vbdk8149ier0@developer.gserviceaccount.com";
            @"772831471255-f0hesu2m0qjg3bjh74agfbrvofs3q6ci@developer.gserviceaccount.com";

        // api info
        private readonly string[] Scopes = {DriveService.Scope.DriveFile};
        private readonly string ApplicationName = @"Arkansalt";

        // user account info


        // GetService()
        public DriveService GetServiceAccountService()
        {
            try
            {
                // get certificate
                X509Certificate2 certificate = (new ApiOAuth()).GetCertificate();

                // create credential
                ServiceAccountCredential credential = new ServiceAccountCredential(
                    new ServiceAccountCredential.Initializer(this.ServiceAccount_Email)
                    {
                        Scopes = this.Scopes
                    }
                        .FromCertificate(certificate)
                    );

                // create service initialiser
                BaseClientService.Initializer initializer = new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = this.ApplicationName
                };

                DriveService service = new DriveService(initializer);
                return service;
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error creating Google Drive service account service: {0}", ex.Message);
                throw new Exception(errorMsg, ex);
            }
        }

        public DriveService GetUserAccountService(string userEmail)
        {
            try
            {
                UserCredential credential = (new ApiOAuth()).GetUserCredential(userEmail, this.Scopes);

                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = this.ApplicationName
                });

                return service;
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error creating Google Drive user account service: {0}", ex.Message);
                throw new Exception(errorMsg, ex);
            }
        }


    }
}