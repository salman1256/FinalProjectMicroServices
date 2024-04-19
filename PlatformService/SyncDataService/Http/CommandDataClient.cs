using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncDataService.Http
{
    public class CommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public CommandDataClient(HttpClient httpClient,IConfiguration configuration)
        {
            _httpClient=httpClient;
            _configuration=configuration;
        }
        public async Task SendPlatformToCommand(PlatformReadDto platform)
        {
            var httpContent=new StringContent(
                JsonSerializer.Serialize(platform),
                Encoding.UTF8,
                "application/json"
            );
            var response= await _httpClient.PostAsync($"{_configuration["CommandService"]}",httpContent);
            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine("Sync Data POST to Command Service was perfectly FINE!");
            }
            else{
                Console.WriteLine("Sync Data POST to Command Service not reached or have issue!");
            }
        }
    }
}