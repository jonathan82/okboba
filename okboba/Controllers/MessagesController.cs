using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using okboba.Repository;
using okboba.Entities;
using PagedList;

// TODO: 
//     Inbox [delete]
//     Sent Messages [delete]
//     Compose new message
//     View conversation and reply 
//     

namespace okboba.Controllers
{
    [Authorize]
    public class MessagesController : OkbBaseController
    {
        const int MESSAGES_PER_PAGE = 25;

        private MessageRepository msgRepo;

        public MessagesController()
        {
            this.msgRepo = MessageRepository.Instance;
        }

        // GET: Messages
        public ActionResult Index(int? pageNumber)
        {
            //var profileId = GetProfileId();

            //int page = pageNumber ?? 1;

            //var listMessages = msgRepo.GetConversationList(profileId).ToPagedList(page, MESSAGES_PER_PAGE);

            //ViewBag.MessageList = listMessages;

            return View();
        }

        public ActionResult Compose(int profileId)
        {
            return View();
        }
    }
}