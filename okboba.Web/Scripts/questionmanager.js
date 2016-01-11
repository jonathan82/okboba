var QuestionManager = (function () {

    ///////////////// Module vars ///////////////////
    var configMap = {
        avatarMeUrl: '',
        avatarThemUrl: '',
        answeredQuestions: [],
        nextQuestions: [],
        answerApi: '/question/answer',
        skipApi: '/question/skip',
        questionTemplateSel: '#questionTemplate',
        answerMeTemplateSel: '',
        answerOtherTemplateSel: ''
    }
    var questionTemplate,
        answerMeTemplate,
        answerOtherTemplate;

    /////////////////// Private functions ///////////////
    /*
     * Setup the question form validation and submit logic. The question form will 
     * fire the following events:
     * 
     *   - question:submit
     *   - question:skip
     *   - question:cancel
     */
    function setupQuestionForm(form) {
        //setup question validation
        form.questionValidation();

        //setup submit, skip, and cancel handlers
        form.find('button.question-submit').click(function (e) {
            e.preventDefault();
            form.trigger('question:submit');
        });
        form.find('button.question-skip').click(function (e) {
            e.preventDefault();
            form.trigger('question:skip');
        });
        form.find('button.question-cancel').click(function (e) {
            e.preventDefault();
            form.trigger('question:cancel');
        });
    }

    /*
     * Submits the question using AJAX. If getNextFlag = true gets the next 2 questions.
     * returns a jquery deferred object.
     */
    function submitQuestion(form, nextFlag) {
        var data;

        data = form.serialize();
        data += '&getNextFlag=' + nextFlag;

        //return jquery deferred object
        return $.post(configMap.answerApi, data);
    }

    /*
     * Finds the the question in the answeredQuestions array given the question id
     */
    function findQuestion(quesId) {
        var i, ques;
        ques = configMap.answeredQuestions;
        for (i = 0; i < ques.length; i++) {
            if (ques[i].Question.Id == quesId) return ques[i].Question;
        }
        return null;
    }

    /*
     * Renders the question to the given container by replacing it or hiding it.
     * 
     *   - If isCancelable = true then hide the container instead of destroying it. That
     *     way we can restore it later.  Question form will have "Cancel" button
     */
    function showQuestion(ques, container, isCancelable) {
        var html, newQuestion, savedElement;

        ques.isCancelable = isCancelable;
        html = questionTemplate.render(ques);

        newQuestion = $(html);

        //Fade to 0 without removing to prevent "screen jumping"
        container.fadeTo('slow', 0, function () {
            
            if (isCancelable) {
                savedElement = this;
                $(this).hide();
                $(this).after(newQuestion);         
            }
            else {
                newQuestion.css('opacity', 0);
                $(this).replaceWith(newQuestion);
                newQuestion.fadeTo('slow', 1);
            }                        
        });

        return newQuestion;
    }
    
    function showNextQuestion(nextQues, prevQuesForm) {
        var nextQuesForm;

        nextQuesForm = showQuestion(nextQues, prevQuesForm, false);

        setupQuestionForm(nextQuesForm);

        nextQuesForm.on('question:submit', function () {
            answer(nextQuesForm);
        });
    }

    /*
     * Handler for when the user answers the main question form. Gets the next 2 questions
     * and shows the next question optimistically.
     */
    function answer(form) {
        var nextQues, nextQuesForm;

        //optimistically show the next question
        if (configMap.nextQuestions.length >= 2) {

            //we have more questions to show
            nextQues = configMap.nextQuestions[1];
            nextQuesForm = showQuestion(nextQues, form, false);
            setupQuestionForm(nextQuesForm);

            nextQuesForm.on('question:submit', function () {
                answer(nextQuesForm);
            });
            nextQuesForm.on('question:skip', function () {
                skip(nextQuesForm);
            });

        } else {
            //no more questions to show
            alert('no more questiosn to show!');
        }

        submitQuestion(form, true).done(function (result) {
            configMap.nextQuestions = result;
        }).fail(function () {
            //show the previous questions
            alert('failed');
        });        
    }

    function skip() {
        alert('skip');
    }

    function answerOther() {
        var quesId, ques, container, quesForm;

        quesId = $(this).data('quesid');
        ques = findQuestion(quesId);
        container = $(this).closest('.question-answered');

        quesForm = showQuestion(ques, container, true);

        setupQuestionForm(quesForm);

        quesForm.on('question:submit', function () {
            submitQuestion($(this), false); //don't get next questions

            //reveal the other users answer

        });
        quesForm.on('question:cancel', function () {
            //restore the element the question originally replaced
            container.css('opacity', 1);
            container.show();
            quesForm.remove();
        });
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

        //setup main question form
        setupQuestionForm($('.question-form'));
        $('.question-form').on('question:submit', function () {
            answer($(this));
        });
        $('.question-form').on('question:skip', function () {
            skip($(this));
        });
    }

    //////////////// Public API ///////////////////////
    return {
        initModule: initModule
    }

})();