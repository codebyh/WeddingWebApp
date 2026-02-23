'use strict';
angular.module('weddingApp', ['ngRoute'])
.config(['$routeProvider', '$httpProvider', '$locationProvider', function ($routeProvider, $httpProvider, $locationProvider) {

    if (!$httpProvider.defaults.headers.get) {
        $httpProvider.defaults.headers.get = {};
    }
    $httpProvider.defaults.headers.get['If-Modified-Since'] = '0';
    $locationProvider.hashPrefix('!');

    $routeProvider
        .when('/home', {
            controller: 'homeCtrl',
            templateUrl: 'app/views/Home.html'
        })
        .when('/Home', {
            redirectTo: '/home'
        })
        .when('/users', {
            controller: 'usersCtrl',
            templateUrl: 'app/views/Users.html'
        })
        .when('/Users', {
            redirectTo: '/users'
        })
        .otherwise({ redirectTo: '/home' });
}])
.run(['$rootScope', '$location', 'authSvc', function ($rootScope, $location, authSvc) {
    authSvc.initialize();
    $rootScope.$on('$routeChangeStart', function (event, next) {
        var path = next && next.originalPath;
        if (path === '/users' && !authSvc.isAuthenticated()) {
            event.preventDefault();
            $location.path('/home');
        }
    });
}]);
