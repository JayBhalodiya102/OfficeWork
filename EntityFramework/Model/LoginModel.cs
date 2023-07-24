namespace EntityFramework.Model
{
    public class LoginModel
    {
        public int? Id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public DateTime? Time { get; set; }

        // Foreign key property
        public int? DId { get; set; }

        // Navigation property
        public DetailsModel DM { get; set; }

    }
}
