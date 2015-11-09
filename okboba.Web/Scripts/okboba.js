/*
 *  Utitlity Functions
 * 
 */
function encodeHtml(str) {
    return String(str).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

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

    $.fn.matchscroller = function (pagesLoaded, matchHost, criteria) {
        currPages = pagesLoaded;
        $container = this;
        matchApiUrl = matchHost;

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
                $.ajax(matchApiUrl + '/matches/getmatches', {
                    data: criteria,
                    dataType: "json",
                    traditional: true,
                    xhrFields: {
                        withCredentials: true
                    }
                }).done(function (data) {
                    alert('sucessfully loaded!');
                    loading = false;
                }).fail(function () {
                    alert('failed loading matches');
                });

                loading = true;
            }
        });
    }

})(jQuery);

/*
 *  Plugin : Location picker
 *  Author : Jonathan Lin
 *  Date   : 10/20/2015
 *  Notes  : Uses popover to present a 2-level province/district chooser
 */
(function ($) {

    var numCols = 6;
    var locationName = "";
    var locationId1, locationId2;
    var url = "/location/GetDistrictJson"
    var $locationInput;
    var locationId1Sel, locationId2Sel;

    function buildProvinceHtml(prov, title) {
        var colspan = numCols > prov.length ? prov.length : numCols;

        html = '<table class="table locationtable"><tr><th style="text-align:center" colspan="' + colspan + '">'+title+'</th></tr>';

        for (var i = 0; i < prov.length; i++) {
            if (i % colspan == 0) html += '<tr>';
            html += '<td><a href="#" data-locationid="' + prov[i].id + '">' + prov[i].name + '</a></td>';
            if (i == prov.length - 1 || i % colspan == colspan - 1) html += '</tr>';
        }

        html += '</table>';

        return html;
    }

    function buildDistrictTable(data, status, jqxhr) {

        html = buildProvinceHtml(data, '区域');

        //alert(html);
        
        //Setup the click handlers
        $newTable = $(html);
        $newTable.find('a[data-locationid]').each(function (index, elem) {
            $(elem).click(function (e) {
                // Populate the textbox with the location name 
                // and set the hidden inputs
                locationName += ', ' + this.innerHTML;
                locationId2 = $(this).data('locationid');

                $locationInput.val(locationName);
                $(locationId1Sel).val(locationId1);
                $(locationId2Sel).val(locationId2);

                //Close the popover
                $locationInput.popover('hide');
            });
        });

        //Replace the table with the next level
        $('.locationtable').replaceWith($newTable);
    }

    // Popover options
    var opts = {
        html: true,
        trigger: 'click focus',
        placement: 'bottom',
        container: 'body'
    }

    // Init the plugin
    $.fn.locationpicker = function (provJson, locId1Sel, locId2Sel) {

        $this = $(this);
        $locationInput = $this;
        locationId1Sel = locId1Sel;
        locationId2Sel = locId2Sel;

        //Setup the popover
        opts.content = buildProvinceHtml(provJson, '省份');
        $this.popover(opts);

        //Setup the click links
        $this.on('shown.bs.popover', function () {
            locationName = "";
            $('.locationtable').find('a[data-locationid]').each(function (index, elem) {
                var locid = $(elem).data('locationid');
                $(elem).click(function (e) {
                    //Set the location variables
                    locationName += this.innerHTML;
                    locationId1 = locid;

                    //Retrieve the 2nd level from Web API
                    $.get(url, 'provinceId=' + locid, buildDistrictTable);
                });
            });
        });
    }

})(jQuery);

/*
 *  Plugin : Photo Upload
 *  Author : Jonathan Lin
 *  Date   : 10/15/2015
 *  Notes  : Requires FileReaderJS library and jQuery UI.
 * 
 */
