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
    self.CanNext = true;
    self.CanPrev = true;
    self.queryCondition = {};

    function canWeGo(currentPageIndex) {
        self.CanPrev = currentPageIndex - 1 > 0;
        self.CanNext = self.lastTimeResultCount === pageSize;
        return { CanPrev: self.CanPrev, CanNext: self.CanNext };
    }

    function go(postData) {
        self.queryCondition = postData == null ? self.queryCondition : postData;
        postData = self.queryCondition;
        postData["pageIndex"] = self.pageIndex;
        postData["pageSize"] = pageSize;
        $.ajax({
            url: url,
            data: postData,
            type: "POST",
            success: function(result) {
                self.lastTimeResultCount = result.data.length;
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
        self.pageIndex = 1;
        canWeGo(1);
        go(postData);
    }
    this.PrevPage = function(postData) {
        canWeGo(self.pageIndex);
        if (self.CanPrev) {
            self.pageIndex--;
            go(postData);
        }

    }
    this.NextPage = function(postData) {
        canWeGo(self.pageIndex);
        if (self.CanNext) {
            self.pageIndex++;
            go(postData);
        }
    }
}