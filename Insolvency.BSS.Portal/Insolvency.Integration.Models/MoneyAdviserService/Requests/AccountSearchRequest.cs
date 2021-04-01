using System;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class AccountSearchRequest : AccountSearchBaseRequest
    {
        public override DateTime GetBirthDate()
        {
            return DateOfBirth;
        }
    }
}
