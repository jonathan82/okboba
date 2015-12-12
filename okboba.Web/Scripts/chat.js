/*
 *  Module : Chat module
 *  Author : Jonathan Lin
 *  Date   : 12/6/2015
 *  Notes  : 
 */

////////////////////////////////////////////////////////////////////
// ChatSlider: 
//  Represents the chat window on screen and contains all the functions
//  for a single chat window. Assumes jQuery and jsrender is available.
////////////////////////////////////////////////////////////////////
function ChatSlider($container, options, who) {
    //// Private vars
    var configMap = {
        templateId: '#chatSliderTemplate',
        minHeight: 200,
        minWidth: 250,
        sliderClosedHeight: 31
    }
    var tmpl, $chat, that, savedHeight, avatarUrl;

    that = this;

    //// Public functions
    this.who = who;

    this.setPosition = function (right) {
        $chat.css('right', right);
    }

    this.getWidth = function () {
        return $chat.width();
    }

    this.remove = function () {
        $chat.remove();
    }

    this.addMessage = function () {
        if (avatarUrl = null) {
            //Get the avatar URL from the web service and save it

        }
    }

    //// Private Functions
    function forceHeight () {
        //force the browser to respect messsage box height
        var remain = $chat.height() - $chat.find('.chat-header-row').height() - $chat.find('.chat-input-row').height();
        $chat.find('.chat-messages').height(remain);
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

    //// Constructor logic
    //  create chat window from template and add to DOM container
    tmpl = $.templates(configMap.templateId);
    $chat = $(tmpl.render());
    $container.append($chat);
    savedHeight = $chat.height();
    forceHeight();

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
            forceHeight();
        }
    });

    //Custom scrollbar
    //$chat.find('.chat-messages').niceScroll();

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
        if (e.which == 13) {
            alert('enter pressed');
        }
    })
}

////////////////////////////////////////////////////////////////////
// chatManager: 
//  Manages multiple chat windows on screen. Constructs chat windows as needed
//  and positions them so they fit in the browser window.  Calls the
//  Chat constructor to create a new chat window. 
////////////////////////////////////////////////////////////////////
var chatManager = (function ($) {

    //// Private vars
    var configMap = {
        sliderGutterWidth: 15, //spacing between chat sliders
        minOverlap: 20, //minimum overlap spacing between sliders
        hubUrl: null
    }
    var createChat, repositionSliders, initModule, chatHubProxy;
    var sliders = [];

    //// Private functions
    function receiveMessage(from, msg) {
        var i;

        //check if chat window is already open
        for (i = 0; i < sliders.length; i++) {
            if (sliders[i].who == from) {
                //add message to window
                return;
            }
        }

        //create new chat window
        createChat(from);
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

    //// Public functions
    repositionSliders = function () {
        var bw, i, sum, n, overlap, lw, right, gw;

        n = sliders.length;
        gw = configMap.sliderGutterWidth;

        if (n == 0) return; //no sliders to position

        console.log('reposition');

        //get browser width minus the left and right gutters
        bw = $(window).width() - (2 * gw);
        //alert('bw: ' + bw);

        //get sum of all the slider widths
        sum = 0;
        for (i = 0; i < n; i++) {
            sum += sliders[i].getWidth();
        }
        sum += gw * (n - 1);

        if (sum < bw || n==1) {
            //layout sliders side by side
            right = gw;
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

    createChat = function (who) {
        //create a new chat window and position it in browser window
        var slider = new ChatSlider($('body'), null, who);
        sliders.push(slider);
        repositionSliders();

        //close handler
        $(slider).on('chat:close', function () {
            var i = sliders.indexOf(this);
            sliders[i].remove();
            delete sliders[i];
            sliders.splice(i, 1);
            repositionSliders();
        });
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
    }

    return {
        initModule: initModule,
        createChat: createChat,
        repositionSliders: repositionSliders
    }

})(jQuery);