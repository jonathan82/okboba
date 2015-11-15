using okboba.Entities;
using okboba.Repository;
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

        public static HtmlString Avatar(this HtmlHelper htmlHelper, string photo, byte gender, int width = OkbConstants.AVATAR_WIDTH, int height = OkbConstants.AVATAR_HEIGHT)
        {
            string src;

            if (photo == "")
            {
                //Use one of the default avatars
                src = "/Content/images/";
                src += gender == OkbConstants.MALE ? "no-avatar-male.png" : "no-avatar-female.png";
            }
            else
            {
                //Use the storage key in web.config to construct URL
                src = ConfigurationManager.AppSettings["StorageUrl"];
                src += photo + "_t";
            }

            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", src);
            builder.MergeAttribute("width", width.ToString());
            builder.MergeAttribute("height", height.ToString());

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

        //public static HtmlString Avatar(this HtmlHelper htmlHelper, Profile profile)
        //{
        //    string photo = profile.GetFirstPhoto();

        //    return htmlHelper.Photo(photo, profile.Gender);
        //}
    }
}