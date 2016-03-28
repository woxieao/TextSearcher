"use strict";

app.directive("ajaxSelect", function ($timeout, $http) {
    return {
        restrict: 'AC',
        require: "ngModel",
        link: function (scope, element, attrs, model) {
            var $element = $(element);
            var url = attrs["ajaxUrl"];

            scope.$watch("ngModel", function (n, o) {
                scope.isLoading = false;
                if (!scope.isLoading && model.$viewValue) {
                    $http.get("/api/dict/" + url , {
                        params: { 'code': model.$viewValue }
                    }).success(function (data) {
                        if (data.items.length > 0) {
                            var newVal = (url === "commonhscodes") ? data.items[0].Name : data.items[0].CnName;
                            //console.log(newVal);
                            $(element).next().find("span.select2-selection__rendered").html(newVal);
                            scope.isLoading = true;
                        }
                    }).error(function (data) {
                    });
                }
            }, true);

            $element.select2({
                theme: "bootstrap",
                minimumInputLength: 1,
                language: "zh-CN",
                ajax: {
                    url: "/api/dict/" + url,
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            code : params.term
                        };
                    },
                    processResults: function (data, params) {
                        var results = [];
                        $.each(data.items, function (i, v) {
                            var o = {};
                            o.id = v.Code;
                            o.name = (url === "commonhscodes") ? v.Name : v.CnName;
                            results.push(o);
                        });
                        return {
                            results: results,
                        };
                    },
                    cache: true
                },
                templateResult: function (repo) {
                    return repo.name;
                },
                templateSelection: function (repo) {
                    return repo.name;
                }
            }).on('change', function () {
                scope.$apply(function () {
                    //console.log("change " + $element.val());
                    model.$setViewValue($element.val());
                });
            });
        }
    };
});