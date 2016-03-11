angular.module('usersManager', ['ngRoute', 'users.directives', 'users.services'])
       .config(function ($routeProvider)
{
    'use strict';

    var usersList = {
        controller: 'usersController',
        templateUrl: '/'
    };
});
