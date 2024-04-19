using System.ComponentModel.Design;
using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;
using CommandService.Repos;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Cotrollers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;
        public CommandsController(ICommandRepo repo,IMapper mapper)
        {
                _repo=repo;
                _mapper=mapper;
        }
        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>>GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"**** Getting started Commands for PlatformId {platformId}*****");
            if(!_repo.PlatformExists(platformId))
            {
                return NotFound();
            }
            var Commands=_repo.GetAllCommandsForPlatform(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(Commands));
           
        }


        [HttpGet("{commandId}",Name ="GetCommandForPlatform")]
        public ActionResult<CommandReadDto>GetCommandForPlatform(int platformId,int commandId)
        {
            Console.WriteLine($"Spcecific Command  for PlatformId {platformId} and CommandId {commandId}");
            if(!_repo.PlatformExists(platformId))
            {
                return NotFound();
            }
            var Command=_repo.GetCommandForPlatform(platformId,commandId);
            return Ok(_mapper.Map<CommandReadDto>(Command));
           
        }

          [HttpPost]
        public ActionResult<CommandCreateDto>CreateCommandForPlatform(int platformId,CommandCreateDto commandCreateDto)
        {
            Console.WriteLine($"Creating Command  for PlatformId {platformId}");
            if(!_repo.PlatformExists(platformId))
            {
                return NotFound();
            }
           var command=_mapper.Map<Command>(commandCreateDto);
           _repo.CreateCommandForPlatform(platformId,command);
           _repo.SaveChanges();
           var commandReadDto=_mapper.Map<CommandReadDto>(command);
           return CreatedAtRoute(nameof(GetCommandForPlatform),new{platformId=platformId,CommandID=commandReadDto.Id},commandReadDto);

           
           
        }
    }
}