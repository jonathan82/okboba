/*
 *  Plugin : Handles the submission of the question. Uses jsRender to render the submitted question
 *           optimistically and insert or update the DOM. After submission immediately show the next question
 *           while waiting for response from server for the next 2 questions.
 * 
 *  Author : Jonathan Lin
 *  Date   : 12/20/2015
 */
(function ($) {

    //// Private vars
    var url = "/question/answer",
        $questionForm,
        questionTemplate,
        answerTemplate;

    //// Private functions
    function submit(e) {
        var data,
            html;

        e.preventDefault();

        data = $(this).serialize();

        //Submit question
        $.post(url, data, function (nextQuestions) {
            //success
            console.log(nextQuestions);
        }).fail(function () {
            //failure
            alert('failed');
        });

        //Optimistically show next question - fade in
        html = questionTemplate.render(NextQuestions[1]);

        $questionForm.fadeTo('slow', 0, function () {
            var div = $(html).css('opacity', 0);
            $(this).replaceWith(div);
            div.fadeTo('slow',1);
            $(this).replaceWith(html);
            $questionForm = div;
            $questionForm.questionValidation();
        });

        //Optimistically add question to list of answered questions
        var html2 = answerTemplate.render();
        $('#question-separator').after(html2);

        console.log('form: ' + data);
    }

    //// Public functions (exposed thru jQuery)
    $.fn.questionSubmit = function () {

        $questionForm = this;
        questionTemplate = $.templates('#questionTemplate');
        answerTemplate = $.templates('#answerTemplate');

        $questionForm.submit(submit);
    }

})(jQuery);