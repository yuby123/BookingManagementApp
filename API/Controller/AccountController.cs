using API.Contracts;
using API.DTOs.Accounts;
using API.Models;
using API.Utilities.Handler;
using API.Utilities.Handlers;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs.Accounts;
using System.Net;
using System.Transactions;

namespace API.Controllers;

[ApiController]
[Route("server/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEducationRepository _educationRepository;
    private readonly IUniversityRepository _universityRepository;
    private readonly IEmailHandler _emailHandler;
    public AccountController(IAccountRepository accountRepository, IEmployeeRepository employeeRepository, IEducationRepository educationRepository, IUniversityRepository universityRepository, IEmailHandler emailHandler)
    {
        _accountRepository = accountRepository;
        _employeeRepository = employeeRepository;
        _educationRepository = educationRepository;
        _universityRepository = universityRepository;
        _emailHandler = emailHandler;
    }

    /*
     * Pada class Controller memiliki function untuk get all data 
     * yang ada dengan melakukan penarikan data berdasarkan atribut yang ada pada calss DTO dengan operator Explicit.
     */

    [HttpPut("ForgetPassword")]
    public IActionResult ForgetPassword(string email)
    {
        var employee = _employeeRepository.GetAll();
        var account = _accountRepository.GetAll();
        if (!(employee.Any() && account.Any()))
        {
            return NotFound(new ResponseErrorHandler
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Data Not Found"
            });
        }
        var data = _employeeRepository.GetEmail(email);
        var toUpdate = (Account)_accountRepository.GetByGuid(data.Guid);


        Random random = new Random();
        toUpdate.Otp = random.Next(100000, 999999);
        toUpdate.ExpiredTime = DateTime.Now.AddMinutes(5);
        toUpdate.IsUsed = false;
        var results = _accountRepository.Update(toUpdate);

        _emailHandler.Send("Forgot Password", $"Your OTP is {toUpdate.Otp}", email);
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

    [HttpPut("ChangePassword")]
    public IActionResult ChangePassword(ChangePasswordDto changePasswordDto)
    {
        try
        {
           
            var data = _employeeRepository.GetEmail(changePasswordDto.Email);
            var accounts = _accountRepository.GetByGuid(data.Guid);

            if (data == null)
            {
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Account Not Found"
                });
            }

            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
                return Ok(new ResponseOKHandler<string>("Password Not Match"));

            if (!accounts.Otp.Equals(changePasswordDto.Otp))
                return NotFound(new ResponseErrorHandler 
                { 
                    Code = StatusCodes.Status404NotFound, 
                    Status = HttpStatusCode.NotFound.ToString(), 
                    Message = "Otp Is Incorrect" 
                });

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

            accounts.IsUsed = true;
            accounts.Password = HashingHandler.HashPassword(changePasswordDto.ConfirmPassword);
            _accountRepository.Update(accounts);

            return Ok(new ResponseOKHandler<string>("Success Change Password"));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
            {
                Code = StatusCodes.Status500InternalServerError,
                Status = HttpStatusCode.InternalServerError.ToString(),
                Message = "Failed to Update Password",
                Error = ex.Message
            });
        }
    }


    [HttpPost("Registration")]
    public IActionResult Registration(RegistrationDto registrationDto)
    {
        try
        {
            using (var transaction = new TransactionScope())
            {
                Employee toCreateEmployee = registrationDto;
                toCreateEmployee.Nik = GenerateHandler.Nik(_employeeRepository.GetLastNik());
                var resultEmp = _employeeRepository.Create(toCreateEmployee);

                var univercity = _universityRepository.GetByCodeAndName(registrationDto.UnivercityCode, registrationDto.UnivercityName);
                if (univercity is null)
                {
                    univercity = _universityRepository.Create(registrationDto);
                }

                Education createEdu = registrationDto;
                createEdu.Guid = resultEmp.Guid;
                createEdu.UniversityGuid = univercity.Guid;
                var resultedu = _educationRepository.Create(createEdu);

                if (registrationDto.NewPassword != registrationDto.ConfirmPassword)
                {
                    return Ok(new ResponseOKHandler<string>("Password Not Match"));
                }

                Account toCreateAcc = registrationDto;
                toCreateAcc.Guid = resultEmp.Guid;
                toCreateAcc.Password = HashingHandler.HashPassword(registrationDto.ConfirmPassword);
                var resultAcc = _accountRepository.Create(toCreateAcc);

                transaction.Complete();


                return Ok(new ResponseOKHandler<string>("Account Success To Registed"));
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
            {
                Code = StatusCodes.Status500InternalServerError,
                Status = HttpStatusCode.InternalServerError.ToString(),
                Message = "Failed to Registration data",
                Error = ex.Message
            });
        }
    }
    [HttpGet("Login")]
    public IActionResult Login(string Email, string Password)
    {
        var employee = _employeeRepository.GetAll();
        var account = _accountRepository.GetAll();
        if (!(employee.Any() && account.Any()))
        {
            return NotFound(new ResponseErrorHandler
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Data Not Found"
            });
        }
        var data = _employeeRepository.GetEmail(Email);
        if (data is null)
        {
            return NotFound(new ResponseErrorHandler
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Email Not Registerd"
            });
        }
        var accounts = _accountRepository.GetByGuid(data.Guid);
        if (!HashingHandler.VerifyPassword(Password, accounts.Password))
        {
            return NotFound(new ResponseErrorHandler
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Wrong Password"
            });
        }
        return Ok(new ResponseOKHandler<string>("Login Success"));
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