﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace YaoGiAdmin.Api
{
    public class CorsMiddleware
    {
        private readonly RequestDelegate next;
        //public CorsMiddleware(RequestDelegate next, IAuthenticateService authenticateService)
        //{
        //    this.next = next;
        //    _authenticateService = authenticateService;
        //}
        public CorsMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey(CorsConstants.Origin))
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", context.Request.Headers["Origin"]);
                context.Response.Headers.Add("Access-Control-Allow-Methods", "PUT,POST,GET,DELETE,OPTIONS,HEAD,PATCH");
                context.Response.Headers.Add("Access-Control-Allow-Headers", context.Request.Headers["Access-Control-Request-Headers"]);
                context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");


                if (context.Request.Method.Equals("OPTIONS"))
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    return;
                }
                var schemeProvider = context.RequestServices.GetService(typeof(IAuthenticationSchemeProvider)) as IAuthenticationSchemeProvider;
                var defaultAuthenticate = await schemeProvider.GetDefaultAuthenticateSchemeAsync();
                if (defaultAuthenticate != null)
                {
                    var result = await context.AuthenticateAsync(defaultAuthenticate.Name);
                    var user = result?.Principal;
                    if (user != null)
                    {
                        var account = user.Identity.Name;
                        //var jwtsession = context.Request.Headers["Authorization"];
                        //if (!string.IsNullOrWhiteSpace(jwtsession))
                        //{
                        //    var userx= context.User;
                        //    //var res = _authenticateService.JwtDecrypt(jwtsession.ToString().Replace("Bearer ",""));
                        //    SessionExtensions.SetString(null, "jwtToken", account);
                        //}

                    }
                }
            }

            await next(context);
        }
    }
}
