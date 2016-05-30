if (typeof jQuery === 'undefined') {
    throw new Error("Giqci's JavaScript requires jQuery");
}


function PageHandler(postUrl, callBackFunc, customPageSize) {
    var self = this;
    var url = postUrl;
    var func = callBackFunc;
    var pageSize = customPageSize == null ? 10 : customPageSize;
    self.pageIndex = 1;
    self.lastTimeResultCount = pageSize;
    self.CanNext = false;
    self.CanPrev = false;
    self.queryCondition = {};

    function canWeGo(currentPageIndex) {
        self.CanPrev = currentPageIndex - 1 > 0;
        self.CanNext = self.lastTimeResultCount === pageSize;
    }

    function go(postData) {
        //防止数据加载完前点击
        self.CanNext = false;
        self.CanPrev = false;
        self.queryCondition = postData == null ? self.queryCondition : postData;
        postData = self.queryCondition;
        postData["pageIndex"] = self.pageIndex;
        postData["pageSize"] = pageSize;
        $.ajax({
            url: url,
            data: postData,
            type: "POST",
            success: function (result) {
                self.lastTimeResultCount = result.data.length;
                canWeGo(self.pageIndex);
                if (result.msg != null) {
                    alert(result.msg);
                }
                func(result);
            },
            error: function (result) {
                console.log(result);
                alert(":(\nWhoops,looks like something went wrong");
            }
        });
    }

    this.FirstPage = function (postData) {
        self.pageIndex = 1;
        go(postData);
    }
    this.PrevPage = function (postData) {
        if (self.CanPrev) {
            self.pageIndex--;
            go(postData);
        }

    }
    this.NextPage = function (postData) {
        if (self.CanNext) {
            self.pageIndex++;
            go(postData);
        }
    }
}

function HtmlDecode(htmlEncodeStr) {
    var elem = document.createElement('textarea');
    elem.innerHTML = htmlEncodeStr;
    return elem.value;
}

function ObjDecodeToJson(htmlEncodeStr) {
    return JSON.parse(HtmlDecode(htmlEncodeStr));

}

function IsJsonStr(value) {
    try {
        $.parseJSON(value);
        return true;
    } catch (e) {
        return false;
    }
}

function CallByValue(obj) {
    if ($.isPlainObject(obj) || $.isArray(obj)) {
        var newObj = {};
        for (var i in obj) {
            if ($.isPlainObject(obj[i]) || $.isArray(obj[i])) {
                newObj[i] = CallByValue(obj[i]);
            } else {
                newObj[i] = obj[i];
            }
        }
        return newObj;
    } else {
        return obj;
    }
}

function Breath(checkLoginUrl, whenLogOutCallBackFunc, whenLoginedCallBackFunc, millisec) {
    var self = this;
    millisec = millisec == null ? 5000 : millisec;
    self.WhenLogOutCallBackFunc = whenLogOutCallBackFunc;
    self.WhenLoginedCallBackFunc = whenLoginedCallBackFunc;
    var breathing = false;
    function sendReuqest() {
        if (breathing) {
            $.ajax({
                url: checkLoginUrl,
                type: "POST",
                success: function (result) {
                    if (result.flag) {
                        self.WhenLoginedCallBackFunc();
                    }
                    else {
                        self.WhenLogOutCallBackFunc();
                    }
                },
                error: function (result) {
                    console.log(result);
                }
            });
        }
    }
    var colock = setInterval(sendReuqest, millisec);
    this.Revive = function () {
        breathing = true;
    }
    this.Die = function () {
        breathing = false;
    }
    this.ChangeTicks = function (newMillisec) {
        window.clearInterval(colock);
        colock = setInterval(sendReuqest, newMillisec);
    }

}


