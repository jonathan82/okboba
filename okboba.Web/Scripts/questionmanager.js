/*
 *  Author : Jonathan Lin
 *  Date   : 1/10/2016
 *  Notes  : Contains functions for answering and updating questions
 */
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
        answerMeTemplateSel: '#answerMeTemplate',
        answerOtherTemplateSel: '#answerOtherTemplate'
    }
    var questionTemplate,
        answerMeTemplate,
        answerOtherTemplate;

    /////////////////// Private functions ///////////////
    /*
     * Returns true if the index matches the acceptable of the answer
     */
    function isMatch(index, answer) {
        var bits;
        bits = (1 << (index - 1)) & answer.ChoiceAccept;
        return bits != 0;
    }

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

    function skipQuestion(form) {
        var data;
        data = form.serialize();
        return $.post(configMap.skipApi, data);
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

    function findAnswer(quesId) {
        var i, ques;
        ques = configMap.answeredQuestions;
        for (i = 0; i < ques.length; i++) {
            if (ques[i].Question.Id == quesId) return ques[i].Answer;
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


    /*
     * Reveals the other user's answer after I answer their question.
     * Renders my answer and their answer by replacing the given container.
     */
    function revealAnswer(ques, myAns, theirAns, cont) {
        var html;

        //reveal the other user's answer
        html = answerOtherTemplate.render({
            questionText: ques.Text,
            avatarMeUrl: configMap.avatarMeUrl,
            avatarThemUrl: configMap.avatarThemUrl,
            myAnswer: ques.Choices[myAns.ChoiceIndex - 1],
            theirAnswer: ques.Choices[theirAns.ChoiceIndex - 1],
            matchMe: isMatch(theirAns.ChoiceIndex, myAns),
            matchThem: isMatch(myAns.ChoiceIndex, theirAns)
        });

        cont.fadeTo('slow', 0, function () {
            cont.replaceWith(html);
        });        
    }

    /*
     * Show my answer by rendering into the DOM
     */
    function showMyAnswer(ques, ans, cont) {
        var html, choices = [];

        //Add the choices with their acceptability flags
        ques.Choices.forEach(function (curr, index) {
            choices.push({
                text: curr,
                isAccept: isMatch(index + 1, ans),
                isMyAnswer: index + 1 == ans.ChoiceIndex
            });
        });

        html = answerMeTemplate.render({
            questionText: ques.Text,
            choices: choices
        });

        cont.fadeTo('slow', 0, function () {
            cont.replaceWith(html);
        });
    }

    /*
     * Shows the next question by rendering it to the DOM.
     * Setup the vaidation and triggers, as well as handlers for
     * handling them.
     */
    function showNextQuestion(ques, cont) {
        var quesForm;

        quesForm = showQuestion(ques, cont, false);

        setupQuestionForm(quesForm);

        quesForm.on('question:submit', function () {
            answer(quesForm);
        });

        quesForm.on('question:skip', function () {
            skip(quesForm);
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

            showNextQuestion(nextQues, form);
            //nextQuesForm = showQuestion(nextQues, form, false);
            //setupQuestionForm(nextQuesForm);

            //nextQuesForm.on('question:submit', function () {
            //    answer(nextQuesForm);
            //});
            //nextQuesForm.on('question:skip', function () {
            //    skip(nextQuesForm);
            //});

        } else {
            //no more questions to show
            alert('no more questiosn to show!');
        }

        //submit the question
        submitQuestion(form, true).done(function (result) {
            configMap.nextQuestions = result;
        }).fail(function () {
            //show the previous questions
            alert('failed');
        });        
    }

    function skip(form) {
        var nextQues, nextQuesForm;

        //submit the question
        skipQuestion(form).done(function (result) {

            if(result==null || result.length < 1) {
                //no more questions to show
                alert('no more questions available!');
                return;
            }

            //load the next question an show it
            configMap.nextQuestions = result;

            nextQues = configMap.nextQuestions[0];

            showNextQuestion(nextQues, form);

        }).fail(function () {
            //show the previous questions
            alert('failed');
        });
    }

    function answerOrUpdate(arg, btn) {
        var quesId, ques, container, quesForm;

        quesId = $(btn).data('quesid');
        ques = findQuestion(quesId);
        container = $(btn).closest('.question-answered');

        quesForm = showQuestion(ques, container, true);

        setupQuestionForm(quesForm);

        quesForm.on('question:submit', function () {
            submitQuestion($(this), false).done(function (result) {

                if (arg == 'answer') {
                    revealAnswer(ques, result, findAnswer(quesId), quesForm);
                } else if (arg == 'update') {
                    showMyAnswer(ques, result, quesForm);
                } else {
                    //error
                }
                
            }).fail(function () {
                //don't do anything, just log error
                console.log('error answering question');
            });
        });
        quesForm.on('question:cancel', function () {
            //restore the element the question originally replaced
            container.css('opacity', 1);
            container.show();
            quesForm.remove();
        });
    }

    function initModule(config) {

        configMap = $.extend(configMap, config);

        //Load Templates
        questionTemplate = $.templates(configMap.questionTemplateSel);
        answerMeTemplate = $.templates(configMap.answerMeTemplateSel);
        answerOtherTemplate = $.templates(configMap.answerOtherTemplateSel);

        $('[data-toggle="answerother"]').click(function () { answerOrUpdate('answer', this) });
        $('[data-toggle="updateanswer"]').click(function () { answerOrUpdate('update', this) });

        //setup main question form
        setupQuestionForm($('.question-form'));
        $('.question-form').on('question:submit', function () {
            answer($(this));
        });
        $('.question-form').on('question:skip', function () {
            skip($(this));
        });

        //var savedTop = $('.question-stats').offset().top;

        ////setup question stat sticky box
        //$(window).scroll(function () {
        //    var divTop, scrollTop;
        //    divTop = $('.question-stats').offset().top;
        //    scrollTop = $(window).scrollTop();
        //    if (scrollTop < savedTop) {
        //        $('.question-stats').css('position', 'static');
        //        return;
        //    }
        //    if (divTop - scrollTop != 100) {
        //        //set div to new position
        //        $('.question-stats').offset({top: scrollTop + 100})
        //    }
        //});
    }

    //////////////// Public API ///////////////////////
    return {
        initModule: initModule
    }

})();