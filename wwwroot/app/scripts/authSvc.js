'use strict';
angular.module('weddingApp')
.factory('authSvc', ['$http', '$rootScope', function ($http, $rootScope) {
    var storageKey = 'weddingAdminBasicAuth';
    var usernameKey = 'weddingAdminUsername';

    function setAuthorizationHeader(encodedCredentials) {
        if (encodedCredentials) {
            $http.defaults.headers.common.Authorization = 'Basic ' + encodedCredentials;
        } else {
            delete $http.defaults.headers.common.Authorization;
        }
    }

    return {
        initialize: function () {
            var saved = window.localStorage.getItem(storageKey);
            setAuthorizationHeader(saved);
            $rootScope.$broadcast('auth:changed', !!saved);
        },
        login: function (username, password) {
            var normalizedUsername = (username || '').trim().toLowerCase();
            var credentialPair = normalizedUsername + ':' + (password || '');
            var encoded = window.btoa(unescape(encodeURIComponent(credentialPair)));
            window.localStorage.setItem(storageKey, encoded);
            window.localStorage.setItem(usernameKey, normalizedUsername);
            setAuthorizationHeader(encoded);
            $rootScope.$broadcast('auth:changed', true);
        },
        logout: function () {
            window.localStorage.removeItem(storageKey);
            window.localStorage.removeItem(usernameKey);
            setAuthorizationHeader(null);
            $rootScope.$broadcast('auth:changed', false);
        },
        isAuthenticated: function () {
            return !!window.localStorage.getItem(storageKey);
        },
        getUsername: function () {
            return window.localStorage.getItem(usernameKey) || '';
        }
    };
}]);
