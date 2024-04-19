
using CommandService.Data;
using CommandService.Models;

namespace CommandService.Repos
{

    public class CommandRepo : ICommandRepo
    {   private readonly AppDbContext _context;
        public CommandRepo(AppDbContext context)
    {
        _context=context;
    }
        public void CreateCommandForPlatform(int platformId, Command command)
        {
           if(command==null)
           {
            throw new ArgumentNullException(nameof(command));
           }
           command.PlatformId=platformId;
           _context.Commands.Add(command);
        }

        public void CreatePlatform(Platform platform)
        {
           if(platform==null)
           {
            throw new ArgumentNullException(nameof(platform));
           }
           _context.Platforms.Add(platform);
           
        }

        public bool ExternalPlatformExists(int externalPlatformId)
        {
            return _context.Platforms.Any(p=>p.ExternalID==externalPlatformId);
        }

        public IEnumerable<Command> GetAllCommandsForPlatform(int platformId)
        {
           return _context.Commands.Where(c=>c.PlatformId==platformId).OrderBy(c=>c.Platform.Name);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }

        public Command GetCommandForPlatform(int platformId, int commandId)
        {
           return _context.Commands.Where(c=>c.PlatformId==platformId && c.Id==commandId).FirstOrDefault();
        }

        public bool PlatformExists(int platformId)
        {
            return _context.Platforms.Any(p=>p.Id==platformId);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges()>=0);
        }
    }
}