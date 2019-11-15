using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp.Configuration;
using Abp.Localization;
using Abp.Net.Mail;

namespace MF.EntityFrameworkCore.Seed.Host
{
    public class DefaultSettingsCreator
    {
        private readonly MFDbContext _context;

        public DefaultSettingsCreator(MFDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            // Emailing
            //AddSettingIfNotExists(EmailSettingNames.DefaultFromAddress, "admin@mydomain.com");
            //AddSettingIfNotExists(EmailSettingNames.DefaultFromDisplayName, "mydomain.com mailer");

            AddSettingIfNotExists(EmailSettingNames.DefaultFromAddress, "xcskgfyx@163.com");
            AddSettingIfNotExists(EmailSettingNames.DefaultFromDisplayName, "xcskgfyx");
            AddSettingIfNotExists(EmailSettingNames.Smtp.UserName, "xcskgfyx@163.com");
            AddSettingIfNotExists(EmailSettingNames.Smtp.Password, "fany2378");
            AddSettingIfNotExists(EmailSettingNames.Smtp.Host, "smtp.163.com");
            AddSettingIfNotExists(EmailSettingNames.Smtp.Port, "25");
            AddSettingIfNotExists(EmailSettingNames.Smtp.Domain, "");
            AddSettingIfNotExists(EmailSettingNames.Smtp.EnableSsl, "false");
            AddSettingIfNotExists(EmailSettingNames.Smtp.UseDefaultCredentials, "false");

            // Languages
            AddSettingIfNotExists(LocalizationSettingNames.DefaultLanguage, "zh-Hans");
        }

        private void AddSettingIfNotExists(string name, string value, int? tenantId = null)
        {
            if (_context.Settings.IgnoreQueryFilters().Any(s => s.Name == name && s.TenantId == tenantId && s.UserId == null))
            {
                return;
            }

            _context.Settings.Add(new Setting(tenantId, null, name, value));
            _context.SaveChanges();
        }
    }
}
