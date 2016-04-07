if (typeof jQuery === 'undefined') {
    throw new Error("Giqci's JavaScript requires jQuery");
}

function PageHandler(postUrl, callBackFunc, customPageSize) {
    var url = postUrl;
    var func = callBackFunc;
    var pageSize = customPageSize == null ? 10 : customPageSize;
    var pageIndex = 1;
    var lastTimeResultCount = pageSize;
    var queryCondition = {};

    function canWeGo(currentPageIndex, type) {
        var isEnabled;
        switch (type) {
        case -1:
        {
            isEnabled = currentPageIndex - 1 > 0;
            if (isEnabled) {
                --pageIndex;
            }
            return isEnabled;
        }
        case 1:
        {
            isEnabled = lastTimeResultCount === pageSize;
            if (isEnabled) {
                ++pageIndex;
            }
            return isEnabled;
        }
        default:
        {
            pageIndex = 1;
            return false;
        }
        }
    }

    function go(postData) {
        queryCondition = postData == null ? queryCondition : postData;
        postData = queryCondition;
        postData["pageIndex"] = pageIndex;
        postData["pageSize"] = pageSize;
        $.ajax({
            url: url,
            data: postData,
            type: "POST",
            success: function(result) {
                lastTimeResultCount = result.data.length;
                if (result.msg != null) {
                    alert(result.msg);
                }
                func(result);
            },
            error: function(result) {
                console.log(result);
                alert(":(\nWhoops,looks like something went wrong");
            }
        });
    }

    this.FirstPage = function(postData) {
        pageIndex = 1;
        go(postData);
    }
    this.PrevPage = function(postData) {
        if (canWeGo(pageIndex, -1))
            go(postData);
    }
    this.NextPage = function(postData) {
        if (canWeGo(pageIndex, 1))
            go(postData);
    }
}