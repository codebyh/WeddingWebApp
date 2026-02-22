'use strict';
angular.module('weddingApp')
.factory('usersSvc', ['$http', function ($http) {
    $http.defaults.useXDomain = true;
    delete $http.defaults.headers.common['X-Requested-With'];

    return {
        getAll: function () {
            return $http.get(apiEndpoint + '/api/admin/users');
        },
        getById: function (id) {
            return $http.get(apiEndpoint + '/api/admin/users/' + id);
        },
        create: function (user) {
            return $http.post(apiEndpoint + '/api/admin/users', user);
        },
        update: function (id, user) {
            return $http.put(apiEndpoint + '/api/admin/users/' + id, user);
        },
        remove: function (id) {
            return $http({
                method: 'DELETE',
                url: apiEndpoint + '/api/admin/users/' + id
            });
        }
    };
}]);
