////////////////////////////////////////////////////////////////////
//
// chatManager: 
//  Manages multiple chat windows on screen. Constructs chat windows as needed
//  and positions them so they fit in the browser window.  Calls the
//  Chat constructor to create a new chat window. 
//
////////////////////////////////////////////////////////////////////
var chatManager = (function ($) {

    //// Private vars
    var configMap = {
        sliderGutterWidth: 15, //spacing between chat sliders
        initialGutterWidth: 60, //position first slider further from right
        minOverlap: 20, //minimum overlap spacing between sliders
        startZIndex: 100, //starting z-index for chat sliders
        chatSliderTemplateId: '#chatSliderTemplate',
        chatDialogThemTemplateId: '#chatDialogThemTemplate',
        chatDialogMeTemplateId: '#chatDialogMeTemplate',
        hubUrl: null
    }
    var createChat, repositionSliders, initModule;
    var sliders = [],
        chatHubProxy,
        chatSliderTemplate,
        chatDialogThemTemplate,
        chatDialogMeTemplate;

    var emoticonSmilies = ['1f600', '1f601', '1f602', '1f603', '1f604', '1f605', '1f606', '1f607', '1f608', '1f609', '1f60a', '1f60b', '1f60c', '1f60d', '1f60e', '1f60f', '1f610', '1f611', '1f612', '1f613', '1f614', '1f615', '1f616', '1f617', '1f618', '1f619', '1f61a', '1f61b', '1f61c', '1f61d', '1f61e', '1f61f', '1f620', '1f621', '1f622', '1f623', '1f624', '1f625', '1f626', '1f627', '1f628', '1f629', '1f62a', '1f62b', '1f62c', '1f62d', '1f62e', '1f62f', '1f630', '1f631', '1f632', '1f633', '1f634', '1f635', '1f636', '1f637', '1f638', '1f639', '1f63a', '1f63b', '1f63c', '1f63d', '1f63e', '1f63f', '1f640'],
        emoticonFood = ['1f354', '1f355', '1f356', '1f357', '1f358', '1f359', '1f35a', '1f35b', '1f35c', '1f35d', '1f35e', '1f35f', '1f360', '1f361', '1f362', '1f363', '1f364', '1f365', '1f366', '1f367', '1f368', '1f369', '1f36a', '1f36b', '1f36c', '1f36d', '1f36e', '1f36f', '1f370', '1f371', '1f372', '1f373', '1f374', '1f375', '1f376', '1f377', '1f378', '1f379', '1f37a', '1f37b'],
        emoticonLove = ['1f46a', '1f46b', '1f46c', '1f46d', '1f46e', '1f46f', '1f470', '1f471', '1f472', '1f473', '1f474', '1f475', '1f476', '1f477', '1f478', '1f479', '1f47a', '1f47b', '1f47c', '1f47d', '1f47e', '1f47f', '1f480', '1f481', '1f482', '1f483', '1f484', '1f485', '1f486', '1f487', '1f488', '1f489', '1f48a', '1f48b', '1f48c', '1f48d', '1f48e', '1f48f', '1f490', '1f491', '1f492', '1f493', '1f494', '1f495', '1f496', '1f497', '1f498', '1f499', '1f49a', '1f49b', '1f49c', '1f49d', '1f49e', '1f49f'];

    //// Private functions
    function receiveMessage(from, convId, msg) {
        var slider;

        //see if any open chat windows
        for (var i = 0; i < sliders.length; i++) {
            if (sliders[i].getProfileId() == from) {
                //chat window already open
                sliders[i].addMessage(msg, from);
                return;
            }
        }

        //We're creating a new window on screen - load initial info
        chatHubProxy.server.getInitialInfo(from, convId).done(function (info) {
            //create new chat
            slider = createChat(info.Profile.Nickname, from);
            slider.loadInitialInfo(info);

        }).fail(function () {
            alert('message recevied but couldnt get initial chat info');
        });
        
    }

    function sendMessage(to, msg, convId) {
        var slider;
        
        //this is bound to the chat slider
        slider = this;

        //send already sanitized text
        console.log('to:' + to + ', ' + msg);
        chatHubProxy.server.sendMessageAsync(to, msg, convId).done(function (status) {
            //successful - display status if needed
            console.log(slider);
            console.log('conversation id: ' + status.ConversationId);
        }).fail(function () {
            //failed - display error status to user
            alert('send message failed');
        });
    }

    function setupSignalR() {
        $.connection.hub.url = configMap.hubUrl;

        chatHubProxy = $.connection.chatHub;
        chatHubProxy.client.receiveMessage = receiveMessage;

        $.connection.hub.start().done(function () {
            //connection established
            //alert('connection established');
        }).fail(function () {
            //connection failed
            alert('signalr connection failed');
        });
    }

    function lastZIndex() {
        var i, max = -1, z;
        for (i = 0; i < sliders.length; i++) {
            z = sliders[i].getZIndex();
            max = z > max ? z : max;
        }
        return max;
    }

    function pushDown(zIndex) {
        var i, z;
        //Push all the sliders above the given z-index down by one
        for (i = 0; i < sliders.length; i++) {
            z = sliders[i].getZIndex();
            if (z > zIndex) {
                sliders[i].setZIndex(z - 1);
            }
        }
    }

    //// Public functions
    repositionSliders = function () {
        var bw, i, sum, n, overlap, lw, right, gw;

        n = sliders.length;
        gw = configMap.sliderGutterWidth;

        if (n == 0) return; //no sliders to position

        //get browser width minus the left and right gutters
        bw = $(window).width() - (configMap.initialGutterWidth + gw);

        //get sum of all the slider widths
        sum = 0;
        for (i = 0; i < n; i++) {
            sum += sliders[i].getWidth();
        }
        sum += gw * (n - 1);

        if (sum < bw || n == 1) {
            //layout sliders side by side
            right = configMap.initialGutterWidth;
            for (i = 0; i < n; i++) {
                sliders[i].setPosition(right);
                right += sliders[i].getWidth() + gw;
            }
        } else {
            //layout sliders overlapping
            lw = sliders[n - 1].getWidth();
            overlap = (bw - lw) / (n - 1);
            overlap = overlap < configMap.minOverlap ? configMap.minOverlap : overlap; //set minimum overlap
            right = gw;
            for (i = 0; i < n; i++) {
                sliders[i].setPosition(right);
                right += overlap;
            }
        }

    }

    /*
     * Creates a new chat window and positions it on the screen. Returns the slider created.
     */
    createChat = function (title, profileId) {
        var slider, zIndex;

        //create a new chat window and position it in browser window
        zIndex = sliders.length == 0 ? configMap.startZIndex : lastZIndex() + 1;
        slider = new ChatSlider($('body'), {
            chatSliderTemplate: chatSliderTemplate,
            chatDialogThemTemplate: chatDialogThemTemplate,
            chatDialogMeTemplate: chatDialogMeTemplate,
            initialZIndex: zIndex,
            emoticonPages: [
                { emoticons: emoticonSmilies, navIcon: '' },
                { emoticons: emoticonFood, navIcon: '' },
                { emoticons: emoticonLove, navIcon: '' }
            ],
            title: title,
            profileId: profileId,
            sendMessageCallback: sendMessage
        });
        sliders.push(slider);
        repositionSliders();

        //close handler
        $(slider).on('chat:close', function () {
            var i = sliders.indexOf(this);
            pushDown(this.getZIndex());
            sliders[i].remove();
            delete sliders[i];
            sliders.splice(i, 1);
            repositionSliders();
        });

        //bring to foreground handler
        $(slider).on('chat:foreground', function () {
            var i, maxZ;
            maxZ = lastZIndex();
            pushDown(this.getZIndex());
            this.setZIndex(maxZ);
        });

        return slider;
    }

    initModule = function (options) {

        configMap = $.extend(configMap, options);

        //listen for resize event
        $(window).resize($.throttle(250, function () {
            console.log('resize');
            repositionSliders();
        }));

        //setup signalr
        setupSignalR();

        //load the templates (so we can reuse them when creating new sliders)
        chatSliderTemplate = $.templates(configMap.chatSliderTemplateId);
        chatDialogThemTemplate = $.templates(configMap.chatDialogThemTemplateId);
        chatDialogMeTemplate = $.templates(configMap.chatDialogMeTemplateId);

        //hookup message button
        /*
         * Tries to get last conversation Id. If none then create chat window with
         * null conversation Id inidcating new conversation.  If there is previous 
         * conversation then loads the first N messages where N is configurable.
         * Also loads the avatar, gender, and profile info.
         */
        $('button[data-toggle="chat"]').click(function () {

            var nickname, profileId, i, slider;

            nickname = $(this).data('name');
            profileId = $(this).data('id');

            //check if window already open
            for (i = 0; i < sliders.length; i++) {
                if (sliders[i].getProfileId() == profileId) return;
            }

            slider = createChat(nickname, profileId);

            //load inital info
            chatHubProxy.server.getInitialInfo(profileId, null).done(function (info) {
                slider.loadInitialInfo(info);
            }).fail(function () {
                alert('loading initial info failed');
            });
        });
    }

    return {
        initModule: initModule,
        createChat: createChat,
        repositionSliders: repositionSliders
    }

})(jQuery);