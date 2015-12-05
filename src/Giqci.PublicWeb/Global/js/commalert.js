angular.module('commonAlert', ['ng']).value("alerts", []).factory('commAlertService', ['$rootScope', '$timeout', 'alerts', function ($rootScope, $timeout, alerts) {
    return {
        "alertService": function () {
            var alertJson = {};
            $rootScope.alerts = alerts;
            alertJson.add = function (type, msg, time) {
                for (var x = 1 in $rootScope.alerts) {
                    $rootScope.alerts.splice(x, 1);
                }
                $rootScope.alerts.push({
                    'type': type, 'msg': msg, 'close': function () {
                        alertJson.closeAlert(this);
                    }
                });
                if (time) {
                    $timeout(function () {
                        $rootScope.alerts = [];
                    }, time);
                }
            };
            alertJson.closeAlert = function (alert) {
                $rootScope.alerts.splice($rootScope.alerts.indexOf(alert), 1);
            };
            return alertJson;
        }
    }
}])