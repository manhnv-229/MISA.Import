
class BaseJS {
    constructor() {
        this.host = "";
        this.apiRouter = null;
        this.setApiRouter();
        this.SubApiRouter = '';
        this.initEvents();
        this.beforeLoadDataCustom();
        this.loadData();
        this.afterLoadDataCustom();
    }

    setSubApiEndPoint() {
        return null;
    }

    setApiRouter() {

    }

    beforeLoadDataCustom() {

    }

    afterLoadDataCustom() {

    }
    initEvents() {
        var me = this;
        // Sự kiện click khi nhấn thêm mới:
        $('#btnAdd').click(me.btnAddOnClick.bind(me));

        // Load lại dữ liệu khi nhấn button nạp:
        $('#btnRefresh').click(function () {
            me.loadData();
        })

        $('#btnRefresh').click(function () {
            me.loadData();
        })

        // Ẩn form chi tiết khi nhấn hủy:
        $('#btnCancel').click(function () {
            // Hiển thị dialog thông tin chi tiết:
            dialogDetail.dialog('close');
        })

        // Thực hiện lưu dữ liệu khi nhấn button [Lưu] trên form chi tiết:
        $('#btnSave').click(me.btnSaveOnClick.bind(me));


        // Hiển thị thông tin chi tiết khi nhấn đúp chuột chọn 1 bản ghi trên danh sách dữ liệu:
        $('table tbody').on('dblclick', 'tr', function (e) {
            $(this).find('td').addClass('row-selected');
            // load form:
            // load dữ liệu cho các combobox:
            var selects = $('select[fieldName]');
            selects.empty();
            $.each(selects, function (index, select) {
                // lấy dữ liệu nhóm khách hàng:
                var api = $(select).attr('api');
                var fieldName = $(select).attr('fieldName');
                var fieldValue = $(select).attr('fieldValue');
                $('.loading').show();
                $.ajax({
                    url: me.host + api,
                    method: "GET",
                    async: true
                }).done(function (res) {
                    if (res) {
                        console.log(res);
                        $.each(res, function (index, obj) {
                            var option = $(`<option value="${obj[fieldValue]}">${obj[fieldName]}</option>`);
                            console.log(select);
                            $(select).append(option);
                            console.log(option);
                        })
                    }
                    $('.loading').hide();
                }).fail(function (res) {
                    $('.loading').hide();
                })
            })
            
            me.FormMode = 'Edit';
            // Lấy khóa chính của bản ghi:
            var recordId = $(this).data('recordId');
            me.recordId = recordId;
            console.log(recordId);
            // Gọi service lấy thông tin chi tiết qua Id:
            $.ajax({
                url: me.host + me.apiRouter + `/${recordId}`,
                method: "GET",
                async: true
            }).done(function (res) {
                // Binding dữ liệu lên form chi tiết:
                console.log(res);

                // Lấy tất cả các control nhập liệu:
                var inputs = $('input[fieldName], select[fieldName]');
                var entity = {};
                $.each(inputs, function (index, input) {
                    var propertyName = $(this).attr('fieldName');
                    var value = res[propertyName];

                    // Đối với dropdowlist (select option):
                    if (this.tagName == "SELECT") {
                        var propValueName = $(this).attr('fieldValue');
                        value = res[propValueName];
                    }
                    // Đối với các input là radio:
                    if ($(this).attr('type') == "radio") {
                        var inputValue = this.value;

                        if (value == inputValue) {
                            this.checked = true;
                        } else {
                            this.checked = false;
                        }
                    } else {
                        $(this).val(value);
                    }
                     // Check với trường hợp input là radio, thì chỉ lấy value của input có attribute là checked:
                    //if ($(this).attr('type') == "radio") {
                    //    if (this.checked) {
                    //        entity[propertyName] = value;
                    //    }
                    //} else {
                    //    entity[propertyName] = value;
                    //}
                })
            }).fail(function (res) {

            })


            dialogDetail.dialog('open');
        })

        /* ------------------------------------
         * validate bắt buộc nhập::
         * CreatedBy: NVMANH (13/11/2020)
         */
        $('input[required]').blur(function () {
            // Kiểm tra dữ liệu đã nhập, nếu để trống thì cảnh báo:
            var value = $(this).val();
            if (!value) {
                $(this).addClass('border-red');
                $(this).attr('title', 'Trường này không được phép để trống');
                $(this).attr("validate", false);
            } else {
                $(this).removeClass('border-red');
                $(this).attr("validate", true);
            }

        })

        /* ------------------------------------
        * validate email đúng định dạng
        * CreatedBy: NVMANH (13/11/2020)
        */
        $('input[type="email"]').blur(function () {
            var value = $(this).val();
            var testEmail = /^[A-Z0-9._%+-]+@([A-Z0-9-]+\.)+[A-Z]{2,4}$/i;
            if (!testEmail.test(value)) {
                $(this).addClass('border-red');
                $(this).attr('title', 'Email không đúng định dạng.');
                $(this).attr("validate", false);
            } else {
                $(this).removeClass('border-red');
                $(this).attr("validate", true);
            }
        })
    }

