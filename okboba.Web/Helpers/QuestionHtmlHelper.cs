using okboba.Entities;
using okboba.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace okboba.Web.Helpers
{
    public static class QuestionHtmlHelper
    {
        private static Answer GetMutualAnswer(short questionId, Dictionary<short, Answer> compare)
        {
            Answer compareAnswer;
            if (compare != null && compare.TryGetValue(questionId, out compareAnswer))
            {
                return compareAnswer;
            }
            return null;
        }

        private static void AcceptableChoice(TagBuilder tag, int index, byte choiceAccept)
        {
            if (((1 << (index - 1)) & choiceAccept) == 0)
            {
                tag.AddCssClass("question-choice-unacceptable");
            }
        }

        private static void ChosenChoice(TagBuilder tag, int index, Answer answer)
        {
            if (index != answer.ChoiceIndex)
            {
                tag.AddCssClass("question-choice-fade");
            }
            else
            {
                tag.AddCssClass("question-choice-bold");
            }
        }

        private static void MatchChoice(TagBuilder tag, int index, byte choiceAccept)
        {
            if (((1 << (index - 1)) & choiceAccept) == 0)
            {
                tag.AddCssClass("question-choice-nomatch");
            }
            else
            {
                tag.AddCssClass("question-choice-match");
            }
        }

        private static void ChoicePart1(TagBuilder tag, Answer answer, Answer compareAnswer, int index)
        {
            ChosenChoice(tag, index, answer);

            if (compareAnswer != null)
            {
                //we both answerd this question - add comparison classes
                AcceptableChoice(tag, index, compareAnswer.ChoiceAccept);
                if (answer.ChoiceIndex == index) MatchChoice(tag, index, compareAnswer.ChoiceAccept);
            }
        }

        private static void ChoicePart2(TagBuilder tag, Answer answer, Answer compareAnswer, int index)
        {
            AcceptableChoice(tag, index, answer.ChoiceAccept);

            if (compareAnswer != null)
            {
                //we both answered this question - add comparison classes
                if (compareAnswer.ChoiceIndex == index) MatchChoice(tag, index, answer.ChoiceAccept);
            }
        }

        public static HtmlString Choices(this HtmlHelper htmlHelper, QuestionAnswerModel qa, Dictionary<short, Answer> compare, bool isFirstPart)
        {
            var html = "";
            var compareAnswer = GetMutualAnswer(qa.Question.Id, compare);

            for (int i = 0; i < qa.Question.Choices.Count; i++)
            {
                var tag = new TagBuilder("span");

                if (isFirstPart)
                {
                    ChoicePart1(tag, qa.Answer, compareAnswer, i + 1);
                }
                else
                {
                    ChoicePart2(tag, qa.Answer, compareAnswer, i + 1);
                }

                tag.InnerHtml = qa.Question.Choices[i];
                html += "<li>" + tag.ToString() + "</li>";
            }

            return new HtmlString(html);
        }

        public static HtmlString Choice(this HtmlHelper htmlHelper, QuestionAnswerModel qa, Dictionary<short, Answer> compare)
        {
            var html = "";
            var compareAnswer = GetMutualAnswer(qa.Question.Id, compare);

            for (int i = 0; i < qa.Question.Choices.Count; i++)
            {
                var tag = new TagBuilder("span");

                ChosenChoice(tag, i + 1, qa.Answer);

                if (compareAnswer != null)
                {
                    //we both answerd this question - add comparison classes
                    AcceptableChoice(tag, i + 1, compareAnswer.ChoiceAccept);
                    if (qa.Answer.ChoiceIndex == i + 1) MatchChoice(tag, i + 1, compareAnswer.ChoiceAccept);
                }

                tag.InnerHtml = qa.Question.Choices[i];
                html += "<li>" + tag.ToString() + "</li>";
            }

            return new HtmlString(html);
        }

        public static HtmlString ChoiceAcceptable(this HtmlHelper htmlHelper, QuestionAnswerModel qa, Dictionary<short, Answer> compare)
        {
            var html = "";
            var compareAnswer = GetMutualAnswer(qa.Question.Id, compare);

            for (int i = 0; i < qa.Question.Choices.Count; i++)
            {
                var tag = new TagBuilder("span");

                AcceptableChoice(tag, i + 1, qa.Answer.ChoiceAccept);

                if (compareAnswer != null)
                {
                    //we both answered this question - add comparison classes
                    if (compareAnswer.ChoiceIndex == i + 1) MatchChoice(tag, i + 1, qa.Answer.ChoiceAccept);
                }

                tag.InnerHtml = qa.Question.Choices[i];
                html += "<li>" + tag.ToString() + "</li>";
            }

            return new HtmlString(html);
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        public static HtmlString ShowAnswer(this HtmlHelper htmlHelper, Answer answer, Answer otherAnswer, IList<string> choices)
        {
            if (answer.ChoiceIndex == null || otherAnswer.ChoiceIndex == null) return new HtmlString("INVALID ANSWER");
            var tag = new TagBuilder("span");
            tag.AddCssClass(answer.IsMatch(otherAnswer) ? "question-choice-green" : "question-choice-red");
            tag.InnerHtml = choices[((int)answer.ChoiceIndex - 1) % choices.Count];
            return new HtmlString(tag.ToString());
        }

        public static HtmlString ShowAnswersMe(this HtmlHelper htmlHelper, Answer answer, IList<string> choices)
        {
            var html = "";

            for (int i = 1; i <= choices.Count; i++)
            {
                var tag = new TagBuilder("li");
                tag.AddCssClass(answer.ChoiceIndex == i ? "question-choice-check" : "question-choice-bullet");
                tag.AddCssClass(answer.IsMatch(i) ? "question-choice-green" : "question-choice-strike");
                tag.InnerHtml = choices[i - 1];

                html += tag.ToString();
            }

            return new HtmlString(html);
        }
    }
}