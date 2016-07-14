"use strict";

/* jquery */
$(function () {

});

var app = angular.module("giqci", ['giqciService', 'ngTipValidate']);

/* account login */
app.controller("LoginController", [
    '$http', '$scope', '$location', 'alertService', function ($http, $scope, $location, alertService) {
        $scope.submitButton = $giqci.KeyToWord('login');
   
        $scope.submitForm = function (isValid) {
            if (isValid) {
                $scope.enableDisableButton = true;
                $scope.submitButton = $giqci.KeyToWord("on_submitting"); 
                $scope.formData = {
                    email: $scope.user.email,
                    password: $scope.user.password
                };
                $giqci.post('/api/account/login', $scope.formData).then(function (response) {
                    if (response.data.result) {
                        window.location.href = $giqci.getLanUrl("/forms/app");
                    } else {
                        alertService.add('danger', response.data.message || $giqci.KeyToWord("unknown_error"));
                        $scope.enableDisableButton = false;
                        $scope.submitButton =  $giqci.KeyToWord("login");
                    }
                }, $scope);
            }
        };
    }
]);


/* account forgot */
app.controller("ForgotController", [
    '$http', '$scope', '$location', 'alertService', function ($http, $scope, $location, alertService) {
        $scope.submitButton =  $giqci.KeyToWord("submit");
        $scope.submitForm = function (isValid) {
            if (isValid) {
                $scope.enableDisableButton = true;
                $scope.submitButton = $giqci.KeyToWord("on_submitting");
                $scope.formData = {
                    email: $scope.user.email
                };
                $giqci.post('/api/account/forgotpassword', $scope.formData).then(function (response) {
                    if (response.data.result) {
                        alertService.add('success', $giqci.KeyToWord("reset_pwd_msg") );
                    } else {
                        alertService.add('danger', response.data.message || $giqci.KeyToWord("unknown_error"));
                    }
                    $scope.enableDisableButton = false;
                    $scope.submitButton =$giqci.KeyToWord("submit") ;
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

        $scope.submitButton = $giqci.KeyToWord("submit");
        $scope.acceptshow = false;
        $scope.regshow = true;
        $scope.accept = function () {
            $scope.acceptshow = false;
            $scope.regshow = true;
        };

        $scope.validateFun = function (str1, str2) {
            if (str1 == undefined || str1 == "") {
                return  $giqci.KeyToWord("pwd_did_not_write_msg") ;
            } else if (str1 != str2) {
                return $giqci.KeyToWord("confirm_pwd_does_not_consistent");
            } else {
                //验证成功返回0 或者 null 或者 false
                return 0;
            }
        };

        $scope.submitForm = function (isValid, PasswordValid) {
            if (isValid && !PasswordValid) {
                $scope.enableDisableButton = true;
                $scope.submitButton = $giqci.KeyToWord("on_submitting");
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
                        alertService.add('danger', response.data.message || $giqci.KeyToWord("unknown_error"));
                    } else {
                        alertService.add('success', $giqci.KeyToWord("register_success_msg"));
                        $scope.regshow = false;
                    }
                    $scope.enableDisableButton = false;
                    $scope.submitButton = $giqci.KeyToWord("submit");
                }, $scope);
            }
        };
    }
]);

/* account changepassword */
app.controller("ChanagePasswordController", [
    '$http', '$scope', '$location', 'alertService', function ($http, $scope, $location, alertService) {
        $scope.submitButton = $giqci.KeyToWord("change_password");
        $scope.submitForm = function (isValid, checkPassword) {
            if (isValid && checkPassword) {
                $scope.enableDisableButton = true;
                $scope.submitButton =$giqci.KeyToWord("on_submitting") ;
                $scope.formData = {
                    oldpassword: $scope.user.oldpassword,
                    newpassword: $scope.user.password
                };
                $giqci.post('/api/account/chanagepassword', $scope.formData).then(function (response) {
                    if (response.data.result) {
                        layer.msg($giqci.KeyToWord("password_modification_success_msg"), { icon: 6 });
                    } else {
                        layer.alert(response.data.message || $giqci.KeyToWord("passwords_do_not_match"), function (index) {
                            layer.close(index);
                        });
                    }
                    $scope.enableDisableButton = false;
                    $scope.submitButton =$giqci.KeyToWord("change_password") ;
                }, $scope);
            }
        };
        $scope.validateFun = function (str1, str2) {
            if (str1 != str2) {
                return $giqci.KeyToWord("confirm_pwd_does_not_consistent");
            } else {
                //验证成功返回0 或者 null 或者 false
                return 0;
            }
        }
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
        $scope.alreadySubmit = false;
        $scope.list = function (postData) {
            $giqci.post('/api/goods/getproductlist', postData).success(function (response) {
                $scope.merchantProductList = response.result;
            }, $scope);
        };
        $scope.list($scope.postData);
        $scope.remove = function (key, index) {
            layer.confirm($giqci.KeyToWord("delete_product_msg"), function (l) {
                $scope.merchantProductList.splice(index, 1);
                $giqci.post('/api/goods/delete', { key: key }).then(function (response) {
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
        $scope.remove2 = function (key, index) {
            layer.confirm($giqci.KeyToWord("delete_product_msg"), function (l) {
                $giqci.post('/api/goods/deletecustomproduct', { key: key }).then(function (response) {
                    $scope.customProductList.splice(index, 1);
                }, $scope);
                layer.close(l);
            });
        }
        $scope.editMerchantProduct = function (index) {
            $scope.CustomDialogModel = CallByValue($scope.customProductList[index]);
            $(".form-add-custom-product-title").html($giqci.KeyToWord("edit_custom_product"));
            $("#CustomDialogModelHsCode").next().find("span.select2-selection__rendered").html("");
            $("#form-add-custom-product").modal("show");
            $("#form-product").modal("hide");
        };
        $scope.lookMerchantProduct = function (index) {
            $scope.CustomDialogModel = CallByValue($scope.customProductList[index]);
            $(".form-look-custom-product-title").html($giqci.KeyToWord("view_custom_product"));
            $("#CustomDialogModelHsCode").next().find("span.select2-selection__rendered").html("");
            $("#form-look-custom-product").modal("show");
            $("#form-product").modal("hide");
        };

        $("#form-add-custom-product").on('hide.bs.modal', function () {
            layer.closeAll();
        });

        $("#form-change-product").on('hide.bs.modal', function () {
            layer.closeAll();
        });

        var _modalEditMerchantProduct = document.getElementById("form-add-custom-product");
        angular.element(_modalEditMerchantProduct).on('hide.bs.modal', function () {
            $scope.CustomDialogModel = null;
        });

        $scope.submitAddCustomProduct = function (isValid) {
            if (isValid) {
                $scope.alreadySubmit = true;
                $giqci.post('/api/goods/addcustomproduct', $scope.CustomDialogModel).success(function (response) {
                    if (response.flag) {
                        $("#form-add-custom-product").modal("hide");
                        layer.msg($giqci.KeyToWord("submit_successfully"), { icon: 6 }, function () { $scope.list2($scope.postData); });
                        layer.close();
                    } else {
                        layer.alert(response.msg ||$giqci.KeyToWord("unknown_error"), function (index) {
                            layer.close(index);
                        });
                    }
                    $scope.alreadySubmit = false;
                }, $scope);
            } else {
                $scope.alreadySubmit = false;
            }
        };

        $scope.loadCountries = function () {
            $giqci.get("/api/dict/countries",
            { 'code': "" }).success(function (data) {
                $scope.loadCountriesDict = data.items;
            }, $scope);
        };
        $scope.loadCountries();
        $scope.showProductList = function () {
            $(".form-add-custom-product-title").html($giqci.KeyToWord("list_of_goods"));
            $("#form-product").modal("show");
            $("#form-add-custom-product").modal("hide");
            $("#form-look-custom-product").modal("hide");
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
        $scope.changeProductSubmitApp = false;
        $scope.getproductlist = function (isValid) {
            if (isValid) {
                $scope.changeProductSubmitApp = true;
                $giqci.post('/api/goods/searchproduct', { ciqCode: $scope.CiqCode })
                .success(function (data) {
                    if (data.result == null) {
                        $scope.Product = null;
                        layer.alert($giqci.KeyToWord("the_record_number_does_not_exist"), function (index) {
                            layer.close(index);
                        });
                    } else {
                        $scope.Product = data.result;
                        $scope.changeProductList = true;
                        $scope.isSearch = true;
                    }
                    $scope.changeProductSubmitApp = false;
                }, $scope);
            } else {
                $scope.changeProductSubmitApp = false;
            }
        };

        var _modal = document.getElementById("form-change-product");
        var _form_submitAddCustomProduct = document.getElementById("submitAddCustomProduct");
        $scope.changeProduct = function (index) {
            $scope.changeProductList = false;
            $scope.CiqCode = '';
            angular.element(_modal).modal("show");
            $scope.ChangeProductDialogModel = CallByValue($scope.customProductList[index]);
        };
        $scope.submitChangeProduct = function (_customProductId, _ciqCode) {
            angular.element(_form_submitAddCustomProduct).attr("disabled", true);
            if (false === $scope.isSearch || _customProductId == null || _ciqCode == null) {
                layer.alert($giqci.KeyToWord("search_record_commodity_msg"), function (index) {
                    layer.close(index);
                    angular.element(_form_submitAddCustomProduct).attr("disabled", false);
                });
                return;
            };
            $giqci.post('/api/goods/convertproduct/' + _customProductId + '/' + _ciqCode,
                { enName: $scope.ChangeProductDialogModel.DescriptionEn }
            ).success(function (response) {
                $scope.isSearch = false;
                if (response.flag) {
                    layer.msg($giqci.KeyToWord("convert_success_msg"), { icon: 6 }, function () {
                        $scope.list($scope.postData);
                        $scope.list2($scope.postData);
                        angular.element(_form_submitAddCustomProduct).attr("disabled", false);
                        angular.element(_modal).modal("hide");
                    });
                } else {
                    angular.element(_form_submitAddCustomProduct).attr("disabled", false);
                    layer.alert(response.msg, function (index) {
                        layer.close(index);
                    });
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
        $scope.alreadySubmit = false;
        $scope.getproductlist = function (isValid) {
            $scope.alreadySubmit = true;
            if (isValid) {
                $giqci.post('/api/goods/searchproduct', {
                    ciqCode: $scope.CiqCode
                }).success(function (data) {
                    if (data.result == null) {
                        $scope.Product = null;
                        layer.alert($giqci.KeyToWord("the_record_number_does_not_exist"), function (index) {
                            layer.close(index);
                        });
                        $("#inputCiqCode").focus();
                    } else {
                        $scope.Product = data.result;
                    }
                    $scope.alreadySubmit = false;
                }, $scope);
            } else {
                $scope.alreadySubmit = false;
            }
        }
        $scope.addProductMsg = null;
        $scope.addproduct = function () {
            layer.confirm($giqci.KeyToWord("add_product_msg"), function (l) {
                if ($scope.Product != null) {
                    $giqci.post('/api/goods/addproduct',
                         { key: $scope.Product.Key }).success(function (data) {
                             if (data.result) {
                                 layer.msg($giqci.KeyToWord("added_successfully"), { icon: 6 });
                             } else {
                                 layer.alert(data.msg || $giqci.KeyToWord("unknown_error"), function (index) {
                                     layer.close(index);
                                 });
                             }
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
    $scope.showError = false;
    $scope.errorMsg = "";
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
        $scope.alreadySubmit = false;
        $scope.dialogModelMerchant = {
            UserName: "",
            UserAddress: "",
            UserContact: "",
            UserPhone: ""
        };
        $("#merchant-add").modal("show");
    };

    $('#merchant-add').on('hidden.bs.modal', function (e) {
        layer.closeAll();
    })

    $scope.validAddMerchant = function (obj) {
        $scope.alreadySubmit = true;
        var reg = !(obj.UserName == "" || obj.UserName == undefined)
        && !(obj.UserAddress == "" || obj.UserAddress == undefined)
        && !(obj.UserContact == "" || obj.UserContact == undefined)
        && !(obj.UserPhone == "" || obj.UserPhone == undefined);
        var _url = "";
        if ($scope.dialogModelMerchant.Id == null) {
            _url = '/api/UserProfile/AddProfile';
        } else {
            _url = '/api/UserProfile/UpdateProfile';
        }
        if (reg) {
            $scope.addMerchant(_url);
        } else {
            $scope.alreadySubmit = false;
        }
    };

    $scope.addMerchant = function (url) {
        $scope.alreadySubmit = true;
        $giqci.post(url, { userProfile: $scope.dialogModelMerchant }).success(function (data) {
            if (data.flag) {
                layer.msg($giqci.KeyToWord("submit_successfully"), { icon: 6 }, function () {
                    $scope.alreadySubmit = false;
                    $scope.loadMerchant();
                    $("#merchant-add").modal("hide");
                });
            } else {
                $scope.alreadySubmit = false;
                var _errormsg = '';
                for (var i = data.errorMsg.length; i > 0 ; i--) {
                    _errormsg += data.errorMsg[i - 1] + "\r\n";
                }
                layer.alert(data.errorMsg[0] || $giqci.KeyToWord("unknown_error"), function (index) {
                    layer.close(index);
                });
            }
        }, $scope);
    };

    $scope.edit = function (_id) {
        $scope.alreadySubmit = false;
        $giqci.post(
          '/api/UserProfile/GetProfileDeatil', { ProfileId: _id }).success(function (data) {
              $scope.dialogModelMerchant = data.result;
          }, $scope);
        $("#merchant-add").modal("show");
    };

    $scope.closeMerchant = function () {
        $("#merchant-add").modal("hide");
    };

    $scope.remove = function (_object) {
        layer.confirm($giqci.KeyToWord("delete_common_merchant_msg"), function (l) {
            $giqci.post(
              '/api/UserProfile/RemoveProfile', { ProfileId: _object.Id }).success(function (data) {
                  if (data.flag) {
                      layer.msg($giqci.KeyToWord("delete_success"), { icon: 6, time: 1000 }, function () {
                          $scope.loadMerchant();
                          $("#merchant-add").modal("hide");
                      });
                  } else {
                      var _errormsg = '';
                      for (var i = data.errorMsg.length; i > 0 ; i--) {
                          _errormsg += data.errorMsg[i - 1] + "\r\n";
                      }
                      layer.alert(data.errorMsg[0] ||$giqci.KeyToWord("unknown_error") , function (index) {
                          layer.close(index);
                      });
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

app.controller("ZcodeApplyListController", [
    '$http', '$scope', function ($http, $scope) {
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
        $scope.ZcodeType = 0;
        $scope.alreadySubmit = false;
        $scope.Add = function (isValid) {
            if (isValid) {
                $scope.alreadySubmit = true;
                if ($scope.Count * 1 > 0) {
                    layer.confirm($giqci.KeyToWord("confirm_apply") + $scope.Count  +$giqci.KeyToWord("confirm_apply_msg_tail") , function (l) {
                        $giqci.post('/api/forms/addzcodeapply', { ZcodeType: $scope.ZcodeType, Count: $scope.Count }).success(function (data) {
                            if (data.flag) {
                                layer.msg($giqci.KeyToWord("apply_success"), { icon: 6, time: 1000 }, function () {
                                    window.location.href =  $giqci.getLanUrl("/forms/zcodeapplylist");
                                });
                            }
                        }, $scope);
                        layer.close(l);
                    });
                } else {
                    $scope.alreadySubmit = false;
                    layer.alert($giqci.KeyToWord("zcode_must_be_a_positive_integer_number"));
                }
            } else {
                $scope.alreadySubmit = false;
            }
        }
        $scope.ZcodeApplyList = {};
        $scope.Paging = new PageHandler("/api/forms/getzcodeapplylogs", function (result) {
            $scope.ZcodeApplyList = result.data;
            $scope.$apply();
        });
        $scope.Paging.FirstPage();
    }
]);