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
function ChatSlider($container, options) {
    // Private vars
    var configMap = {
        templateId: '#chatSliderTemplate',
        minHeight: 200,
        minWidth: 250
    }
    var tmpl, $chat;

    // Public functions
    this.setPosition = function (right) {
        $chat.css('right', right);
    }

    this.getWidth = function () {
        return $chat.width();
    }

    // Constructor logic
    //  create chat window from template and add to DOM container
    tmpl = $.templates(configMap.templateId);
    $chat = $(tmpl.render());
    $container.append($chat);

    //Set resize options
    $chat.resizable({
        minWidth: configMap.minWidth,
        minHeight: configMap.minHeight,
        handles: 'nw, n',
        resize: function (event, ui) { //fix for jquery resizeable not working for fixed position divs
            $chat.css('position', 'fixed');
            $chat.css('bottom', '0');
            $chat.css('top', '');
            $chat.css('left', '');
        }
    });

    //Custom scrollbar
    $chat.find('.chat-messages').niceScroll();
}

////////////////////////////////////////////////////////////////////
// chatManager: 
//  Manages multiple chat windows on screen. Constructs chat windows as needed
//  and positions them so they fit in the browser window.  Calls the
//  Chat constructor to create a new chat window. 
////////////////////////////////////////////////////////////////////
var chatManager = (function ($) {

    // Private vars
    var configMap = {
        sliderGutterWidth: 15, //spacing between chat sliders
        minOverlap: 20 //minimum overlap spacing between sliders
    }
    var createChat, repositionSliders, initModule;
    var sliders = [];

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

    createChat = function () {
        //create a new chat window and position it in browser window
        sliders.push(new ChatSlider($('body'), null));
        repositionSliders();
    }

    initModule = function () {
        //listen for resize event
        $(window).resize($.throttle(250, function () {
            console.log('resize');
            repositionSliders();
        }));
    }

    return {
        initModule: initModule,
        createChat: createChat,
        repositionSliders: repositionSliders
    }

})(jQuery);