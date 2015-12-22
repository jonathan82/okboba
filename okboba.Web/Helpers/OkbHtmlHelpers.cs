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

        

        //public static HtmlString Choice(this HtmlHelper htmlHelper, QuestionAnswerModel qa, Dictionary<short, Answer> compare, bool isFirst)
        //{
        //    var html = "";
        //    var compareAnswer = GetMutualAnswer(qa.Question.Id, compare);

        //    if (compareAnswer != null)
        //    {
        //        //we both answered this questions
        //        for (int i = 0; i < qa.Question.Choices.Count; i++)
        //        {
        //            var tag = new TagBuilder("span");
        //            if(isFirst) ChosenChoice(tag, i + 1, qa.Answer); //we're in the first part of the question - the chosen choice
        //            AcceptableChoice(tag, i + 1, isFirst ? compareAnswer.ChoiceAccept : qa.Answer.ChoiceAccept);
        //            if (isFirst)
        //            {
        //                if (qa.Answer.ChoiceIndex == (i + 1)) MatchChoice(tag, i + 1, compareAnswer.ChoiceAccept);
        //            }
        //            else
        //            {
        //                if (compareAnswer.ChoiceIndex == (i + 1)) MatchChoice(tag, i + 1, qa.Answer.ChoiceAccept);
        //            }                    
        //            tag.InnerHtml = qa.Question.Choices[i];
        //            html += "<li>" + tag.ToString() + "</li>";
        //        }
        //    }
        //    else
        //    {
        //        //viewing own question or non mutually answered question
        //        for (int i = 0; i < qa.Question.Choices.Count; i++)
        //        {
        //            var tag = new TagBuilder("span");
        //            if (isFirst)
        //            {
        //                ChosenChoice(tag, i + 1, qa.Answer);
        //            }
        //            else
        //            {
        //                AcceptableChoice(tag, i + 1, qa.Answer.ChoiceAccept);
        //            }
        //            tag.InnerHtml = qa.Question.Choices[i];
        //            html += "<li>" + tag.ToString() + "</li>";
        //        }
        //    }

        //    return new HtmlString(html);
        //}

        //public static HtmlString Choice(this HtmlHelper htmlHelper, string choiceText, int index, Answer ans, Dictionary<short,Answer> compare )
        //{
        //    var html = "";
        //    var className = "";
        //    Answer compareAnswer;

        //    if (ans.ChoiceIndex != index)
        //    {
        //        //not the chosen answer. show faded text
        //        className += "question-choice-fade ";
        //    }

        //    if (compare != null && compare.TryGetValue(ans.QuestionId, out compareAnswer))
        //    {
        //        //we're viewing other's question and we both answered this question
        //        //compare answers for acceptability
        //        bool accept = true;

        //        if((compareAnswer.ChoiceAccept & (1 << (index - 1))) == 0 )
        //        {
        //            //the choice we're on is unacceptable to the other person
        //            className += "question-choice-unacceptable "; //strikethrough
        //            accept = false;
        //        }

        //        if (index==ans.ChoiceIndex)
        //        {
        //            //we're on the choice that the user made. show red or green depending on
        //            //whether answer is acceptable (match or no match)
        //            if (accept)
        //            {
        //                className += "question-choice-match "; //green
        //            }
        //            else
        //            {
        //                className += "question-choice-nomatch "; //red
        //            }
        //        }
        //    }

        //    html = "<li><span class=\""+className+"\">"+choiceText+"</span></li>";

        //    return new HtmlString(html);
        //}

        //public static HtmlString ChoiceAccept(this HtmlHelper htmlHelper, string choiceText, int index, Answer ans, Dictionary<short, Answer> compare)
        //{
        //    var className = "";
        //    bool accept = true;
        //    Answer compareAnswer;       

        //    if (((1 << (index -1)) & ans.ChoiceAccept) == 0)
        //    {
        //        //the choice we're on is unacceptable
        //        className += "question-choice-unacceptable "; //strikethrough
        //        accept = false;
        //    }

        //    if (compare != null && 
        //        compare.TryGetValue(ans.QuestionId, out compareAnswer) && 
        //        compareAnswer.ChoiceIndex == index)
        //    {
        //        //we're comparing answers and we both answered this question and 
        //        //we're on the choice that the other person answered
        //        if (accept)
        //        {
        //            //compare answer is acceptable to this person

        //        }
        //        else
        //        {
        //            //compare answer is un-acceptable to this person
        //        }
        //    }

        //    var html = "<li><span class=\""+className+"\">"+choiceText+"</span></li>";

        //    return new HtmlString(html);
        //}
    }
}