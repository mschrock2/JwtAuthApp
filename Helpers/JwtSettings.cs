using System.Text;

namespace JwtAuthApp.Helpers
{
    public class JwtSettings
    {
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? SecretKey { get; set; }
        private byte[]? _secretKeyBytes;
        public byte[]? SecretKeyBytes
        {
            get
            {
                if (string.IsNullOrEmpty(SecretKey))
                {
                    return null;
                }
                if (_secretKeyBytes == null)
                {
                    _secretKeyBytes = Encoding.ASCII.GetBytes(SecretKey);
                }
                return _secretKeyBytes;

            }
        }
    }
}