var $giqci = {};
$giqci.RunLastPost = function () {
    $giqci.LastPostInfo.SendRequest($giqci.LastPostInfo.Scope, $giqci.LastPostInfo.CallBackFunc, $giqci.LastPostInfo.WithData);
}
$giqci.LastPostInfo = {};
$giqci.CacheLastPost = function (sendRequest, scope, callBackFunc, withData) {
    $giqci.LastPostInfo.SendRequest = sendRequest;
    $giqci.LastPostInfo.CallBackFunc = callBackFunc;
    $giqci.LastPostInfo.WithData = withData;
    $giqci.LastPostInfo.Scope = scope;

};

$giqci.get = function (url, data) {
    var self = this;
    console.log(self);
    self.LoginFunc = function () {
        $("#breathBox").modal("show");
    }
    self.ShowMsgFunc = function (msg) {
        alert(msg);
    }
    self.DefaultSuccessFunc = function (result) {
        var callbackData = result.callBackPackage;
        if (callbackData.callBackUrl !== null) {
            window.location.href = callbackData.callBackUrl;
        }
        alert("提交成功");
    }

    function handlerResult(scope, result, callBackFunc, withData) {
        switch (result.status) {
            case -1:
                {
                    $giqci.CacheLastPost(sendRequest, scope, callBackFunc, withData);
                    self.LoginFunc();
                    break;
                }
            case 0:
                {
                    self.ShowMsgFunc(result.msg);
                    break;
                }
            case 1:
                {
                    callBackFunc = callBackFunc === undefined ? self.DefaultSuccessFunc : callBackFunc;
                    var tempResult = withData ? result : result.data;
                    callBackFunc(tempResult);
                    break;
                }
            default:
                {
                    alert("UnknowStatus");
                }
        }
        try {
            scope.$apply();
        } catch (ex) {
            //not AngularJS 
        }
    }
    function sendRequest(scope, callBackFunc, withData) {
        $.ajax({
            type: "GET",
            data: data,
            url: url,
            success: function (result) {
                handlerResult(scope, result, callBackFunc, withData);
            },
            error: function (result) {
                args.error(result);
            }
        });
    }

    self.then = function (callBackFunc, scope) {
        sendRequest(scope, callBackFunc, true);
    }
    self.success = function (callBackFunc, scope) {
        sendRequest(scope, callBackFunc, false);
    }
    return self;
}

$giqci.post = function (url, data) {
    var self = this;
    console.log(self);
    self.LoginFunc = function () {
        $("#breathBox").modal("show");
    }
    self.ShowMsgFunc = function (msg) {
        alert(msg);
    }
    self.DefaultSuccessFunc = function (result) {
        var callbackData = result.callBackPackage;
        if (callbackData.callBackUrl !== null) {
            window.location.href = callbackData.callBackUrl;
        }
        alert("提交成功");
    }

    function handlerResult(scope, result, callBackFunc, withData) {
        switch (result.status) {
            case -1:
                {
                    $giqci.CacheLastPost(sendRequest, scope, callBackFunc, withData);
                    self.LoginFunc();
                    break;
                }
            case 0:
                {
                    self.ShowMsgFunc(result.msg);
                    break;
                }
            case 1:
                {
                    callBackFunc = callBackFunc === undefined ? self.DefaultSuccessFunc : callBackFunc;
                    var tempResult = withData ? result : result.data;
                    callBackFunc(tempResult);
                    break;
                }
            default:
                {
                    alert("UnknowStatus");
                }
        }
        try {
            scope.$apply();
        } catch (ex) {
            //not AngularJS 
        }
    }
    function sendRequest(scope, callBackFunc, withData) {
        $.ajax({
            type: "POST",
            data: data,
            url: url,
            success: function (result) {
                handlerResult(scope, result, callBackFunc, withData);
            },
            error: function (result) {
                args.error(result);
            }
        });
    }

    self.then = function (callBackFunc, scope) {
        sendRequest(scope, callBackFunc, true);
    }
    self.success = function (callBackFunc, scope) {
        sendRequest(scope, callBackFunc, false);
    }
    return self;
}
