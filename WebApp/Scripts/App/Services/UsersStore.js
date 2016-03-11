angular.module('users.services', ['ngResource']).factory('usersStore', ['$resource', function ($resource)
{
    'use strict';

    // Get CSRF token
    var csrf = function () {
        var csrfElement = document.getElementById("csrfHost")
                       .getElementsByTagName("input")[0];

        var name = csrfElement.attributes["name"].value;
        var value = csrfElement.attributes["value"].value;

        var csrfParams = {};
        csrfParams[name] = value;

        return csrfParams;
    };

    // Cache disable routine
    var noCache = function () {
        var noCacheHeaders = {};
        noCacheHeaders['If-Modified-Since'] = 'Mon, 26 Jul 1997 05:00:00 GMT';
        noCacheHeaders['Cache-Control'] = 'no-cache';
        noCacheHeaders['Pragma'] = 'no-cache';

        return noCacheHeaders;
    };

    var usersApi = 'Api.svc/users/:id'; // (hosted in IIS)
    // var usersApi = 'http://localhost/UsersApi/users/:id'; // (hosted as service)

    var usersApi = $resource(usersApi, null,
        {
            'update': { method: 'PUT', headers: csrf() },
            'delete': { method: 'DELETE', headers: csrf() },
            'create':   { method: 'POST', headers: csrf() },
            'query':  { isArray: false, headers: noCache() }
        }
    );

    usersApi.save = function (user) {
        if (user.Id) {
            return usersApi.update({ id: user.Id}, user);
        } else {
            return usersApi.create(user);
        }
    };

    return usersApi;
}]);