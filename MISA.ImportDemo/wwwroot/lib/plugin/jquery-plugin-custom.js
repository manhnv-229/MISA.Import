$(document).ready(function () {
    new MPlugin();
    (function ($) {
        $.fn.extend({
            // Lấy value với các element xác định là combobox:
            getValue: function (options) {
                var controlType = this.attr("control-type");
                switch (controlType) {
                    case "combobox":
                        var data = $(this).data('selected');
                        if (data && data.value) {
                            return data.value;
                        } else {
                            return null;
                        }
                        break;
                    default:
                        return null;
                }
            },
            getText: function (options) {
                var controlType = this.attr("control-type");
                switch (controlType) {
                    case "combobox":
                        var data = $(this).data('selected');
                        if (data && data.text) {
                            return data.text;
                        } else {
                            return null;
                        }
                        break;
                    default:
                        return null;
                }
            },
            getData: function (options) {
                var controlType = this.attr("control-type");
                switch (controlType) {
                    case "combobox":
                        return $(this).data('data');
                        break;
                    default:
                        return null;
                }
            }
        });
    })(jQuery);
})

class MPlugin {
    constructor() {
        this.init();
        this.initEvents();
    }
    init() {
        this.buildElements();
    }

    initEvents() {
        var me = this;
        $(document).on('keyup', 'input[date-picker].hasDatepicker', function () {
            var value = this.value;
            if (value && (value.length == 2 || value.length == 5)) {
                value = value + '/';
            }
            $(this).val(value);
        })


        $(document).on('keydown', '[control-type="combobox"] input.m-combobox-input,[control-type="combobox"] a.m-combobox-trigger', function (e) {
            var keyCode = e.keyCode;
            switch (keyCode) {
                case 13:
                    me.selectItemOnEnter(this.parentElement);
                    break;
                case 40:
                    me.setFocusComboboxItem(this.parentElement, true);
                    break;
                case 38:
                    me.setFocusComboboxItem(this.parentElement, false);
                    break;
                default:
                    break;
            }

        })

        /* -----------------------------------------------------
         * Thực hiện auto complete - nhập liệu thì thực hiện tìm kiếm dữ liệu
         * Author: NVMANH (07/12/2020)
         */
        $(document).on('input', '[control-type="combobox"] input.m-combobox-input', function () {
            debugger
            console.log('input');
            var inputValue = this.value;
            var combobox = $(this).parent();
            var data = combobox.data('data');
            var entity = combobox.data('entity');
            var dataFilter = me.getDataMathComboboxInputText(combobox, inputValue);
            var dataFilter = $.grep(data, function (item) {
                return item[entity.FieldText].toLowerCase().includes(inputValue.toLowerCase()) == true;
            })

            me.buildHTMLComboboxData(combobox, dataFilter, entity.FieldText, entity.FieldValue);
            combobox.data('areFiltering', true);
            combobox.data('selected', null);
            if (dataFilter && dataFilter.length > 0) {
                console.log('input event');
                var comboboxData = $(this).siblings('.m-combobox-data');
                // mặc định focus luôn vào item đầu tiên:
                comboboxData.children().first().addClass('mCombobox__item--focus');
                comboboxData.show();
            } else {
                console.log('hide data on input: ' + combobox);
                $(this).siblings('.m-combobox-data').hide();
            }
        })


        $(document).on('blur', '[control-type="combobox"] input.m-combobox-input, [control-type="combobox"] a', function (e) {
            console.log(e);
            var combobox = this.parentElement;
            var inputCombobox = $(combobox).children('input.m-combobox-input');
            var inputText = inputCombobox.val();
            var entity = $(combobox).data('entity')
            var dataMatch = me.getDataMathComboboxInputText(combobox, inputText);
            console.log(dataMatch);

            // Có nhập liệu nhưng không có data phù hợp:
            if (dataMatch && dataMatch.length == 0 && inputText.trim() != '') {
                $(inputCombobox).addClass('border-red');
                $(inputCombobox).attr('title', 'Dữ liệu không tồn tại trong hệ thống.');

            }// có nhập liệu và tìm được data phù hợp
            else if (dataMatch.length > 0 && inputText.trim() != '') {
                $(inputCombobox).removeClass('border-red');
                $(inputCombobox).removeAttr('title');
                var itemSelected = dataMatch[0]; // chỉ lấy phần tử đầu tiên.
                inputCombobox.val(itemSelected[entity["FieldText"]]);
                $(combobox).data('selected', itemSelected);
            } else {
                $(inputCombobox).removeClass('border-red');
                $(inputCombobox).removeAttr('title');
            }
            console.log('----hide when blur-----')
            $(inputCombobox).siblings('.m-combobox-data').hide();
            //setTimeout(function () {
            //    console.log('hide data on blur: ' + combobox);
            //    $(inputCombobox).siblings('.m-combobox-data').hide();
            //}, 1)

            //if ($(combobox).has(e.relatedTarget).length == 0) {
            //    // Check dữ liệu có hợp lệ hay không

            //    // Focus ra ngoài combobox thì ẩn box chọn data đi:
            //    $(this).siblings('.m-combobox-data').hide();
            //}
        })

        //TODO: Chọn item trong combobox:
        $(document).on('click', '.m-combobox a.m-combobox-trigger', function () {
            var comboboxData = $(this).siblings('.m-combobox-data');
            var combobox = comboboxData.parent();

            // Build lại dữ liệu cho combobox:
            var data = combobox.data('data');
            var areFiltering = combobox.data('areFiltering');

            // Nếu đang có việc filter dữ liệu thì load lại data, không thì thôi:
            if (data && areFiltering) {
                combobox.data('areFiltering', false);
                var entity = combobox.data('entity');
                me.buildHTMLComboboxData(combobox, data, entity.FieldText, entity.FieldValue);
            }

            // Hiển thị màu sắc item được chọn có trong danh sách:
            me.setStyleItemSelected(combobox);
            //var itemSelected = $(combobox).data('selected');
            //comboboxData.children().removeClass('mCombobox__item--selected');
            //if (itemSelected && itemSelected.value) {
            //    var value = itemSelected.value;
            //    comboboxData.children("[value='" + value + "']").addClass('mCombobox__item--selected');
            //}
            comboboxData.toggle();
            $(this).siblings('input').focus();
        })

        //TODO: xây dựng combobox động
        /* -------------------------------------
         * Khi thực hiện nhấn chuột chọn 1 item
         * Author: NVMANH (09/12/2020)
         */
        $(document).on('mousedown', '.m-combobox .m-combobox-item', function (sender) {
            console.log(sender);
            event.preventDefault();
            var comboboxData = this.parentElement;
            var input = $(comboboxData).siblings('input');
            var value = this.getAttribute('value'),
                text = this.firstElementChild.textContent;
            input.val(text);
            $(input.parent()).data("selected", { text: text, value: value });
            console.log('toggle combobox when click item');
            $(comboboxData).toggle();
            input.removeClass('border-red');
            event.stopPropagation();
        })
    }

