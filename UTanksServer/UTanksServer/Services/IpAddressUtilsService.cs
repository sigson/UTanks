using System.Net;
using System.Threading.Tasks;

using Newtonsoft.Json;

using RestSharp;

namespace UTanksServer.Services
{
    public class PublicIpResponse {
    [JsonProperty("ip")] public IPAddress Address { get; }

    public PublicIpResponse(IPAddress address) {
      Address = address;
    }
  }

  public interface IIpAddressUtilsService {
    public Task<IPAddress> GetPublicAddress();
  }

  [UTanksServer.ECS.ECSCore.Service]
  public class IpAddressUtilsService : IIpAddressUtilsService {
    private readonly IJsonSerializerService _jsonSerializerService;

    public IpAddressUtilsService(IJsonSerializerService jsonSerializerService) {
      _jsonSerializerService = jsonSerializerService;
    }

    public async Task<IPAddress> GetPublicAddress() {
      RestClient client = new RestClient();
      //client.UseNewtonsoftJson(_jsonSerializerService.SerializerSettings);

      RestRequest request = new RestRequest("https://api.ipify.org/?format=json", Method.GET);
      IRestResponse<PublicIpResponse> response = await client.ExecuteAsync<PublicIpResponse>(request);

      return response.Data.Address;
    }
  }
}
