angular.module('giqciService', ['ng'])
.value("alerts", []).factory('alertService', [
    '$rootScope', '$timeout', 'alerts', function ($rootScope, $timeout, alerts) {
        var alertService = {};
        $rootScope.alerts = alerts;
        alertService.add = function (type, msg, time) {
            for (var x = 1 in $rootScope.alerts) {
                $rootScope.alerts.splice(x, 1);
            }
            $rootScope.alerts.push({
                'type': type,
                'msg': msg,
                'close': function () {
                    alertService.closeAlert(this);
                }
            });
            if (time) {
                $timeout(function () {
                    $rootScope.alerts = [];
                }, time);
            }
        };
        alertService.closeAlert = function (alert) {
            $rootScope.alerts.splice($rootScope.alerts.indexOf(alert), 1);
        };
        return alertService;
    }
]);