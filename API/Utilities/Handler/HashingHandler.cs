using BCrypt.Net;

namespace API.Utilities.Handler;
// Kelas HashingHandler digunakan untuk mengelola fungsi-fungsi pengamanan kata sandi menggunakan BCrypt.
public class HashingHandler
{
    // Metode GetRandomSalt digunakan untuk menghasilkan salt acak.
    private static string GetRandomSalt()
    {
        return BCrypt.Net.BCrypt.GenerateSalt(12); // 12 adalah panjang salt yang dihasilkan.
    }

    // Metode HashPassword digunakan untuk menghash sebuah kata sandi dengan salt acak.
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, GetRandomSalt());
    }

    // Metode VerifyPassword digunakan untuk memverifikasi apakah kata sandi cocok dengan hash yang ada.
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}



