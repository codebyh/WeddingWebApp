'use strict';
angular.module('weddingApp')
.factory('usersSvc', ['$http', function ($http) {
    $http.defaults.useXDomain = true;
    delete $http.defaults.headers.common['X-Requested-With'];

    return {
        getAll: function () {
            return $http.get(apiEndpoint + '/api/users');
        },
        getById: function (id) {
            return $http.get(apiEndpoint + '/api/users/' + id);
        },
        create: function (user) {
            return $http.post(apiEndpoint + '/api/users', user);
        },
        update: function (id, user) {
            return $http.put(apiEndpoint + '/api/users/' + id, user);
        },
        remove: function (id) {
            return $http({
                method: 'DELETE',
                url: apiEndpoint + '/api/users/' + id
            });
        }
    };
}]);
