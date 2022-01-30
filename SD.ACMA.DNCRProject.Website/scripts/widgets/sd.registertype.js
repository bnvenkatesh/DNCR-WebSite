$(function() {
    $.widget("sd.registertype", {
        // default options
        options: {
        },

        _create: function () {
            var type = this.element.data('register-type');
            var headerText = this.element.data('header-text');
			var that = this.element;

			this.element.on('click', function () {
                if (headerText != null) {
                    var accordionItem = that.closest(".accordion li");
                    accordionItem.next().find(".accordionHeader span.title").text("2. " + headerText);
                }
			    var accordion = that.closest(".accordion");
			    accordion.find(".registertype").hide();
			    accordion.find("." + type).show();
			});
        },

		_destroy: function () {
		}
    });
})