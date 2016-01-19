using System;
using System.Text;
using System.Threading;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace MyDevice
{
    class Program
    {
        private static Microsoft.Azure.Devices.Client.DeviceClient _deviceClient;
        private static string _iotHubHostName = "{iothub hostname}";
        private static string _deviceKey = "{device key}";
        private static string _deviceId = "{device id}";

        private static void Main(string[] args)
        {
            Console.WriteLine("Simulated device\n");

            _deviceClient = DeviceClient.Create(_iotHubHostName, new DeviceAuthenticationWithRegistrySymmetricKey(_deviceId, _deviceKey));

            SendDeviceToCloudMessagesAsync();
            ReceiveCloudToDeviceMessagesAsync();

            Console.ReadLine();
        }

        private static async void SendDeviceToCloudMessagesAsync()
        {
            while (true)
            {
                var messageString = BuildMessage();
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await _deviceClient.SendEventAsync(message);
                Console.WriteLine($"{DateTime.Now} > Sending message: {messageString}");

                Thread.Sleep(1000);
            }
        }

        #region Message Methods

        private static string BuildMessage()
        {
            double avgTemperature = 225;
            double variance = 5;

            double currentTemperature = GetRandomTemperature(
                avgTemperature - variance,
                avgTemperature + variance);

            var deviceToCloudMessage = new
            {
                deviceId = _deviceId,
                temperature = Math.Round(currentTemperature, 2)
            };
            var messageString = JsonConvert.SerializeObject(deviceToCloudMessage);

            return messageString;
        }

        private static double GetRandomTemperature(double min, double max)
        {
            Random rand = new Random();
            return rand.NextDouble()*(max - min) + min;
        }

        #endregion

        #region Cloud-to-Device Messages

        private static async void ReceiveCloudToDeviceMessagesAsync()
        {
            Console.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {
                Message receivedMessage = await _deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received message: {0}", Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                Console.ResetColor();

                await _deviceClient.CompleteAsync(receivedMessage);
            }
        }

        #endregion
    }
}
