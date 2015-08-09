using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;

namespace Arkansalt.GoogleServices
{
    public class GoogleGmailService
    {
        public GoogleGmailService(string emailAddress)
        {
            this.EmailAddress = emailAddress;
        }

        public string EmailAddress { get; private set; }


        #region headers list

        public string[] ListMessageHeaderNames(string userEmail, GmailService service)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }

        public string[] ListMessageHeaderNames(string userEmail, GmailService service)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error listing email headers: {0}", ex.Message);
                throw new Exception(errorMsg, ex);
            }
        }

        #endregion

        #region subjects list

        public string[] ListMessageSubjects()
        {
            try
            {
                GmailServiceProvider serviceProvider = new GmailServiceProvider();
                GmailService service = serviceProvider.GetServiceAccountService();

                return this.ListMessageSubjects(this.EmailAddress, service);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error listing email subjects: {0}", ex.Message);
                throw new Exception(errorMsg, ex);
            }
        }

        public string[] ListMessageSubjects(string userEmail)
        {
            try
            {
                GmailServiceProvider serviceProvider = new GmailServiceProvider();
                GmailService service = serviceProvider.GetUserAccountService(userEmail);

                return this.ListMessageSubjects(userEmail, service);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error listing email subjects: {0}", ex.Message);
                throw new Exception(errorMsg, ex);
            }
        }

        public string[] ListMessageSubjects(string userEmail, GmailService service)
        {
            try
            {
                List<string> subjectsList = new List<string>();

                UsersResource.MessagesResource.ListRequest listRequest = service.Users.Messages.List(userEmail);

                //listRequest.Q = "is:unread";
                ListMessagesResponse response = listRequest.Execute();

                IList<Message> messages = response.Messages;
                if (messages != null)
                {
                    if (messages.Count > 10)
                    {
                        listRequest.Q = "is:unread";
                        response = listRequest.Execute();
                        messages = response.Messages;
                    }

                    foreach (Message listMessage in messages)
                    {
                        UsersResource.MessagesResource.GetRequest getRequest = service.Users.Messages.Get(userEmail, listMessage.Id);
                        Message message = getRequest.Execute();

                        string subject = string.Empty;

                        IList<MessagePartHeader> headers = message.Payload.Headers;
                        foreach (MessagePartHeader header in headers)
                        {
                            if (header.Name.ToLower() == "subject")
                            {
                                subject = header.Value;
                                break;
                            }
                        }

                        subjectsList.Add(subject);
                    }
                }

                return subjectsList.ToArray();
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error listing email subjects: {0}", ex.Message);
                throw new Exception(errorMsg, ex);
            }
        }

        #endregion

        #region summaries

        public string[] ListMessageSummaries(string userEmail, GmailService service)
        {
            try
            {
                List<string> subjectsList = new List<string>();

                UsersResource.MessagesResource.ListRequest listRequest = service.Users.Messages.List(userEmail);

                //listRequest.Q = "is:unread";
                ListMessagesResponse response = listRequest.Execute();

                IList<Message> messages = response.Messages;
                if (messages != null)
                {
                    if (messages.Count > 10)
                    {
                        listRequest.Q = "is:unread";
                        response = listRequest.Execute();
                        messages = response.Messages;
                    }

                    foreach (Message listMessage in messages)
                    {
                        UsersResource.MessagesResource.GetRequest getRequest = service.Users.Messages.Get(userEmail, listMessage.Id);
                        Message message = getRequest.Execute();

                        string subject = string.Empty;

                        IList<MessagePartHeader> headers = message.Payload.Headers;
                        foreach (MessagePartHeader header in headers)
                        {
                            if (header.Name.ToLower() == "subject")
                            {
                                subject = header.Value;
                                break;
                            }
                        }

                        subjectsList.Add(subject);
                    }
                }

                return subjectsList.ToArray();
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Error listing email subjects: {0}", ex.Message);
                throw new Exception(errorMsg, ex);
            }
        }

        #endregion

    }
}
