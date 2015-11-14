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

        public static HtmlString Photo(this HtmlHelper htmlHelper, string photo, byte gender)
        {
            string src;

            if (photo == "")
            {
                //Use one of the default avatars
                src = "~/Content/images/";
                src += gender == OkbConstants.MALE ? "no-avatar-male.png" : "no-avatar-female.png";
            }
            else
            {
                //Use the storage key in web.config to construct URL
                src = ConfigurationManager.AppSettings["StorageUrl"];
                src += photo + "_t";
            }

            return new HtmlString(src);
        }

        public static HtmlString Avatar(this HtmlHelper htmlHelper, Profile profile)
        {
            string photo = profile.GetFirstPhoto();

            return htmlHelper.Photo(photo, profile.Gender);
        }
    }
}