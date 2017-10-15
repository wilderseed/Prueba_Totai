(function ($) {
    $.fn.initTable = function (options) {
        var defaults = $.extend({
            header: [],
            data: [],
            emptyMessage: "No se encontraron resultados"
        }, options);

        var current_table = $(this);
        var table = $.extend(defaults, options);

        var methods = {
            init: function () {
                return this.each(function () {
                    methods.apendHtml();
                    methods.setEventHandlers();
                });
            },
            apendHtml: function () {
                var data_rows_count = table.data.length;

                if (data_rows_count > 0) {

                }
                else {
                    current_table.append('<h4>' + table.emptyMessage + '</>');
                }
            },
            setEventHandlers: function () {
                $('.footable').footable();
            }
        }

        if (methods[options]) {
            return methods[options].apply(this, Array.prototype.slice.call(arguments, 1));
        }
        else {
            if (typeof options == "object" || !options) {
                return methods.init.apply(this);
            }
            else {
                $.error('Método "' + method + '" no existe en este plugin!');
            }
        }
    }
})
(jQuery);