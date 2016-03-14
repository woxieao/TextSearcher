"use strict";

app.directive("ajaxSelect", function ($timeout) {
    return {
        restrict: 'AC',
        require: "ngModel",
        link: function (scope, element, attrs, model) {
            var $element = $(element);
            var url = attrs["ajaxUrl"];
            $element.select2({
                theme: "bootstrap",
                ajax: {
                    url: "/api/dict/" + url,
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            q: params.term
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
                    console.log($element.val());
                    model.$setViewValue($element.val());
                });
            });
            // model -> view
            model.$render = function () {
                $element.html(model.$viewValue);
            };

            scope.$watch(attrs["ngModel"], function (n, o) {
                //console.log("change to  " + n);
                //element.select2().val(n);
            }, true);
        }
    };
});