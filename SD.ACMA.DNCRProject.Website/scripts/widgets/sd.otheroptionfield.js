$(function() {
    $.widget("sd.otheroptionfield", {
        // default options
        options: {
        },

        _create: function () {
            var otherOptionField = "." + this.element.data('other-option-field');
            var optionName = this.element.attr("name");
            var that = this.element;

            var value = $("input:radio[name='" + optionName + "']:checked").val();
            if (value === that.val()) {
                $(that).closest(".accordionContent").find(otherOptionField).show();
            } else $(that).closest(".accordionContent").find(otherOptionField).hide();

            $("input:radio[name='" + optionName + "']").on('click', function () {
                var value = $(this).val();
                if (value === that.val()) {
                    $(this).closest(".accordionContent").find(otherOptionField).show();
                } else $(this).closest(".accordionContent").find(otherOptionField).hide();
            });
        },

		_destroy: function () {
		}
    });
})