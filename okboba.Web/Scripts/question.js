/*
 *  Plugin : Importance bar selector
 *  Author : Jonathan Lin
 *  Date   : 11/23/2015
 */
(function ($) {

    $.fn.questionimportance = function () {
        this.hover(function () {
            var imp = $(this).data('importance');
            switch (imp) {
                case 3:
                    $('[data-importance="3"]').toggleClass('active');
                case 2:
                    $('[data-importance="2"]').toggleClass('active');
                case 1:
                    $('[data-importance="1"]').toggleClass('active');
                    break;
                default:
                    alert('broke');
            }
        });

        this.click(function () {

        });
    }

})(jQuery);