'use strict';
angular.module('weddingApp')
.controller('appCtrl', ['$scope', '$location', 'authSvc', function ($scope, $location, authSvc) {
    $scope.isAdminLoggedIn = authSvc.isAuthenticated();

    $scope.$on('auth:changed', function (_evt, isLoggedIn) {
        $scope.isAdminLoggedIn = !!isLoggedIn;
    });

    $scope.logoutFromHeader = function () {
        authSvc.logout();
        $location.path('/home');
    };
}]);
