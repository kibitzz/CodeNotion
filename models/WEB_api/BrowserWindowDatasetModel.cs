using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.WEB_api
{
   public class BrowserWindowDatasetModel:ModelBase
    {
        public static readonly string url = "url";
        public static readonly string cookiez = "cookiez";
        public static readonly string formName = "formName";
        public static readonly string pocStateBanner = "pocStateBanner";
        public static readonly string editeddataset = "editeddataset";
        [model("FlagModelSpec")]
        [info("тільки якщо цей флаг встановлено , буде оновлена сторінка")]
        public static readonly string needUpdate = "needUpdate";
        
    }
}
