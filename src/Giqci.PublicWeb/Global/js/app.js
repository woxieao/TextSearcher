"use strict";

/* jquery */
$(function () {

});

var app = angular.module("giqci", ['commonAlert']);

/* account login */
app.controller("LoginController", [
    '$http', '$scope', '$location', 'commAlertService', function ($http, $scope, $location, commAlertService) {
        $scope.submitButton = ' 登录 ';
        $scope.submitForm = function (isValid) {
            if (isValid) {
                $scope.enableDisableButton = true;
                $scope.submitButton = '正在提交...';
                $scope.formData = {
                    email: $scope.user.email,
                    password: $scope.user.password
                };
                $http.post('/api/account/login', $scope.formData).then(function (response) {
                    if (response.data.result) {
                        window.location.href = "/forms/app";
                    } else {
                        commAlertService.alertService().add('danger', response.data.message || "未知错误");
                        $scope.enableDisableButton = false;
                        $scope.submitButton = ' 登录 ';
                    }
                });
            }
        };
    }
]);


/* account forgot */
app.controller("ForgotController", [
    '$http', '$scope', '$location', 'commAlertService', function ($http, $scope, $location, commAlertService) {
        $scope.submitButton = ' 提交 ';
        $scope.submitForm = function (isValid) {
            if (isValid) {
                $scope.enableDisableButton = true;
                $scope.submitButton = '正在提交...';
                $scope.formData = {
                    email: $scope.user.email
                };
                $http.post('/api/account/forgotpassword', $scope.formData).then(function (response) {
                    if (response.data.result) {
                        commAlertService.alertService().add('success', "您的密码已经重置，并发往您的注册邮箱，请检查。");
                    } else {
                        commAlertService.alertService().add('danger', response.data.message || "未知错误");
                    }
                    $scope.enableDisableButton = false;
                    $scope.submitButton = ' 提交 ';
                });
            }
        };
    }
]);


/* account reg */
app.controller("RegController", [
    '$http', '$scope', '$location', "commAlertService", function ($http, $scope, $location, commAlertService) {
        //$scope.acceptshow = true; 暂时不显示
        $scope.regshow = true;

        $scope.submitButton = ' 提交 ';
        $scope.acceptshow = false;
        $scope.regshow = true;
        $scope.accept = function () {
            $scope.acceptshow = false;
            $scope.regshow = true;
        };
        $scope.submitForm = function (isValid, PasswordValid) {
            if (isValid && !PasswordValid) {
                $scope.enableDisableButton = true;
                $scope.submitButton = '正在提交...';
                $scope.formData = {
                    email: $scope.user.email,
                    password: $scope.user.password,
                    name: $scope.user.name,
                    address: $scope.user.address,
                    contact: $scope.user.contact,
                    phone: $scope.user.phone
                };
                $http.post('/api/account/reg', $scope.formData).then(function (response) {
                    if (!response.data.result) {
                        commAlertService.alertService().add('danger', response.data.message || "未知错误");
                    } else {
                        commAlertService.alertService().add('success', "注册成功，请检查邮件并认证。");
                        $scope.regshow = false;
                    }
                    $scope.enableDisableButton = false;
                    $scope.submitButton = ' 提交 ';
                });
            }
        };
    }
]);

/* account changepassword */
app.controller("ChanagePasswordController", [
    '$http', '$scope', '$location', 'commAlertService', function ($http, $scope, $location, commAlertService) {
        $scope.submitButton = ' 更改密码 ';
        $scope.submitForm = function (isValid, checkPassword) {
            if (isValid && checkPassword) {
                $scope.enableDisableButton = true;
                $scope.submitButton = '正在提交...';
                $scope.formData = {
                    oldpassword: $scope.user.oldpassword,
                    newpassword: $scope.user.password
                };
                $http.post('/api/account/chanagepassword', $scope.formData).then(function (response) {
                    if (response.data.result) {
                        commAlertService.alertService().add('success', "密码修改成功");
                    } else {
                        commAlertService.alertService().add('danger', response.data.message || "密码不匹配");
                    }
                    $scope.enableDisableButton = false;
                    $scope.submitButton = ' 更改密码 ';
                });
            }
        };
    }
]);

/* certificate search */
app.controller('CertificateSearchController', [
    '$scope', '$http', function ($scope, $http) {
        $scope.showtable = false;

        $scope.postData = {
            certNo: $scope.certNo
        }
        $scope.searchApplication = function () {
            $scope.postData = {
                certNo: $scope.certNo || "",
            };
            $scope.list($scope.postData);
        }
        $scope.list = function (postData) {
            $http.post('/api/certificate/search', $scope.postData).then(function (response) {
                $scope.persons = response.data.items;
                $scope.showtable = true;
            });
        };
    }
]);

