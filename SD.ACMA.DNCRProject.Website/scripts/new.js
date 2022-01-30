window.onunload = function () { };

(function ($) {
    //regx method for preventing cross site scripting
    $.validator.addMethod("denyhtmlregx",
        function (value, element, regexp) {
            var re = new RegExp(regexp);
            return this.optional(element) || re.test(value);
        },
        "Your response cannot contain a '<' or '>' character. Please edit your response."
    );

    //class defition which will not allow html tags in the input
    $.validator.addClassRules('deny-html', {
        denyhtmlregx: "^[^<>]*$",
    });

    //mandatory checkbox
    $.validator.addMethod("mandatory", function (value, element, param) {
        return element.checked;
    });
    $.validator.unobtrusive.adapters.addBool("mandatory");

    //date less than or equal to today
    $.validator.addMethod("datelessthanorequaltotoday", function (value, element, param) {
        var dateParts = value.split("/");
        var dateValue = new Date(dateParts[2], dateParts[1] - 1, dateParts[0]);
        return dateValue <= new Date();
    });
    $.validator.unobtrusive.adapters.addBool("datelessthanorequaltotoday");
    
    //date greater than
    $.validator.addMethod("dategreaterthan", function (value, element, params) {
        var parts = value.split("/");
        var date = new Date(parseInt(parts[2], 10),
                          parseInt(parts[1], 10) - 1,
                          parseInt(parts[0], 10));
        var otherParts = $(params).val().split("/");
        var otherDate = new Date(parseInt(otherParts[2], 10),
                          parseInt(otherParts[1], 10) - 1,
                          parseInt(otherParts[0], 10));
        return date >= otherDate;
    });
    $.validator.unobtrusive.adapters.add("dategreaterthan", ["otherpropertyname"], function (options) {
        options.rules["dategreaterthan"] = "input[name='" + options.params.otherpropertyname + "']:visible";
        options.messages["dategreaterthan"] = options.message;
    });

    //check duplicate phone numbers in Complaints form
    $.validator.addMethod("valuenotequalto", function (value, element, params) {      
        return ($(params).val() == "" || $(params).val() != value);
    });
    $.validator.unobtrusive.adapters.add("valuenotequalto", ["otherpropertyname"], function (options) {
        options.rules["valuenotequalto"] = "input[name='" + options.params.otherpropertyname + "'], select[name='" + options.params.otherpropertyname + "']";
        options.messages["valuenotequalto"] = options.message;
    });

    //fileupload extension
    function getFileExtension(fileName) {
        var extension = (/[.]/.exec(fileName)) ? /[^.]+$/.exec(fileName) : undefined;
        if (extension != undefined) {
            return extension[0];
        }
        return extension;
    };

    $.validator.unobtrusive.adapters.add('fileextensions', ['fileextensions'], function(options) {
        var params = {
            fileextensions: options.params.fileextensions.split(',')
        };

        options.rules['fileextensions'] = params;
        if (options.message) {
            options.messages['fileextensions'] = options.message;
        }
    });
    $.validator.addMethod("fileextensions", function(value, element, param) {
        if (value === "") {
            return true;
        } else {
            var extension = getFileExtension(value);
            var validExtension = $.inArray(extension, param.fileextensions) !== -1;
            return validExtension;
        }
    });

    //postcode
    $.validator.addMethod(
        "postcodeRegexAU",
        function (value, element, regexp) {
            var re = new RegExp(regexp);
            return this.optional(element) || re.test(value);
        },
        "The number you have entered is not an Australian Postcode"
    );

    $.validator.addMethod(
        "postcodeRegex",
        function (value, element, regexp) {
            var re = new RegExp(regexp);
            return this.optional(element) || re.test(value);
        },
        "Please enter a valid postcode"
    );

    /*$.fn.digits = function () {
        return this.each(function() {
            $(this).text($(this).text().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,"));
        });
    }*/

    //maxfilesize extension
    jQuery.validator.unobtrusive.adapters.add('filesize', ['maxsize'], function (options) {
        options.rules['filesize'] = options.params;
        if (options.message) {
            options.messages['filesize'] = options.message;
        }
    });

    jQuery.validator.addMethod('filesize', function (value, element, params) {
        if (!element.files || !element.files[0].size) {
            // This browser doesn't support the HTML5 API
            return true;
        }

        if (element.files.length < 1) {
            // No files selected
            return true;
        }
        return element.files[0].size < params.maxsize;
    });

    //minimum elements for numbers
    $.validator.unobtrusive.adapters.add("minimumelements", ["minimumelements", "selector"], function(options) {
        options.rules["minimumelements"] = options.params;
        options.messages["minimumelements"] = options.message;
    });
    $.validator.addMethod("minimumelements", function (value, element, param) {
        var minimum = parseInt(param.minimumelements);
        var selector = param.selector;
        var count = 0;

        if (selector.indexOf(":checked") > -1) {
            count = $(selector).length;
        } else {
            $(selector).each(function() {
                if ($(this).val() !== "" && $(this).val() !== "0") {
                    count++;
                }
            });
        }
        if (count >= minimum) {
            return true;
        }
        else return false;
    });

    //require one from group
    $.validator.unobtrusive.adapters.add('group', ['propertynames'], function (options) {
        options.rules['group'] = options.params;
        options.messages['group'] = options.message;
    });
    $.validator.addMethod('group', function (value, element, params) {
        var properties = params.propertynames.split(',');
        var isValid = false;
        for (var i = 0; i < properties.length; i++) {
            var property = properties[i];
            if ($('#' + property).val() != "") {
                isValid = true;
                break;
            }
        }
        return isValid;
    });

    //Validation for checking atleast one check box is checked in Registration page
    $.validator.addMethod("OneCheckBoxRequired",
       function () {
           if ($(".check-one:checked").length > 0)
                       {
                           $('.check-box-error').hide();
                           return true;
                       }
                       else
                       {
                           $('.check-box-error').show();
                           return false;
                       }     
       },
       "Please confirm how the numbers are used"
   );

    $.validator.addClassRules('check-one', {
        OneCheckBoxRequired: true,
    });  
}(jQuery));

