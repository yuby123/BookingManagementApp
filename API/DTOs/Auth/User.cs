namespace API.DTOs.Auth
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // Dalam produksi, gunakan hash password, jangan simpan password mentah
    }
}
