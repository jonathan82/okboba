var QuestionManager = (function () {

    ///////////////// Module vars ///////////////////
    var configMap = {
        avatarMeUrl: '',
        avatarThemUrl: '',
        answeredQuestions: [],
        nextQuestions: [],
        questionTemplateSel: '#questionTemplate',
        answerMeTemplateSel: '',
        answerOtherTemplateSel: ''
    }
    var questionTemplate,
        answerMeTemplate,
        answerOtherTemplate;

    /////////////////// Private functions ///////////////
    /*
     * Renders the question to the given container by replace it in the DOM.
     * Fades out the container and fades in the question. Setup validation
     * on question form. Fires "question:submit" event when question is submitted.
     */
    function showQuestion(ques, container) {
        var html;

        html = questionTemplate.render(ques);

        container.fadeTo('slow', 0, function () {
            var newQuestion;
            
            //Fade to 0 without removing to prevent "screen jumping"
            newQuestion = $(html).css('opacity', 0);
            $(this).replaceWith(newQuestion);
            newQuestion.fadeTo('slow', 1);

            //setup question validation
            newQuestion.questionValidation();

            //setup submit and skip question handlers
            newQuestion.find('button.question-submit').trigger('question:submit');
            newQuestion.find('button.question-skip').trigger('question:skip');
        });
    }

    function answerOther() {

    }

    function updateAnswer() {

    }

    function initModule(config) {

        configMap = $.extend(configMap, config);

        //Load Templates
        questionTemplate = $.templates(configMap.questionTemplateSel);
        answerMeTemplate = $.templates(configMap.answerMeTemplateSel);
        answerOtherTemplate = $.templates(configMap.answerOtherTemplateSel);

        $('[data-toggle="answerother"]').click(answerOther);
        $('[data-toggle="updateanswer"]').click(updateAnswer);
    }

    //////////////// Public API ///////////////////////
    return {
        initModule: initModule
    }

})();