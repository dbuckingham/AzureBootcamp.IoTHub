using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace ReceiveDeviceToCloudMessages
{
    class Program
    {
        private static string _connectionString = "{connection string}";
        private static string _endpoint = "messages/events";
        private static EventHubClient _eventHubClient;

        private static void Main(string[] args)
        {
            Console.WriteLine("Receiving messages...");

            _eventHubClient = EventHubClient.CreateFromConnectionString(_connectionString, _endpoint);

            var partitions = _eventHubClient.GetRuntimeInformation().PartitionIds;
            foreach (var partition in partitions)
            {
                ReceiveMessagesFromDeviceAsync(partition);
            }

            Console.ReadLine();
        }

        private static async Task ReceiveMessagesFromDeviceAsync(string partition)
        {
            var eventHubReceiver = _eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.Now);

            while (true)
            {
                EventData eventdata = await eventHubReceiver.ReceiveAsync();
                if (eventdata == null) continue;

                string data = Encoding.UTF8.GetString(eventdata.GetBytes());
                Console.WriteLine($"{DateTime.Now.ToString("h:mm:ss tt")}> Received: {data}");
            }
        }
    }
}
