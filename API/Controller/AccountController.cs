using API.Contracts;
using API.DTOs.Accounts;
using API.Models;
using API.Utilities.Handler;
using API.Utilities.Handlers;
using Microsoft.AspNetCore.Mvc;
using API.DTOs.Accounts;
using System.Net;
using System.Security.Principal;
using System.Transactions;

namespace API.Controllers;

[ApiController]
[Route("server/[controller]")]
public class AccountController : ControllerBase
{
    // Deklarasi variabel untuk repository dan handler
    private readonly IAccountRepository _accountRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEducationRepository _educationRepository;
    private readonly IUniversityRepository _universityRepository;
    private readonly IEmailHandler _emailHandler;

    // Konstruktor untuk inject dependency
    public AccountController(IAccountRepository accountRepository, IEmployeeRepository employeeRepository,
        IEducationRepository educationRepository, IUniversityRepository universityRepository,
        IEmailHandler emailHandler)
    {
        _accountRepository = accountRepository;
        _employeeRepository = employeeRepository;
        _educationRepository = educationRepository;
        _universityRepository = universityRepository;
        _emailHandler = emailHandler;
    }

    // Endpoint untuk fitur lupa password
    [HttpPut("ForgetPassword")]
    public IActionResult ForgetPassword(string email)
    {
        // Mendapatkan semua data employee dan akun
        var employee = _employeeRepository.GetAll();
        var account = _accountRepository.GetAll();

        // Cek jika tidak ada data employee atau akun
        if (!(employee.Any() && account.Any()))
        {
            return NotFound(new ResponseErrorHandler
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Data Not Found"
            });
        }

        // Mendapatkan data berdasarkan email
        var data = _employeeRepository.GetEmail(email);

        // Update data akun dengan OTP baru
        var toUpdate = (Account)_accountRepository.GetByGuid(data.Guid);
        Random random = new Random();
        toUpdate.Otp = random.Next(100000, 999999);
        toUpdate.ExpiredTime = DateTime.Now.AddMinutes(5);
        toUpdate.IsUsed = false;
        _accountRepository.Update(toUpdate);

        // Kirim OTP melalui email
        _emailHandler.Send("Forgot Password", $"Your OTP is {toUpdate.Otp}", email);

