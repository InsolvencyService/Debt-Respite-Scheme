namespace Insolvency.Notifications.BSSRegisterStatusUpdate
{
    public class TokenResponse
    {
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public int ext_expires_in { get; set; }
        public int expires_on { get; set; }
        public int not_before { get; set; }
        public string resource { get; set; }
        public string access_token { get; set; }
    }
}
