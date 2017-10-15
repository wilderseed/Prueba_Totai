(function ($) {
    $.fn.initTable = function (options) {
        var defaults = $.extend({
            headFixed: true,
            footFixed: false,
            leftFixed: 0,
            rightFixed: 0,
            zIndex: 50
        }, options);

        var current_table = $(this);
        var current_parent = current_table.parent();

        current_parent.css({
            "overflow-x": "auto",
            "overflow-y": "auto"
        });

        var table = $.extend(defaults, options);

        var leftColumns = $();
        var rightColumns = $();

        var methods = {
            init: function () {
                return this.each(function () {
                    methods.apendHtml();
                    methods.setEventHandlers();
                });
            },
            apendHtml: function () {
                if (table.headFixed) {
                    methods.fixHead();
                }

                if (table.footFixed) {
                    methods.fixFoot();
                }

                if (table.leftFixed > 0) {
                    methods.fixLeft();
                }

                if (table.rightFixed > 0) {
                    methods.fixRight();
                }

                methods.setCorner();
            },
            setEventHandlers: function () {
                current_parent.on("scroll", function () {
                    var scrollTop = current_parent.scrollTop();
                    var scrollLeft = current_parent.scrollLeft();

                    var scrollWidth = current_parent[0].scrollWidth;
                    var scrollHeight = current_parent[0].scrollHeight;

                    var clientWidth = current_parent[0].clientWidth;
                    var clientHeight = current_parent[0].clientHeight;

                    if (table.headFixed) {
                        current_table.find("thead tr > *").css("top", scrollTop);
                    }

                    if (table.footFixed) {
                        current_table.find("tfoot tr > *").css("bottom", scrollHeight - clientHeight - scrollTop);
                    }

                    if (table.leftFixed > 0) {
                        leftColumns.css("left", scrollLeft);
                    }

                    if (table.rightFixed > 0) {
                        rightColumns.css("right", scrollWidth - clientWidth - scrollLeft);
                    }
                });
            },
            fixHead: function () {
                var cells = current_table.find("thead tr > *");

                methods.setBackground(cells);
                cells.css("position", "relative");
            },
            fixFoot: function () {
                var cells = current_table.find("tfoot tr > *");

                methods.setBackground(cells);
                cells.css("position", "relative");
            },
            fixLeft: function () {
                var tr = current_table.find("tr");

                tr.each(function (k, row) {
                    methods.solveLeftColspan(row, function (cell) {
                        $(cell).addClass("leftColumn");
                    });
                })

                leftColumns = current_table.find('.leftColumn');

                leftColumns.each(function (k, c) {
                    var cell = $(c);

                    methods.setBackground(cell);
                    cell.css("position", "relative");
                });
            },
            fixRight: function () {
                var tr = current_table.find("tr");

                tr.each(function (k, row) {
                    methods.solveRightColspan(row, function (cell) {
                        $(cell).addClass("rightColumn");
                    });
                })

                rightColumns = current_table.find('.rightColumn');

                rightColumns.each(function (k, c) {
                    var cell = $(c);

                    methods.setBackground(cell);
                    cell.css("position", "relative");
                });
            },
            setCorner: function () {
                if (table.headFixed) {
                    if (table.leftFixed > 0) {
                        var tr = current_table.find("thead tr");

                        tr.each(function (k, row) {
                            methods.solveLeftColspan(row, function (cell) {
                                $(cell).css("z-index", table.zIndex + 1);
                            });
                        })
                    }

                    if (table.rightFixed > 0) {
                        var tr = current_table.find("thead tr");

                        tr.each(function (k, row) {
                            methods.solveRightColspan(row, function (cell) {
                                $(cell).css("z-index", table.zIndex + 1);
                            });
                        })
                    }
                }

                if (table.footFixed) {
                    if (table.leftFixed > 0) {
                        var tr = current_table.find("thead tr");

                        tr.each(function (k, row) {
                            methods.solveLeftColspan(row, function (cell) {
                                $(cell).css("z-index", table.zIndex);
                            });
                        })
                    }

                    if (table.rightFixed > 0) {
                        var tr = current_table.find("thead tr");

                        tr.each(function (k, row) {
                            methods.solveRightColspan(row, function (cell) {
                                $(cell).css("z-index", table.zIndex);
                            });
                        })
                    }
                }
            },
            solveLeftColspan: function (row, action) {
                var fixColumn = table.leftFixed;
                var inc = 1;

                for (var i = 1; i <= fixColumn; i = i + inc) {
                    var nth = inc > 1 ? i - 1 : i;

                    var cell = $(row).find("> *:nth-child(" + nth + ")");
                    var colspan = cell.prop("colspan");

                    if (cell.cellPos().left < fixColumn) {
                        action(cell);
                    }

                    inc = colspan;
                }
            },
            solveRightColspan: function (row, action) {
                var fixColumn = table.rightFixed;
                var inc = 1;

                for (var i = 1; i <= fixColumn; i = i + inc) {
                    var nth = inc > 1 ? i - 1 : i;

                    var cell = $(row).find("> *:nth-child(" + nth + ")");
                    var colspan = cell.prop("colspan");

                    action(cell);
                    inc = colspan;
                }
            },
            setBackground: function (elements) {
                elements.each(function (k, e) {
                    var element = $(e);
                    var parent = element.parent();

                    var elementBackground = element.css("background-color");
                    elementBackground = (elementBackground == "transparent" || elementBackground == "rgba(0, 0, 0, 0)") ? null : elementBackground;

                    var parentBackground = parent.css("background-color");
                    parentBackground = (parentBackground == "transparent" || parentBackground == "rgba(0, 0, 0, 0)") ? null : parentBackground;

                    var background = parentBackground ? parentBackground : "white";
                    background = elementBackground ? elementBackground : background;

                    element.css("background-color", background);
                });
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
})(jQuery);

/*  cellPos jQuery plugin
 ---------------------
 Get visual position of cell in HTML table (or its block like thead).
 Return value is object with "top" and "left" properties set to row and column index of top-left cell corner.
 Example of use:
 $("#myTable tbody td").each(function(){
 $(this).text( $(this).cellPos().top +", "+ $(this).cellPos().left );
 });
 */

(function ($) {
    /* scan individual table and set "cellPos" data in the form { left: x-coord, top: y-coord } */
    function scanTable($table) {
        var m = [];

        $table.children("tr").each(function (y, row) {
            $(row).children("td, th").each(function (x, cell) {
                var $cell = $(cell),
                    cspan = $cell.attr("colspan") | 0,
                    rspan = $cell.attr("rowspan") | 0,
                    tx, ty;

                cspan = cspan ? cspan : 1;
                rspan = rspan ? rspan : 1;

                for (; m[y] && m[y][x]; ++x);  //skip already occupied cells in current row

                for (tx = x; tx < x + cspan; ++tx) {  //mark matrix elements occupied by current cell with true
                    for (ty = y; ty < y + rspan; ++ty) {
                        if (!m[ty]) {  //fill missing rows
                            m[ty] = [];
                        }

                        m[ty][tx] = true;
                    }
                }

                var pos = { top: y, left: x };
                $cell.data("cellPos", pos);
            });
        });
    };

    /* plugin */
    $.fn.cellPos = function (rescan) {
        var $cell = this.first(),
            pos = $cell.data("cellPos");

        if (!pos || rescan) {
            var $table = $cell.closest("table, thead, tbody, tfoot");
            scanTable($table);
        }

        pos = $cell.data("cellPos");
        return pos;
    }
})(jQuery);