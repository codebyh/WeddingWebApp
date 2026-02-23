'use strict';
angular.module('weddingApp')
.controller('appCtrl', ['$scope', 'authSvc', function ($scope, authSvc) {
    $scope.isAdminLoggedIn = authSvc.isAuthenticated();

    $scope.$on('auth:changed', function (_evt, isLoggedIn) {
        $scope.isAdminLoggedIn = !!isLoggedIn;
    });
}]);
