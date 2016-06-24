/*
 ng-tip-validate.js v1.0
 author: wangzhuo   
 email: wzfindjob@163.com
 require: jquery,angularjs,layer.js
*/
(function(){
	
	//辅助函数
	function prepare(elem , attrs, directiveName , ngTipValidateConfig){
		var id =  elem.attr("id");
      	if (!id){
      	 	console.warn(directiveName + " must has 'id'. if not it will auto generate one.");
      		id = Math.random().toString(36).substr(10);
	      	elem.attr("id",id);
      	}
      	var submitBtn;
      	if (attrs.submitBtnId){
      	 	submitBtn = $("#" + attrs.submitBtnId);
      	}
      	
      	if (!submitBtn || submitBtn.length ==0){
      	 		//找submitBtnClass
      	 		if (attrs.submitBtnClass){
      	 			submitBtn = $("." + attrs.submitBtnClass);
      	 		}
      	 	}
      	
      	if (!submitBtn || submitBtn.length ==0){
      		console.warn(directiveName + " which id is '" + id + "' does not have submitBtn, forget to write attrs of 'sumbmit-btn-id' or 'sumbmit-btn-class' ?");
      	}
      	
      	//弹出提示反向
      	var tipDirection = attrs.uccTipDirection ? attrs.uccTipDirection : ngTipValidateConfig.tipDirection;
      	
      	return {
      		submitBtn :submitBtn,
      		id : id,
      		tipDirection: tipDirection
      	};
	}
	
	
	angular.module('ngTipValidate',[])
	
	//默认配置项
	.service("ngTipValidateConfig", function(){
		this.uccRequireMsg = "必填项";
		this.uccPhoneMsg = "手机格式错误";
		this.uccTelMsg = "电话格式错误";
		this.uccEmailMsg = "邮箱格式错误";
		this.uccBankCardMsg = "银行卡格式错误";
		this.uccMinLengthMsg = "长度不能小于$";
		this.uccMaxLengthMsg = "长度不能大于$";
		//错误提示的方向
		this.tipDirection = 2; // 1, 2 , 3, 4 分别代表上、右、 下 、 左
	})
	
	
	//预置验证函数
	.service("validator",function(){
		
		/* 对象是否为空 */
		var isNullOrEmpty = function (obj) {
			if (obj == null || obj == undefined || typeof (obj) == 'undefined'
					|| obj == '') {
				return true;
			} else if (typeof (obj) == 'string') {
				obj = obj.trim();
				if (obj == '') {
					return true;
				} else {
					obj = obj.toUpperCase();
					if (obj == 'NULL' || obj == 'UNDEFINED' || obj == '{}') {
						return true;
					}
				}
			}
			return false;
		};
		
		// 验证手机号码
		this.isPhoneNumber = function(phoneNumber) {
			if (!isNullOrEmpty(phoneNumber)) {
				var reg = /^0?1[3|4|5|8][0-9]\d{8}$/;
				return reg.test(phoneNumber);
			}
			return true;
	
		};
		// 验证 电话号码
		this.isTel = function(tel) {
			if (!isNullOrEmpty(tel)) {
				var reg = /^0\d{2,3}-?\d{7,8}$/;
				return reg.test(tel);
			}
			return true;
	
		};
		// 验证邮箱
		this.isEmail = function(email) {
			if (!isNullOrEmpty(email)) {
				var reg = /^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/;
				return reg.test(email);
			}
			return true;
		};
		// 验证银行卡
		this.isBankCard = function(cardNo) {
			if (!isNullOrEmpty(cardNo)) {
				// 19位卡号
				return /^\d{19}$/.test(cardNo);
			}
			return true;
		};
		
		//是否正整数
		this.isPositiveNumber = function(input){
			if (isNullOrEmpty(input)){
				return false;
			}
			return /^\d+$/.test(input);
		}
		//是否匹配正则
		this.isMatch = function(input,pattern){
			var patt =new RegExp(pattern);
			return patt.test(input);
		}
	})
	
	.directive('uccRequire',["$timeout","ngTipValidateConfig",
		function( $timeout, ngTipValidateConfig){
			return {
		      restrict: 'A',
		      require:  'ngModel',
		     
		      link: function(scope, elem, attrs, ngModel) {
		      	 var errMsg = attrs['uccRequire'];
		      	 if (!errMsg){
		      	 	errMsg = ngTipValidateConfig.uccRequireMsg;
		      	 }
		      	 
		      	 var pObject = prepare(elem,attrs,"ucc-require",ngTipValidateConfig);
		      	 var id =  pObject.id;
		      	 var submitBtn = pObject.submitBtn;
		      	 
		      	 var tipId = null;
		      	 function validateMothed(){
		      	 	if (!ngModel.$modelValue || ngModel.$modelValue.trim() == ""){
		      	 		ngModel.uccError = errMsg;
		      	 		ngModel.$setValidity('uccRequire', false);
		      	 		//先把之前的错误提示关闭
	                	if (tipId){
	                		layer.close(tipId); tipId = null;
	                	}
		      	 		tipId = layer.tips(errMsg, '#' + id ,  {
						    tipsMore: true, time:5000, tips: pObject.tipDirection 
						});
		      	 	}
		      	 	else{
		      	 		ngModel.$setValidity('uccRequire', true);
		      	 		layer.close(tipId); tipId = null;
		      	 	}
		      	 }
		      	 
		      	 //初始化
		      	 //$timeout(function(){
		      	 //	validateMothed();
		      	 //});
		      	 
		      	 //监控model变化
		      	 elem.blur(function(){
		      	 	validateMothed();
		      	 });
		      	 
		      	 //submit提交时显示错误
		      	 submitBtn.click(function(){
		      	 	validateMothed();
		      	 });
		      }
		    };
		}]
	)
	
	.directive('uccEmail',["$timeout","ngTipValidateConfig", "validator",
		function( $timeout, ngTipValidateConfig, validator){
			return {
		      restrict: 'A',
		      require:  'ngModel',
		     
		      link: function(scope, elem, attrs, ngModel) {
		      	 var errMsg = attrs['uccEmail'] ? attrs['uccEmail'] : ngTipValidateConfig.uccEmailMsg;
		      	 
		      	 var pObject = prepare(elem,attrs,"ucc-email",ngTipValidateConfig);
		      	 var id =  pObject.id;
		      	 var submitBtn = pObject.submitBtn;
		      	 
		      	 
		      	 var tipId = null;
		      	 function validateMothed(){
		      	 	if ( !validator.isEmail(ngModel.$modelValue )){
		      	 		ngModel.uccError = errMsg;
		      	 		ngModel.$setValidity('uccEmail', false);
		      	 		//先把之前的错误提示关闭
	                	if (tipId){
	                		layer.close(tipId); tipId = null;
	                	}
		      	 		tipId = layer.tips(errMsg, '#' + id ,  {
						    tipsMore: true, time:5000 , tips: pObject.tipDirection
						});
		      	 	}
		      	 	else{
		      	 		ngModel.$setValidity('uccEmail', true);
		      	 		layer.close(tipId); tipId = null;
		      	 	}
		      	 }
		      	 //初始化
		      	 //$timeout(function(){
		      	 //	validateMothed();
		      	 //});
		      	 
		      	 //监控model变化
		      	 elem.blur(function(){
		      	 	validateMothed();
		      	 });
		      	 
		      	 //submit提交时显示错误
		      	 submitBtn.click(function(){
		      	 	validateMothed();
		      	 });
		      }
		    };
		}]
	)
	
	.directive('uccPhone',["$timeout","ngTipValidateConfig", "validator",
		function( $timeout, ngTipValidateConfig, validator){
			return {
		      restrict: 'A',
		      require:  'ngModel',
		     
		      link: function(scope, elem, attrs, ngModel) {
		      	 var errMsg = attrs['uccPhone'] ? attrs['uccPhone'] : ngTipValidateConfig.uccPhoneMsg;
		      	 var pObject = prepare(elem,attrs,"ucc-phone",ngTipValidateConfig);
		      	 var id =  pObject.id;
		      	 var submitBtn = pObject.submitBtn;
		      	 
		      	 var tipId = null;
		      	 function validateMothed(){
		      	 	if ( !validator.isPhoneNumber(ngModel.$modelValue )){
		      	 		ngModel.uccError = errMsg;
		      	 		//先把之前的错误提示关闭
	                	if (tipId){
	                		layer.close(tipId); tipId = null;
	                	}
		      	 		tipId = layer.tips(errMsg, '#' + id ,  {
						    tipsMore: true, time:5000 , tips: pObject.tipDirection
						});
						ngModel.$setValidity('uccPhone', false);
		      	 	}
		      	 	else{
		      	 		ngModel.$setValidity('uccPhone', true);
		      	 		layer.close(tipId); tipId = null;
		      	 	}
		      	 }
		      	 //初始化
		      	 //$timeout(function(){
		      	 //	validateMothed();
		      	 //});
		      	 
		      	 //监控model变化
		      	 elem.blur(function(){
		      	 	validateMothed();
		      	 });
		      	 
		      	 //submit提交时显示错误
		      	 submitBtn.click(function(){
		      	 	validateMothed();
		      	 });
		      	 
		      }
		    };
		}]
	)

	.directive('uccTel',["$timeout","ngTipValidateConfig", "validator",
		function( $timeout, ngTipValidateConfig, validator){
			return {
		      restrict: 'A',
		      require:  'ngModel',
		     
		      link: function(scope, elem, attrs, ngModel) {
		      	 var errMsg = attrs['uccTel'] ? attrs['uccTel'] : ngTipValidateConfig.uccTelMsg;
		      	 var pObject = prepare(elem,attrs,"ucc-tel",ngTipValidateConfig);
		      	 var id =  pObject.id;
		      	 var submitBtn = pObject.submitBtn;
		      	 
		      	 var tipId = null;
		      	 function validateMothed(){
		      	 	if ( !validator.isTel(ngModel.$modelValue )){
		      	 		ngModel.uccError = errMsg;
		      	 		//先把之前的错误提示关闭
	                	if (tipId){
	                		layer.close(tipId); tipId = null;
	                	}
		      	 		tipId = layer.tips(errMsg, '#' + id ,  {
						    tipsMore: true, time:5000 , tips: pObject.tipDirection
						});
						ngModel.$setValidity('uccTel', false);
		      	 	}
		      	 	else{
		      	 		ngModel.$setValidity('uccTel', true);
		      	 		layer.close(tipId); tipId = null;
		      	 	}
		      	 }
		      	 //初始化
		      	 //$timeout(function(){
		      	 //	validateMothed();
		      	 //});
		      	 
		      	 //监控model变化
		      	 elem.blur(function(){
		      	 	validateMothed();
		      	 });
		      	 
		      	 //submit提交时显示错误
		      	 submitBtn.click(function(){
		      	 	validateMothed();
		      	 });
		      	 
		      }
		    };
		}]
	)
	
	.directive('uccBankCard',["$timeout","ngTipValidateConfig", "validator",
		function( $timeout, ngTipValidateConfig, validator){
			return {
		      restrict: 'A',
		      require:  'ngModel',
		     
		      link: function(scope, elem, attrs, ngModel) {
		      	 var errMsg = attrs['uccBankCard'] ? attrs['uccBankCard'] : ngTipValidateConfig.uccBankCardMsg;
		      	 var pObject = prepare(elem,attrs,"ucc-bank-card",ngTipValidateConfig);
		      	 var id =  pObject.id;
		      	 var submitBtn = pObject.submitBtn;
		      	 
		      	 var tipId = null;
		      	 function validateMothed(){
		      	 	if ( !validator.isBankCard(ngModel.$modelValue )){
		      	 		ngModel.uccError = errMsg;
		      	 		//先把之前的错误提示关闭
	                	if (tipId){
	                		layer.close(tipId); tipId = null;
	                	}
		      	 		tipId = layer.tips(errMsg, '#' + id ,  {
						    tipsMore: true, time:5000 , tips: pObject.tipDirection
						});
						ngModel.$setValidity('uccTel', false);
		      	 	}
		      	 	else{
		      	 		ngModel.$setValidity('uccTel', true);
		      	 		layer.close(tipId); tipId = null;
		      	 	}
		      	 }
		      	 //初始化
		      	 //$timeout(function(){
		      	 //	validateMothed();
		      	 //});
		      	 
		      	 //监控model变化
		      	 elem.blur(function(){
		      	 	validateMothed();
		      	 });
		      	 
		      	 //submit提交时显示错误
		      	 submitBtn.click(function(){
		      	 	validateMothed();
		      	 });
		      	 
		      }
		    };
		}]
	)
	
	.directive('uccMinLength',["$timeout","ngTipValidateConfig", "validator",
		function( $timeout, ngTipValidateConfig, validator){
			return {
		      restrict: 'A',
		      require:  'ngModel',
		     
		      link: function(scope, elem, attrs, ngModel) {
		      	 
		      	 var len = attrs['uccMinLength'] ; 
		      	 if (!validator.isPositiveNumber(len)){
		      	 	throw new Error('ucc-min-length must has positive number!' );
		      	 }
		      	 var pObject = prepare(elem,attrs,"ucc-min-length",ngTipValidateConfig);
		      	 var id =  pObject.id;
		      	 var submitBtn = pObject.submitBtn;
		      	 
		      	 var tipId = null;
		      	 function validateMothed(){
		      	 	if (!ngModel.$modelValue || ngModel.$modelValue.length < len){
		      	 		ngModel.uccError = ngTipValidateConfig.uccMinLengthMsg.replace("$", len);
		      	 		//先把之前的错误提示关闭
	                	if (tipId){
	                		layer.close(tipId); tipId = null;
	                	}
		      	 		tipId = layer.tips(ngModel.uccError, '#' + id ,  {
						    tipsMore: true, time:5000 , tips: pObject.tipDirection
						});
						ngModel.$setValidity('uccMinLength', false);
		      	 	}
		      	 	else{
		      	 		ngModel.$setValidity('uccMinLength', true);
		      	 		layer.close(tipId); tipId = null;
		      	 	}
		      	 }
		      	 //初始化
		      	 //$timeout(function(){
		      	 //	validateMothed();
		      	 //});
		      	 
		      	 //监控model变化
		      	 elem.blur(function(){
		      	 	validateMothed();
		      	 });
		      	 
		      	 //submit提交时显示错误
		      	 submitBtn.click(function(){
		      	 	validateMothed();
		      	 });
		      	 
		      }
		    };
		}]
	)
	
	.directive('uccMaxLength',["$timeout","ngTipValidateConfig", "validator",
		function( $timeout, ngTipValidateConfig, validator){
			return {
		      restrict: 'A',
		      require:  'ngModel',
		     
		      link: function(scope, elem, attrs, ngModel) {
		      	 
		      	 var len = attrs['uccMaxLength'] ; 
		      	 if (!validator.isPositiveNumber(len)){
		      	 	throw new Error('ucc-max-length must has positive number!' );
		      	 }
		      	 var pObject = prepare(elem,attrs,"ucc-max-length",ngTipValidateConfig);
		      	 var id =  pObject.id;
		      	 var submitBtn = pObject.submitBtn;
		      	 
		      	 var tipId = null;
		      	 function validateMothed(){
		      	 	if (ngModel.$modelValue && ngModel.$modelValue.length > len){
		      	 		ngModel.uccError = ngTipValidateConfig.uccMaxLengthMsg.replace("$", len);
		      	 		//先把之前的错误提示关闭
	                	if (tipId){
	                		layer.close(tipId); tipId = null;
	                	}
		      	 		tipId = layer.tips(ngModel.uccError, '#' + id ,  {
						    tipsMore: true, time:5000 , tips: pObject.tipDirection
						});
						ngModel.$setValidity('uccMaxLength', false);
		      	 	}
		      	 	else{
		      	 		ngModel.$setValidity('uccMaxLength', true);
		      	 		layer.close(tipId); tipId = null;
		      	 	}
		      	 }
		      	 //初始化
		      	 //$timeout(function(){
		      	 //	validateMothed();
		      	 //});
		      	 
		      	 //监控model变化
		      	 elem.blur(function(){
		      	 	validateMothed();
		      	 });
		      	 
		      	 //submit提交时显示错误
		      	 submitBtn.click(function(){
		      	 	validateMothed();
		      	 });
		      	 
		      }
		    };
		}]
	)
	
	.directive('uccPattern',["$timeout","ngTipValidateConfig", "validator",
		function( $timeout, ngTipValidateConfig, validator){
			return {
		      restrict: 'A',
		      require:  'ngModel',
		     
		      link: function(scope, elem, attrs, ngModel) {
		      	 
		      	 var pattern = attrs['uccPattern'] ; 
		      	 if (!pattern){
		      	 	throw new Error('ucc-pattern must has value!' );
		      	 }
		      	 var patternErrMsg = attrs['uccPatternMessage'];
		      	 if (!patternErrMsg){
		      	 	throw new Error('ucc-pattern must has value!' );
		      	 }
		      	 var pObject = prepare(elem,attrs,"ucc-pattern",ngTipValidateConfig);
		      	 var id =  pObject.id;
		      	 var submitBtn = pObject.submitBtn;
		      	 
		      	 var tipId = null;
		      	 function validateMothed(){
		      	 	if (!validator.isMatch(ngModel.$modelValue , pattern)){
		      	 		ngModel.uccError = patternErrMsg;
		      	 		//先把之前的错误提示关闭
	                	if (tipId){
	                		layer.close(tipId); tipId = null;
	                	}
		      	 		tipId = layer.tips(ngModel.uccError, '#' + id ,  {
						    tipsMore: true, time:5000 , tips: pObject.tipDirection
						});
						ngModel.$setValidity('uccPattern', false);
		      	 	}
		      	 	else{
		      	 		ngModel.$setValidity('uccPattern', true);
		      	 		layer.close(tipId); tipId = null;
		      	 	}
		      	 }
		      	 //初始化
		      	 //$timeout(function(){
		      	 //	validateMothed();
		      	 //});
		      	 
		      	 //监控model变化
		      	 elem.blur(function(){
		      	 	validateMothed();
		      	 });
		      	 
		      	 //submit提交时显示错误
		      	 submitBtn.click(function(){
		      	 	validateMothed();
		      	 });
		      	 
		      }
		    };
		}]
	)
	
	.directive('uccValidateFunc',["$timeout","ngTipValidateConfig", "validator","$parse",
		function( $timeout, ngTipValidateConfig, validator,$parse){
			return {
		      restrict: 'A',
		      require:  'ngModel',
		     
		      link: function(scope, elem, attrs, ngModel) {
		      	 
		      	 var uccValidateFunc = attrs['uccValidateFunc'] ; 
		      	 if (!uccValidateFunc){
		      	 	throw new Error('ucc-validate-func must has value!' );
		      	 }
		      	 
		      	 var pObject = prepare(elem,attrs,"ucc-validate-func",ngTipValidateConfig);
		      	 var id =  pObject.id;
		      	 var submitBtn = pObject.submitBtn;
		      	 
		      	 var tipId = null;
	      	 	 var parseFunc = $parse(uccValidateFunc);
		      	 function validateMothed(){
		      	 	
		      	 	var invalidMsg = parseFunc(scope, {value:ngModel.$modelValue});
		      	 	
		      	 	if (invalidMsg){
		      	 		ngModel.uccError = invalidMsg;
		      	 		//先把之前的错误提示关闭
	                	if (tipId){
	                		layer.close(tipId); tipId = null;
	                	}
		      	 		tipId = layer.tips(ngModel.uccError, '#' + id ,  {
						    tipsMore: true, time:5000 , tips: pObject.tipDirection
						});
						ngModel.$setValidity('uccPattern', false);
		      	 	}
		      	 	else{
		      	 		ngModel.$setValidity('uccPattern', true);
		      	 		layer.close(tipId); tipId = null;
		      	 	}
		      	 }
		      	 //初始化
		      	 //$timeout(function(){
		      	 //	validateMothed();
		      	 //});
		      	 
		      	 //监控model变化
		      	 elem.blur(function(){
		      	 	validateMothed();
		      	 });
		      	 
		      	 //submit提交时显示错误
		      	 submitBtn.click(function(){
		      	 	validateMothed();
		      	 });
		      	 
		      }
		    };
		}]
	)
	
	
	.directive('uccRemote',["$timeout","ngTipValidateConfig", "validator", "$http","$parse",
		function( $timeout, ngTipValidateConfig, validator, $http,$parse){
			return {
		      restrict: 'A',
		      require:  'ngModel',
		     
		      link: function(scope, elem, attrs, ngModel) {
		      	 
		      	 var url = attrs['uccRemote'] ; 
		      	 if (!url){
		      	 	throw new Error('ucc-remote must has value!' );
		      	 }
		      	
		      	 var pObject = prepare(elem,attrs,"ucc-remote",ngTipValidateConfig);
		      	 var id =  pObject.id;
		      	 var submitBtn = pObject.submitBtn;
		      	 
		      	 var tipId = null;
		      	 function validateMothed(){
		      	 	
		      	 	var func = $parse(attrs['uccRemoteData']);
		      	 	var postData = func(scope);
		      	 	if (!postData) postData = {};
		      	 	postData.value = ngModel.$modelValue;
		      	 	$http.post(url, postData)
		      	 	.success(function (data, status, headers, config) {
		                if (data.success){
		                  	ngModel.$setValidity('uccRemote', true);
		                  	layer.close(tipId); tipId = null;
		                }
		                else{
		                	ngModel.$setValidity('uccRemote', false);
		                	ngModel.uccError = data.msg;
		                	//先把之前的错误提示关闭
		                	if (tipId){
		                		layer.close(tipId); tipId = null;
		                	}
		      	 			
		      	 			tipId = layer.tips(ngModel.uccError, '#' + id ,  {
							    tipsMore: true, time:5000 , tips: pObject.tipDirection
							});
		                }
		            }).error(function (data, status, headers, config) {
		                    ngModel.$setValidity('uccRemote', false);
		                	ngModel.uccError = "远程验证错误：" + status;
		      	 			//先把之前的错误提示关闭
		                	if (tipId){
		                		layer.close(tipId); tipId = null;
		                	}
		      	 			tipId = layer.tips(ngModel.uccError, '#' + id ,  {
							    tipsMore: true, time:5000 , tips: pObject.tipDirection
							});
		            });
		      	 	
		      	 	
		      	 }
		      	 //初始化
		      	 //$timeout(function(){
		      	 //	validateMothed();
		      	 //});
		      	 
		      	 //监控model变化
		      	 elem.blur(function(){
		      	 	validateMothed();
		      	 });
		      	 
		      	 //submit提交时显示错误
		      	 submitBtn.click(function(){
		      	 	validateMothed();
		      	 });
		      	 
		      }
		    };
		}]
	)
	
	
}());






