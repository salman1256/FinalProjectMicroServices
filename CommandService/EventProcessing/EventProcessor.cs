using System.Text.Json;
using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;
using CommandService.Repos;
namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;
        public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {   _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            switch (eventType)
            {      case EventType.PlatformPublished:
                    {    AddPlatform(message);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

        }

        private EventType DetermineEvent(string notificationMsg)
        {
            System.Console.WriteLine("Determinig Event");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMsg);
            switch (eventType.Event)
            {
                case "Platform_Published":
                    {
                        Console.WriteLine("Platform published event notified and detected");
                        return EventType.PlatformPublished;
                    }
                default:
                    {
                        Console.WriteLine("Could not determine the event type");
                        return EventType.Undetermined;
                    }
            }
        }

        private void AddPlatform(string message)
        {    using (var scope = _serviceScopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(message);
                try
                {   var platform = _mapper.Map<Platform>(platformPublishedDto);
                    if (!repo.ExternalPlatformExists(platform.ExternalID))
                    {     repo.CreatePlatform(platform);
                        repo.SaveChanges();
                        Console.WriteLine("New Platform Added");

                    }
                    else
                    {
                        Console.WriteLine("Platform alterady exist");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not add the Platform to Database Error" + ex.Message);
                }
            }
        }
    }
    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}