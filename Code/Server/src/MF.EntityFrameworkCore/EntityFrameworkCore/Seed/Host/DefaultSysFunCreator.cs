using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp.Localization;
using MF.SystemFunctions;

namespace MF.EntityFrameworkCore.Seed.Host
{
    public class DefaultSysFunCreator
    {
        public static List<SysFun> InitialFun => GetInitialFun();

        private readonly MFDbContext _context;

        private static List<SysFun> GetInitialFun()
        {
            return new List<SysFun>
            {
                new SysFun(){ Name="技能"},
                new SysFun(){ Name="谜题等级"},
            };
        }

        public DefaultSysFunCreator(MFDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateFuns();
        }

        private void CreateFuns()
        {
            foreach (var item in InitialFun)
            {
                AddFunIfNotExists(item);
            }
        }

        private void AddFunIfNotExists(SysFun Fun)
        {
            if (_context.SysFun.IgnoreQueryFilters().Any(l => l.Name == Fun.Name))
            {
                return;
            }

            _context.SysFun.Add(Fun);
            _context.SaveChanges();
        }
    }
}
