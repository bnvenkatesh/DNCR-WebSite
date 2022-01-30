$(function() {
    $.widget("sd.addmorenumbers", {
        // default options
        options: {
        },

        _create: function () {
            var amount = this.element.data('add-more-numbers');
            var addFaxes = this.element.data('add-faxes');
            var checkFaxes = this.element.data('check-faxes');
			var that = this;

			this.element.click(function () {
			    var count = that.element.closest(".accordionContent").find(".phoneNumber").length;
			    that.addPhoneNumberField(amount, count, addFaxes, checkFaxes);
				if (count >= 15) {
				    that._destroy();
				}
			});
        },

        addPhoneNumberField: function (number, count, addFaxes, checkFaxes) {
            for (var i = 0; i < number; i++) {
                var htmlElements = "<div class='formField fieldMargin phoneNumber'>" +
                    "<div class='floatLeft'><label class='inputLabel' for='Numbers_" + (count + i) + "__Number'>Phone " + (count + 1 + i) +
                    "<span class='visuallyhidden'> Numbers must be 10 digits beginning with 0. Numbers starting with 18 must be 7 or 10 digits. Numbers starting with 13 must be 6, 8 or 10 digits. E.g. 0212345678 or 0412345678</span></label></div>" +
                    "<div class='floatLeft'><input id='Numbers_" + (count + i) + "__Number' class='long phoneInput' type='text' name='Numbers[" + (count + i) + "].Number'" +
                    "data-val-regex-pattern='^(([ ().-]*(0)([ ().-]*[0-9][ ().-]*){9})|([ ().-]*(18)([ ().-]*[0-9][ ().-]*){5}(([ ().-]*[0-9][ ().-]*){3})?)|([ ().-]*(13)([ ().-]*[0-9][ ().-]*){4}(([ ()-]*[0-9][ ()-]*){2})?(([ ()-]*[0-9][ ()-]*){2})?))$' data-val-regex='The number you have provided is not valid. <br/>Numbers must start with 0, 13 or 18. <br/>Numbers starting with 0 must be 10 digits. <br/>Numbers starting with 18 must be 7 or 10 digits. <br/>Numbers starting with 13 must be 6, 8 or 10 digits. <br/>Optional spaces or - dashes or ( ) brackets are allowed. <br/>Letters (a-z), plus (+) and other characters are not accepted. E.g. 02 1234 5678 or 0412 345 678' data-val='true'>";
                if (addFaxes) {
                    if (checkFaxes) {
                        htmlElements = htmlElements + "<input id='Numbers_" + (count + i) + "__IsFax' class='faxes readonly' type='checkbox' value='true' name='Numbers[" + (count + i) + "].IsFax' disabled='true' checked/><label for='Numbers_" + (count + i) + "__IsFax'>Used for faxes</label>";
                    } else {
                        htmlElements = htmlElements + "<input id='Numbers_" + (count + i) + "__IsFax' class='faxes readonly' type='checkbox' value='true' name='Numbers[" + (count + i) + "].IsFax' disabled='true' /><label for='Numbers_" + (count + i) + "__IsFax'>Used for faxes</label>";
                    }
                }

                htmlElements = htmlElements + "<span class='error numberError field-validation-valid' data-valmsg-replace='true' data-valmsg-for='Numbers[" + (count + i) + "].Number'></span>";

                if (addFaxes) {
                    htmlElements = htmlElements + "<span class='error numberError mobileFax' style='display:none'><span>Mobile numbers cannot be registered as fax numbers. Please enter a fixed line number or uncheck the 'Used for faxes' checkbox.</span></span>";
                }

                htmlElements = htmlElements + "</div><div class='clear'></div></div>";

                this.element.parent().before(htmlElements);
            }
            var form = this.element.closest("form").removeData("validator").removeData("unobtrusiveValidation");
            $.validator.unobtrusive.parse(form);
		},

		_destroy: function () {
		    this.element.parent().remove();
		}
    });
})