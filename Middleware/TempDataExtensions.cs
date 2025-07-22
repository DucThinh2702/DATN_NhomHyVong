using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;

namespace DATN.Middleware
{
    public static class TempDataExtensions
    {
        private const string TempDataKey = "Notification";

        public static void SetNotification(this ITempDataDictionary tempData, string message, string type = "info")
        {
            var model = new NotificationModel { Message = message, Type = type };
            tempData[TempDataKey] = JsonSerializer.Serialize(model);
        }

        public static NotificationModel? GetNotification(this ITempDataDictionary tempData)
        {
            if (tempData.TryGetValue(TempDataKey, out var obj) && obj is string json)
            {
                return JsonSerializer.Deserialize<NotificationModel>(json);
            }
            return null;
        }
    }

}
