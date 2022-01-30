$(function() {
    $.widget("sd.bulktransactiontype", {
        // default options
        options: {
        },

        _create: function () {
            var type = this.element.data('bulk-transaction-type');
            var headerText = this.element.data('header-text');
			var that = this.element;

			this.element.on('click', function () {
			    var accordionContent = that.closest(".accordionContent");
			    //accordionContent.siblings(".accordionHeader").find("span.title").html("2. " + headerText);
			    accordionContent.find(".bulkchange").hide();
			    accordionContent.find("." + type).show();
			});
        },

		_destroy: function () {
		}
    });
})