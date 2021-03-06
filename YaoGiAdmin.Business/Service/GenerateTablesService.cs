﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using YaoGiAdmin.Business.IService;
using YaoGiAdmin.Core;
using YaoGiAdmin.Models;
using YaoGiAdmin.Models.Jwt;
using YaoGiAdmin.Models.Tools;

namespace YaoGiAdmin.Business.Service
{
    public class GenerateTablesService : IGenerateTablesService
    {
        BuildingDbContext context;
        private readonly ISysUserService _sysUser;
        public GenerateTablesService(BuildingDbContext dbcontext, ISysUserService sysUser)
        {
            context = dbcontext;
            _sysUser = sysUser;
        }

        public async Task<Response> CreateTable(GenerateTables model)
        {
            Response res = new Response();
            try
            {
                //var con= RequestFilter.GetContext();
                model.SysUser = _sysUser.ResponseToken(new LoginRequestDTO() { Account = "yaogi", Password = "1" });
                model.CreateUser = model.SysUser.UserName;
                //model.SysUser = UserCacheHelper.GetSys();
                var data = await context.GenerateTables.Where(m => m.TableName == model.TableName).FirstOrDefaultAsync();
                if (data != null)
                {
                    res.Code = 0;
                    res.Message = "该表已存在";
                    return res;
                }


                await context.GenerateTables.AddAsync(model);
                await context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                res.Code = 0;
                res.Message = e.Message;
            }
            return res;
        }

        public async Task<Response> DeleteTable(Guid? _tableId)
        {
            Response res = new Response();
            try
            {
                if (_tableId == null)
                {
                    res.Code = 3;
                    res.Message = "参数为空";
                    return res;
                }
                var data = await context.GenerateTables.SingleOrDefaultAsync(b => b.Id == _tableId);
                if (data == null)
                {
                    res.Code = 3;
                    res.Message = "该表不存在";
                    return res;
                }
                else
                {
                    if (data.IsGenerate == 1)
                    {
                        res.Code = 3;
                        res.Message = "该表已经生成无法删除";
                        return res;
                    }

                }

                context.GenerateTables.Remove(data);

                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                res.Code = 2;
                res.Message = ex.Message;
            }
            return res;
        }

        public async Task<Response> GetList()
        {
            Response res = new Response();
            try
            {
                var data = await context.GenerateTables.Where(m => m.IsDel == 0).ToListAsync();                
                res.Data = data;
            }
            catch (Exception e)
            {
                res.Code = 2;
                res.Message = e.Message;
            }
            return res;
        }
    }
}
