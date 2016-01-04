////////////////////////////////////////////////////////////////////
//
// ChatSlider: 
//  Represents the chat window on screen and contains all the functions
//  for a single chat window. Assumes jQuery and jsrender is available.
//
// Events:
//  chat:close      - fired when "close" button is clicked
//  chat:foreground - fired when anywhere on chat slider is clicked on
//
////////////////////////////////////////////////////////////////////
function ChatSlider($container, config) {

    //// Private vars
    var configMap = {
        chatSliderTemplate: null,
        chatDialogThemTemplate: null,
        chatDialogMeTemplate: null,
        initialZIndex: 100,
        minHeight: 200,
        minWidth: 250,
        sliderClosedHeight: 31,
        emoticonUrl: 'http://twemoji.maxcdn.com/36x36/',
        emoticonUrlSmall: 'http://twemoji.maxcdn.com/16x16/',
        emoticonPages: null, //an array of objects (emoticon pages) with an array of emoticons and their nav icon
        title: '',
        profileId: 0,
        convId: null,
        avatarUrl: '',
        sendMessageCallback: null
    }
    var $chat,
        that,
        savedHeight,
        loading = true;

    that = this;

    //// Merge the config options
    configMap = $.extend(configMap, config);

    //// Public functions
    this.setPosition = function (right) {
        $chat.css('right', right);
    }

    this.getWidth = function () {
        return $chat.width();
    }

    this.remove = function () {
        $chat.remove();
    }

    this.addStatusMessage = function (msg) {
        var div, msgContainer;
        div = '<div><div class="chat-status-message">' + msg + '</div></div>';
        msgContainer = $chat.find('.chat-messages-inner');
        msgContainer.append(div);
    }

    this.addMessage = function (msg, from) {
        var dialogHtml, msgDiv, isMe;

        isMe = configMap.profileId != from;

        if (isMe) {
            dialogHtml = configMap.chatDialogMeTemplate.render({ message: msg });
        } else {
            dialogHtml = configMap.chatDialogThemTemplate.render({ message: msg, avatarUrl: configMap.avatarUrl });
        }

        msgDiv = $chat.find('.chat-messages-inner');

        msgDiv.append(dialogHtml);

        //scroll to bottom of messages
        //msgDiv.animate({ scrollTop: msgDiv.prop("scrollHeight") }, 0);
    }

    this.getZIndex = function () {
        return parseInt($chat.css('z-index'));
    }

    this.setZIndex = function (z) {
        $chat.css('z-index', z);
    }

    this.getProfileId = function () {
        return configMap.profileId;
    }

    this.setConversationId = function (id) {
        configMap.convId = id;
    }

    this.scrollBottom = function () {
        var div = $chat.find('.chat-messages-inner');
        div.scrollTop(div.prop('scrollHeight'));
    }

    /*
     * info = {
     *   ConversationId: integer nullable
     *   Profile: Profile
     *   Messages: [array of Message] 
     *   AvatarUrl: string
     *   IsOnline: boolean
     * }
     *   - load avatar url
     *   - load messages
     *   - set conversation Id
     */
    this.loadInitialInfo = function (info) {
        var i, msg;

        //load the profile info of the other person and conversation id
        configMap.avatarUrl = info.AvatarUrl;
        configMap.convId = info.ConversationId;

        if (info.Messages != null) {
            //load the messages - in reverse order
            for (i = info.Messages.length - 1; i >= 0; i--) {
                msg = info.Messages[i];
                this.addMessage(msg.MessageText, msg.From);
            }
        }
        
        //show online status of user if they're offline
        if (!info.IsOnline) {
            this.addStatusMessage('User offline. Messages will be delivered to inbox.');
        }

        this.scrollBottom();
    }

    this.setLoadingState = function (state) {
        loading = state;
    }

    //// Private Functions
    function isMinimized() {
        return $chat.height() == configMap.sliderClosedHeight;
    }

    function togglePosition() {
        var $icon;
        if ($chat.height() == configMap.sliderClosedHeight) {
            //open
            $chat.height(savedHeight);
            $icon = $chat.find('.glyphicon-chevron-up');
            $icon.removeClass('glyphicon-chevron-up').addClass('glyphicon-chevron-down');
        } else {
            //close
            savedHeight = $chat.height(); //save the current height
            $chat.height(configMap.sliderClosedHeight);
            $icon = $chat.find('.glyphicon-chevron-down');
            $icon.removeClass('glyphicon-chevron-down').addClass('glyphicon-chevron-up');
        }
    }

    function emoticonChooserHtml(emoticons, url) {
        var i,
            html = '<ul class="chat-emoticon-chooser">';
        for (i = 0; i < emoticons.length; i++) {
            html += '<li data-id="' + emoticons[i] + '"><img src="' + url + emoticons[i] + '.png" alt="Emoticon" width="25" height="25" /></li>';
        }
        html += '</ul>';
        return html;
    }

    function insertEmoticon() {
        var id;
        id = $(this).data('id');
        $chat.find('.chat-input').focus();
        //$chat.find('.chat-input-placeholder').hide();
        html = '<img src="' + configMap.emoticonUrl + id + '.png" class="emoji" alt="' + id + '" />';
        pasteHtmlAtCaret(html);
    }

    function sanitizeHtml(html) {
        var re;
        //convert emoji to unicode
        re = /<img.*?data-id="(.*?)".*?>/mg;
        //trim whitespace
    }

    function clearInput() {
        $chat.find('.chat-input').html('');
    }

    //// Constructor logic
    //create chat window from template and add to DOM container
    $chat = $(configMap.chatSliderTemplate.render({
        emoticonUrl: configMap.emoticonUrl,
        emoticonUrlSmall: configMap.emoticonUrlSmall,
        emoticons: configMap.emoticonPages[0].emoticons,
        nickname: configMap.title
    }));
    $container.append($chat);
    savedHeight = $chat.height();
    this.setZIndex(configMap.initialZIndex);
    $chat.find('.chat-emoticon-chooser').children('li').click(insertEmoticon);

    //Enable resizable 
    $chat.resizable({
        minWidth: configMap.minWidth,
        minHeight: configMap.minHeight,
        handles: 'nw, n, w',
        resize: function (event, ui) { //fix for jquery resizeable not working for fixed position divs
            $chat.css('position', 'fixed');
            $chat.css('bottom', '0');
            $chat.css('top', '');
            $chat.css('left', '');
        }
    });

    //Close handler
    $chat.find('.chat-remove').click(function () {
        $(that).trigger('chat:close');
    });

    //Minimize handler
    $chat.find('.chat-minimize').click(function () {
        togglePosition();
    });

    //Capture Enter key
    $chat.find('.chat-input').keypress(function (e) {
        var html;

        //get the input text - remove the <br>'s that IE adds
        html = trimBr($(this).html());

        //handle 'enter'
        if (e.which == 13) {
            e.preventDefault();
            that.addMessage(html, -1); //pass -1 for from to indicate message is from me            
            clearInput();
            configMap.sendMessageCallback(configMap.profileId, html, configMap.convId);
            that.scrollBottom();
        }
    });    

    //Open emoticon slider
    $chat.find('.chat-input-emo-button').click(function () {
        $chat.find('.chat-emoticon-slider').slideToggle('fast');
    });

    //Bring chat to foreground
    $chat.click(function () {
        $(that).trigger('chat:foreground');
    });

    //Maximize slider if header is clicked and slider is minimized
    $chat.find('.chat-header').click(function (e) {
        if (isMinimized()) {
            if (!$(e.target).hasClass('glyphicon-chevron-up')) {
                //make sure we didn't click on the minimize button
                togglePosition();
            }
        }
    });

    //Emoticon nav tab click handler
    $chat.find('.chat-emoticon-nav').children('li').not('.chat-emoticon-close').click(function () {
        var page, html;
        page = $(this).data('page');
        html = emoticonChooserHtml(configMap.emoticonPages[page - 1].emoticons, configMap.emoticonUrl);
        $(this).parent().children('li.active').removeClass('active');
        $(this).addClass('active');
        $chat.find('.chat-emoticon-chooser').replaceWith(html);

        //Insert emoticon at caret
        $chat.find('.chat-emoticon-chooser').children('li').click(insertEmoticon);
    });

    //Close emoticon slider
    $chat.find('.chat-emoticon-close').click(function () {
        $chat.find('.chat-emoticon-slider').hide();
    });
}
