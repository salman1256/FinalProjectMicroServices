using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.Repos;
using PlatformService.SyncDataService.Http;
using System;

namespace  PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repo;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
         private readonly IMessageBusClient  _messageBusClient;

        public PlatformsController(IPlatformRepo repo, IMapper mapper,ICommandDataClient commandDataClient,IMessageBusClient  messageBusClient)
        {
            _repo = repo;
            _mapper = mapper;
            _commandDataClient=commandDataClient;
              _messageBusClient=messageBusClient;

        }
        [HttpGet]
        public ActionResult <PlatformReadDto>GetPlatforms()
        {
          Console.WriteLine("Start Getting All Platforms");
          var allPlatforms=_repo.GetAllPlatforms();
          return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(allPlatforms));
        
        }
        [HttpGet("{id}",Name ="GetPlatformById")]
        public ActionResult<PlatformReadDto>GetPlatformById(int id)
        {
         
          var rcvdPlat=_repo.GetPlatform(id);
          if(rcvdPlat!=null)
          {
            return Ok(_mapper.Map<PlatformReadDto>(rcvdPlat));
          }
          else
          {
            return NotFound();
          }
        
        }
        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>>CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            var platformModel=_mapper.Map<Platform>(platformCreateDto);
            _repo.CreatPlatform(platformModel);
            _repo.SaveChanges();
            var platformReadDto=_mapper.Map<PlatformReadDto>(platformModel);
            try{
              await _commandDataClient.SendPlatformToCommand(platformReadDto);


            }
            catch(Exception ex)
            {
              System.Console.WriteLine("Could not send Synchronously your message Error!! "+ex.Message);
            }
            //Using asynData
            try{
              
                var platformPublishedDto=_mapper.Map<PlatformPublishedDto>(platformReadDto);
                platformPublishedDto.Event="Platform_Published";
               _messageBusClient.PublishNewPlatform(platformPublishedDto);


            }
            catch(Exception ex)
            {
              System.Console.WriteLine("Asynchronously Data not send   Error!! "+ex.Message);
            }
            return CreatedAtRoute(nameof(GetPlatformById),new{Id=platformReadDto.Id},platformReadDto);
        }
    }

}