function addCommas(nStr) {
    nStr += '';
    var x = nStr.split('.');
    var x1 = x[0];
    var x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

function formatCurrency(nStr) {
    return parseFloat(nStr, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
}

function openNextAccordion(that) {
    var errorContainer = $(that).closest(".accordionContent").find(".errorContainer");
    if (errorContainer != null) {
        errorContainer.empty();
    }
    var header = $(that).closest(".accordionContent").siblings(".accordionHeader");
    header.addClass("accordion-filled-up");

    if (header.closest("li").nextAll("li:visible").first().find(".accordionHeader").hasClass("accordion-filled-up")) {
        header.closest("li").nextAll("li:visible").first().find("input:not(.readonly), select:not(.readonly), textarea:not(.readonly)").prop('disabled', true);
        header.closest("li").nextAll("li:visible").first().find(".hideDisabled").hide();
    }

    var index = header.closest("li").nextAll("li:visible").first().find(".accordionHeader").index(".accordionHeader");
    $(window).scrollTop($("#formAccordion .ui-state-active").position().top - 59);
    $(that).closest(".accordion").accordion({ active: index });
    setTimeout(function () {
        header.closest("li").nextAll("li:visible").first().find("input:visible:enabled:not([readonly]), select:visible:enabled:not([readonly]), textarea:visible:enabled:not([readonly])").first().focus();
    }, 500);
}

function invalidateAccordion(that) {
    var errorContainer = $(that).closest(".accordionContent").find(".errorContainer");
    errorContainer.empty();
    $(".accordionContent:visible select.input-validation-error").each(function () {
        $(this).parent().addClass("input-validation-error");
    });
    $(that).siblings(".failedMandatory").show();
    $("span.error:visible span").each(function () {
        if ($(this).text().trim() != "") {
            var labelName = $(this).closest(".formField").find("label:first").html().replace('*', '');
            if (labelName == "") {
                errorContainer.append("<p><span class='error'>" + $(this).text() + "</span></p>");
            } else errorContainer.append("<p><span class='error'>" + labelName + " : " + $(this).text() + "</span></p>");
        }
    });
    $(window).scrollTop(errorContainer.position().top - 195);
    $(that).closest(".accordionContent").siblings(".accordionHeader").focus();
}

function recaptchaVerified() {
    $(".accordionContent .g-recaptcha").addClass("validated");
    $(".accordionContent span.recaptchaerror").hide();
}

function refreshState() {
    $("#State.textbox").attr("name", "StateHidden");
    if($("select#Country").val() === "AU") {
        if ($("#hiddenState").length > 0) {
            $("#State.textbox").hide();
            $("#hiddenState").attr("name", "StateHidden");
        } else {
            $("#State.textbox").attr("name", "StateHidden").hide();
        }
        $("#State.dropdown").attr("name", "State").closest(".selectParent").show();
        $("#stateError").show();
        $("#Postcode").rules("remove", "postcodeRegex");
        $("#Postcode").rules("add", { postcodeRegexAU: "^[0-9]{4}$" });
        if ($("#State.notrequired").length > 0) {
            $("#State").closest(".formField").find(".inputLabel").text("State");
        } else {
            $("#State").closest(".formField").find(".inputLabel").html("State<abbr title='required'>*</abbr>");
        }
    } else {
        $("#State.dropdown").attr("name", "StateHidden").closest(".selectParent").hide();
        if ($("#hiddenState").length > 0) {
            $("#hiddenState").attr("name", "State");
            $("#State.textbox").show();
            $("#stateError").hide();
        } else {
            $("#State.textbox").attr("name", "State").show();
        }
        $("#Postcode").rules("remove", "postcodeRegexAU");
        $("#Postcode").rules("add", { postcodeRegex: "^[a-zA-Z0-9]{1,10}$" });
        $("#State").closest(".formField").find(".inputLabel").text("State");
    }
}

function bindHiddenState() {
    $("#State.textbox").change(function() {
        $("#hiddenState").val($(this).val());
    });
}

function hideDetails() {
    if ($(".accordionContent input#IsAnonymous:checked").length > 0) {
        $(".accordionContent .anonymousHide").hide();
    } else {
        $(".accordionContent .anonymousHide").show();
    }
}

function applyDatePicker() {
    $('input[type=date]')
        .attr('type', 'text')
        .datepicker({
            dateFormat: 'dd/mm/yy',
            maxDate: 0
        });
}

function changeBackground(slideElement, duration) {
    var desktopBackgroundImage = $(slideElement).data("desktop-background-image");
    var tabletBackgroundImage = $(slideElement).data("tablet-background-image");

    var desktopContainer;
    var desktopNextContainer;
    var tabletContainer;
    var tabletNextContainer;

    if ($(".content .desktopBannerContainer").css("opacity") === "1") {
        desktopContainer = $('.content .desktopBannerContainer');
        desktopNextContainer = $('.content .desktopBannerNextContainer');
        tabletContainer = $('.content .tabletBannerContainer');
        tabletNextContainer = $('.content .tabletBannerNextContainer');
    } else {
        desktopContainer = $('.content .desktopBannerNextContainer');
        desktopNextContainer = $('.content .desktopBannerContainer');
        tabletContainer = $('.content .tabletBannerNextContainer');
        tabletNextContainer = $('.content .tabletBannerContainer');
    }
    
    desktopNextContainer.css("background", "url(" + desktopBackgroundImage + ") no-repeat center top").css("background-size", "cover");
    tabletNextContainer.css("background", "url(" + tabletBackgroundImage + ") no-repeat center top").css("background-size", "cover");

    desktopContainer.fadeTo(duration, 0);
    tabletContainer.fadeTo(duration, 0);

    desktopNextContainer.fadeTo(duration, 1);
    tabletNextContainer.fadeTo(duration, 1);
}

function resizeText(selector, number) {
    var currentFontSize = $(selector).css('font-size');
    var currentFontSizeNum = parseFloat(currentFontSize, 10);
    var newFontSize = currentFontSizeNum + number;
    $(selector).css('font-size', newFontSize);
}

function setMinHeight() {
    //$(".content .contentContainer").css("min-height", $(window).height() - 190 + "px");
    $(".content .addSectionMinHeight").css("min-height", $(window).height() - 221 + "px");
}

function openUserLogin() {
    $("#Password").val("");
    $("#Password.input-validation-error").removeClass("input-validation-error");
    $("#Password-error").hide();
    $("#userLogin").dialog("open");
    $(".mobileMenu-dark:visible").click();
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for(var i=0; i<ca.length; i++) {
        var c = ca[i];
        while(c.charAt(0) ==' ') c = c.substring(1);
        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
        }
    return "";
}

$(function () {
    $("#formAccordion").accordion({ heightStyle: "content" });

    $("a.openSignIn").on("click", function (event) {
        event.preventDefault();
        openUserLogin();
    });

    $(".emergencyMessage .closeError").click(function (event) {
        event.preventDefault();
        $(this).parent().hide();
        $.cookie("Broadcast" + $(this).parent().attr("broadcastId"), "true", { path: "/", expires: 10000 });
    });

    //adjust the content min-height based on window height so the footer will be at the bottom
    setMinHeight();
    $(window).resize(function() {
        setMinHeight();
    });

    //disable accordion click to open
    $("#formAccordion").accordion({ heightStyle: "content", disabled: true });

    //javascript widgets
    $("[data-add-more-numbers]").addmorenumbers();
    $("[data-other-option-field]").otheroptionfield();
    $("[data-checkbox-subfield]").checkboxsubfield();
    
    //re-enable submit button on initial load
    if ($(".accordionContent button.submit").length > 0) {
        $(".accordionContent button.submit").prop('disabled', false);
    }

    //hook up next button
    $(".accordion").on("click", ".accordionContent button.next.validate", function (event) {
        event.preventDefault();
        if ($(this).closest(".accordionContent").find("input, select, textarea").valid()) {
            $(this).closest(".accordionContent").find(".errorContainer").empty();
            openNextAccordion(this);
            $(this).siblings(".failedMandatory").hide();
        } else {
            invalidateAccordion(this);
        }
    });

    //hook up edit button
    $(".accordion").on("click", ".accordionHeader .editButton", function (event) {
        event.preventDefault();
        event.stopPropagation();
        var header = $(this).closest(".accordionHeader");
        header.closest("li").find("input:not(.readonly), select:not(.readonly), textarea:not(.readonly)").prop('disabled', false);
        header.closest("li").find(".hideDisabled").show();
        header.removeClass("accordion-filled-up");
        header.parent().nextAll().find(".accordionHeader").removeClass("accordion-filled-up");
        header.parent().nextAll().find(".accordionHeader").removeClass("lastActive");
        header.parent().nextAll().find(".accordionHeader").closest("li").find("input:not(.readonly), select:not(.readonly), textarea:not(.readonly)").prop('disabled', false);
        header.parent().nextAll().find(".accordionHeader").closest("li").find(".hideDisabled").show();
        var index = header.index(".accordionHeader");
        $(this).closest(".accordion").accordion({ active: index });
    });

    //read only mode
    $(".accordion#formAccordion").on("click", ".accordion-filled-up", function (event) {
        event.preventDefault();
        $("#formAccordion .ui-accordion-header-active:not(.accordion-filled-up)").addClass("lastActive");
        var header = $(this).closest(".accordionHeader");
        header.closest("li").find("input:not(.readonly), select:not(.readonly), textarea:not(.readonly)").prop('disabled', true);
        header.closest("li").find(".hideDisabled").hide();
        var index = header.index(".accordionHeader");
        $(this).closest(".accordion").accordion({ active: index });
    });

    //reactivate lastActive
    $(".accordion").on("click", ".lastActive", function (event) {
        event.preventDefault();
        var header = $(this).closest(".accordionHeader");
        header.removeClass("lastActive");
        var index = header.index(".accordionHeader");
        $(this).closest(".accordion").accordion({ active: index });
    });

    //hook up submit button
    $(".accordion").on("click", ".accordionContent button.submit", function (event) {
        var isValid = true;
        var valid = $(this).closest(".accordionContent").find("input, select, textarea").valid();
        if (!valid) {
            isValid = false;
        }
        if ($(".accordionContent .g-recaptcha").length > 0) {
            if (!$(".accordionContent .g-recaptcha").hasClass("validated")) {
                $(".accordionContent span.recaptchaerror").show();
                isValid = false;
            } else {
                $(".accordionContent span.recaptchaerror").hide();
            }
        }
        $(this).closest(".accordionContent").find(".errorContainer").empty();
        if (!isValid) {
            event.preventDefault();
            invalidateAccordion(this);
        } else {
            $(".accordion#formAccordion").find("input:not(.readonly):disabled, select:not(.readonly):disabled, textarea:not(.readonly):disabled").prop('disabled', false);
            $(this).siblings(".failedMandatory").hide();
            $(this).prop('disabled', true);
            $(this).text("SUBMITTING...");
            if ($("input.faxes").length > 0) {
                $("input.faxes").prop('disabled', false);
            }
            this.form.submit();
            event.preventDefault();
        }
    });

    //reset select validation class
    $(".accordionContent select").on("change", function () {
        if ($(this).valid()) {
            $(this).parent().removeClass("input-validation-error");
        } else $(this).parent().addClass("input-validation-error");
    });

    //check if IE7
    if (navigator.appVersion.indexOf("MSIE 7.") != -1) {
        alert("This website does not support Internet Explorer 7. Please use a newer version of Internet Explorer, Google Chrome, or Mozilla Firefox.");
    }

    var currentSize = 3;
    var currentFontSize;
    var currentFontSizeNum;
    var newFontSize;

    //adjust text size
    $("button.btposiA").on("click", function() {
        if (currentSize < 5) {
            resizeText('.contentItem', 1);
            resizeText('.post-title', 1);
            resizeText('.post-holder', 1);
            resizeText('.article-holder', 1);
            resizeText('.contentHeadline', 1);
            resizeText('.form-holder', 1);

            currentSize++;
        }
        return false;
    });

    $("button.btnegaA").on("click", function () {
        if (currentSize > 1) {
            resizeText('.contentItem', -1);
            resizeText('.post-title', -1);
            resizeText('.post-holder', -1);
            resizeText('.article-holder', -1);
            resizeText('.contentHeadline', -1);
            resizeText('.form-holder', -1);

            currentSize--;
        }
        return false;
    });

    $("#closeUserLogin").click(function () {
        $("#userLogin").dialog("close");
    });
    $("#closeForgotPassword").click(function () {
        $("#userForgotPassword").dialog("close");
    });
    $("#closeAccountLocked").click(function () {
        $("#accountLocked").dialog("close");
    });

    //IE8 background size fix for footer
    $(".footer").css("background-size", "cover");

    $("#article a").each(function() {
        if ($(this).text() === "") {
            $(this).replaceWith("<span class='anchor' id='" + $(this).attr("id") + "'></span>");
        }
    });

    $("body").on("click", "input.hasDatepicker", function() {
        if ($("body").css('color') === "rgb(0, 0, 0)") {
            $(".datepicker").remove();
        }
    });

    $(".tabButtons div").focus(function () {
        $(this).keydown(function (event) {
            if (event.keyCode == 13) {
                $(this).click();
            }
        });
    });

    $(".accordionContent").on("click", ".tooltip", function (event) {
        event.preventDefault();
    });

    $('tr:first-child').children('td').replaceWith(function(i, html) {
        return '<th>' + html + '</th>';
    });

    jQuery.extend(jQuery.validator.messages, {
        email: "This does not appear to be a valid email address e.g. john.smith@domain.com",
    });

    $(".wbr").each(function() {
        $(this).html($(this).text().replace(/\./g, '.<wbr>').replace(/\_/g, '_<wbr>'));
    });
});

