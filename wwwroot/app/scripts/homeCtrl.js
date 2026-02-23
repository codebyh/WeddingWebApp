'use strict';
angular.module('weddingApp')
.controller('homeCtrl', ['$scope', '$location', 'usersSvc', 'authSvc', function ($scope, $location, usersSvc, authSvc) {
    $scope.credentials = {
        username: authSvc.getUsername(),
        password: ''
    };
    $scope.loginError = '';
    $scope.loginSuccess = '';
    $scope.isLoggingIn = false;
    $scope.isLoggedIn = authSvc.isAuthenticated();

    $scope.$on('auth:changed', function (_evt, isLoggedIn) {
        $scope.isLoggedIn = !!isLoggedIn;
        if ($scope.isLoggedIn) {
            $scope.credentials.username = authSvc.getUsername();
        }
    });

    $scope.login = function () {
        $scope.loginError = '';
        $scope.loginSuccess = '';

        var username = ($scope.credentials.username || '').trim();
        var password = $scope.credentials.password || '';
        if (!username || !password) {
            $scope.loginError = 'Email and password are required.';
            return;
        }

        authSvc.login(username, password);
        $scope.isLoggingIn = true;
        usersSvc.getAll().then(function () {
            $scope.isLoggingIn = false;
            $scope.isLoggedIn = true;
            $scope.credentials.password = '';
            $scope.loginSuccess = 'Login successful. Admin APIs are now unlocked in this browser.';
            $location.path('/users');
        }).catch(function (response) {
            $scope.isLoggingIn = false;
            $scope.isLoggedIn = false;
            authSvc.logout();
            var status = response ? response.status : 0;
            if (status === 401) {
                $scope.loginError = 'Invalid username or password.';
                return;
            }
            $scope.loginError = 'Login failed. Please verify API/Cosmos configuration and try again.';
        });
    };

    $scope.logout = function () {
        authSvc.logout();
        $scope.isLoggedIn = false;
        $scope.credentials.password = '';
        $scope.loginError = '';
        $scope.loginSuccess = 'Logged out.';
    };
}]);
