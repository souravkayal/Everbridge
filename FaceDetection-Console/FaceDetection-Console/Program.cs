using Azure.Messaging.EventHubs.Consumer;
using FaceDetection_Console.Model;
using System.Text;
using System.Text.Json;

public class Program
{
    public static async Task Main(string[] args)
    {
        string connectionString = "Endpoint=sb://everbridge-face-hub.servicebus.windows.net/;SharedAccessKeyName=receiver;SharedAccessKey=+07kXDiYQgP3kXMtsgqvm6wSAxDF3H79e+AEhA5C1Zw=;EntityPath=face-detector-hub";
        string eventHubName = "face-detector-hub";
        string consumerGroup = "console"; 

        await ReceiveMessagesAsync(connectionString, eventHubName, consumerGroup);
    }

    static async Task ReceiveMessagesAsync(string connectionString, string eventHubName, string consumerGroup)
    {
        await using (var consumerClient = new EventHubConsumerClient(consumerGroup, connectionString, eventHubName))
        {
            await foreach (PartitionEvent partitionEvent in consumerClient.ReadEventsAsync())
            {
                var result = JsonSerializer.Deserialize<FaceNotificationMessage>(Encoding.UTF8.GetString(partitionEvent.Data.Body.ToArray()));

                Console.WriteLine($"ATTENTION : {result?.FaceCount} Face(s) detected in GATE ID : {result?.gateId }");
            }
        }
    }
}