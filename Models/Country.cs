namespace JwtAuthApp.Models
{
    public class Country
    {
        public required string id { get; set; }
        public required string name { get; set; }
        public required string officialName { get; set; }
        public required string sovereignty { get; set; }
        public required string a2 { get; set; }
        public required string a3 { get; set; }
        public required int number { get; set; }
        public required string tld { get; set; }
    }
}
