using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RestApi.Helpers;
using RestApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RestApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly JWT _jwt;
        public AuthService(UserManager<User> userManager , IOptions<JWT> jwt)
        {
            _jwt = jwt.Value;
            _userManager = userManager;
        }
        public async Task<AuthModel> RegisterAsync(Register model)
        {
            if(await _userManager.FindByEmailAsync(model.Email) != null)
            {
                return new AuthModel { Message = "Email is already exict" };
            }
            if (await _userManager.FindByNameAsync(model.UserName) != null)
            {
                return new AuthModel { Message = "UserName is already Exict" };
            }
            var newUser = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach(var error in result.Errors)
                {
                    errors += $"{error.Description} ,";
                }
                return new AuthModel { Message = errors };
            }
            await _userManager.AddToRoleAsync(newUser, "User");

            var jwtSecurityToken = await GenerateToken(newUser);

            return new AuthModel
            {
                Email = newUser.Email,
                ExpireOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = newUser.UserName
            };

        }

        public async Task<JwtSecurityToken> GenerateToken(User user) 
        {
            //var userClaims =await  _userManager.GetClaimsAsync(user);
            var roles= await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach(var role in roles)
            {
                roleClaims.Add(new Claim("roles", role));
            }

            var claims = new[]
            {
                new Claim (JwtRegisteredClaimNames.Sub , user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()),
                new Claim (JwtRegisteredClaimNames.Email , user.Email),
                new Claim("uid" , user.Id),
            }
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        public async Task<AuthModel> GetTokenAsync(GetToken model)
        {
            var auth = new AuthModel();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user , model.Password))
            {
                auth.Message = "you have same problem in ur info";
                return auth;
            }

            var jwtSecurityToken = await GenerateToken(user);
            var roleList = await _userManager.GetRolesAsync(user);

            auth.IsAuthenticated = true;
            auth.Email = user.Email;
            auth.UserName = user.UserName;
            auth.ExpireOn = jwtSecurityToken.ValidTo;
            auth.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            auth.Roles = roleList.ToList();
            return auth;

            //var authModel = new AuthModel();

            //var user = await _userManager.FindByEmailAsync(model.Email);

            //if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            //{
            //    authModel.Message = "Email or Password is incorrect!";
            //    return authModel;
            //}

            //var jwtSecurityToken = await GenerateToken(user);
            //var rolesList = await _userManager.GetRolesAsync(user);

            //authModel.IsAuthenticated = true;
            //authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            //authModel.Email = user.Email;
            //authModel.UserName = user.UserName;
            //authModel.ExpireOn = jwtSecurityToken.ValidTo;
            //authModel.Roles = rolesList.ToList();

            //return authModel;
        }
    }
}
