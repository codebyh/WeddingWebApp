'use strict';
angular.module('weddingApp')
.controller('usersCtrl', ['$scope', 'usersSvc', function ($scope, usersSvc) {
    $scope.users = [];
    $scope.error = '';
    $scope.success = '';
    $scope.loadingMessage = 'Loading admin profiles...';
    $scope.uiMode = 'list';
    $scope.lookupId = '';
    $scope.lookupResult = null;
    $scope.lookupMissing = false;
    $scope.isEditMode = false;
    $scope.editingUserId = null;

    $scope.formUser = createEmptyUser();

    function createEmptyUser() {
        return {
            displayName: 'Rama Krishna',
            birthDate: '',
            gender: '',
            age: null,
            maritalStatus: '',
            height: '',
            weight: '',
            bloodGroup: '',
            religion: '',
            casteSubCaste: '',
            motherTongue: '',
            timeOfBirth: '',
            placeOfBirth: '',
            starNakshatra: '',
            rashi: '',
            gothram: '',
            manglik: '',
            horoscopeCopyAttached: '',
            highestQualification: '',
            occupation: '',
            companyName: '',
            jobLocation: '',
            annualIncome: '',
            fatherName: '',
            fatherOccupation: '',
            motherName: '',
            motherOccupation: '',
            brothers: null,
            sisters: null,
            familyStatus: '',
            mobileNumber: '',
            alternateNumber: '',
            email: 'rama.krishna@example.com',
            address: '',
            preferredAgeMin: null,
            preferredAgeMax: null,
            preferredHeight: '',
            educationPreference: '',
            locationPreference: '',
            otherExpectations: '',
            declarationAccepted: false,
            signature: '',
            declarationDate: ''
        };
    }

    function normalizeOptionalString(value) {
        if (value === undefined || value === null) {
            return null;
        }

        var text = String(value).trim();
        return text ? text : null;
    }

    function normalizeOptionalInt(value) {
        if (value === undefined || value === null || value === '') {
            return null;
        }

        var number = parseInt(value, 10);
        return isNaN(number) ? null : number;
    }

    function normalizeOptionalBool(value) {
        if (value === undefined || value === null || value === '') {
            return null;
        }

        if (value === true || value === 'true') {
            return true;
        }

        if (value === false || value === 'false') {
            return false;
        }

        return null;
    }

    function normalizeDate(value) {
        if (!value) {
            return null;
        }

        if (Object.prototype.toString.call(value) === '[object Date]') {
            return value.toISOString().slice(0, 10);
        }

        return value;
    }

    function normalizeUser(model) {
        return {
            displayName: (model.displayName || '').trim(),
            birthDate: normalizeDate(model.birthDate),
            gender: normalizeOptionalString(model.gender),
            age: normalizeOptionalInt(model.age),
            maritalStatus: normalizeOptionalString(model.maritalStatus),
            height: normalizeOptionalString(model.height),
            weight: normalizeOptionalString(model.weight),
            bloodGroup: normalizeOptionalString(model.bloodGroup),
            religion: normalizeOptionalString(model.religion),
            casteSubCaste: normalizeOptionalString(model.casteSubCaste),
            motherTongue: normalizeOptionalString(model.motherTongue),
            timeOfBirth: normalizeOptionalString(model.timeOfBirth),
            placeOfBirth: normalizeOptionalString(model.placeOfBirth),
            starNakshatra: normalizeOptionalString(model.starNakshatra),
            rashi: normalizeOptionalString(model.rashi),
            gothram: normalizeOptionalString(model.gothram),
            manglik: normalizeOptionalBool(model.manglik),
            horoscopeCopyAttached: normalizeOptionalBool(model.horoscopeCopyAttached),
            highestQualification: normalizeOptionalString(model.highestQualification),
            occupation: normalizeOptionalString(model.occupation),
            companyName: normalizeOptionalString(model.companyName),
            jobLocation: normalizeOptionalString(model.jobLocation),
            annualIncome: normalizeOptionalString(model.annualIncome),
            fatherName: normalizeOptionalString(model.fatherName),
            fatherOccupation: normalizeOptionalString(model.fatherOccupation),
            motherName: normalizeOptionalString(model.motherName),
            motherOccupation: normalizeOptionalString(model.motherOccupation),
            brothers: normalizeOptionalInt(model.brothers),
            sisters: normalizeOptionalInt(model.sisters),
            familyStatus: normalizeOptionalString(model.familyStatus),
            mobileNumber: normalizeOptionalString(model.mobileNumber),
            alternateNumber: normalizeOptionalString(model.alternateNumber),
            email: normalizeOptionalString(model.email),
            address: normalizeOptionalString(model.address),
            preferredAgeMin: normalizeOptionalInt(model.preferredAgeMin),
            preferredAgeMax: normalizeOptionalInt(model.preferredAgeMax),
            preferredHeight: normalizeOptionalString(model.preferredHeight),
            educationPreference: normalizeOptionalString(model.educationPreference),
            locationPreference: normalizeOptionalString(model.locationPreference),
            otherExpectations: normalizeOptionalString(model.otherExpectations),
            declarationAccepted: model.declarationAccepted === true,
            signature: normalizeOptionalString(model.signature),
            declarationDate: normalizeDate(model.declarationDate)
        };
    }

    function extractErrorMessage(err, fallbackText) {
        if (!err) {
            return fallbackText;
        }

        if (typeof err === 'string' && err.trim()) {
            return err;
        }

        if (err.error) {
            if (typeof err.error === 'string' && err.error.trim()) {
                return err.error;
            }

            if (err.error.message) {
                return err.error.message;
            }
        }

        if (err.message) {
            return err.message;
        }

        return fallbackText;
    }

    function mapUserToForm(user) {
        var form = createEmptyUser();
        form.displayName = user.displayName || '';
        form.birthDate = user.birthDate || '';
        form.gender = user.gender || '';
        form.age = user.age;
        form.maritalStatus = user.maritalStatus || '';
        form.height = user.height || '';
        form.weight = user.weight || '';
        form.bloodGroup = user.bloodGroup || '';
        form.religion = user.religion || '';
        form.casteSubCaste = user.casteSubCaste || '';
        form.motherTongue = user.motherTongue || '';
        form.timeOfBirth = user.timeOfBirth || '';
        form.placeOfBirth = user.placeOfBirth || '';
        form.starNakshatra = user.starNakshatra || '';
        form.rashi = user.rashi || '';
        form.gothram = user.gothram || '';
        form.manglik = user.manglik === true ? true : (user.manglik === false ? false : '');
        form.horoscopeCopyAttached = user.horoscopeCopyAttached === true ? true : (user.horoscopeCopyAttached === false ? false : '');
        form.highestQualification = user.highestQualification || '';
        form.occupation = user.occupation || '';
        form.companyName = user.companyName || '';
        form.jobLocation = user.jobLocation || '';
        form.annualIncome = user.annualIncome || '';
        form.fatherName = user.fatherName || '';
        form.fatherOccupation = user.fatherOccupation || '';
        form.motherName = user.motherName || '';
        form.motherOccupation = user.motherOccupation || '';
        form.brothers = user.brothers;
        form.sisters = user.sisters;
        form.familyStatus = user.familyStatus || '';
        form.mobileNumber = user.mobileNumber || '';
        form.alternateNumber = user.alternateNumber || '';
        form.email = user.email || '';
        form.address = user.address || '';
        form.preferredAgeMin = user.preferredAgeMin;
        form.preferredAgeMax = user.preferredAgeMax;
        form.preferredHeight = user.preferredHeight || '';
        form.educationPreference = user.educationPreference || '';
        form.locationPreference = user.locationPreference || '';
        form.otherExpectations = user.otherExpectations || '';
        form.declarationAccepted = user.declarationAccepted === true;
        form.signature = user.signature || '';
        form.declarationDate = user.declarationDate || '';
        return form;
    }

    function resetFormToCreate() {
        $scope.isEditMode = false;
        $scope.editingUserId = null;
        $scope.formUser = createEmptyUser();
    }

    $scope.showList = function () {
        $scope.uiMode = 'list';
    };

    $scope.openCreate = function () {
        resetFormToCreate();
        $scope.uiMode = 'form';
        $scope.error = '';
        $scope.success = '';
    };

    $scope.showLookup = function () {
        $scope.uiMode = 'lookup';
        $scope.error = '';
        $scope.success = '';
    };

    $scope.openLookupWithId = function (id) {
        $scope.lookupId = id;
        $scope.uiMode = 'lookup';
        $scope.findById();
    };

    $scope.downloadPdf = function (id) {
        if (!id) {
            return;
        }

        window.open(usersSvc.pdfUrl(id), '_blank');
    };

    $scope.populate = function () {
        $scope.error = '';
        $scope.loadingMessage = 'Loading admin profiles...';
        usersSvc.getAll().then(function (response) {
            $scope.users = response.data || [];
            $scope.loadingMessage = '';
        }).catch(function (response) {
            $scope.loadingMessage = '';
            $scope.error = extractErrorMessage(response && response.data, 'Unable to load admin profiles.');
        });
    };

    $scope.submitForm = function () {
        var payload = normalizeUser($scope.formUser);
        if (!payload.displayName) {
            $scope.error = 'Full name is required.';
            $scope.success = '';
            return;
        }

        if ($scope.isEditMode && $scope.editingUserId) {
            usersSvc.update($scope.editingUserId, payload).then(function () {
                $scope.error = '';
                $scope.success = 'Profile updated.';
                resetFormToCreate();
                $scope.uiMode = 'list';
                $scope.populate();
            }).catch(function (response) {
                $scope.success = '';
                $scope.error = extractErrorMessage(response && response.data, 'Unable to update profile.');
            });
            return;
        }

        usersSvc.create(payload).then(function () {
            $scope.error = '';
            $scope.success = 'Profile created.';
            resetFormToCreate();
            $scope.uiMode = 'list';
            $scope.populate();
        }).catch(function (response) {
            $scope.success = '';
            $scope.error = extractErrorMessage(response && response.data, 'Unable to create profile.');
        });
    };

    $scope.startEdit = function (user) {
        if (!user || !user.id) {
            return;
        }

        $scope.error = '';
        $scope.success = '';
        usersSvc.getById(user.id).then(function (response) {
            var fullUser = response.data;
            $scope.isEditMode = true;
            $scope.editingUserId = user.id;
            $scope.formUser = mapUserToForm(fullUser);
            $scope.uiMode = 'form';
        }).catch(function (response) {
            $scope.error = extractErrorMessage(response && response.data, 'Unable to load profile for edit.');
        });
    };

    $scope.cancelEdit = function () {
        resetFormToCreate();
        $scope.uiMode = 'list';
    };

    $scope.delete = function (id) {
        usersSvc.remove(id).then(function () {
            $scope.error = '';
            $scope.success = 'Profile deleted.';
            if ($scope.lookupResult && $scope.lookupId === id) {
                $scope.lookupResult = null;
                $scope.lookupMissing = true;
            }
            if ($scope.editingUserId === id) {
                resetFormToCreate();
            }
            $scope.populate();
        }).catch(function (response) {
            $scope.success = '';
            $scope.error = extractErrorMessage(response && response.data, 'Unable to delete profile.');
        });
    };

    $scope.findById = function () {
        if (!$scope.lookupId) {
            return;
        }

        $scope.lookupMissing = false;
        $scope.lookupResult = null;
        $scope.error = '';

        usersSvc.getById($scope.lookupId).then(function (response) {
            $scope.lookupResult = response.data;
        }).catch(function (response) {
            var status = response ? response.status : 0;
            if (status === 404) {
                $scope.lookupMissing = true;
                return;
            }
            $scope.error = extractErrorMessage(response && response.data, 'Unable to fetch public profile.');
        });
    };
}]);