    /** -----------------------------
     * Load dữ liệu
     * CreatedBy: NVMANH (11/11/2020)
     * */
    loadData() {
        var me = this;
        try {
            me.setSubApiEndPoint();
            $('table tbody').empty();
            // Lấy thông tin các cột dữ liệu:
            var columns = $('table thead th');
            var getDataUrl = this.getDataUrl;
            $('.loading').show();
            $.ajax({
                url: me.host + me.apiRouter + me.SubApiRouter,
                method: "GET",
                async: true,
            }).done(function (res) {
                me.buildTableHTML(res).bind(me);
               
                $('.loading').hide();
                $('#tbListData').tableScroll({ height: 150 });

                $('#thetable2').tableScroll();
                // Thực hiện thay đổi lại with của tiêu đề các cột:
                //var thorigins = $('#tbListData thead th');
                //var thResets = $('table.el-table__header thead th');
                //$.each(thorigins, function (index, th) {
                //    var withChange = th.offsetWidth;
                //    thResets[index].width = withChange;
                //    debugger;
                //})
            }).fail(function (res) {
                $('.loading').hide();
            })
        } catch (e) {
            console.log(e);
        }
    }

    buildTableHTML(data) {
        var me = this;
        var columns = $('table thead th');
        $.each(data, function (index, obj) {
            var tr = $(`<tr class="el-table__row"></tr>`);
            if (index == 0) {
                tr.addClass('first');
            }
            $(tr).data('recordId', obj.CustomerId);
            // Lấy thông tin dữ liệu sẽ map tương ứng với các cột:
            $.each(columns, function (index, th) {
                var td = $(`<td rowspan="1" colspan="1"><div class="cell"></div></td>`);
                var fieldName = $(th).attr('fieldname');
                var value = obj[fieldName];
                var formatType = $(th).attr('formatType');
                switch (formatType) {
                    case "ddmmyyyy":
                        td.find('div').addClass("text-align-center");
                        value = formatDate(value);
                        break;
                    case "Money":
                        td.addClass("text-align-right");
                        value = formatMoney(value);
                        break;
                    default:
                        break;
                }

                td.find('div').append(value);
                $(tr).append(td);
            })
            $('table tbody').append(tr);
        })
    }

    /** ----------------------------------
     * Hàm xử lý khi nhấn button thêm mới
     * Author: MANHNV (17/11/2020)
     * */
    btnAddOnClick() {
        try {
            var me = this;
            me.FormMode = 'Add';
            // Hiển thị dialog thông tin chi tiết:
            dialogDetail.dialog('open');
            $('input').val(null);
            // load dữ liệu cho các combobox:
            var select = $('select#cbxCustomerGroup');
            select.empty();
            // lấy dữ liệu nhóm khách hàng:
            $('.loading').show();
            $.ajax({
                url: me.host + "/api/customergroups",
                method: "GET"
            }).done(function (res) {
                if (res) {
                    console.log(res);
                    $.each(res, function (index, obj) {
                        var option = $(`<option value="${obj.CustomerGroupId}">${obj.CustomerGroupName}</option>`);
                        select.append(option);
                        console.log(option);
                    })
                }
                $('.loading').hide();
            }).fail(function (res) {
                $('.loading').hide();
            })
        } catch (e) {
            console.log(e);
        }
    }

    /** ----------------------------------
     * Hàm xử lý khi nhấn button Lưu
     * Author: MANHNV (17/11/2020)
     * */
    btnSaveOnClick() {
        var me = this;
        // validate dữ liệu:
        var inputVaidates = $('input[required], input[type="email"]');
        $.each(inputVaidates, function (index, input) {
            $(input).trigger('blur');
        })
        var inputNotValids = $('input[validate="false"]');
        if (inputNotValids && inputNotValids.length > 0) {
            alert("Dữ liệu không hợp lệ vui lòng kiểm tra lại.");
            inputNotValids[0].focus();
            return;
        }
        // thu thập thông tin dữ liệu được nhập -> build thành object:
        // Lấy tất cả các control nhập liệu:
        var inputs = $('input[fieldName], select[fieldName]');
        var entity = {};
        $.each(inputs, function (index, input) {
            var propertyName = $(this).attr('fieldName');
            var value = $(this).val();

            // Check với trường hợp input là radio, thì chỉ lấy value của input có attribute là checked:
            if ($(this).attr('type') == "radio") {
                if (this.checked) {
                    entity[propertyName] = value;
                }
            } else {
                entity[propertyName] = value;
            }

            if (this.tagName == "SELECT") {
                var propertyName = $(this).attr('fieldValue');
                entity[propertyName] = value;
            }
        })
        var method = "POST";
        if (me.FormMode == 'Edit') {
            method = "PUT";
            entity.CustomerId = me.recordId;
        }
        // Gọi service tương ứng thực hiện lưu dữ liệu:
        $.ajax({
            url: me.host + me.apiRouter,
            method: method,
            data: JSON.stringify(entity),
            contentType: 'application/json'
        }).done(function (res) {
            // Sau khi lưu thành công thì: 
            // + đưa ra thông báo thành công, 
            // + ẩn form chi tiết, 
            // + load lại lại dữ liệu
            showSuccessMessenger();
            dialogDetail.dialog('close');
            me.loadData();
            debugger
        }).fail(function (res) {
            debugger
        })
    }
}