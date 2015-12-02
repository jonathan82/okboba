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

        public static string PhotoUrl(this HtmlHelper htmlHelper,string photo, int profileId)
        {
            var storageUrl = ConfigurationManager.AppSettings["StorageUrl"];
            return storageUrl + profileId.ToString() + "/" + photo;
        }

        public static HtmlString Thumbnail(this HtmlHelper htmlHelper, Photo photo, int profileId)
        {
            var storageUrl = ConfigurationManager.AppSettings["StorageUrl"];
            var img = new TagBuilder("img");
            img.MergeAttribute("src", PhotoUrl(null, photo.Thumb,profileId));
            img.MergeAttribute("data-original", PhotoUrl(null, photo.Original, profileId));
            img.MergeAttribute("data-w", photo.Width.ToString());
            img.MergeAttribute("data-h", photo.Height.ToString());
            img.AddCssClass("photo-thumbnail-full");
            return new HtmlString(img.ToString(TagRenderMode.SelfClosing));
        }

        public static HtmlString FeedBlurb(this HtmlHelper htmlHelper, ActivityModel act)
        {
            var str = act.Profile.Nickname + " ";

            switch ((ActivityCategories)act.Activity.CategoryId)
            {
                case ActivityCategories.Joined:
                    str += i18n.Feed_Joined;
                    break;
                case ActivityCategories.AnsweredQuestion:
                    str +=  i18n.Feed_AnsweredQuestion + "<br />";
                    str += "“" + act.Activity.What + "”";
                    break;
                case ActivityCategories.EditedProfileText:
                    str += i18n.Feed_EditedProfileText + "<br />";
                    str += "“" + act.Activity.What + "”";
                    break;
                case ActivityCategories.UploadedPhoto:
                    str += i18n.Feed_UploadedPhoto;
                    break;
                default:
                    break;
            }

            return new HtmlString(str);
        }

        public static HtmlString Avatar(this HtmlHelper htmlHelper, string photo, byte gender, int profileId, int width = OkbConstants.AVATAR_WIDTH, int height = OkbConstants.AVATAR_HEIGHT, bool small = false, bool circle = false)
        {
            string src;

            if (string.IsNullOrEmpty(photo))
            {
                //Use one of the default avatars
                src = "/Content/images/";
                src += gender == OkbConstants.MALE ? "no-avatar-male.png" : "no-avatar-female.png";
            }
            else
            {
                //Use the storage key in web.config to construct URL
                src = ConfigurationManager.AppSettings["StorageUrl"] + profileId.ToString() + "/";
                src += photo;
            }

            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", src);
            builder.MergeAttribute("width", width.ToString());
            builder.MergeAttribute("height", height.ToString());

            if (circle) builder.AddCssClass("img-circle");

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
            return text.Replace(Environment.NewLine, "<br>");
        }

        public static HtmlString ProfileText(this HtmlHelper htmlHelper, string text, string qid, bool isMe)
        {
            var qDiv = new TagBuilder("div");
            qDiv.MergeAttribute("id", qid);            

            if (string.IsNullOrEmpty(text))
            {
                //use placeholder text
                qDiv.AddCssClass("profile-text-placeholder");
                qDiv.InnerHtml = isMe ? i18n.Profile_Text_Prompt : i18n.Profile_Text_None;
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
    }
}