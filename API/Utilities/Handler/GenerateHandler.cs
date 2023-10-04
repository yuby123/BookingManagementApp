namespace API.Utilities.Handlers;
public static class GenerateHandler
{
    // Metode Nik digunakan untuk menghasilkan nomor NIK berikutnya berdasarkan nomor NIK terakhir.
    public static string Nik(string? lastNik=null)
    {
        // Jika nomor NIK terakhir kosong atau null, maka nomor NIK yang dihasilkan adalah "111111".
        if (lastNik is null)
        {
            return "111111";
        }

        // Mengonversi nomor NIK terakhir ke tipe data long, menambahkan 1, dan mengembalikan hasilnya sebagai string.
        var generateNik = Convert.ToInt32(lastNik) + 1;
        return generateNik.ToString();
    }
}
