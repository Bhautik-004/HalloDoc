namespace HalloDocService.ViewModels
{
    public class Profile
    {
        public int Id { get; set; }
        public int? AspNetUserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? PhoneNo { get; set; }
        public string? Email { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
    }
}
