using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using okboba.Repository;
using okboba.Entities;
using PagedList;
using okboba.Repository.EntityRepository;

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
        private IMessageRepository _msgRepo;

        public MessagesController()
        {
            _msgRepo = EntityMessageRepository.Instance;
        }

        /// <summary>
        /// Returns the "Inbox" view - a list of received messages
        /// </summary>
        public ActionResult Index()
        {

            return View();
        }

        /// <summary>
        /// Returns the "Sent" view - a list of sent messages
        /// </summary>
        public ActionResult Sent(int profileId)
        {
            return View();
        }

        /// <summary>
        /// Returns the "Conversation" view - a conversation with another user where you can reply
        /// </summary>
        public ActionResult Conversation(int id)
        {
            return View();
        }
    }
}