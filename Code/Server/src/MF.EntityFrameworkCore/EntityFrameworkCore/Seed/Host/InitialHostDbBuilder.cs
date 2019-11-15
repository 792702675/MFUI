using MF.EntityFrameworkCore.Seed.Host;

namespace MF.EntityFrameworkCore.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly MFDbContext _context;

        public InitialHostDbBuilder(MFDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new DefaultMenuCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
            new LanguageTextOverrider(_context).Create();

            _context.SaveChanges();
        }
    }
}
