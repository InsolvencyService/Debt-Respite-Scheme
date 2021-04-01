using System;
using Insolvency.Common.Enums;

namespace Insolvency.Integration.Gateways.Entities
{
    public static class MoratoriumIdStatusMap
    {
        public static readonly Guid Active = new Guid("3c3c12ff-3ec0-ea11-a812-0022480062fb");
        public static readonly Guid Cancelled = new Guid("3da12705-3fc0-ea11-a812-0022480062fb");
        public static readonly Guid Draft = new Guid("5001d9f8-3ec0-ea11-a812-0022480062fb");
        public static readonly Guid Expired = new Guid("4aa12705-3fc0-ea11-a812-0022480062fb");

        public static MoratoriumStatus GetStatusFromId(Guid? id) => id switch
        {
            null => MoratoriumStatus.None,
            _ when id == Active => MoratoriumStatus.Active,
            _ when id == Cancelled => MoratoriumStatus.Cancelled,
            _ when id == Draft => MoratoriumStatus.Draft,
            _ when id == Expired => MoratoriumStatus.Expired,
            _ => MoratoriumStatus.None
        };
    }
}
