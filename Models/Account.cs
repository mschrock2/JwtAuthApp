namespace JwtAuthApp.Models
{
    public class AccountRequest
    {
        public required string username { get; set; }
        public required string password { get; set; }
    }
    public class AccountCreateResponse
    {
        public required string id { get; set; }
    }
    public class AccountLogonResponse
    {
        public required string token { get; set; }
    }
    public class SetStatusChange
    {
        public required string id { get; set; }
        public required string country { get; set; }
        public required UserStatus status { get; set; }
    }
    public class SpecificUser
    {
        public required string id { get; set; }
        public required string country { get; set; }
    }
}
