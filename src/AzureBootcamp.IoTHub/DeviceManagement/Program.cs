using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;

namespace DeviceManagement
{
    class Program
    {
        private static Microsoft.Azure.Devices.RegistryManager _registryManager;
        private static string _connectionString = "{connectionString}";

        private static void Main(string[] args)
        {
            _registryManager = RegistryManager.CreateFromConnectionString(_connectionString);

            #region Menu

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("(A)dd a device.");
                Console.WriteLine("(L)ist devices.");
                Console.WriteLine("(R)emove a device.");
                Console.WriteLine("E(x)it.");
                Console.Write("Select an option: ");

                var choice = Console.ReadLine();
                Console.WriteLine();

                string deviceId;
                switch (choice)
                {
                    case "A":
                    case "a":
                        deviceId = PromptForDeviceId();
                        AddDeviceAsync(deviceId).Wait();
                        break;

                    case "L":
                    case "l":
                        ViewDevicesAsync().Wait();
                        break;

                    case "R":
                    case "r":
                        deviceId = PromptForDeviceId();
                        RemoveDeviceAsync(deviceId).Wait();
                        break;

                    case "X":
                    case "x":
                        exit = true;
                        break;

                    default:
                        break;
                }
            }

            #endregion
        }

        #region Menu Methods

        private static string PromptForDeviceId()
        {
            Console.Write("Device Id: ");
            return Console.ReadLine();
        }

        private static void PromptToReturnToMenu()
        {
            Console.Write("\nPress any key to return to the menu...");
            Console.ReadKey();
            Console.WriteLine();
        }

        #endregion

        private static async Task AddDeviceAsync(string deviceId)
        {
            Device device;

            try
            {
                device = await _registryManager.AddDeviceAsync(new Device(deviceId));

            }
            catch (DeviceAlreadyExistsException)
            {
                device = await _registryManager.GetDeviceAsync(deviceId);
            }

            File.WriteAllText($"{deviceId}.txt", device.Authentication.SymmetricKey.PrimaryKey);
            Console.WriteLine(device.Authentication.SymmetricKey.PrimaryKey);
        }

        private static async Task ViewDevicesAsync()
        {
            try
            {
                var devices = await _registryManager.GetDevicesAsync(10);

                foreach (var device in devices)
                {
                    Console.WriteLine($"{device.Id} ({device.Authentication.SymmetricKey.PrimaryKey}");
                }
            }
            catch (Exception)
            {
                throw;
            }

            PromptToReturnToMenu();
        }

        private static async Task RemoveDeviceAsync(string deviceId)
        {
            try
            {
                await _registryManager.RemoveDeviceAsync(deviceId);
            }
            catch (Exception)
            {
                throw;
            }

            Console.WriteLine($"{deviceId} has been removed.");
            PromptToReturnToMenu();
        }
    }
}
