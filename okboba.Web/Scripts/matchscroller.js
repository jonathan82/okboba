/*
 *  Plugin : Match scroller
 *  Author : Jonathan Lin
 *  Date   : 11/8/2015
 *  Notes  : 
 */
(function ($) {

    var currPages = [];
    var $container;
    var loading = false;
    var matchApiUrl = '';

    function getThumbnailUrl(photo, storageUrl, gender) {
        var url = "";
        if (photo == "") {
            url = '/content/images/no-avatar-';
            url += gender == 'M' ? 'male.png' : 'female.png';
            return url;
        }
        url = storageUrl + photo + '_t';
        return url;
    }

    function loadPreviousMatches() {
        alert('not implemented yet');
    }

    $.fn.matchscroller = function (pagesLoaded, matchHost, criteria, storageUrl) {
        currPages = pagesLoaded;
        $container = this;
        matchApiUrl = matchHost;
        var MAX_PAGES = 10;

        //setup scroll event handler
        $(window).scroll(function () {
            if ($(window).scrollTop() + $(window).height() > $(document).height() - 100) {

                if (loading) return;

                //append the loading text
                $container.append('<div class="matches-loading"><span class="opacity-secondary-text">倒入中...</span></div>');

                //get the page to load
                var pageToLoad = currPages[currPages.length - 1] + 1;
                criteria.page = pageToLoad;

                //make ajax call to get next page
                $.ajax(matchApiUrl + '/api/matches', {
                    data: criteria,
                    dataType: "json",
                    traditional: true,
                    xhrFields: {
                        withCredentials: true
                    }
                }).done(function (data) {
                    loading = false;
                    var html = '<div id="p' + pageToLoad + '">';

                    //append next page of matches
                    for (var i = 0; i < data.length; i++) {
                        var thumbUrl = getThumbnailUrl(data[i].Photo, storageUrl, data[i].Gender);
                        html += '<div class="match-result">';
                        html += '<a href="/profile/' + data[i].ProfileId + '"><img src="' + thumbUrl + '" alt="Profile Photo" width="200" height="200" /></a>';
                        html += '<p>' + data[i].MatchPercent + '% Match - ' + data[i].Age + data[i].Gender + ' - ' + data[i].Name + '</p>';
                        html += '</div>';
                    }
                    html += '</div>';

                    //remove loading text
                    $container.children().remove(':last');

                    //empty page
                    if (data.length == 0) return;

                    //append
                    $container.append(html);
                    currPages.push(pageToLoad);

                    if (currPages.length > MAX_PAGES) {
                        //remove first page
                        $container.children().remove('#p' + currPages[0]);
                        currPages.shift();

                        if (!$container.children(':first').is('button')) {
                            //add new previous button
                            var $prevButton = $('<button type="button" class="btn btn-default">回到之前页面...</button>');
                            $prevButton.click(loadPreviousMatches);
                            $container.prepend($prevButton);
                        }
                    }

                }).fail(function () {
                    loading = false;
                    alert('failed loading matches');

                    //remove loading text
                    $container.children().remove(':last');
                });

                loading = true;
            }
        });
    }

})(jQuery);
