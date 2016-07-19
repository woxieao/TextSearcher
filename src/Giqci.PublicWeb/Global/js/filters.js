'use strict';
app.filter('zcode_language', function () {
    return function (data) {
        var result ;
        if ($giqci.getLanType() === 'en') {
            switch (data) {
                case '实体码':
                    result = $giqci.KeyToWord('entity_code');
                    break;
                case '电子码':
                    result = $giqci.KeyToWord('electronic_code');
                    break;
                default:
                    result = '';
                    break;
            }
            return result;
        } else {
            return data;
        }
    }
});

//打印页面的多语言过滤器
app.filter('trade_type_language', function () {
    return function (data) {
        var result = '';
        if ($giqci.getLanType() === 'en') {
            switch (data) {
                case '一般贸易':
                    result = $giqci.KeyToWord('general_trade');
                    break;
                case '电商贸易':
                    result = $giqci.KeyToWord('electricity_trade');
                    break;
                default:
                    result = '';
                    break;
            }
            return result;
        } else {
            return data;
        }
    }
});

//'是'和'否'的多语言处理
app.filter('TorF_language', function () {
    return function (data) {
        var result = '';
        if ($giqci.getLanType() === 'en') {
            switch (data) {
                case '是':
                    result = 'Yes';
                    break;
                case '否':
                    result = 'No';
                    break;
                default:
                    result = '';
                    break;
            }
            return result;
        } else {
            return data;
        }
    }
});

//申请列表中，申请状态的多语言处理
app.filter('application_status_language', function () {
        return function (data) {
            var result = '';
            if ($giqci.getLanType() === 'en') {
                switch (data) {
                    case '已接受申请':
                        result = $giqci.KeyToWord('application_accepted');
                        break;
                    case '新申请':
                        result =  $giqci.KeyToWord('new_application');
                        break;
                    case '编辑结束':
                        result =  $giqci.KeyToWord('edit_finish');
                        break;
                    case '申请结束':
                        result =  $giqci.KeyToWord('application_finish');
                        break;
                    case '申请驳回':
                        result = $giqci.KeyToWord('application_dismissed');
                        break;
                    default:
                        result = '';
                        break;
                }
                return result;
            } else {
                return data;
            }
        }
});

//产品列表包装的多语言处理
app.filter('product_package_language', function () {
    return function (data) {
        var result = '';
        if ($giqci.getLanType() === 'en') {
            switch (data) {
                case '盒装':
                    result = $giqci.KeyToWord('box');
                    break;
                case '瓶装':
                    result = $giqci.KeyToWord('bottled');
                    break;
                case '罐装':
                    result = $giqci.KeyToWord('canned');
                    break;
                case '袋装':
                    result = $giqci.KeyToWord('bag');
                    break;
                default:
                    result = '';
                    break;
            }
            return result;
        } else {
            return data;
        }
    }
});