        // Dapatkan semua data akun untuk response
        account = _accountRepository.GetAll();
        var result = from acc in account
                     join emp in employee on acc.Guid equals emp.Guid
                     where emp.Email == email
                     select new ForgetPasswordDto
                     {
                         Email = emp.Email,
                         Otp = acc.Otp,
                         ExpiredDate = acc.ExpiredTime
                     };
        return Ok(new ResponseOKHandler<IEnumerable<ForgetPasswordDto>>(result));
    }

    // Endpoint untuk mengubah password
    [HttpPut("ChangePassword")]
    public IActionResult ChangePassword(ChangePasswordDto changePasswordDto)
    {
        try
        {
            var data = _employeeRepository.GetEmail(changePasswordDto.Email);
            var accounts = _accountRepository.GetByGuid(data.Guid);
            
            //validasi cek akun
            if (accounts == null)
            {
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Account Not Found"
                });
            }

            // Validasi input pengguna
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
                return Ok(new ResponseOKHandler<string>("Password Not Match"));

            if (!accounts.Otp.Equals(changePasswordDto.Otp))
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Otp Is Incorrect"
                });

            // Validasi status OTP
            if (accounts.IsUsed)
                return BadRequest(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = HttpStatusCode.BadRequest.ToString(),
                    Message = "Otp Has Been Used"
                });

            if (DateTime.Now > accounts.ExpiredTime)
                return BadRequest(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = HttpStatusCode.BadRequest.ToString(),
                    Message = "Otp Was Expired"
                });

            // Update data akun dengan password baru
            accounts.IsUsed = true;
            accounts.Password = HashingHandler.HashPassword(changePasswordDto.ConfirmPassword);
            _accountRepository.Update(accounts);

            return Ok(new ResponseOKHandler<string>("Success Change Password"));
        }
        catch (Exception ex)
        {
            // Mengembalikan response jika terjadi error
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
            {
                Code = StatusCodes.Status500InternalServerError,
                Status = HttpStatusCode.InternalServerError.ToString(),
                Message = "Failed to Update Password",
                Error = ex.Message
            });
        }
    }

    // Endpoint untuk registrasi akun baru
    [HttpPost("Register")]
    public IActionResult Register(RegistrationDto registrationDto)
    {
        using (var transaction = new TransactionScope()) //mengelola transaction dg using (clear after used)
        {
            try
            {
                Employee toCreateEmp = registrationDto; //convert data DTO dari inputan user menjadi objek Employee
                toCreateEmp.Nik = GenerateHandler.Nik(_employeeRepository.GetLastNik()); //set nik dg generate nik
                var resultEmp = _employeeRepository.Create(toCreateEmp); //create data account menggunakan format data DTO implisit

                //cek apakah nama univ dan code nya sudah ada di DB
                var univFindResult = _universityRepository.GetCodeName(registrationDto.UniversityCode, registrationDto.UniversityName);
                if (univFindResult is null)
                {
                    //jika tidak ada maka membuat data baru
                    univFindResult = _universityRepository.Create(registrationDto);
                }

                Education toCreateEdu = registrationDto;
                toCreateEdu.Guid = resultEmp.Guid; //set Guid Education dengan Guid yang ada pada employee
                toCreateEdu.UniversityGuid = univFindResult.Guid;
                var resultedu = _educationRepository.Create(toCreateEdu);

                //cek apakah password tidak sama dengan confirm password
                if (registrationDto.Password != registrationDto.ConfirmPassword)
                {
                    return BadRequest(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Status = HttpStatusCode.BadRequest.ToString(),
                        Message = "NewPassword and ConfirmPassword do not match"
                    });
                }

                Account toCreateAcc = registrationDto;
                toCreateAcc.Guid = resultEmp.Guid; //set Guid Account dengan Guid yang ada pada employee
                toCreateAcc.Password = HashingHandler.HashPassword(registrationDto.Password);
                _accountRepository.Create(toCreateAcc);


                transaction.Complete(); // Commit transaksi 
                return Ok(new ResponseOKHandler<string>("Registration successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Failed Registration Account",
                    Error = ex.Message
                });
            }
        }
    }

        // Endpoint untuk login
        [HttpGet("Login")]
    public IActionResult AuthenticateUser(string userEmail, string userPassword)
    {
        // Mengambil semua data employee dan akun
        var allEmployees = _employeeRepository.GetAll();
        var allAccounts = _accountRepository.GetAll();

        // Verifikasi apakah ada data untuk employee dan akun
        if (!(allEmployees.Any() && allAccounts.Any()))
        {
            return NotFound(new ResponseErrorHandler
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "No Records Found"
            });
        }

        // Mencocokkan email dan password
        var matchedEmployee = _employeeRepository.GetEmail(userEmail);
        if (matchedEmployee == null)
        {
            return NotFound(new ResponseErrorHandler
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Email Not Recognized"
            });
        }

        // Mengambil akun pengguna berdasarkan GUID dari employee yang cocok
        var userAccount = _accountRepository.GetByGuid(matchedEmployee.Guid);
        // Verifikasi apakah password yang dimasukkan sesuai dengan password pada akun yang bersangkutan
        if (!HashingHandler.VerifyPassword(userPassword, userAccount.Password))
        {
            return NotFound(new ResponseErrorHandler
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Invalid Password"
            });
        }

        return Ok(new ResponseOKHandler<string>("Successfully Authenticated"));
    }



    [HttpGet]
    public IActionResult GetAll()
    {
        // Mengambil semua account dari repository
        var result = _accountRepository.GetAll();
        // Memeriksa jika tidak ada data account
        if (!result.Any())
        {
            // Mengembalikan respon error dengan kode 404 jika tidak ada data
            return NotFound(new ResponseErrorHandler
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Data Not Found"
            });
        }
        // Mengkonversi hasil ke DTO
        var data = result.Select(x => (AccountDto)x);
        // Mengembalikan data account dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<IEnumerable<AccountDto>>(data));
    }

    [HttpGet("{guid}")] //digunakan untuk mendapatkan data Account berdasarkan GUID yang diberikan sebagai parameter.
    public IActionResult GetByGuid(Guid guid)  //Method ini digunakan untuk mendapatkan data Account berdasarkan GUID.
    {
        // Mengambil account berdasarkan GUID dari repository
        var result = _accountRepository.GetByGuid(guid);
        // Jika data account tidak ditemukan
        if (result is null)
        {
            // Mengembalikan respon error dengan kode 404
            return NotFound(new ResponseErrorHandler
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Data Not Found"
            });
        }
        // Mengembalikan data account dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<AccountDto>((AccountDto)result));
    }

    [HttpPost]
    public IActionResult Create(CreateAccountDto createAccountDto)
    {
        try
        {
            //Hashing
            Account toCreate = createAccountDto;
            toCreate.Password = HashingHandler.HashPassword(toCreate.Password);

            //Mapping secara implisit pada createAccountDto untuk dijadikan objek Account
            var result = _accountRepository.Create(toCreate);


            // Mengembalikan data account yang baru dibuat dalam format DTO dengan kode 200
            return Ok(new ResponseOKHandler<AccountDto>((AccountDto)result));
        }
        catch (ExceptionHandler ex)
        {
            // Jika terjadi error saat pembuatan, mengembalikan respon error dengan kode 500
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
            {
                Code = StatusCodes.Status500InternalServerError,
                Status = HttpStatusCode.InternalServerError.ToString(),
                Message = "Failed to create data",
                Error = ex.Message
            });
        }

    }

    // HTTP PUT endpoint untuk memperbarui data Account.
    [HttpPut] //menangani request update ke endpoint /Account
              //parameter berupa objek menggunakan format DTO explicit agar crete data disesuaikan dengan format DTO
    public IActionResult Update(AccountDto accountDto)
    {
     
        try
        {
            //get data by guid dan menggunakan format DTO 
            var entity = _accountRepository.GetByGuid(accountDto.Guid);
            // Jika data account tidak ditemukan
            if (entity is null)
            {
                // Mengembalikan respon error dengan kode 404
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Data Not Found"
                });
            }

            //convert data DTO dari inputan user menjadi objek Account
            Account toUpdate = accountDto;
            //menyimpan createdate yg lama 
            toUpdate.CreatedDate = entity.CreatedDate;
            toUpdate.Password = HashingHandler.HashPassword(accountDto.Password);

            // Memperbarui data account di repository
            _accountRepository.Update(toUpdate);

            // Mengembalikan pesan bahwa data telah diperbarui dengan kode 200
            return Ok(new ResponseOKHandler<string>("Data Updated"));
        }
        catch (ExceptionHandler ex)
        {
            // Jika terjadi error saat pembaruan, mengembalikan respon error dengan kode 500
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
            {
                Code = StatusCodes.Status500InternalServerError,
                Status = HttpStatusCode.InternalServerError.ToString(),
                Message = "Failed to update data",
                Error = ex.Message
            });
        }
    }

    [HttpDelete("{guid}")] //digunakan untuk menghapus data Account berdasarkan GUID.
    public IActionResult Delete(Guid guid)
    {
        try
        {
            // Mengambil data account berdasarkan GUID
            var entity = _accountRepository.GetByGuid(guid);
            // Jika data account tidak ditemukan
            if (entity is null)
            {
                // Mengembalikan respon error dengan kode 404
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Data Not Found"
                });
            }

            // Menghapus data account dari repository
            _accountRepository.Delete(entity);

            // Mengembalikan pesan bahwa data telah dihapus dengan kode 200
            return Ok(new ResponseOKHandler<string>("Data Deleted"));
        }
        catch (ExceptionHandler ex)
        {
            // Jika terjadi error saat penghapusan, mengembalikan respon error dengan kode 500
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
            {
                Code = StatusCodes.Status500InternalServerError,
                Status = HttpStatusCode.InternalServerError.ToString(),
                Message = "Failed to create data",
                Error = ex.Message
            });
        }
    }
}