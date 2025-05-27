using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;

        /// <summary>
        /// AccountController sınıfının yeni bir örneğini başlatır.
        /// </summary>
        /// <param name="userManager">Kullanıcı yönetimi işlemleri için kullanılan UserManager servisi.</param>
        /// <param name="tokenService">JWT token oluşturma işlemleri için kullanılan ITokenService servisi.</param>
        /// <param name="signInManager">Kullanıcı oturum açma işlemleri için kullanılan SignInManager servisi.</param>
        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }
        /// <summary>
        /// Kullanıcının sisteme giriş yapmasını sağlar ve başarılı olursa JWT token döner.
        /// </summary>
        /// <param name="loginDto">Giriş için kullanıcı adı ve şifre bilgilerini içeren DTO.</param>
        /// <returns>Başarılı giriş durumunda JWT token ve kullanıcı bilgilerini içeren bir Ok (200) yanıtı;
        /// geçersiz kimlik bilgileri durumunda Unauthorized (401) veya doğrulama hatası durumunda BadRequest (400) yanıtı döner.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded) return Unauthorized(" username not found or password is incorrect.");

            return Ok(new NewUserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user)
            });
        }

        /// <summary>
        /// Yeni bir kullanıcı kaydı oluşturur ve kullanıcıya "User" rolünü atar.
        /// Başarılı kayıt durumunda yeni kullanıcının bilgilerini ve JWT token'ını döner.
        /// </summary>
        /// <param name="registerDto">Kayıt için kullanıcı adı, e-posta ve şifre bilgilerini içeren DTO.</param>
        /// <returns>Başarılı kayıt durumunda yeni kullanıcı bilgilerini ve token'ı içeren Ok (200) yanıtı;
        /// doğrulama hatası durumunda BadRequest (400) veya sunucu hatası durumunda StatusCode (500) yanıtı döner.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new AppUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email
            };

            var createResult = await _userManager.CreateAsync(user, registerDto.Password);
            if (!createResult.Succeeded)
                return StatusCode(500, FormatErrors(createResult.Errors, "Kullanıcı oluşturulamadı."));

            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
                return StatusCode(500, FormatErrors(roleResult.Errors, "Rol atama başarısız."));

            var newUserDto = new NewUserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user)
            };

            return Ok(newUserDto);
        }

        /// <summary>
        /// Identity hatalarını birleştirerek okunabilir bir hata mesajı oluşturur.
        /// </summary>
        /// <param name="errors">Identity hata nesnelerinin koleksiyonu.</param>
        /// <param name="message">Hata mesajının başına eklenecek ana metin.</param>
        /// <returns>Birleştirilmiş hata açıklamalarını içeren bir string.</returns>
        private string FormatErrors(IEnumerable<IdentityError> errors, string message)
        {
            var errorDescriptions = string.Join("; ", errors.Select(e => e.Description));
            return $"{message} Hatalar: {errorDescriptions}";
        }

    }
}