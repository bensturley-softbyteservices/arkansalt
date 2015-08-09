using System;
using Arkansalt.GoogleServices;

namespace Arkansalt.DevConsole
{
    public class GoogleGmailTests
    {
        public GoogleGmailTests()
        {
        }


        #region list subjects

        public void ListServiceAccountSubjects(ConsoleFunctionOutput output)
        {
            string userEmail = output.NotifyGetInput(this, "Gmail user email: ", true);

            GoogleGmailService service = new GoogleGmailService(userEmail);
            string[] subjects = service.ListMessageSubjects();

            output.NotifyOutputReady(this, "Message subjects: ", false, true);

            foreach (string subject in subjects)
            {
                output.NotifyOutputReady(this, subject, true);
            }

            output.NotifyOutputReady(this, "List finished.", false, true);

        }

        public void ListUserAccountSubjects(ConsoleFunctionOutput output)
        {
            try
            {
                string userEmail = output.NotifyGetInput(this, "Gmail user email: ", true);

                GoogleGmailService service = new GoogleGmailService(userEmail);
                string[] subjects = service.ListMessageSubjects(userEmail);

                output.NotifyOutputReady(this, "Message subjects: ", false, true);

                foreach (string subject in subjects)
                {
                    output.NotifyOutputReady(this, subject, true);
                }

                output.NotifyOutputReady(this, "List finished.", false, true);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error listing user account email subjects: {0}", ex.Message);
                throw new Exception(errorMsg, ex);
            }
        }

        #endregion

    }
}