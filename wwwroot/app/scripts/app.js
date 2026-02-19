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
}]);
