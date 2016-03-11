using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Validation;

namespace DatabaseService.Repositories
{
    /// <summary>
    /// Exception: some validations failed.
    /// </summary>
    internal class ValidationFailedException : Exception
    {
        public IEnumerable<DbEntityValidationResult> FailedValidations { get; private set; }

        public ValidationFailedException(IEnumerable<DbEntityValidationResult> failedValidations)
        {
            if(failedValidations == null)
            {
                throw new ArgumentNullException(nameof(failedValidations));
            }

            FailedValidations = failedValidations;
        }
    }
}