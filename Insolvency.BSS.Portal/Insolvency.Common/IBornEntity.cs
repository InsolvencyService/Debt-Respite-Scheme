using System;

namespace Insolvency.Common
{
    public interface IBornEntity
    {
        DateTime GetBirthDate();        
        bool IsValidDateOfBirth { get; set; }
    }
}
