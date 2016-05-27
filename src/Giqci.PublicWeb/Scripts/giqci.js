if (typeof jQuery === 'undefined') {
    throw new Error("Giqci's JavaScript requires jQuery");
}
var Send = {}

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
Send.Ajax = function (args) {
    var self = this;
    self.LoginFunc = function () {

    }
    self.ShowMsgFunc = function (msg) {
        alert(msg);
    }
    self.DefaultSuccessFunc = function (result) {
        var callbackData = result.CallBackPackage;
        if (callbackData.CallBackUrl !== null) {
            window.location.href = callbackData.CallBackUrl;
        }
        alert("提交成功");

    }

    function handlerResult(result, callBackFunc) {
        switch (result.Flag) {
            case -1:
                {
                    self.LoginFunc();
                    break;
                }
            case 0:
                {
                    self.ShowMsgFunc(result.Msg);
                    break;
                }
            case 1:
                {
                    callBackFunc = callBackFunc === undefined ? self.DefaultSuccessFunc : callBackFunc;
                    callBackFunc(result);
                    break;
                }
            default:
                {
                }
        }
    }

    $.ajax({
        type: args.type === undefined ? "POST" : args.type,
        data: args.data === undefined ? "" : args.data,
        url: args.url,
        success: function (result) {
            handlerResult(result, args.success);
        },
        error: function (result) {
            console.log(result);
            args.error(result);
        }
    });
}