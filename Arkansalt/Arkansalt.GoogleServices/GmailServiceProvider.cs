using System;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;

namespace Arkansalt.GoogleServices
{
    public class GmailServiceProvider
    {

        // service account info
        public readonly string ServiceAccount_ClientId =
            //@"830662531701-s63bcgl6ck7tvuajau68vbdk8149ier0.apps.googleusercontent.com";
            @"772831471255-f0hesu2m0qjg3bjh74agfbrvofs3q6ci.apps.googleusercontent.com";

        public readonly string ServiceAccount_Email =
            //@"830662531701-s63bcgl6ck7tvuajau68vbdk8149ier0@developer.gserviceaccount.com";
            @"772831471255-f0hesu2m0qjg3bjh74agfbrvofs3q6ci@developer.gserviceaccount.com";

        // api info
        private readonly string[] Scopes = {GmailService.Scope.MailGoogleCom};
        private readonly string ApplicationName = @"Arkansalt";

        // user account info


        // GetService()
        public GmailService GetServiceAccountService()
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

                GmailService service = new GmailService(initializer);
                return service;
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error creating Gmail service account service: {0}", ex.Message);
                throw new Exception(errorMsg, ex);
            }
        }

        public GmailService GetUserAccountService(string userEmail)
        {
            try
            {
                UserCredential credential = (new ApiOAuth()).GetUserCredential(userEmail, this.Scopes);

                GmailService service = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = this.ApplicationName
                });

                return service;
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error creating Gmail user account service: {0}", ex.Message);
                throw new Exception(errorMsg, ex);
            }
        }


    }
}