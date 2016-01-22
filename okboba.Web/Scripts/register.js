/*
 *  Author : Jonathan Lin
 *  Date   : 12/30/2015
 *  Notes  : Plugin for handling Registration logic.  Sets up validation and other related logic
 *           as well as the Next and Back buttons for navigating between step 1 and 2.
 */
var okbregister = (function ($) {

    /////////////////////////// Private Variables ////////////////////////
    var
        configMap = {
            nextButtonSel: '#NextButton',
            step1Sel: '#register-step-1',
            step2Sel: '#register-step-2',
            step1Name: 'step1',
            step2Name: 'step2',
            bdayTextSel: '#BirthdateText',
            bdaySel: '#Birthdate',
            locationTextSel: '#locationText',
            registerFormSel: '#registerForm',
            provinces: null //a JSON array of provinces used for initial state of location picker. 
        },
        validationOpts = {
            rules: {
                Gender: "required",
                LookingForGender: "required",
                Email: {
                    required: true,
                    email: true,
                    remote: {
                        url: '/account/verifyemail', //check with server to see if email already registered 
                        //error: function () { $(configMap.registerFormSel).valid();}
                    }
                },
                Password: {
                    required: true,
                    minlength: 6
                },
                Nickname: {
                    required: true,
                    minlength: 2,
                    maxlength: 15,
                    pattern: /^[a-zA-Z0-9]{2,15}$/
                },
                BirthdateText: "required",
                locationText: "required"
            },
            submitHandler: function (form) {
                //Start the ladda button
                $(form).find('button[type="submit"]').ladda().ladda('start');
                form.submit();
            }
        },
        initModule, Next, Back, SetupBirthdayPicker, validator;


    //////////////////////// Private Methods ///////////////////////////////
    Next = function (e) {
        e.preventDefault();

        $(configMap.step1Sel).fadeOut('fast', function (e) {
            $(configMap.step2Sel).fadeIn('fast');
        });            

        window.history.pushState({step: configMap.step2Name},null,null);
    }

    Back = function (e) {
        if (e.originalEvent.state.step == configMap.step1Name) {
            $(configMap.step2Sel).fadeOut('fast', function (e) {
                $(configMap.step1Sel).fadeIn('fast');
            });
        } else if(e.originalEvent.state.step == configMap.step2Name) {
            $(configMap.step1Sel).fadeOut('fast', function (e) {
                $(configMap.step2Sel).fadeIn('fast');
            });
        }
    }

    SetupBirthdayPicker = function myfunction() {
        //Setup the birthdate picker
        $(configMap.bdayTextSel).datepicker( {
            format: "yyyy年mm月",
            startView: 2, 
            minViewMode: "months",
            language: "zh-CN",
            orientation: "bottom auto",
            defaultViewDate: {year: 1980},
            startDate: '-100y',
            endDate: '-15y',
            autoclose: true
        }).on('hide', function (e) {
            //validate field
            $(configMap.bdayTextSel).valid();
        }).on('changeDate', function (e) {
            //convert chinese date to standard date - mm/dd/yyyy
            var re = /(\d\d\d\d)年(\d\d)月/;
            var match = re.exec($(configMap.bdayTextSel).val());
            if (match != null) {
                var birthdate = match[2] + '/01/' + match[1];
                $(configMap.bdaySel).val(birthdate);
            }
        });
    }


    /////////////////////// Public Methods //////////////////////////
    initModule = function (config) {

        configMap = $.extend(configMap, config);
       
        window.history.pushState({step: configMap.step1Name},configMap.step1Name,null);

        //Next button
        $(configMap.nextButtonSel).click(Next);

        //Back button
        $(window).on('popstate', Back);

        //Setup the birthdate picker
        SetupBirthdayPicker();

        //Setup the location picker
        $(configMap.locationTextSel).locationpicker(configMap.provinces)
            .on('locationpicker:close', function () {
                //force validation of location when picker is closed
                $(configMap.locationTextSel).valid();
            });
        
        //Setup validation
        validator = $(configMap.registerFormSel).validate(validationOpts);
    }

    //// return Public Interface
    return {
        initModule: initModule
    }

})(jQuery);