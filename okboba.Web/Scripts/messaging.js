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
        getPreviousApi: '/messages/previous',
        replyApi: '/messages/reply',
        deleteApi: '/messages/delete',
        fadeInTime: 1500 //ms to fade in new messages
    }
    var _numMessagesLoaded,
        _msgRowTemplate,
        _msgContainer,
        _replyBox;

    /////// Private Methods ////////////////
    function composeHandler() {

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
                friendlyTime: 'just now'
            });

            //fade in new message
            $(html).hide().appendTo(_msgContainer).fadeIn(configMap.fadeInTime);

        }).fail(function (jqxhr, status, error) {
            alert('failed: ' + status + ' : ' + error);
        }).done(function () {
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
            $(this).closest('.conv-row').fadeOut(configMap.fadeInTime, function () {
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