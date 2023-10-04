using System.Net;

namespace API.Utilities.Handler
{
    // Kelas ResponseOKHandler digunakan untuk mengelola respon HTTP 200 OK.
    public class ResponseOKHandler<TEntity>
    {
        // Properti-properti berikut ini akan digunakan dalam pembuatan respons HTTP.
        public int Code { get; set; }     // Kode status HTTP.
        public string Status { get; set; } // Status HTTP dalam bentuk string.
        public string Message { get; set; } // Pesan yang akan dikirim dalam respons.
        public TEntity? Data { get; set; }  // Data yang akan dikirim dalam respons.

        // Konstruktor untuk ResponseOKHandler dengan data.
        public ResponseOKHandler(TEntity? data)
        {
            Code = StatusCodes.Status200OK; // Mengatur kode status ke 200 OK.
            Status = HttpStatusCode.OK.ToString(); // Mengatur status HTTP ke "OK".
            Message = "Success to Retrieve Data"; // Mengatur pesan default.
            Data = data; // Mengatur data sesuai dengan yang diberikan.
        }

        // Konstruktor lain untuk ResponseOKHandler dengan pesan khusus.
        public ResponseOKHandler(string message)
        {
            Code = StatusCodes.Status200OK; // Mengatur kode status ke 200 OK.
            Status = HttpStatusCode.OK.ToString(); // Mengatur status HTTP ke "OK".
            Message = message; // Mengatur pesan sesuai dengan yang diberikan.
        }
    }
}
