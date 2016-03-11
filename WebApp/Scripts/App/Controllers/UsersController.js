angular.module('usersManager')
    .controller('usersController', ['$scope', '$routeParams', 'usersStore',
    function ($scope, $routeParams, usersStore)
    {
        'use strict';

        var usersPerPage = 20;

        $scope.pagesTotal = 0;
        $scope.usersOnPage = [];
        $scope.pages = [];
        
        $scope.user = {
            Id: null,
            Name: null,
            Surname: null,
            EMail: null
        };

        $scope.currentPage = 0;
        
        // Generate page numbers representation
        var generatePages = function (pagesTotal) {
            var newPages = [];
            for(var i = 0; i < pagesTotal; i++)
            {
                newPages.push({
                    Page: i,
                    Number: i + 1
                });
            }

            angular.copy(newPages, $scope.pages);
        }

        // Determine if the page is available
        $scope.pageAvailable = function(page) {
            return (page >= 0) && (page < $scope.pagesTotal);
        }

        // Get users on a page
        $scope.getUsers = function (page) {
            $scope.currentPage = page;

            var offsetValue = usersPerPage * page;
            var limitValue = usersPerPage;

            usersStore.query({ offset: offsetValue, limit: limitValue }, function (userCollection) {
                $scope.pagesTotal = Math.ceil(userCollection.Count / usersPerPage);

                angular.copy(userCollection.Users, $scope.usersOnPage);

                generatePages($scope.pagesTotal);
            });
        };

        // Save a user
        $scope.save = function (user) {
            var savePromise = usersStore.save(user).$promise;
            savePromise.then(function () {
                $scope.getUsers($scope.currentPage);
            }, function (error) {
                alert(error.data.Error);
            });
        };

        // Edit a user
        $scope.edit = function (user) {
            angular.copy(user, $scope.user);
        };

        // Clear user form
        $scope.clear = function () {
            $scope.user = {
                Id: null,
                Name: null,
                Surname: null,
                EMail: null
            };
        }

        // Delete a user
        $scope.delete = function (user) {
            if (confirm('Are you sure?')) {
                usersStore.delete({ id: user.Id }, function () {
                    $scope.getUsers($scope.currentPage);
                }, function () {
                });
            }
        };

        // Initialize the controller
        var init = function () {
            $scope.getUsers(0);
        };

        init();
}]);
