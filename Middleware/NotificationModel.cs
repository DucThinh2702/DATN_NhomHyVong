namespace DATN.Middleware
{
    public class NotificationModel
    {
        public string Message { get; set; } = "";
        public string Type { get; set; } = "info"; // success, error, warning, info
    }
}