/* forms list */
app.controller('FormsListController', [
    '$scope', '$http', function ($scope, $http) {
        $scope.paginationConf = {
            totalItems: 0
        };
        $scope.postData = {
            applyNo: $scope.applyNo,
            status: $scope.status,
            start: $scope.start,
            end: $scope.end,
            pageIndex: 1,
            pageSize: 10
        }

        $http({
            url: '/api/forms/getstatus',
            method: 'GET'
        }).success(function (data, header, config, status) {
            $scope.statusValues = data.items;
        }).error(function (data, header, config, status) {
            $log.warn(response);
        });

        $scope.prevPage = function () {
            if ($scope.postData.pageIndex > 1) {
                $scope.postData.pageIndex -= 1;
                $scope.list($scope.postData);
            }
        };
        $scope.nextPage = function () {
            if ($scope.paginationConf.totalItems == $scope.postData.pageSize) {
                $scope.postData.pageIndex += 1;
                $scope.list($scope.postData);
            }
        };
        $scope.indexPage = function () {
            if ($scope.postData.pageIndex > 1) {
                $scope.postData.pageIndex = 1;
                $scope.list($scope.postData);
            }
        }
        $scope.searchApplication = function () {
            $scope.postData = {
                applyNo: $scope.applyNo || "",
                status: $scope.status || "",
                start: $scope.start || "",
                end: $scope.end || "",
                pageIndex: 1,
                pageSize: $scope.postData.pageSize
            };
            $scope.list($scope.postData);
        }
        $scope.list = function (postData) {
            $http.post('/api/forms/list', postData).then(function (response) {
                $scope.paginationConf.totalItems = response.data.count;
                $scope.persons = response.data.items;
            });
        };
        $scope.list($scope.postData);
    }
]);


/* goods lists */
app.controller('GoodsListController', [
    '$scope', '$http', function ($scope, $http) {
        $scope.paginationConf = {
            totalItems: 0
        };
        $scope.postData = {
            pageIndex: 1,
            pageSize: 10
        }
        $scope.prevPage = function () {
            if ($scope.postData.pageIndex > 1) {
                $scope.postData.pageIndex -= 1;
                $scope.list($scope.postData);
            }
        };
        $scope.nextPage = function () {
            if ($scope.paginationConf.totalItems == $scope.postData.pageSize) {
                $scope.postData.pageIndex += 1;
                $scope.list($scope.postData);
            }
        };
        $scope.indexPage = function () {
            if ($scope.postData.pageIndex > 1) {
                $scope.postData.pageIndex = 1;
                $scope.list($scope.postData);
            }
        }
        $scope.searchGoods = function () {
            $scope.postData = {
                keywords: $scope.keywords || "",
                pageIndex: 1,
                pageSize: $scope.postData.pageSize
            };
            $scope.list($scope.postData);
        }

        $scope.list = function (postData) {
            $http.post('/api/goods/getproductlist', postData).success(function (response) {
                $scope.paginationConf.totalItems = response.result.count;
                $scope.merchantProductList = response.result;
            });
        };
        $scope.list($scope.postData);

        $scope.remove = function(ciqCode,index) {
            if (confirm("您确定要删除该商品吗?")) {
                $scope.merchantProductList.splice(index, 1);
                $http.post('/api/goods/delete', { ciqCode: ciqCode }).then(function (response) {
                    if (response.data.result) {

                    }
                });
            }
        }
    }
]);


/**
 * Goods add
 */
app.controller("GoodsAddController", ['$http', '$scope', '$log', '$location', '$anchorScroll', '$timeout', 'commAlertService', function ($http, $scope, $log, $location, $anchorScroll, $timeout, commAlertService) {
    $scope.CiqCode = "";
    $scope.Product = null;
    $scope.getproductlist = function () {
        if ($scope.CiqCode != "") {
            $http({
                url: '/api/goods/searchproduct',
                method: 'POST',
                data: {
                    ciqCode: $scope.CiqCode
                }
            }).success(function (data) {
                $scope.Product = data.result;
            }).error(function (data, header, config, status) {
            });
        }
    }
    $scope.addProductMsg = null;
    $scope.addproduct = function () {
        if (confirm('是否添加该商品为常用商品')) {
            if ($scope.Product != null) {
                $http({
                    url: '/api/goods/addproduct',
                    method: 'POST',
                    data: {
                        ciqCode: $scope.Product.CiqCode
                    }
                }).success(function (data) {
                    //$scope.addProductMsg = data.msg;
                    var _tipType = data.result ? "success" : "danger";
                    commAlertService.alertService().add(_tipType, data.msg || "未知错误");
                }).error(function (data, header, config, status) {
                });
            }
        }
    }
}
]);