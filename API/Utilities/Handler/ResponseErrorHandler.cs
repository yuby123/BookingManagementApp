namespace API.Utilities.Handler;

public class ResponseErrorHandler
{
    public int Code { get; set; }     // Kode status HTTP yang menunjukkan kesalahan.
    public string Status { get; set; } // Status HTTP dalam bentuk string.
    public string Message { get; set; } // Pesan kesalahan yang akan dikirim dalam respons.
    public string? Error { get; set; }  // Detail kesalahan jika ada.
}