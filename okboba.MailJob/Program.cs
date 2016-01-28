using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using okboba.Entities;
using okboba.Repository.EntityRepository;
using okboba.Repository.RedisRepository;
using okboba.Resources;
using okboba.Web.Helpers;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace okboba.MailJob
{
    class Program
    {
        private static SendGridMessage BuildEmail(string name, string email, string link, string otherName)
        {
            var msg = new SendGridMessage();
            msg.EnableTemplateEngine("5f7c4376-3b0c-4972-9049-ee13ef5b59af");
            msg.DisableClickTracking();
            msg.From = new MailAddress("notify@em.okboba.com", "OkBoba 邮件");
            msg.To = new MailAddress[] { new MailAddress(email, name) };
            msg.Subject = string.Format(i18n.Messaging_Notification_Subject, otherName);
            msg.Html = string.Format(i18n.Messaging_Notification_Body, otherName);

            msg.AddSubstitution("-link-", new List<string>() { link });

            return msg;
        }

        /// <summary>
        /// This function processes all the unread mail by notifying users by email they have a new message.
        /// It is called every N minutes where N is configurable.  Peforms the following tasks:
        /// 
        ///     - Loop thru ConversationMap and return all unread conversations that haven't been sent by email
        ///     - Send email thru SendGrid 
        ///     
        /// From: OkBoba Mail
        /// Subj: [Name] sent you a message!
        /// Body: You have a new message from [Name]: [message text]
        /// Link: /account/login?token=[token]&url=/messages/conversation/[id]
        /// </summary>
        private static void ProcessUnreadNotifications()
        {
            //create the usermanager
            var provider = new MachineKeyProtectionProvider();
            var userManager = new UserManager<OkbUser>(new UserStore<OkbUser>(new OkbDbContext()));
            userManager.UserTokenProvider = new DataProtectorTokenProvider<OkbUser>(provider.Create("ASP.NET Identity"));

            //Setup the transport
            var apiKey = ConfigurationManager.AppSettings["SendGridApiKey"];
            var transportWeb = new SendGrid.Web(apiKey);

            //Get a list of all the users to email
            var msgRepo = EntityMessageRepository.Instance;
            var unread = msgRepo.GetUnreadConversations();
            var domain = ConfigurationManager.AppSettings["Domain"];

            int count = 0;

            foreach (var conv in unread)
            {
                var code = userManager.GenerateUserToken(OkbConstants.ONECLICK_LOGIN_PURPOSE, conv.UserId);
                code = HttpUtility.UrlEncode(code);
                var url = "http://{3}/account/login?code={0}&userId={1}&url=%2Fmessages%2Fconversation%2F{2}";
                var link = string.Format(url, code, conv.UserId, conv.Map.ConversationId, domain); 
                var msg = BuildEmail(conv.Name, conv.Email, link, conv.OtherProfile.Nickname);
                transportWeb.DeliverAsync(msg);
                count++;
            }
            
            //mark all as emailed
            msgRepo.MarkAllAsEmailed();

            Console.WriteLine("{0} unread messages delivered", count);
        }

        static void Main(string[] args)
        {
            // Create singleton for Redis connection object
            var redisConnStr = ConfigurationManager.ConnectionStrings["RedisConnectionString"].ConnectionString;
            SXGenericRepository.Create(redisConnStr);

            while (true)
            {
                try
                {                    
                    ProcessUnreadNotifications();
                }
                catch (Exception ex)
                {
                    //swallow all exceptions so process keeps running
                    Console.WriteLine("Error: {0}", ex.ToString());
                }
                
                Thread.Sleep(OkbConstants.MAIL_NOTIFICATION_INTERVAL * 60 * 1000);
            }
            
        }
    }
}
