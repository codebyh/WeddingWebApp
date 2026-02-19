'use strict';
angular.module('weddingApp')
.controller('usersCtrl', ['$scope', 'usersSvc', function ($scope, usersSvc) {
    $scope.users = [];
    $scope.error = '';
    $scope.success = '';
    $scope.loadingMessage = 'Loading profiles...';
    $scope.lookupId = '';
    $scope.lookupResult = null;
    $scope.lookupMissing = false;
    $scope.editingUserId = null;
    $scope.newUser = {
        displayName: 'Rama Krishna',
        birthDate: '',
        gender: '',
        email: 'rama.krishna@example.com'
    };
    $scope.editUser = {
        displayName: '',
        birthDate: '',
        gender: '',
        email: ''
    };

    function normalizeUser(model) {
        return {
            displayName: (model.displayName || '').trim(),
            birthDate: model.birthDate || null,
            gender: model.gender || null,
            email: model.email || null
        };
    }

    $scope.populate = function () {
        $scope.error = '';
        $scope.loadingMessage = 'Loading profiles...';
        usersSvc.getAll().success(function (results) {
            $scope.users = results || [];
            $scope.loadingMessage = '';
        }).error(function () {
            $scope.loadingMessage = '';
            $scope.error = 'Unable to load profiles.';
        });
    };

    $scope.add = function () {
        var payload = normalizeUser($scope.newUser);
        if (!payload.displayName) {
            $scope.error = 'Display name is required.';
            $scope.success = '';
            return;
        }

        usersSvc.create(payload).success(function () {
            $scope.newUser = {
                displayName: 'Rama Krishna',
                birthDate: '',
                gender: '',
                email: 'rama.krishna@example.com'
            };
            $scope.error = '';
            $scope.success = 'Profile created.';
            $scope.populate();
        }).error(function () {
            $scope.success = '';
            $scope.error = 'Unable to create profile.';
        });
    };

    $scope.startEdit = function (user) {
        $scope.editingUserId = user.id;
        $scope.editUser = {
            displayName: user.displayName || '',
            birthDate: user.birthDate || '',
            gender: user.gender || '',
            email: user.email || ''
        };
        $scope.error = '';
        $scope.success = '';
    };

    $scope.cancelEdit = function () {
        $scope.editingUserId = null;
    };

    $scope.saveEdit = function (id) {
        var payload = normalizeUser($scope.editUser);
        if (!payload.displayName) {
            $scope.error = 'Display name is required.';
            $scope.success = '';
            return;
        }

        usersSvc.update(id, payload).success(function () {
            $scope.editingUserId = null;
            $scope.error = '';
            $scope.success = 'Profile updated.';
            $scope.populate();
        }).error(function () {
            $scope.success = '';
            $scope.error = 'Unable to update profile.';
        });
    };

    $scope.delete = function (id) {
        usersSvc.remove(id).success(function () {
            $scope.error = '';
            $scope.success = 'Profile deleted.';
            if ($scope.lookupResult && $scope.lookupResult.id === id) {
                $scope.lookupResult = null;
            }
            $scope.populate();
        }).error(function () {
            $scope.success = '';
            $scope.error = 'Unable to delete profile.';
        });
    };

    $scope.findById = function () {
        if (!$scope.lookupId) {
            return;
        }

        $scope.lookupMissing = false;
        $scope.lookupResult = null;
        $scope.error = '';

        usersSvc.getById($scope.lookupId).success(function (user) {
            $scope.lookupResult = user;
        }).error(function (err, status) {
            if (status === 404) {
                $scope.lookupMissing = true;
                return;
            }
            $scope.error = 'Unable to fetch profile.';
        });
    };
}]);

