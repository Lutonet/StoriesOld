using Microsoft.Extensions.Logging;
using RestSharp;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stories.Services
{
    public interface ISmsSender
    {
        public Task<bool> SendSmsAsync(int phoneNumber, string sender, string text);
    }

    public class SmsSender : ISmsSender
    {
        private ILogger<SmsSender> _logger;

        public SmsSender(ILogger<SmsSender> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendSmsAsync(int phoneNumber, string sender, string text)
        {
            var client = new RestClient("https://rest-api.d7networks.com/secure/send?username=qucc1167&password=EIgRfLXq");
            RestRequest request = new RestRequest(Method.Post.ToString());
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "{\n\t\"to\":\"" + phoneNumber + "\",\n\t\"content\":\"" + text + "\",\n\t\"from\":\"" + sender + "\",\n\t\"dlr\":\"yes\",\n\t\"dlr-method\":\"GET\", \n\t\"dlr-level\":\"2\", \n\t\"dlr-url\":\"http://yourcustompostbackurl.com\"\n}", ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                return true;
            }

            return false;
        }
    }
}