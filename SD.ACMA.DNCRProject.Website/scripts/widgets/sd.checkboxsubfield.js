$(function() {
    $.widget("sd.checkboxsubfield", {
        // default options
        options: {
        },

        _create: function () {
            var checkboxsubfield = "." + this.element.data('checkbox-subfield');
            var that = this.element;

            this.element.on('change', function () {
                if (this.checked) {
                    $(this).closest(".accordionContent").find(checkboxsubfield).show();
                } else {
                    $(this).closest(".accordionContent").find(checkboxsubfield).hide();
                }
            });
        },

		_destroy: function () {
		}
    });
})