    buildElements() {
        this.buildCombobox();
        this.buildDatePicker();
    }

    //TODO: build html cho combobox
    buildCombobox(inputs) {
        var me = this;
        if (!inputs) {
            inputs = $('mcombobox');
        }
        $.each(inputs, function (index, combobox) {
            var apiGetUrl = $(this).attr('apiGetUrl');
            if (apiGetUrl) {
                $.ajax({
                    method: 'GET',
                    url: apiGetUrl,
                }).done(function (res) {
                    me.buildHTMLCombobox(combobox, res);
                }).fail(function () {
                    me.buildHTMLCombobox(combobox, null);
                })
            } else {
                me.buildHTMLCombobox(combobox, null);
            }
        })
    }

    buildHTMLCombobox(combobox, data) {
        var label = $(combobox).attr('label'),
            id = $(combobox).attr('id'),
            labelCls = $(combobox).attr('label-cls'),
            controlCls = $(combobox).attr('input-cls'),
            dataIndex = $(combobox).attr('dataIndex'),
            fieldValue = $(combobox).attr('fieldValue'),
            fieldText = $(combobox).attr('fieldText');
        var controlHtml = $(`<div id="` + id + `" class="m-combobox" control-type="combobox">
                                    <div class="m-label `+ labelCls + `">` + label + `</div>
                                    <input class="m-combobox-input `+ controlCls + `" type="text" autocomplete="off" />
                                    <a class="m-combobox-trigger" title="Hiển thị tất cả ${label}"><i class="fas fa-chevron-down"></i></a>
                                    <div class="m-combobox-data">
                                    </div>
                                </div>`);
        // Lưu trữ dữ liệu của combobox
        $(controlHtml).data('data', data);

        // Lưu trữ thông tin entity sẽ bindding của Object:
        $(controlHtml).data('entity', {
            DataIndex: dataIndex,
            FieldText: fieldText,
            FieldValue: fieldValue
        });
        if (data) {
            this.buildHTMLComboboxData(controlHtml, data, fieldText, fieldValue);
        }
        $(combobox).replaceWith(controlHtml);
    }

