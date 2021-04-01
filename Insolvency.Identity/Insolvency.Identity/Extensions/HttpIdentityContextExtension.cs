using Insolvency.Models.Audit;
using Microsoft.AspNetCore.Http;

namespace Insolvency.Identity.Extensions
{
    public static class HttpIdentityContextExtension
    {
        public const string AuditItemName = "IdentityAuditDetail";

        public static void SetAuditDetail(this HttpContext httpContext, AuditDetail auditDetail)
        {
            httpContext.Items.Add(AuditItemName, auditDetail);
        }

        public static AuditDetail GetIdentityAuditDetail(this HttpContext httpContext)
        {
            return httpContext.Items[AuditItemName] as AuditDetail;
        }
    }
}
