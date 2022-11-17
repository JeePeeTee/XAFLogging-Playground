#region MIT License

// ==========================================================
// 
// XAFLogging project - Copyright (c) 2022 JeePeeTee
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// 
// ===========================================================

#endregion

#region usings

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Authentication;
using DevExpress.ExpressApp.Security.Authentication.ClientServer;
using Microsoft.IdentityModel.Tokens;

#endregion

namespace XAFLogging.WebApi.JWT;

public class JwtTokenProviderService : IAuthenticationTokenProvider {
    readonly IConfiguration configuration;
    readonly IStandardAuthenticationService securityAuthenticationService;

    public JwtTokenProviderService(IStandardAuthenticationService securityAuthenticationService, IConfiguration configuration) {
        this.securityAuthenticationService = securityAuthenticationService;
        this.configuration = configuration;
    }

    public string Authenticate(object logonParameters) {
        ClaimsPrincipal user = securityAuthenticationService.Authenticate(logonParameters);

        if (user != null) {
            var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:Jwt:IssuerSigningKey"]));
            var token = new JwtSecurityToken(
                //issuer: configuration["Authentication:Jwt:Issuer"],
                //audience: configuration["Authentication:Jwt:Audience"],
                claims: user.Claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        throw new AuthenticationException("User name or password is incorrect.");
    }
}