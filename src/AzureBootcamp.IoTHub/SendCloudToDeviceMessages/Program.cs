using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace SendCloudToDeviceMessages
{
    class Program
    {
        private static ServiceClient _serviceClient;
        private static string _connectionString = "{connection string}";
        private static string _deviceId = "{device id}";

        private static void Main(string[] args)
        {
            Console.WriteLine("Sending Cloud-to-Device messages.");

            _serviceClient = ServiceClient.CreateFromConnectionString(_connectionString);

            Console.WriteLine("Press ENTER to send a cloud to device message.");
            while (true)
            {
                Console.ReadLine();
                SendCloudToDeviceMessage(_deviceId);
            }
        }

        private static async Task SendCloudToDeviceMessage(string deviceId)
        {
            var m = new Message(Encoding.ASCII.GetBytes("Cloud to device message."));
            await _serviceClient.SendAsync(deviceId, m);
        }
    }
}
