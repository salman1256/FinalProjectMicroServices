using System.Windows.Input;
using AutoMapper;
using CommandService.Dtos;
using CommandService.Repos;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Cotrollers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController:ControllerBase
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;
        public PlatformsController(ICommandRepo repo, IMapper mapper)
        {
            _mapper=mapper;
            _repo=repo;
        }
        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>>GetPlatforms()
        {
            Console.WriteLine("**** Getting Platforms from Command Service *****");
            var rcvdPlatforms=_repo.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(rcvdPlatforms));
        }
        [HttpPost]
        public ActionResult TestInBoundConnection()
        {
            System.Console.WriteLine("In bound POST Command Service Started!!!");
            return Ok("Inbound test of Command Service from Platform Controller");
        }

    }
}