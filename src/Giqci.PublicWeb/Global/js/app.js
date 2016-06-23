﻿"use strict";

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
                $giqci.post('/api/account/login', $scope.formData).then(function (response) {
                    if (response.data.result) {
                        window.location.href = "/forms/app";
                    } else {
                        alertService.add('danger', response.data.message || "未知错误");
                        $scope.enableDisableButton = false;
                        $scope.submitButton = ' 登录 ';
                    }
                }, $scope);
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
                $giqci.post('/api/account/forgotpassword', $scope.formData).then(function (response) {
                    if (response.data.result) {
                        alertService.add('success', "您的密码已经重置，并发往您的注册邮箱，请检查。");
                    } else {
                        alertService.add('danger', response.data.message || "未知错误");
                    }
                    $scope.enableDisableButton = false;
                    $scope.submitButton = ' 提交 ';
                }, $scope);
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
                $giqci.post('/api/account/reg', $scope.formData).then(function (response) {
                    if (!response.data.result) {
                        alertService.add('danger', response.data.message || "未知错误");
                    } else {
                        alertService.add('success', "注册成功，请检查邮件并认证。");
                        $scope.regshow = false;
                    }
                    $scope.enableDisableButton = false;
                    $scope.submitButton = ' 提交 ';
                }, $scope);
            }
        };
    }
]);

/* account changepassword */
app.controller("ChanagePasswordController", [
    '$http', '$scope', '$location', 'alertService', function ($http, $scope, $location, alertService) {
        $scope.submitButton = ' 更改密码 ';
        $scope.submitForm = function (isValid, checkPassword) {
            if (isValid && checkPassword) {
                $scope.enableDisableButton = true;
                $scope.submitButton = '正在提交...';
                $scope.formData = {
                    oldpassword: $scope.user.oldpassword,
                    newpassword: $scope.user.password
                };
                $giqci.post('/api/account/chanagepassword', $scope.formData).then(function (response) {
                    if (response.data.result) {
                        alertService.add('success', "密码修改成功");
                    } else {
                        alertService.add('danger', response.data.message || "密码不匹配");
                    }
                    $scope.enableDisableButton = false;
                    $scope.submitButton = ' 更改密码 ';
                }, $scope);
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
            $giqci.post('/api/certificate/search', $scope.postData).then(function (response) {
                $scope.persons = response.data.items;
                $scope.showtable = true;
            }, $scope);
        };
    }
]);

/* forms list */
app.controller('FormsListController', [
    '$scope', "$log", '$http', function ($scope, $log, $http) {
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

        $giqci.get('/api/forms/getstatus').success(function (data, header, config, status) {
            $scope.statusValues = data.items;
        }, $scope);

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
            $giqci.post('/api/forms/list', postData).then(function (response) {
                $scope.paginationConf.totalItems = response.data.count;
                $scope.persons = response.data.items;
            }, $scope);
        };
        $scope.list($scope.postData);
    }
]);


