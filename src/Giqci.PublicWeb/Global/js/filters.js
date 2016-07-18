'use strict';
app.filter('zcode_language', function () {
    return function (data) {
        var result = '';
        if (window.location.pathname.split('/')[1].toLocaleLowerCase() === 'en') {
            switch (data) {
                case '实体码':
                    result = 'Entity code';
                    break;
                case '电子码':
                    result = 'Electricity code';
                    break;
                default:
                    result = 'Entity code';
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
        if (window.location.pathname.split('/')[1].toLocaleLowerCase() === 'en') {
            switch (data) {
                case '一般贸易':
                    result = 'General trade';
                    break;
                case '电商贸易':
                    result = 'E-commerce trade';
                    break;
                default:
                    result = 'General trade';
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
        if (window.location.pathname.split('/')[1].toLocaleLowerCase() === 'en') {
            switch (data) {
                case '是':
                    result = 'Yes';
                    break;
                case '否':
                    result = 'No';
                    break;
                default:
                    result = 'Yes';
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
            if (window.location.pathname.split('/')[1].toLocaleLowerCase() === 'en') {
                switch (data) {
                    case '已接受申请':
                        result = 'Application accepted';
                        break;
                    case '新申请':
                        result = 'New application';
                        break;
                    case '编辑结束':
                        result = 'Edit end';
                        break;
                    case '申请结束':
                        result = 'End application';
                        break;
                    case '申请驳回':
                        result = 'Application dismissed';
                        break;
                    default:
                        result = 'Application accepted';
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
        if (window.location.pathname.split('/')[1].toLocaleLowerCase() === 'en') {
            switch (data) {
                case '盒装':
                    result = 'Box';
                    break;
                case '瓶装':
                    result = 'Bottled';
                    break;
                case '罐装':
                    result = 'Canned';
                    break;
                case '袋装':
                    result = 'Bag';
                    break;
                default:
                    result = 'Box';
                    break;
            }
            return result;
        } else {
            return data;
        }
    }
});
