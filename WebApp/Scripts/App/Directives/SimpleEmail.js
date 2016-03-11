angular.module('users.directives', []).directive('simpleEmail', function ()
{
    'use strict';

    return {
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl)
        {
            ctrl.$validators.simpleEmail = function (modelValue, viewValue) {
                // Emtpy email is valid
                if (ctrl.$isEmpty(modelValue)) {
                    return true;
                }

                // Email should contain @
                if (viewValue.split("@").length == 2) {
                    return true;
                }

                // it is invalid
                return false;
            };
        }
    };
});
