using Azure.Messaging.EventHubs.Producer;
using Face_Detector.API_Service.Model;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Face_Detector.API_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceController : ControllerBase
    {

        public FaceController() { }

        private readonly string subscriptionKey = "b00fd0191b5b485bad8a3c2bbc25b470";
        private readonly string endpoint = "https://face-sensor.cognitiveservices.azure.com/";

        [HttpPost("detect/{gate}")]
        public async Task<IActionResult> DetectFace([FromRoute] int gate)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                    string requestUrl = $"{endpoint}/face/v1.0/detect";

                    using (Stream stream = Request.Body)
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);

                            using (ByteArrayContent content = new ByteArrayContent(memoryStream.ToArray()))
                            {
                                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                                HttpResponseMessage response = await client.PostAsync(requestUrl, content);
                                var imageResponse = await response.Content.ReadAsStringAsync();

                                List<FaceData> face = JsonSerializer.Deserialize<List<FaceData>>(imageResponse);

                                if(face != null && face.Any())
                                {
                                    await NotifyFaceDetection(face, gate);
                                }

                                return Ok(face);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Function to send face-notification to event hub
        private async Task NotifyFaceDetection(List<FaceData> faces, int gateId)
        {
            string connectionString = "Endpoint=sb://everbridge-face-hub.servicebus.windows.net/;SharedAccessKeyName=sender;SharedAccessKey=crivyiR8siXjN7yRbeiDolPiFkZjnkefU+AEhOoYaY0=;EntityPath=face-detector-hub";
            string eventHubName = "face-detector-hub";

            await using (var producerClient = new EventHubProducerClient(connectionString, eventHubName))
            {
                using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

                eventBatch.TryAdd(new Azure.Messaging.EventHubs.EventData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new FaceNotificationMessage {  FaceCount = faces.Count , Message = "detected" , gateId = gateId }))));
                
                await producerClient.SendAsync(eventBatch);
            }
        }
    }
}
