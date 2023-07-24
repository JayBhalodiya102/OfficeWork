namespace EntityFramework.Model
{
    public class DetailsModel
    {
        public int? Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? HashKey { get; set; }

        public string? Address { get; set; }

        public long? Phoneno { get; set; }
        public int? OTP { get; set; }
    }
}
