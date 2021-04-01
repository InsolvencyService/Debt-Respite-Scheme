using System;
using System.Collections.Generic;

namespace Insolvency.Common.Attributes
{
    public interface IMultiConditionalRequiredValidation
    {
        Dictionary<string, Func<bool>> Actions { get; }
    }
}
