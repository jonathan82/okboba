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
    var answerUrl = "/question/answer",
        skipUrl = "/question/skip",
        $questionForm,
        questionTemplate,
        answerTemplate,
        currentQuestion,
        nextQuestion;

    //// Private functions
    function showQuestion(ques) {
        var html;

        html = questionTemplate.render(ques);

        //Fade out the old question and fade in the new one. We use fadeTo to fade to 0
        //without removing it from the DOM to prevent "screen jumping".  Likewise we fade
        //in the new div by inserting it as completetly transparent first.
        $questionForm.fadeTo('slow', 0, function () {
            var newDiv = $(html).css('opacity', 0);
            $(this).replaceWith(newDiv);
            newDiv.fadeTo('slow', 1);

            $questionForm = newDiv;

            //Setup the validation and submit handlers
            //pass false to questionSubmit since we don't want a full initialization
            $questionForm.questionValidation();
            $questionForm.questionSubmit(false);
        });
    }

    function skip (e) {
        var data,
            html;

        e.preventDefault();

        data = $questionForm.serialize();

        $.post(skipUrl, data, function success(nextQuestions) {

            //update pointers
            currentQuestion = nextQuestion;
            nextQuestion = nextQuestions[1];

            //show the next question
            showQuestion(currentQuestion);

        }).fail(function (xhr, textStatus, errorThrown) {

            alert('error submitting question');

            //log error on console
            console.log('answer question failed: ' + textStatus);
            console.log(xhr);
            console.log(textStatus);
            console.log(errorThrown);
        });
    }

    function submit(e) {
        var data,
            html,
            failed = false;

        e.preventDefault();

        data = $questionForm.serialize();

        //Submit question
        $.post(answerUrl, data, function (nextQuestions) {

            //success - update next and current questions
            currentQuestion = nextQuestion;
            nextQuestion = nextQuestions[1];

        }).fail(function (xhr, textStatus, errorThrown) {

            //we set the failed flag in case we reach here before optimisticlly showing the next question
            failed = true;

            //failure - show the previous question (in this case currentQuestion since it hasn't been updated)
            showQuestion(currentQuestion);

            //log error on console
            console.log('answer question failed: ' + textStatus);
            console.log(xhr);
            console.log(textStatus);
            console.log(errorThrown);
        });

        //Optimistically show next question
        if (!failed) {
            showQuestion(nextQuestion);
        }        
    }

    //// Public functions (exposed thru jQuery)

    /*
     * Initializes the plugin for the form by setting up the event handlers. If initFlag
     * is true we do some addtional initialization like loading the templates and setting the initial
     * values for current and next questions. Subsequent requests to update the current/next question
     * pointers will be handled by the submit/skip functions. initFlag should be set to true the first
     * time page is loaded, false other times.
     */
    $.fn.questionSubmit = function (initFlag) {

        $questionForm = this;        
        
        //setup the submit and skip event handlers
        $questionForm.find('button.btn-primary').click(submit);
        $questionForm.find('button.btn-default').click(skip);

        if (initFlag) {
            questionTemplate = $.templates('#questionTemplate');
            answerTemplate = $.templates('#answerTemplate');

            //NextQuestions is a global variable returned by the first load of the Question page.
            currentQuestion = NextQuestions[0];
            nextQuestion = NextQuestions[1];
        }
    }

})(jQuery);