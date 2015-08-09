using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;

namespace Arkansalt.GoogleServices
{
    public class GoogleDriveService
    {
        public GoogleDriveService()
        {
        }


        #region list file titles

        public string[] ListFileTitles()
        {
            try
            {
                DriveServiceProvider serviceProvider = new DriveServiceProvider();
                DriveService service = serviceProvider.GetServiceAccountService();

                FilesResource.ListRequest request = service.Files.List();
                FileList fileList = request.Execute();

                IEnumerable<string> titles = fileList.Items.Select(item => item.Title);
                return titles.ToArray();
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error listing file titles: {0}", ex.Message);
                throw new Exception(errorMsg, ex);
            }
        }

        public string[] ListFileTitles(string userEmail)
        {
            try
            {
                DriveServiceProvider serviceProvider = new DriveServiceProvider();
                DriveService service = serviceProvider.GetUserAccountService(userEmail);

                FilesResource.ListRequest request = service.Files.List();
                FileList fileList = request.Execute();

                IEnumerable<string> titles = fileList.Items.Select(item => item.Title);
                return titles.ToArray();
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error listing file titles: {0}", ex.Message);
                throw new Exception(errorMsg, ex);
            }
        }

        #endregion

        #region create public folder

        public void CreatePublicFolder(string userEmail, string folderName)
        {
            try
            {
                DriveServiceProvider serviceProvider = new DriveServiceProvider();
                DriveService service = serviceProvider.GetUserAccountService(userEmail);

                File folder = this.CreatePublicFolder(service, folderName);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error creating public folder: {0}", ex.Message);
                throw new Exception(errorMsg, ex);
            }
        }

        public void CreatePublicFolder(string folderName)
        {
            try
            {
                DriveServiceProvider serviceProvider = new DriveServiceProvider();
                DriveService service = serviceProvider.GetServiceAccountService();

                File folder = this.CreatePublicFolder(service, folderName);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error creating public folder: {0}", ex.Message);
                throw new Exception(errorMsg, ex);
            }
        }

        private File CreatePublicFolder(DriveService service, String folderName)
        {
            try
            {
                File body = new File
                {
                    Title = folderName,
                    MimeType = "application/vnd.google-apps.folder"
                };

                File file = service.Files.Insert(body).Execute();

                try
                {
                    Permission permission = new Permission
                    {
                        Value = "",
                        Type = "anyone",
                        Role = "reader"
                    };

                    service.Permissions.Insert(permission, file.Id).Execute();
                }
                catch (Exception ex)
                {
                    string errorMsg = string.Format("Error inserting permission: {0}", ex.Message);
                    throw new Exception(errorMsg, ex);
                }
        
                return file;
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error creating public folder: {0}", ex.Message);
                throw new Exception(errorMsg, ex);
            }
        }

        #endregion

        #region upload file

        public void UploadFile(string filePath)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error uploading file: {0}", ex.Message);
                throw new Exception(errorMsg, ex);
            }
        }

        public void UploadFile(string userEmail, string filePath)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error uploading file: {0}", ex.Message);
                throw new Exception(errorMsg, ex);
            }
        }

        #endregion

    }
}