(function ($) {

    const MAX_FILE_SIZE = 5000000;
    const MIN_RESOLUTION = 200;
    const MAX_SCREEN_WIDTH = 800;

    var photoArea;
    var origImageWidth;
    var scaledImageWidth;

    // Options for thumbnail selector
    var dragOpts = {
        containment: 'parent'

    }
    var resizeOpts = {
        containment: 'parent',
        minWidth: 100,
        minHeight: 100,
        aspectRatio: true
    }

    // Options for FilereaderJS
    var opts = {
        on: {
            load: function (e, file) {
                //Check Size
                if (file.size > MAX_FILE_SIZE) {
                    alert('Upload file size must be less than 5MB!');
                    return;
                }

                //Check Type
                if (!file.type.match(/image.*/)) {
                    alert('You must choose an image file!');
                    return;
                }

                //Check minimum resolution
                var $img = $('<img id=""/>')
                    .attr("src", e.target.result)
                    .load(function () {
                        if (this.width < MIN_RESOLUTION || this.height < MIN_RESOLUTION) {
                            //Image too small
                            alert('Photo must be at leat 200 x 200 pixels!');
                            return;
                        }

                        origImageWidth = scaledImageWidth = this.width;

                        // Limit the on screen size
                        if (this.width > MAX_SCREEN_WIDTH) {
                            $(this).width(MAX_SCREEN_WIDTH);
                            scaledImageWidth = MAX_SCREEN_WIDTH;
                        }                        

                        //Everythign OK, create thumbnail selector and add to DOM
                        var $thumbDiv = $('<div class="photo-upload-container"> \
                        <div class="photo-upload-innercontainer"><div class="photo-thumbnail-selector"></div></div> \
                        </div>');

                        $thumbDiv.append($img);
                        $(photoArea).after($thumbDiv);
                        $(photoArea).hide();

                        //Setup the thumbail selector to be resizeable and draggeable
                        $('.photo-thumbnail-selector').draggable(dragOpts).resizable(resizeOpts);
                    });
            },
            error: function (e, file) {
                alert('error');
            }
        }
    }

    // Initialize the plugin for a modal
    $.fn.photoUpload = function () {

        photoArea = this.data('target');

        this.find('input[type="file"]').fileReaderJS(opts);

        //Setup the dismiss event handler
        this.on('hidden.bs.modal', function (e) {

            if ($(photoArea).next().hasClass('photo-upload-container')) {
                $(photoArea).next().remove();
            }

            $(photoArea).show();
        });

        //Setup the Save handler -- do the real work
        $('[data-submit="modal"]').click(function (e) {

            // Make sure an image is selected first
            var $thumb = $('.photo-thumbnail-selector');
            if ($thumb.length == 0) {
                alert('You must select an image first!');
                return;
            }

            var pos = $thumb.position();

            // Set the thumbnail values in the form
            // Compute the scaled thumbnail coordinates base on original image
            var intTop = Math.round(origImageWidth / scaledImageWidth * pos.top);
            var intLeft = Math.round(origImageWidth / scaledImageWidth * pos.left);
            var intThumbWidth = Math.round(origImageWidth / scaledImageWidth * $thumb.width())

            $('#topThumb').val(intTop);
            $('#leftThumb').val(intLeft);
            $('#widthThumb').val(intThumbWidth);

            alert("submitting form. \
                   \nleft: " + intTop +
                   '\ntop: ' + intLeft +
                   '\nwidth: ' + intThumbWidth);
        });
    }

})(jQuery);

/*
 *  Plugin : Edit in place for OkBoba
 *  Author : Jonathan Lin
 *  Date   : 10/13/2015
 *  Notes  : 
 * 
 */
(function ($) {

    // Initialize the plugin
    $.fn.editinplace = function () {

        // The URL to post to
        var url = '/profile/editprofiletext';

        // Attach click handler to all divs with data-editinplace attribute
        $('div[data-editinplace]').click(function (e) {

            var $this = $(this);

            var txt = $.trim($this.text());
            var id = $this.data('editinplace');

            //Replace the text with the form
            var $newElement = $('<form><textarea class="profile-text-edit" id=qText-"'+id+'" name="qText">' + txt + '</textarea> \
                                 <button type="button" class="btn btn-default">Cancel</button> \
                                 <button type="button" class="btn btn-primary">Save</button> \
                                 <input type="hidden" name="whichQuestion" value="'+id+'" /></form>');

            $this.after($newElement);

            var $savedElement = $this.detach();

            //Setup event handler for the Save button
            $newElement.children('button:nth-of-type(2)').click(function (e) {                

                e.preventDefault();

                var postData = $newElement.serialize();

                alert('save me! - ' + postData);

                //Disable save button
                var $submitBtn = $(this);
                $submitBtn.prop("disabled", true);

                //Submit the question text
                $.post(url, postData, function (data) {

                    //Replace form with the new text
                    var txt = $newElement.children('textarea:first').val();                    
                    $savedElement.text(txt);
                    $newElement.replaceWith($savedElement);

                }).fail(function (data, status) {
                    alert('failed! - ' + status);

                    //Re-enable save button
                    $submitBtn.prop("disabled", false);
                });

            });

            //setup event handler for the Cancel button
            $newElement.children('button:first').click(function (e) {
                $newElement.replaceWith($savedElement);
                return false;
            });

        });

        return this;
    }

})(jQuery);

