using Newtonsoft.Json;
using okboba.Entities;
using okboba.Repository;
using okboba.Repository.Models;
using okboba.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace okboba.Web.Helpers
{
    public static class OkbHtmlHelpers
    {

        public static string Gender(this HtmlHelper htmlHelper, byte gender)
        {
            switch (gender)
            {
                case OkbConstants.MALE:
                    return "男";
                case OkbConstants.FEMALE:
                    return "女";
                default:
                    return "??";
            }
        }

        public static string ChatUrl(this HtmlHelper htmlHelper)
        {
            return ConfigurationManager.AppSettings["ChatUrl"];
        }

        public static string PhotoUrl(this HtmlHelper htmlHelper,string photo, string userId)
        {
            var storageUrl = ConfigurationManager.AppSettings["StorageUrl"];
            return storageUrl + OkbConstants.PHOTO_CONTAINER + "/" + userId + "/" + photo;
        }

        public static HtmlString Thumbnail(this HtmlHelper htmlHelper, Photo photo, string userId, string className)
        {
            var img = new TagBuilder("img");
            img.MergeAttribute("src", PhotoUrl(null, photo.Thumb, userId));
            img.MergeAttribute("data-original", PhotoUrl(null, photo.Original, userId));
            img.MergeAttribute("data-w", photo.Width.ToString());
            img.MergeAttribute("data-h", photo.Height.ToString());
            img.AddCssClass(className);
            return new HtmlString(img.ToString(TagRenderMode.SelfClosing));
        }

        public static HtmlString FeedBlurb(this HtmlHelper htmlHelper, ActivityModel act)
        {
            var str = act.Profile.Nickname + " ";

            switch ((OkbConstants.ActivityCategories)act.Activity.CategoryId)
            {
                case OkbConstants.ActivityCategories.Joined:
                    str += i18n.Feed_Joined;
                    break;
                case OkbConstants.ActivityCategories.AnsweredQuestion:
                    str +=  i18n.Feed_AnsweredQuestion + "<br />";
                    str += "“" + act.Activity.Field1 + "”";
                    break;
                case OkbConstants.ActivityCategories.EditedProfileText:
                    str += i18n.Feed_EditedProfileText + "<br />";
                    str += "“" + act.Activity.Field1 + "”";
                    break;
                case OkbConstants.ActivityCategories.UploadedPhoto:
                    str += i18n.Feed_UploadedPhoto;
                    break;
                default:
                    break;
            }

            return new HtmlString(str);
        }


        public static string AvatarUrl(this HtmlHelper htmlHelper, string photo, byte gender, string userId)
        {
            string url;

            if (string.IsNullOrEmpty(photo))
            {
                //Use one of the default avatars
                url = "/Content/images/";
                url += gender == OkbConstants.MALE ? "no-avatar-male.png" : "no-avatar-female.png";
            }
            else
            {
                //Use the storage key in web.config to construct URL
                url = ConfigurationManager.AppSettings["StorageUrl"] + OkbConstants.PHOTO_CONTAINER + "/" + userId + "/";
                url += photo;
            }

            return url;
        }

        public static HtmlString Avatar(this HtmlHelper htmlHelper, Profile profile, string className, bool isSmall = false)
        {
            return htmlHelper.Avatar(
                profile.GetFirstHeadshot(isSmall),
                profile.Gender,
                profile.UserId,
                profile.Nickname,
                className);
        }

        /// <summary>
        /// Generates an Avatar by buidling an img tag with the given parameters. If no photo is 
        /// specified it will use the "no avatar" default photo. The image is styled with the given className.
        /// The photo passed in is retrieved from one of the helper functions in the Profile entity class.
        /// </summary>        
        public static HtmlString Avatar(this HtmlHelper htmlHelper, 
            string photo, 
            byte gender, 
            string userId,
            string name, 
            string className)
        {            
            var src = htmlHelper.AvatarUrl(photo, gender, userId);

            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", src);
            builder.MergeAttribute("alt", name);
            builder.AddCssClass(className);

            return new HtmlString(builder.ToString(TagRenderMode.SelfClosing));
        }

        public static HtmlString NavTab(this HtmlHelper htmlHelper, string currSection, string section, string link)
        {
            //"<li role="presentation" class="@profileActive"><a href=" / profile">Profile</a></li>"
            var liTag = new TagBuilder("li");
            if (section==currSection)
            {
                liTag.AddCssClass("active");
            }

            var aTag = new TagBuilder("a");
            aTag.MergeAttribute("href", link);
            aTag.InnerHtml = section;

            liTag.InnerHtml = aTag.ToString();

            return new HtmlString(liTag.ToString());
        }

        private static string nl2br(string text)
        {
            return text.Replace(Environment.NewLine, "<br/>");
        }

        public static HtmlString ProfileText(this HtmlHelper htmlHelper, string text)
        {
            if (string.IsNullOrEmpty(text)) return new HtmlString("");
            return new HtmlString(nl2br(text));
        }

        public static HtmlString ProfileText(this HtmlHelper htmlHelper, string text, string qid, bool isMe)
        {
            var qDiv = new TagBuilder("div");
            qDiv.MergeAttribute("id", qid);            

            if (string.IsNullOrEmpty(text))
            {
                //use placeholder text
                qDiv.AddCssClass("profile-text-placeholder");
                qDiv.InnerHtml = isMe ? i18n.Profile_Question_Placeholder : i18n.Profile_Question_Empty;
                //var span = new TagBuilder("span");
                //span.AddCssClass("profile-text-placeholder");
                //span.InnerHtml = isMe ? i18n.Profile_Text_Prompt : i18n.Profile_Text_None;
            }
            else
            {
                qDiv.InnerHtml = nl2br(text);
            }

            return new HtmlString(qDiv.ToString());
        }

        public static HtmlString EditTextIcon(this HtmlHelper htmlHelper, string target, bool me)
        {
            //<div class="js-editinplace-editicon" data-target="#q5"><span class="glyphicon glyphicon-pencil "></span></div>
            if (me)
            {
                var div = new TagBuilder("div");
                div.AddCssClass("js-editinplace-editicon");
                div.MergeAttribute("data-target", target);

                var span = new TagBuilder("span");
                span.AddCssClass("glyphicon");
                span.AddCssClass("glyphicon-pencil");

                div.InnerHtml = span.ToString();

                return new HtmlString(div.ToString());
            }

            return new HtmlString("");
        }
        
        public static HtmlString DetailDropdown(this HtmlHelper htmlHelper, string detailName, IEnumerable<ProfileDetailOption> options, byte id)
        {
            var selectTag = new TagBuilder("select");
            selectTag.MergeAttribute("name", detailName);
            selectTag.MergeAttribute("id", detailName);
            selectTag.AddCssClass("form-control");

            var optionTag = new TagBuilder("option");

            //Add the "unselected" option - value = 0
            optionTag.MergeAttribute("value", "0");
            optionTag.InnerHtml = "--";
            if (id == 0) optionTag.MergeAttribute("selected","");
            selectTag.InnerHtml += optionTag.ToString();

            foreach (var option in options)
            {
                optionTag = new TagBuilder("option");
                optionTag.MergeAttribute("value", ""+option.Id);
                if (option.Id == id) optionTag.MergeAttribute("selected", "");
                optionTag.InnerHtml = option.Value;
                selectTag.InnerHtml += optionTag.ToString();
            }
            return new HtmlString(selectTag.ToString());
        }

        public static HtmlString HeightDropdown(this HtmlHelper htmlHelper, short height)
        {
            var selectTag = new TagBuilder("select");
            selectTag.MergeAttribute("name", OkbConstants.DETAIL_HEIGHT);
            selectTag.MergeAttribute("id", OkbConstants.DETAIL_HEIGHT);
            selectTag.AddCssClass("form-control");

            //add unselected option
            var optionTag = new TagBuilder("option");
            optionTag.MergeAttribute("value", "0");
            optionTag.InnerHtml = "--";
            if (height == 0) optionTag.MergeAttribute("selected", "");
            selectTag.InnerHtml += optionTag.ToString();

            for (int i = OkbConstants.MIN_HEIGHT; i <= OkbConstants.MAX_HEIGHT; i++)
            {
                optionTag = new TagBuilder("option");
                optionTag.MergeAttribute("value", i + "");
                if (i == height) optionTag.MergeAttribute("selected", "");
                optionTag.InnerHtml = i + " CM";
                selectTag.InnerHtml += optionTag.ToString();
            }
            return new HtmlString(selectTag.ToString());
        }

        public static HtmlString DetailValue(this HtmlHelper htmlHelper, string val)
        {
            var tag = new TagBuilder("div");
            if (string.IsNullOrEmpty(val))
            {
                val = "--";
            }
            tag.AddCssClass("profile-detail-overlay");
            tag.InnerHtml = val;
            return new HtmlString(tag.ToString());
        }

        public static HtmlString UnreadCount(this HtmlHelper htmlHelper, int count)
        {
            if (count == 0) return new HtmlString("");
            var html = "<div class=\"unread-conv-count\">"+count+"</div>";
            return new HtmlString(html);
        }

        /// <summary>
        /// Given an object serializes it into a Json Array.  Normalizes the json string to 
        /// be an empty array [] if it is null or empty.
        /// </summary>      
        public static HtmlString JsonArray(this HtmlHelper htmlHelper, object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return new HtmlString(string.IsNullOrEmpty(json) ? "[]" : json);
        }

        public static HtmlString JsonObject(this HtmlHelper htmlHelper, string json)
        {
            return new HtmlString(json == "" ? null : json);
        }

        public static HtmlString ChooseLanguage(this HtmlHelper htmlHelper, string culture, string label)
        {            
            if(CultureHelper.GetCurrentCulture() == culture)
            {
                //no link
                return new HtmlString(label);
            }

            // return link
            var tag = new TagBuilder("a");
            tag.MergeAttribute("href", "/language/set?culture=" + culture);
            tag.InnerHtml = label;
            return new HtmlString(tag.ToString());
        }
    }
}