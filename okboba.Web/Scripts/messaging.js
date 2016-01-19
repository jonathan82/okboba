/*
 *  Author : Jonathan Lin
 *  Date   : 1/5/2016
 *  Notes  : This is the module for handling messaging functions.  It handles the following features
 *           across multiple pages:
 * 
 *             - Composing new message when user cicks "Message" button on profile page
 *                 [data-toggle=compose]
 * 
 *             - Replying to messages in the conversation view
 *                 [data-toggle=reply]
 * 
 *             - Deleting messages in the inbox
 *                 [data-toggle=deleteconversation]
 * 
 *             - Loading previous messages in Conversation view
 *                 [data-toggle=loadpreviousmessages]
 */
var messaging = (function ($) {

    /////// Module scope variables /////////
    var configMap = {
        initialLow: 0, //initial value for the number of messages already loaded
        convId: 0,     //conversation Id for the reply and load functions
        msgContainerSel: '', //where to load new messages
        replyBoxSel: '', //textarea where to get send message from
        me: null,      //my profile Id
        other: null,   //the other person's profile Id
        avatarMeUrl: '',
        avatarOtherUrl: '',
        msgRowTemplateSel: '#messageRowTemplate',
        composeTemplateSel: '#composeTemplate',
        getPreviousApi: '/messages/previous',
        replyApi: '/messages/reply',
        startConvApi: '/messages/startconversation',
        deleteApi: '/messages/delete',
        fadeInTime: 1500, //ms to fade in new messages
        emoticonSmilies: ['1f600', '1f601', '1f602', '1f603', '1f604', '1f605', '1f606', '1f607', '1f608', '1f609', '1f60a', '1f60b', '1f60c', '1f60d', '1f60e', '1f60f', '1f610', '1f611', '1f612', '1f613', '1f614', '1f615', '1f616', '1f617', '1f618', '1f619', '1f61a', '1f61b', '1f61c', '1f61d', '1f61e', '1f61f', '1f620', '1f621', '1f622', '1f623', '1f624', '1f625', '1f626', '1f627', '1f628', '1f629', '1f62a', '1f62b', '1f62c', '1f62d', '1f62e', '1f62f', '1f630', '1f631', '1f632', '1f633', '1f634', '1f635', '1f636', '1f637', '1f638', '1f639', '1f63a', '1f63b', '1f63c', '1f63d', '1f63e', '1f63f', '1f640'],
        emoticonFood: ['1f354', '1f355', '1f356', '1f357', '1f358', '1f359', '1f35a', '1f35b', '1f35c', '1f35d', '1f35e', '1f35f', '1f360', '1f361', '1f362', '1f363', '1f364', '1f365', '1f366', '1f367', '1f368', '1f369', '1f36a', '1f36b', '1f36c', '1f36d', '1f36e', '1f36f', '1f370', '1f371', '1f372', '1f373', '1f374', '1f375', '1f376', '1f377', '1f378', '1f379', '1f37a', '1f37b'],
        emoticonLove: ['1f46a', '1f46b', '1f46c', '1f46d', '1f46e', '1f46f', '1f470', '1f471', '1f472', '1f473', '1f474', '1f475', '1f476', '1f477', '1f478', '1f479', '1f47a', '1f47b', '1f47c', '1f47d', '1f47e', '1f47f', '1f480', '1f481', '1f482', '1f483', '1f484', '1f485', '1f486', '1f487', '1f488', '1f489', '1f48a', '1f48b', '1f48c', '1f48d', '1f48e', '1f48f', '1f490', '1f491', '1f492', '1f493', '1f494', '1f495', '1f496', '1f497', '1f498', '1f499', '1f49a', '1f49b', '1f49c', '1f49d', '1f49e', '1f49f'],
        emoticonUrl: 'http://twemoji.maxcdn.com/36x36/',
        emoticonUrlSmall: 'http://twemoji.maxcdn.com/16x16/'
    }
    var _numMessagesLoaded,
        _msgRowTemplate,
        _composeTemplate,
        _msgContainer,
        _replyBox;

    /////// Private Methods ////////////////
    function composeHandler() {
        var html, win;

        html = _composeTemplate.render({
            nickname: $(this).data('name'),
            id: $(this).data('id') //profile Id of the "to" user
        });

        //add to body element
        win = $(html);        
        $('body').append(win);

        //close
        win.find('.compose-close').click(function () {
            win.remove();
        });

        //send
        win.find('.send-button').click(function (e) {
            var subj, msg, postData, sendBtn;

            e.preventDefault();

            sendBtn = $(this);

            subj = win.find('#subject').val().trim();
            msg = win.find('#message').val().trim();

            if (subj == '' || msg == '') {
                // don't submit empty form
                return false;
            }

            //set button loading state
            sendBtn.ladda().ladda('start');

            //ajax post
            postData = win.find('#compose-form').serialize();

            $.post(configMap.startConvApi, postData, function () {
                //show confirmation message
                win.find('#compose-form').fadeTo('slow', 0, function () {
                    win.find('.compose-confirmation-wrapper').fadeIn('slow');
                });
            }).fail(function () {
                alert('failed');
            }).always(function () {
                sendBtn.ladda().ladda('stop');
            });
        });
    }

    function replyHandler() {
        var txt, button;
       
        //get the message to send from the textarea
        txt = trimBr(_replyBox.val());
        if (txt == '') return;

        //set loading state on button
        button = $(this);
        button.ladda().ladda('start');

        //make ajax call to reply to message
        $.post(configMap.replyApi, { convId: configMap.convId, message: txt }, function () {
            var html;

            html = _msgRowTemplate.render({
                avatarUrl: configMap.avatarMeUrl,
                messageText: txt,
                isMe: true,
                friendlyTime: '现在！'
            });

            //fade in new message
            $(html).hide().appendTo(_msgContainer).fadeIn(configMap.fadeInTime);

            //clear old message
            _replyBox.val('');

        }).fail(function (jqxhr, status, error) {
            alert('failed: ' + status + ' : ' + error);
        }).always(function () {
            button.ladda().ladda('stop');
        });
    }

    function deleteConvHandler(e) {
        var name, convId;

        //prevent from following link
        e.stopPropagation();

        name = $(this).data('name');

        if (confirm('Delete conversation with ' + name + '?')) {

            convId = $(this).data('id');

            //make ajax call to delete conversation
            $.post(configMap.deleteApi, { convId: convId }, function () {
                //success
            }).fail(function myfunction() {
                alert('failed');
            });

            //optimistically remove
            $(this).closest('.conv-row').fadeOut('fast', function () {
                $(this).remove();
            });
        } 
    }

    function loadPreviousHandler() {
        var button = $(this);

        //change button to loading state
        button.ladda().ladda('start');

        //make ajax call to next page of previous matches
        $.getJSON(configMap.getPreviousApi, { low: _numMessagesLoaded, convId: configMap.convId }, function success(result) {

            var i, html = "", isMe, avatarUrl, msg;

            //if empty result then remove load button
            if (result.length == 0) {
                $('.msg-load-previous').remove();
                button = null;
                return;
            }

            //prepend messages
            for (i = 0; i < result.length; i++) {

                msg = result[i];

                isMe = msg.From == configMap.me;

                avatarUrl = isMe ? configMap.avatarMeUrl : configMap.avatarOtherUrl;

                html += _msgRowTemplate.render({
                    avatarUrl: avatarUrl,
                    messageText: msg.MessageText,
                    isMe: isMe,
                    friendlyTime: msg.Timestamp
                });
            }

            //fade in previous messages
            $(html).hide().prependTo(_msgContainer).fadeIn(configMap.fadeInTime);

            //update the mesages loaded count
            _numMessagesLoaded += result.length;

        }).fail(function () {
            alert('error');

        }).done(function () {
            if (button == null) return;
            button.ladda().ladda('stop');
        });
    }

    /////// Public Methods /////////////////
    function initModule(config) {

        config = $.extend(configMap, config);

        _numMessagesLoaded = configMap.initialLow;

        //load DOM elements
        _msgContainer = $(configMap.msgContainerSel);
        _replyBox = $(configMap.replyBoxSel);

        //load templates
        _msgRowTemplate = $.templates(configMap.msgRowTemplateSel);
        _composeTemplate = $.templates(configMap.composeTemplateSel);

        //setup click handler for conversation row
        $('.conv-row').click(function () {
            var link = $(this).data('link');
            window.location.href = link;
        });

        $('[data-toggle="compose"]').click(composeHandler);
        $('[data-toggle="reply"]').click(replyHandler);
        $('[data-toggle="deleteconversation"]').click(deleteConvHandler);
        $('[data-toggle="loadpreviousmessages"]').click(loadPreviousHandler);
    }

    /////// Return the public API //////////
    return {
        initModule: initModule
    }

})(jQuery);