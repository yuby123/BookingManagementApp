using API.Contracts;
using API.DTOs.Accounts;
using API.Models;
using API.Utilities.Handler;
using API.Utilities.Handlers;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Principal;
using System.Transactions;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace API.Controllers;

[ApiController]
[Route("API/[controller]")]
[Authorize]
public class AccountController : ControllerBase
{
    // Deklarasi variabel untuk repository dan handler
    private readonly IAccountRepository _accountRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEducationRepository _educationRepository;
    private readonly IUniversityRepository _universityRepository;
    private readonly IEmailHandler _emailHandler;
    private readonly ITokenHandler _tokenHandler;
    private readonly IAccountRoleRepository _accountRoleRepository;
    private readonly IRoleRepository _roleRepository;

    // Konstruktor untuk inject dependency
    public AccountController(IAccountRepository accountRepository, IEmployeeRepository employeeRepository,
        IEducationRepository educationRepository, IUniversityRepository universityRepository,
        IEmailHandler emailHandler, ITokenHandler tokenHandler, IRoleRepository roleRepository, IAccountRoleRepository accountRoleRepository)
    {
        _accountRepository = accountRepository;
        _employeeRepository = employeeRepository;
        _educationRepository = educationRepository;
        _universityRepository = universityRepository;
        _emailHandler = emailHandler;
        _tokenHandler = tokenHandler;
        _roleRepository = roleRepository;
        _accountRoleRepository = accountRoleRepository;
    }

    // Endpoint untuk fitur lupa password
    [HttpPut("ForgetPassword")]
    [AllowAnonymous]
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
    [AllowAnonymous]
    public IActionResult Register(RegistrationDto registrationDto)
    {
        // Memulai transaksi dengan TransactionScope
        using (var transaction = new TransactionScope())
        {
            try
            {
                // Mengonversi DTO menjadi objek Employee untuk persiapan penyimpanan
                Employee toCreateEmp = registrationDto;
                toCreateEmp.Nik = GenerateHandler.Nik(_employeeRepository.GetLastNik());

                // Menyimpan data Employee ke database dan mendapatkan hasilnya
                _employeeRepository.Create(toCreateEmp);

                // Mencari universitas berdasarkan kode dan nama dari inputan
                var univFindResult = _universityRepository.GetCodeName(registrationDto.UniversityCode, registrationDto.UniversityName);
                if (univFindResult is null)
                {
                    // Jika universitas tidak ditemukan, buat entri baru di database
                    univFindResult = _universityRepository.Create(registrationDto);
                }

                // Mengonversi DTO menjadi objek Education dan menyiapkan untuk penyimpanan
                Education toCreateEdu = registrationDto;
                toCreateEdu.Guid = toCreateEmp.Guid;
                toCreateEdu.UniversityGuid = univFindResult.Guid;
                _educationRepository.Create(toCreateEdu);

                // Memastikan kata sandi dan konfirmasinya cocok
                if (registrationDto.Password != registrationDto.ConfirmPassword)
                {
                    return BadRequest(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Status = HttpStatusCode.BadRequest.ToString(),
                        Message = "NewPassword and ConfirmPassword do not match"
                    });
                }

                // Mengonversi DTO menjadi objek Account untuk persiapan penyimpanan
                Account toCreateAcc = registrationDto;
                toCreateAcc.Guid = toCreateEmp.Guid;
                toCreateAcc.Password = HashingHandler.HashPassword(registrationDto.Password);
                _accountRepository.Create(toCreateAcc);

                var accountRole = _accountRoleRepository.Create(new AccountRole
                {
                    AccountGuid = toCreateAcc.Guid,
                    RoleGuid = _roleRepository.GetDefaultRoleGuid() ?? throw new Exception("Default Role Not Found")
                });
                // Mengakhiri transaksi dan menyimpan semua perubahan
                transaction.Complete();

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
    [HttpPost("Login")]
    [AllowAnonymous]
    public IActionResult Login(LoginDto loginDto)
    {
        try
        {
            // Mengambil semua data employee dan akun
            var employees = _employeeRepository.GetEmail(loginDto.Email);

            // Verifikasi apakah ada data untuk employee dan akun
            if (employees is null)
            {
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Acount or Password is invalid!"
                });
            }

            var account = _accountRepository.GetByGuid(employees.Guid);
            if (!HashingHandler.VerifyPassword(loginDto.Password, account!.Password))
            {
                return BadRequest(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = HttpStatusCode.BadRequest.ToString(),
                    Message = "Account or Password is invalid!"
                });
            }

            var claims = new List<Claim>();
            claims.Add(new Claim("Email", employees.Email));
            claims.Add(new Claim("FullName", string.Concat(employees.FirstName + " " + employees.LastName)));

            var getRoleName = from ar in _accountRoleRepository.GetAll()
                              join r in _roleRepository.GetAll() on ar.RoleGuid equals r.Guid
                              where ar.AccountGuid == account.Guid
                              select r.Name;

            foreach (var roleName in getRoleName)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            }


            var generateToken = _tokenHandler.Generate(claims);

            return Ok(new ResponseOKHandler<object>("Login Success", new { Token = generateToken }));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
            {
                Code = StatusCodes.Status500InternalServerError,
                Status = HttpStatusCode.InternalServerError.ToString(),
                Message = "Failed to Delete data",
                Error = ex.Message
            });

        }
    }
    



    [HttpGet]
    [Authorize(Roles = "admin, superAdmin")]
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
    [Authorize(Roles = "admin, superAdmin")]
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
    [Authorize(Roles = "admin, superAdmin")]
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