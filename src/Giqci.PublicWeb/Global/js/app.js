"use strict";

/* jquery */
$(function () {

});

var app = angular.module("giqci", ['giqciService']);

/* account login */
app.controller("LoginController", [
    '$http', '$scope', '$location', 'alertService', function ($http, $scope, $location, alertService) {
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
                        alertService.add('danger', response.data.message || "未知错误");
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
    '$http', '$scope', '$location', 'alertService', function ($http, $scope, $location, alertService) {
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
                        alertService.add('success', "您的密码已经重置，并发往您的注册邮箱，请检查。");
                    } else {
                        alertService.add('danger', response.data.message || "未知错误");
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
    '$http', '$scope', '$location', "alertService", function ($http, $scope, $location, alertService) {
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
                        alertService.add('danger', response.data.message || "未知错误");
                    } else {
                        alertService.add('success', "注册成功，请检查邮件并认证。");
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
    '$http', '$scope', '$location', 'alertService', function ($http, $scope, $location, alertService) {
        userBreath.Revive();
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
                        alertService.add('success', "密码修改成功");
                    } else {
                        alertService.add('danger', response.data.message || "密码不匹配");
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
        userBreath.Revive();
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
    '$scope', "$log", '$http', function ($scope, $log, $http) {
        //userBreath.Revive();
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
        };
        $(function () {
            $('.datetimepicker').datetimepicker({
                language: 'zh-CN',
                format: 'yyyy/mm/dd',
                autoclose: true,
                minView: "month",
                todayBtn: true,
            }).on("hide",
                function () {
                    var $this = $(this);
                    var _this = this;
                    $scope.$apply(function () {
                        $scope[$this.attr('ng-model')] = _this.value;
                    });
                });
        });

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
    '$scope', '$http', 'alertService', function ($scope, $http, alertService) {
        userBreath.Revive();
        $scope.list = function (postData) {
            $http.post('/api/goods/getproductlist', postData).success(function (response) {
                $scope.merchantProductList = response.result;
            });
        };
        $scope.list($scope.postData);
        $scope.remove = function (ciqCode, index) {
            if (confirm("您确定要删除该商品吗?")) {
                $scope.merchantProductList.splice(index, 1);
                $http.post('/api/goods/delete', { ciqCode: ciqCode }).then(function (response) {
                    if (response.data.result) {

                    }
                });
            }
        }
        $scope.list2 = function (postData) {
            $http.post('/api/goods/getcustomproductlist', postData).success(function (response) {
                $scope.customProductList = response.result;
            });
        };
        $scope.list2($scope.postData);
        $scope.remove2 = function (id, index) {
            if (confirm("您确定要删除该商品吗?")) {
                $http.post('/api/goods/deletecustomproduct', { id: id }).then(function (response) {
                    $scope.customProductList.splice(index, 1);
                });
            }
        }
        $scope.editMerchantProduct = function (index) {
            $scope.CustomDialogModel = CallByValue($scope.customProductList[index]);
            $(".form-add-custom-product-title").html("编辑非备案商品");
            $("#CustomDialogModelHsCode").next().find("span.select2-selection__rendered").html("");
            $("#form-add-custom-product").modal("show");
            $("#form-product").modal("hide");
        };

        $scope.submitAddCustomProduct = function () {
            $http.post('/api/goods/addcustomproduct', $scope.CustomDialogModel).success(function (response) {
                if (response.flag) {
                    alert("提交成功");
                    $("#form-add-custom-product").modal("hide");
                    window.location.reload();
                }
                var _tipType = response.flag ? "success" : "danger";
                alertService.add(_tipType, response.msg || "未知错误", 3000);
            });
        };
        $scope.loadCountries = function () {
            $http.get("/api/dict/countries", {
                params: { 'code': "" }
            }).success(function (data) {
                $scope.loadCountriesDict = data.items;
            }).error(function (data) {
            });
        };
        $scope.loadCountries();
        $scope.showProductList = function () {
            $(".form-add-custom-product-title").html("商品列表");
            $("#form-product").modal("show");
            $("#form-add-custom-product").modal("hide");
            $("#form-add-goods-product").modal("hide");
        };
        //change product
        $scope.CiqCode = '';
        $scope.changeProductList = false;
        $scope.getproductlist = function () {
            var reg = /^\d{10}$/;
            var reg2 = /^ICIP\d{14}$/i;
            if (!(reg.test($scope.CiqCode) || reg2.test($scope.CiqCode))) {
                $scope.Product = null;
                alertService.add('danger', "正确的备案号格式为[ICIP+14个数字]或[10个数字]", 3000);
                return;
            } else {
                $http({
                    url: '/api/goods/searchproduct',
                    method: 'POST',
                    data: {
                        ciqCode: $scope.CiqCode
                    }
                })
                .success(function (data) {
                    if (data.result == null) {
                        $scope.Product = null;
                        alertService.add('danger', "该备案号不存在", 3000);
                    } else {
                        $scope.Product = data.result;
                        $scope.changeProductList = true;
                    }
                })
                .error(function (data, header, config, status) {
                });
            }
        };

        var _modal = document.getElementById("form-change-product");
        $scope.changeProduct = function (index) {
            $scope.changeProductList = false;
            $scope.CiqCode = '';
            angular.element(_modal).modal("show");
            $scope.ChangeProductDialogModel = CallByValue($scope.customProductList[index]);
        };
        $scope.submitChangeProduct = function (_customProductId, _ciqCode) {
            if (_customProductId == null || _ciqCode == null) {
                alertService.add('danger', "参数错误", 3000);
            }
            $http({
                url: '/api/goods/convertproduct/' + _customProductId + '/' + _ciqCode,
                method: 'POST',
                data: {
                    enName: $scope.ChangeProductDialogModel.DescriptionEn
                }
            })
            .success(function (response) {
                console.log(response);
                if (response.flag) {
                    alertService.add('success', "数据已经转换成功");
                } else {
                    alertService.add('danger', response.msg);
                }
            })
            .error(function (data, header, config, status) {
            });
        };
    }
]);



/**
 * Goods add
 */
app.controller("GoodsAddController", [
    '$http', '$scope', '$log', '$location', '$anchorScroll', '$timeout', 'alertService', function ($http, $scope, $log, $location, $anchorScroll, $timeout, alertService) {
        userBreath.Revive();
        $scope.CiqCode = "";
        $scope.Product = null;
        $scope.getproductlist = function () {
            var reg = /^\d{10}$/;
            var reg2 = /^ICIP\d{14}$/i;
            if (!(reg.test($scope.CiqCode) || reg2.test($scope.CiqCode))) {
                $scope.Product = null;
                alertService.add('danger', "正确的备案号格式为[ICIP+14个数字]或[10个数字]", 3000);
                return;
            }
            else {
                $http({
                    url: '/api/goods/searchproduct',
                    method: 'POST',
                    data: {
                        ciqCode: $scope.CiqCode
                    }
                }).success(function (data) {
                    if (data.result == null) {
                        $scope.Product = null;
                        alertService.add('danger', "该备案号不存在", 3000);
                    } else {
                        $scope.Product = data.result;
                    }
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
                        alertService.add(_tipType, data.msg || "未知错误", 3000);
                    }).error(function (data, header, config, status) {
                    });
                }
            }
        }
    }
]);



/**
 * MerchantList 常用商户列表
 */
app.controller("MerchantListController", ['$http', '$scope', '$log', '$location', '$anchorScroll', '$timeout', 'alertService', function ($http, $scope, $log, $location, $anchorScroll, $timeout, alertService) {
    userBreath.Revive();
    $scope.loadMerchantList = null;
    $scope.dialogModelMerchant = {
        UserName: "",
        UserAddress: "",
        UserContact: "",
        UserPhone: ""
    };
    $scope.loadMerchant = function () {
        $http.post("/api/UserProfile/GetProfileList", {
        }).success(function (data) {
            $scope.loadMerchantList = data.result;
        }).error(function (data) {
        });
    };
    $scope.loadMerchant();

    $scope.add = function () {
        $scope.dialogModelMerchant = {
            UserName: "",
            UserAddress: "",
            UserContact: "",
            UserPhone: ""
        };
        $("#merchant-add").modal("show");
    };

    $scope.submitAddMerchant = function () {
        var _url = "";
        if ($scope.dialogModelMerchant.Id == null) {
            _url = '/api/UserProfile/AddProfile';
        } else {
            _url = '/api/UserProfile/UpdateProfile';
        }
        $http({
            url: _url,
            method: 'POST',
            data: {
                userProfile: $scope.dialogModelMerchant
            }
        }).success(function (data) {
            if (data.flag) {
                $scope.loadMerchant();
                $("#merchant-add").modal("hide");
            } else {
                var _errormsg = '';
                for (var i = data.errorMsg.length; i > 0 ; i--) {
                    _errormsg += data.errorMsg[i - 1] + "\r\n";
                }
                alertService.add("danger", _errormsg || "未知错误", 3000);
            }
        }).error(function (response) {
            alertService.add("danger", response.msg || "未知错误", 3000);
        });
    };
    $scope.edit = function (_id) {
        $http({
            url: '/api/UserProfile/GetProfileDeatil',
            method: 'POST',
            data: {
                ProfileId: _id
            }
        }).success(function (data) {
            $scope.dialogModelMerchant = data.result;
        }).error(function (response) {
        });
        $("#merchant-add").modal("show");
    };
    $scope.remove = function (_object) {
        if (confirm("是否删除该常用商户")) {
            $http({
                url: '/api/UserProfile/RemoveProfile',
                method: 'POST',
                data: {
                    ProfileId: _object.Id
                }
            }).success(function (data) {
                if (data.flag) {
                    $scope.loadMerchant();
                    $("#merchant-add").modal("hide");
                } else {
                    var _errormsg = '';
                    for (var i = data.errorMsg.length; i > 0 ; i--) {
                        _errormsg += data.errorMsg[i - 1] + "\r\n";
                    }
                    alertService.add("danger", _errormsg || "未知错误", 3000);
                }
            }).error(function (response) {
                alertService.add("danger", response.msg || "未知错误", 3000);
            });
        }
    };
}
]);

app.controller("BreathController", [
    '$http', '$scope', '$log', '$location', '$anchorScroll', '$timeout', 'alertService', function ($http, $scope, $log, $location, $anchorScroll, $timeout, alertService) {
        $scope.login = { result: true, error: "" };
        $scope.user = { email: "", password: "" }
        $scope.Logining = false;

        function beforeLogin() {
            $scope.Logining = true;
            userBreath.Die();
        }

        function afterLogin() {
            $scope.Logining = false;
            userBreath.Revive();
        }

        $scope.ModalLogin = function () {
            beforeLogin();
            $http.post('/api/account/login', $scope.user).then(function (response) {
                if (response.data.result) {
                    $('#breathBox').modal('hide');
                } else {
                    $scope.login.result = response.data.result;
                    $scope.login.error = response.data.message;
                }
                afterLogin();
            });
        };
    }
]);