    /** ----------------------------------------------------------------
     * Thực hiện buil các item html cho element chứa dữ liệu cho combobox
     * @param {HTMLElement} comboboxHTML HTML của combobox
     * @param {Array} data mảng dữ liệu truyền vào
     * CreatedBy: NVMANH (03/12/2020)
     */
    buildHTMLComboboxData(comboboxHTML, data, fieldText, fieldValue) {
        var comboboxDataEl = $(comboboxHTML).find('.m-combobox-data');
        // clear toàn bộ dữ liệu cũ:
        $(comboboxDataEl).empty();
        // Không có data thì lấy ở .data()
        if (!data) {
            data = $(comboboxHTML).data('data');
        }
        // Không có thông tin text và value thì lấy ở .data()
        if (!fieldText && !fieldValue) {
            var entityInfo = $(comboboxHTML).data('entity');
            fieldText = entityInfo['FieldText'];
            fieldValue = entityInfo['FieldValue'];
        }
        $.each(data, function (index, item) {
            var text = item[fieldText],
                value = item[fieldValue];
            var itemHTML = `<a class="m-combobox-item" value="` + value + `"><span>` + text + `</span></a>`;
            comboboxDataEl.append(itemHTML);
        })
    }

    //TODO: đang lỗi không set được focus
    setFocusComboboxItem(combobox, isNext) {
        console.log('setFocusComboboxItem');
        this.setStyleItemSelected(combobox);
        var comboboxData = $(combobox).children('.m-combobox-data');
        var childrenFirst = comboboxData.children().first();
        var childrenLast = comboboxData.children().last();
        var childrenSelected = comboboxData.children('.mCombobox__item--selected');
        var itemFocusCurrent = comboboxData.children('.mCombobox__item--focus').first();
        var comboboxDataNotShow = (comboboxData.css('display') == 'none');
        comboboxData.show();
        
        // Hiển thị các item lựa chọn:
        if (comboboxDataNotShow && isNext || (isNext && !comboboxDataNotShow && itemFocusCurrent.length == 0)) {
            if (childrenSelected.length > 0) {
                return;
            }
            childrenFirst.addClass('mCombobox__item--focus');
        } else if (comboboxDataNotShow && !isNext || (!isNext && !comboboxDataNotShow && itemFocusCurrent.length == 0)) {
            if (childrenSelected.length > 0) {
                return;
            }
            childrenLast.addClass('mCombobox__item--focus');
        } else {
            itemFocusCurrent.removeClass('mCombobox__item--focus');
            /* - Chưa có item nào được focus thì focus luôn thằng đầu tiên
               - Có thằng focus rồi thì thực hiện focus thằng tiếp theo: */
            if (isNext) {
                if (itemFocusCurrent.next().length > 0) {
                    itemFocusCurrent.next().addClass('mCombobox__item--focus');
                } else {
                    if (childrenSelected.length > 0) {
                        return;
                    }
                    childrenFirst.addClass('mCombobox__item--focus');
                }

            } else {
                if (itemFocusCurrent.prev().length > 0) {
                    itemFocusCurrent.prev().addClass('mCombobox__item--focus');
                } else {
                    if (childrenSelected.length > 0) {
                        return;
                    }
                    childrenLast.addClass('mCombobox__item--focus');
                }
            }
        }
    }

