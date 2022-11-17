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

using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Authentication.ClientServer;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace XAFLogging.WebApi.JWT;

[ApiController]
[Route("api/[controller]")]
// This is a JWT authentication service sample.
public class AuthenticationController : ControllerBase {
    readonly IAuthenticationTokenProvider tokenProvider;

    public AuthenticationController(IAuthenticationTokenProvider tokenProvider) {
        this.tokenProvider = tokenProvider;
    }

    [HttpPost("Authenticate")]
    [SwaggerOperation("Checks if the user with the specified logon parameters exists in the database. If it does, authenticates this user.",
        "Refer to the following help topic for more information on authentication methods in the XAF Security System: <a href='https://docs.devexpress.com/eXpressAppFramework/119064/data-security-and-safety/security-system/authentication'>Authentication</a>.")]
    public IActionResult Authenticate(
        [FromBody] [SwaggerRequestBody(@"For example: <br /> { ""userName"": ""Admin"", ""password"": """" }")]
        AuthenticationStandardLogonParameters logonParameters
    ) {
        try {
            return Ok(tokenProvider.Authenticate(logonParameters));
        }
        catch (AuthenticationException) {
            return Unauthorized("User name or password is incorrect.");
        }
    }
}