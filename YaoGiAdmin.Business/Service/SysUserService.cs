﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using YaoGiAdmin.Business.IService;
using YaoGiAdmin.Core;
using YaoGiAdmin.Models;
using YaoGiAdmin.Models.Jwt;
using YaoGiAdmin.Models.Sys;

namespace YaoGiAdmin.Business.Service
{
    public class SysUserService : ISysUserService
    {
        BuildingDbContext context;

        public SysUserService(BuildingDbContext dbContext)
        {
            context = dbContext;
        }
        public async Task<Response> RegisterUser(SysUser model)
        {
            Response res = new Response();
            try
            {
                var data = await context.SysUser.Where(m => m.UserMobile == model.UserMobile&&m.IsDel==0).FirstOrDefaultAsync();
                if (data != null)
                {
                    res.Code = 3;
                    res.Message = "该手机号码已经被注册";
                    return res;
                }
                var data1 = await context.SysUser.Where(m=>m.UserAccount == model.UserAccount && m.IsDel == 0).FirstOrDefaultAsync();
                if (data1 != null)
                {
                    res.Code = 3;
                    res.Message = "该用户已存在";
                    return res;
                }
                model.CreateTime = DateTime.Now;
                model.UserPassword = ValueConvert.MD5(model.UserPassword);

                await context.SysUser.AddAsync(model);

                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                res.Code = 2;
                res.Message = e.Message;
            }
            return res;
        }


        public async Task<Response> UserLogin(string account, string password)
        {
            Response res = new Response();
            try
            {
                var data = await context.SysUser.Where(m => m.UserAccount == account && m.UserPassword == ValueConvert.MD5(password)).FirstOrDefaultAsync();
                if (data == null)
                {
                    res.Code = 3;
                    res.Message = "登陆失败";
                    return res;
                }
            }
            catch (Exception e)
            {
                res.Code = 2;
                res.Message = e.Message;
            }
            return res;
        }


        public SysUser ResponseToken(LoginRequestDTO model)
        {
            var data = context.SysUser.Where(m => m.UserAccount == model.Account && m.UserPassword == ValueConvert.MD5(model.Password)).FirstOrDefault();
            if (data == null)
                return null;
            else
                return data;
        }
    }
}
