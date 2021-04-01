namespace Insolvency.Models.OperationResults
{
    public static class OperationErrors
    {
        public static OperationError DuplicateOrganisation
            => new OperationError { Message = "Organisation already exists!", Code = 409 };

        public static OperationError DuplicateClientId
            => new OperationError { Message = "Client with this ClientId already exists!", Code = 409 };

        public static OperationError DuplicateEmailAndRole
            => new OperationError { Message = "Email already exists!", Code = 409 };

        public static OperationError EmailAlreadyInUse
            => new OperationError { Message = "Email already in use!", Code = 409 };

        public static OperationError ClientSecretCountLimitReached
            => new OperationError { Message = "The maximum number of secrets for this client has been reached!", Code = 400 };

        public static OperationError Unauthorised
            => new OperationError { Message = "Unauthorised to do this action!", Code = 401 };
    }
}
