using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JwtAuthApp.Models
{
    public class User
    {
        [Key]
        public required string id { get; set; }
        public required string email { get; set; }
        public required UserStatus userStatus { get; set; }
        public required string userType { get; set; }
        public string? displayName { get; set; }
        public required string country { get; set; }
        public string? hash { get; set; }
        public string? salt { get; set; }
    }

    public class UpdateUserRequest
    {
        public required string id { get; set; }
        public required string email { get; set; }
        public string? displayname { get; set; }
    }

    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserStatus
    {
        InActive = 0,
        Active = 1,
        Suspended = 2,
        Deleted = 3
    }
}
