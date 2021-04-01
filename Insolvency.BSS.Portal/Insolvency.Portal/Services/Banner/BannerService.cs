using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Insolvency.Portal.Services.Banner
{
    public class BannerService
    {     
        private readonly ITempDataDictionary _tempData;

        public BannerService(IHttpContextAccessor httpContextAccessor, ITempDataDictionaryFactory tempDataDictionaryFactory)
        {
            _tempData = tempDataDictionaryFactory.GetTempData(httpContextAccessor.HttpContext);
        }

        public static string BannerKey = "BannerKey";
      

        public void ShowBanner(Banner banner)
        {
            var bannerContents = JsonSerializer.Serialize(banner);

            _tempData.Remove(BannerKey);
            _tempData.Add(BannerKey, bannerContents);
        }

        public Banner GetBanner()
        {
            var bannerContents = (string)_tempData[BannerKey];

            if (string.IsNullOrEmpty(bannerContents))
            {
                return null;
            }

            return JsonSerializer.Deserialize<Banner>(bannerContents);
        }
    }
}
