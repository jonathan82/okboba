using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using okboba.Repository;
using okboba.Entities;
using PagedList;
using okboba.Repository.EntityRepository;
using okboba.Web.Models;
using okboba.Resources;
using Newtonsoft.Json;
using System.Threading.Tasks;

// TODO: 
//     Inbox [delete]
//     Sent Messages [delete]
//     Compose new message
//     View conversation and reply 
//     

namespace okboba.Web.Controllers
{
    [Authorize]
    public class MessagesController : OkbBaseController
    {        
        private IMessageRepository _msgRepo;
        private IProfileRepository _profileRepo;
        private ILocationRepository _locRepo;

        public MessagesController()
        {
            _msgRepo = EntityMessageRepository.Instance;
            _profileRepo = EntityProfileRepository.Instance;
            _locRepo = EntityLocationRepository.Instance;
        }

        /// <summary>
        /// Returns the "Inbox" view - a list of received messages
        /// </summary>
        public ActionResult Index()
        {
            var me = GetMyProfileId();

            var vm = _msgRepo.GetConversations(me);

            return View(vm);
        }

        /// <summary>
        /// Returns the "Sent" view - a list of sent messages
        /// </summary>
        public ActionResult Sent()
        {
            var me = GetMyProfileId();

            var msgs = _msgRepo.GetSent(me);

            return View(msgs);
        }

        /// <summary>
        /// Returns the "Conversation" view - a conversation with another user where you can reply
        ///     - Marks the conversation as read
        ///     - Decrement the uread count
        /// </summary>
        public ActionResult Conversation(int id)
        {
            var me = GetMyProfileId();

            var matchClient = GetMatchApiClient();

            //make sure we're getting our own conversation
            var map = _msgRepo.GetConversationMap(me, id);
            if (map == null) return HttpNotFound();

            //Get the first N messages
            var messages = _msgRepo.GetMessages(id,0, OkbConstants.INITIAL_NUM_MESSAGES);
            var profile = _profileRepo.GetProfile(map.Other);
            var myProfile = _profileRepo.GetProfile(me);
            var conv = _msgRepo.GetConversation(id);

            //Calculate the match score
            var matchInfo = matchClient.CalculateMatchAsync(map.Other).Result;

            var vm = new ReplyViewModel
            {
                ConversationId = id,
                Me = myProfile,
                Messages = messages,
                Other = profile,
                MatchInfo = matchInfo,
                Subject = conv.Subject
            };

            vm.Other.LocationSring = _locRepo.GetLocationString(profile.LocationId1, profile.LocationId2);

            //Mark conversation as read which also will decrement the count
            _msgRepo.MarkAsRead(me, id);            

            return View(vm);
        }

        /// <summary>
        /// API call.  Retrieves the previous page of messages starting at low
        /// and returns a JSON result
        /// </summary>
        public ActionResult Previous(int low, int convId)
        {
            var me = GetMyProfileId();

            //make sure we're getting our own conversation
            var map = _msgRepo.GetConversationMap(me, convId);
            if (map.ProfileId != me) throw new HttpException(404, "conversation not found");

            var msgs = _msgRepo.GetMessages(convId, low, OkbConstants.MESSAGES_PER_PAGE);

            //Format the time to be more friendly
            foreach (var msg in msgs)
            {
                msg.FriendlyTime = FriendlyTime.Format(msg.Timestamp);
            }

            //use Json.NET serializer
            var json = JsonConvert.SerializeObject(msgs);

            return Content(json, "application/json");
        }

        /// <summary>
        /// API call.  Replies to an existing conversation. Accepts raw/dangerous HTML input
        /// so we have to sanitize the input first.
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Reply(string message, int convId)
        {
            var me = GetMyProfileId();

            //sanitize input
            message = Server.HtmlEncode(message);
            message = Truncate(message, OkbConstants.MAX_MESSAGE_LENGTH);

            //make sure we're replying to our own conversation
            var map = _msgRepo.GetConversationMap(me, convId);
            if (map.ProfileId != me) throw new HttpException(404, "conversation not found");

            await _msgRepo.Reply(me, convId, message);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Starts a new conversation with the given user. Accepts dangerous input.
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> StartConversation(int to, string subject, string message)
        {
            var me = GetMyProfileId();

            //sanitize the input
            subject = Server.HtmlEncode(subject);
            message = Server.HtmlEncode(message);
            subject = Truncate(subject, OkbConstants.MAX_SUBJECT_LENGTH);            
            message = Truncate(message, OkbConstants.MAX_MESSAGE_LENGTH);

            await _msgRepo.StartConversation(me, to, subject, message);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Delete(int convId)
        {
            var me = GetMyProfileId();

            await _msgRepo.DeleteConversationAsync(me, convId);

            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}