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

        html = '<table class="table locationtable"><tr><th style="text-align:center" colspan="' + colspan + '">' + title + '</th></tr>';

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
