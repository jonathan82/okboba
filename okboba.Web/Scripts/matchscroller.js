/*
 *  Plugin : Match scroller
 *  Author : Jonathan Lin
 *  Date   : 11/8/2015
 *  Notes  : The match scroller plugin will load the next page of matches when the user
 *           scrolls to the bottom of the screen. It will call the Match API endpoint using
 *           CORS. 
 */
(function ($) {

    //// Private variables
    const MAX_PAGES = 10;
    var configMap = {
        storageUrl: '', //used to generate Avatar photo
        matchUrl: '', //the endpoint where to retrieve matches from
        searchCriteria: '', //the match criteria passed to the api
        templateId: 'matchesTemplate', //the jsrender template used to render a page of matches
        matchTemplate: null
    }
    var $container,
        lastPage = 1,
        loading = false;

    //// Private functions
    function avatarUrl(match, storageUrl) {
        var url = "";
        if (match.Photo == "") {
            url = '/content/images/no-avatar-';
            url += match.Gender == 'M' ? 'male.png' : 'female.png';
            return url;
        }
        url = storageUrl + match.UserId + '/' + match.Photo;
        return url;
    }

    function removeFirstMatches() {

        if (lastPage > MAX_PAGES) {
            //remove first page
            $container.children().remove('#p' + (lastPage - MAX_PAGES));
        }
    }

    function showMatches(matches, page) {
        var html, i;

        //generate and add the avatar url's to the data
        for (i = 0; i < matches.length; i++) {
            matches[i].AvatarUrl = avatarUrl(matches[i], configMap.storageUrl);
        }

        //load the next page of matches using the template
        html = configMap.matchTemplate.render({
            Page: page,
            Matches: matches
        });

        //empty page - maybe last page of matches
        if (matches.length == 0) return;

        //append
        $container.append(html);
    }

    //// Public functions (exposed thru jQuery)
    $.fn.matchscroller = function (config) {
                
        configMap = $.extend(configMap, config);

        $container = this;

        configMap.matchTemplate = $.templates(configMap.templateId);
       
        //setup scroll event handler
        $(window).scroll(function () {
            if ($(window).scrollTop() + $(window).height() > $(document).height() - 100) {

                //prevent loading while already loading
                if (loading) return;

                //append the loading text
                $container.append('<div class="matches-loading"><span class="opacity-secondary-text">倒入中...</span></div>');

                configMap.searchCriteria.page = lastPage + 1;

                //make ajax call to get next page
                $.ajax(configMap.matchUrl, {
                    data: configMap.searchCriteria,
                    dataType: "json",
                    traditional: true,
                    xhrFields: {
                        withCredentials: true
                    }
                }).done(function (data) {

                    lastPage++;
                    showMatches(data, lastPage);
                    removeFirstMatches();                    

                }).fail(function () {

                    alert('failed loading matches');
                    
                }).always(function () {

                    loading = false;

                    //remove loading text
                    $container.children().remove('.matches-loading');
                });

                loading = true;
            }
        });
    }

})(jQuery);