    setStyleItemSelected(combobox) {
        // Hiển thị màu sắc item được chọn có trong danh sách:
        var comboboxData = $(combobox).children('.m-combobox-data');
        var itemSelected = $(combobox).data('selected');
        comboboxData.children().removeClass('mCombobox__item--selected');
        if (itemSelected && itemSelected.value != null && itemSelected.value != undefined) {
            var value = itemSelected.value;
            comboboxData.children("[value='" + value + "']").addClass('mCombobox__item--selected');
        }
    }

    /** -----------------------------------------------------------------
     * Thực hiện nhấn enter trong input có thể lựa chọn các item:
     * @param {HTMLElement} combobox
     * CreatedBy : NVMANH (09/12/2020)
     */
    selectItemOnEnter(combobox) {
        var comboboxData = $(combobox).children('.m-combobox-data');
        // Kiểm tra xem box data có hiển thị không, 
        // nếu không thì hiển thị, hiển thị rồi thì thực hiện select luôn item đang focus:
        if (comboboxData.css('display') == 'none') {
            console.log('show data when select item on ENTER: ' + combobox);
            console.log(comboboxData);
            comboboxData.show();
            //setTimeout(function () {
            //    comboboxData.show();
            //}, 1)
            
        } else {
            var itemSelected = comboboxData.children('a.mCombobox__item--focus');
            itemSelected.trigger('mousedown');
            console.log('hide data selectItemOnEnter: ' + combobox);
            comboboxData.hide();
            //setTimeout(function () {
            //    comboboxData.hide();
            //}, 1)
        }
    }

    /**
     * Lấy dữ liệu có text hiển thị trùng với text nhập từ input
     * @param {Element} combobox element combobox
     * @param {string} inputText text nhập trong input
     * Author: NVMANH(09/12/2020)
     */
    getDataMathComboboxInputText(combobox, inputText) {
        var entity = $(combobox).data('entity'),
            data = $(combobox).data('data');
        var dataMatch = $.grep(data, function (item) {
            return item[entity.FieldText].toLowerCase() == (inputText.toLowerCase()) == true;
        })
        return dataMatch;
    }

    //TODO: build html date picker:
    buildDatePicker() {
        var inputs = $('m-date-picker');
        $.each(inputs, function (index, input) {
            var label = $(input).attr('label'),
                id = $(input).attr('id'),
                labelCls = $(input).attr('label-cls'),
                controlCls = $(input).attr('input-cls'),
                format = $(input).attr('format'),
                fieldName = $(input).attr('fieldName');
            var controlHtml = $(`<div class="m-date-picker">
                                     <div class="` + (labelCls ? labelCls : '') + `">` + (label ? label : '') + `</div>
                                     <div class="` + (controlCls ? controlCls : '') + `">
                                        <div class="dateInput">
                                            <input id="` + (id ? id : '') + `" date-picker format="` + (format ? format : '') + `" fieldName="` + (fieldName ? fieldName : '') + `" type="text" placeholder="_ _/ _ _/ _ _ _ _" autocomplete="off"/>
                                            <div class="dateInput-icon-box"></div>
                                        </div>
                                    </div>
                                </div>`);
            $(this).replaceWith(controlHtml);
            $("#" + id + "").datepicker({
                showOn: "button",
                buttonImage: "/content/icon/date-picker.svg",
                buttonImageOnly: true,
                buttonText: "Chọn ngày",
                dateFormat: "dd/mm/yy"
            });
        })
    }
}