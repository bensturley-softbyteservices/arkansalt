using Arkansalt.GoogleServices;

namespace Arkansalt.DevConsole
{
    public class GoogleDriveTests
    {
        public void ConnectToDrive(ConsoleFunctionOutput output)
        {
            string userEmail = output.NotifyGetInput(this, "Drive user email: ", true);


        }

        #region list files

        public void ListServiceAccountFiles(ConsoleFunctionOutput output)
        {
            GoogleDriveService service = new GoogleDriveService();
            string[] titles = service.ListFileTitles();

            output.NotifyOutputReady(this, "File titles: ", false, true);

            foreach (string title in titles)
            {
                output.NotifyOutputReady(this, title, true);
            }

            output.NotifyOutputReady(this, "List finished.", false, true);

        }

        public void ListUserAccountFiles(ConsoleFunctionOutput output)
        {
            string userEmail = output.NotifyGetInput(this, "Drive user email: ", true);

            GoogleDriveService service = new GoogleDriveService();
            string[] titles = service.ListFileTitles(userEmail);

            output.NotifyOutputReady(this, "File titles: ", false, true);

            foreach (string title in titles)
            {
                output.NotifyOutputReady(this, title, true);
            }

            output.NotifyOutputReady(this, "List finished.", false, true);

        }

        #endregion

        #region create public folder
        public void CreateServiceAccountPublicFolder(ConsoleFunctionOutput output)
        {
            string folderName = output.NotifyGetInput(this, "Folder name: ", true);

            GoogleDriveService service = new GoogleDriveService();
            service.CreatePublicFolder(folderName);

            output.NotifyOutputReady(this, "Folder created.", false, true);
        }

        public void CreateUserAccountPublicFolder(ConsoleFunctionOutput output)
        {
            string userEmail = output.NotifyGetInput(this, "Drive user email: ", true);
            string folderName = output.NotifyGetInput(this, "Folder name: ", true);

            GoogleDriveService service = new GoogleDriveService();
            service.CreatePublicFolder(userEmail, folderName);

            output.NotifyOutputReady(this, "Folder created.", false, true);
        }

        #endregion

    }
}