/* goods lists */
app.controller('GoodsListController', [
    '$scope', '$http', 'alertService', function ($scope, $http, alertService) {
        $scope.list = function (postData) {
            $giqci.post('/api/goods/getproductlist', postData).success(function (response) {
                $scope.merchantProductList = response.result;
            }, $scope);
        };
        $scope.list($scope.postData);
        $scope.remove = function (ciqCode, index) {
            layer.confirm("您确定要删除该商品吗?", function (l) {
                $scope.merchantProductList.splice(index, 1);
                $giqci.post('/api/goods/delete', { ciqCode: ciqCode }).then(function (response) {
                    if (response.data.result) {
                    }
                }, $scope);
                layer.close(l);
            });
        }
        $scope.list2 = function (postData) {
            $giqci.post('/api/goods/getcustomproductlist', postData).success(function (response) {
                $scope.customProductList = response.result;
            }, $scope);
        };
        $scope.postData = {
            Index: 1,
            pageSize: 10,
            keywords: $scope.keyword
        };
        $scope.search = function () {
            $scope.postData = {
                keywords: $scope.keyword
            };
            $scope.list($scope.postData);
            $scope.list2($scope.postData);
        }
        $scope.list2($scope.postData);
        $scope.remove2 = function (id, index) {
            layer.confirm("您确定要删除该商品吗?", function (l) {
                $giqci.post('/api/goods/deletecustomproduct', { id: id }).then(function (response) {
                    $scope.customProductList.splice(index, 1);
                }, $scope);
                layer.close(l);
            });
        }
        $scope.editMerchantProduct = function (index) {
            $scope.CustomDialogModel = CallByValue($scope.customProductList[index]);
            $(".form-add-custom-product-title").html("编辑非备案商品");
            $("#CustomDialogModelHsCode").next().find("span.select2-selection__rendered").html("");
            $("#form-add-custom-product").modal("show");
            $("#form-product").modal("hide");
        };
        var _modalEditMerchantProduct = document.getElementById("form-add-custom-product");
        angular.element(_modalEditMerchantProduct).on('hide.bs.modal', function () {
            $scope.CustomDialogModel = null;
        });

        $scope.submitAddCustomProduct = function () {
            $giqci.post('/api/goods/addcustomproduct', $scope.CustomDialogModel).success(function (response) {
                if (response.flag) {
                    $("#form-add-custom-product").modal("hide");
                    layer.alert("提交成功", function (index) {
                        window.location.reload();
                        layer.close(index);
                    });
                }
                var _tipType = response.flag ? "success" : "danger";
                alertService.add(_tipType, response.msg || "未知错误", 3000);
            }, $scope);
        };
        $scope.loadCountries = function () {
            $giqci.get("/api/dict/countries",
            { 'code': "" }).success(function (data) {
                $scope.loadCountriesDict = data.items;
            }, $scope);
        };
        $scope.loadCountries();
        $scope.showProductList = function () {
            $(".form-add-custom-product-title").html("商品列表");
            $("#form-product").modal("show");
            $("#form-add-custom-product").modal("hide");
            $("#form-add-goods-product").modal("hide");
        };
        $scope.showproduct = function (ciqCode) {
            $scope.ProductDialogModel = null;
            $giqci.post('/api/goods/searchproduct', { ciqCode: ciqCode }).success(function (response) {
                $scope.ProductDialogModel = response.result;
            }, $scope);
            $("#form-product-view").modal("show");
        }
        //change product
        $scope.CiqCode = '';
        $scope.changeProductList = false;
        $scope.isSearch = false;
        $scope.getproductlist = function () {
            $scope.isSearch = true;
            var reg = /^\d{10}$/;
            var reg2 = /^ICIP\d{14}$/i;
            if (!(reg.test($scope.CiqCode) || reg2.test($scope.CiqCode))) {
                $scope.Product = null;
                alertService.add('danger', "正确的备案号格式为[ICIP+14个数字]或[10个数字]", 3000);
                return;
            } else {
                $giqci.post('/api/goods/searchproduct', { ciqCode: $scope.CiqCode })
                .success(function (data) {
                    if (data.result == null) {
                        $scope.Product = null;
                        alertService.add('danger', "该备案号不存在", 3000);
                    } else {
                        $scope.Product = data.result;
                        $scope.changeProductList = true;
                    }
                }, $scope);
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
            if (false === $scope.isSearch || _customProductId == null || _ciqCode == null) {
                alertService.add('danger', "请搜索并获取备案商品信息", 3000);
                return;
            };
            $giqci.post('/api/goods/convertproduct/' + _customProductId + '/' + _ciqCode,
                { enName: $scope.ChangeProductDialogModel.DescriptionEn }
            ).success(function (response) {
                $scope.isSearch = false;
                if (response.flag) {
                    alertService.add('success', "数据已经转换成功", 3000);
                    $scope.list($scope.postData);
                    $scope.list2($scope.postData);
                    angular.element(_modal).modal("hide");
                } else {
                    alertService.add('danger', response.msg, 3000);
                }
            }, $scope);
        };
        angular.element(_modal).on('hide.bs.modal', function () {
            $scope.isSearch = false;
        });
    }
]);

/**
 * Goods add
 */
app.controller("GoodsAddController", [
    '$http', '$scope', '$log', '$location', '$anchorScroll', '$timeout', 'alertService', function ($http, $scope, $log, $location, $anchorScroll, $timeout, alertService) {
        $scope.CiqCode = "";
        $scope.Product = null;
        $scope.errorMsg = "";
        var reg = /^\d{10}$/;
        var reg2 = /^ICIP\d{14}$/i;
        $scope.showError = false;

        //每次按键抬起都会校验输入的商品编号合法性，只是不会自动发请求，除非点击回车或者搜索按钮
        $scope.validCiqCode = function () {
            if (!(reg.test($scope.CiqCode) || reg2.test($scope.CiqCode))) {
                $scope.showError = true;
                $scope.errorMsg = "正确的备案号格式为[ICIP+14个数字]或[10个数字]";
            } else {
                $scope.showError = false;
            }
        };

        $scope.getproductlist = function () {
            if (!(reg.test($scope.CiqCode) || reg2.test($scope.CiqCode))) {
                $scope.Product = null;
                alertService.add('danger', "正确的备案号格式为[ICIP+14个数字]或[10个数字]", 3000);
                $scope.showError = true;
                $scope.errorMsg = "正确的备案号格式为[ICIP+14个数字]或[10个数字]";
                return;
            }
            else {
                $giqci.post('/api/goods/searchproduct', {
                    ciqCode: $scope.CiqCode
                }).success(function (data) {
                    if (data.result == null) {
                        $scope.Product = null;
                        $scope.showError = true;
                        alertService.add('danger', "该备案号不存在", 3000);
                        $scope.errorMsg = "该备案号不存在";
                    } else {
                        $scope.Product = data.result;
                    }
                }, $scope);
            }
        }
        $scope.addProductMsg = null;
        $scope.addproduct = function () {
            layer.confirm('是否添加该商品为常用商品', function (l) {
                if ($scope.Product != null) {
                    $giqci.post('/api/goods/addproduct',
                         {
                             ciqCode: $scope.Product.CiqCode
                         }).success(function (data) {
                             //$scope.addProductMsg = data.msg;
                             var _tipType = data.result ? "success" : "danger";
                             alertService.add(_tipType, data.msg || "未知错误", 3000);
                         }, $scope);
                }
                layer.close(l);
            });
        }
    }
]);

/**
 * MerchantList 常用商户列表
 */
app.controller("MerchantListController", ['$http', '$scope', '$log', '$location', '$anchorScroll', '$timeout', 'alertService', function ($http, $scope, $log, $location, $anchorScroll, $timeout, alertService) {
    $scope.loadMerchantList = null;
    $scope.dialogModelMerchant = {
        UserName: "",
        UserAddress: "",
        UserContact: "",
        UserPhone: ""
    };
    $scope.loadMerchant = function () {
        $giqci.post("/api/UserProfile/GetProfileList", {
        }).success(function (data) {
            $scope.loadMerchantList = data.result;
        }, $scope);
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
        $giqci.post(_url, { userProfile: $scope.dialogModelMerchant }).success(function (data) {
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
        }, $scope);
    };
    $scope.edit = function (_id) {
        $giqci.post(
          '/api/UserProfile/GetProfileDeatil', { ProfileId: _id }).success(function (data) {
              $scope.dialogModelMerchant = data.result;
          }, $scope);
        $("#merchant-add").modal("show");
    };
    $scope.remove = function (_object) {
        layer.confirm("是否删除该常用商户", function (l) {
            $giqci.post(
              '/api/UserProfile/RemoveProfile', { ProfileId: _object.Id }).success(function (data) {
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
              }, $scope);
            layer.close(l);
        });
    };
}
]);

app.controller("BreathController", [
    '$http', '$scope', '$log', '$location', '$anchorScroll', '$timeout', 'alertService', function ($http, $scope, $log, $location, $anchorScroll, $timeout, alertService) {
        $scope.login = { result: true, error: "" };
        $scope.user = { email: "", password: "" }
        $scope.Logining = false;
        $scope.ModalLogin = function () {
            $scope.Logining = true;
            $giqci.post('/api/account/login', $scope.user).then(function (response) {
                $scope.Logining = false;
                if (response.data.result) {
                    $('#breathBox').modal('hide');
                    $giqci.RunLastPost();
                } else {
                    $scope.login.result = response.data.result;
                    $scope.login.error = response.data.message;
                }
            }, $scope);
        };
    }
]);

app.controller("ZcodeApplyController", [
    '$http', '$scope', function ($http, $scope) {
        $scope.ZcodeType = 0;
        $scope.Add = function () {
            if ($scope.Count * 1 > 0) {
                layer.confirm("确定申请" + $scope.Count + "个真知码?", function (l) {
                    $giqci.post('/api/forms/addzcodeapply', { ZcodeType: $scope.ZcodeType, Count: $scope.Count }).success(function (data) {
                        if (data.flag) {
                            layer.alert("申请成功", function (index) {
                                window.location.href = "/forms/zcodeapplylist";
                                layer.close(index);
                            });
                        }
                    }, $scope);
                    layer.close(l);
                });
            } else {
                layer.alert("真知码数量必须大于0");
            }
        }
    }
]);
app.controller("ZcodeApplyListController", [
    '$http', '$scope', function ($http, $scope) {
        $scope.ZcodeApplyList = {};
        $scope.Paging = new PageHandler("/api/forms/getzcodeapplylogs", function (result) {
            $scope.ZcodeApplyList = result.data;
            $scope.$apply();
        });
        $scope.Paging.FirstPage();
    }
]);