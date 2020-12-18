$(document).ready(function () {
    //$('input[type="text"].dateInput img').on('click', function () {
    //    var k = this;
    //    return k.datepicker._datepickerShowing && k.datepicker._lastInput === t[0] ? k.datepicker._hideDatepicker() : (k.datepicker._datepickerShowing && k.datepicker._lastInput !== t[0] && k.datepicker._hideDatepicker(),
    //        k.datepicker._showDatepicker(t[0])),
    //        !1
    //})
})
/** ----------------------------------------------
 * Format dữ liệu ngày tháng sang ngày/tháng/năm
 * @param {any} date tham số có kiểu dữ liệu bất kỳ
 * CreatedBy: NVMANH (11/11/2020)
 */
function formatDate(date) {
    var date = new Date(date);
    if (Number.isNaN(date.getTime())) {
        return "";
    } else {
        var day = date.getDate(),
            month = date.getMonth() + 1,
            year = date.getFullYear();
        day = day < 10 ? '0' + day : day;
        month = month < 10 ? '0' + month : month;

        return day + '/' + month + '/' + year;
    }
}

/** -------------------------------------
 * Hàm định dạng hiển thị tiền tệ
 * @param {Number} money Số tiền
 * CreatedBy: NVMANH (11/11/2020)
 */
function formatMoney(money) {
    if (money) {
        return money.toFixed(0).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1.");
    }
    return "";
}


function showSuccessMessenger() {
    var html = `<div class="box-toast-msg">Thành công</div>`;
    if ($('body').find('.box-toast-msg').length == 0) {
        $('body').append(html);
    }
    $('.box-toast-msg').toggle();
    setTimeout(function () {
        $('.box-toast-msg').toggle();
    }, 2000)

}

var delayFunction = (function () {
    var ticker = null;
    return function (callback, ms, jsObject) {
        if (ticker !== null) {
            clearTimeout(ticker)
        }
        if (jsObject == null) {
            ticker = setTimeout(callback, ms)
        } else {
            ticker = setTimeout(callback.bind(jsObject), ms)
        }
    }
}());

var commonJs = {
    showLoading: function () {
        var html = `<div class="m-loading" style="display: none;">
                        <div class="loading-modal"></div>
                        <div class="loading-icon"></div>
                    </div>`;
        var loadingEl = $('.m-loading');
        if (loadingEl.length == 0) {
            $('body').append(html);
        }
        $('.m-loading').show();
    },
    hideLoading: function () {
        $('.m-loading').hide();
    }
}