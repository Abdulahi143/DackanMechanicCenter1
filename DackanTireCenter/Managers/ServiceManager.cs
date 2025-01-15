    using System;
    using System.Collections.Generic;
    using DackanTireCenter.Models;

    namespace DackanTireCenter.Managers
    {
        public class ServiceManager
        {
            private readonly List<Service> availableServices;

            public ServiceManager()
            {
                availableServices = new List<Service>
                {
                    new Service("Oljebyte", 499),
                    new Service("Däckbyte", 999),
                    new Service("Bromsinspektion", 299)
                };
            }

            public List<Service> GetAvailableServices()
            {
                return availableServices;
            }

            public void AddService(string serviceName, decimal price)
            {
                // Check if the service already exists
                if (ServiceExists(serviceName))
                {
                    Console.WriteLine($"Tjänsten '{serviceName}' finns redan.");
                    return;
                }

                availableServices.Add(new Service(serviceName, price));
            }
            
            public void UpdateServiceName(string oldName, string newName)
            {
                var serviceToUpdate = availableServices.Find(s => s.Name.Equals(oldName, StringComparison.OrdinalIgnoreCase));
                if (serviceToUpdate != null)
                {
                    serviceToUpdate.Name = newName;
                }
            }

            public void UpdateServicePrice(string serviceName, decimal newPrice)
            {
                var serviceToUpdate = availableServices.Find(s => s.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
                if (serviceToUpdate != null)
                {
                    serviceToUpdate.Price = newPrice;
                }
            }

            public void DeleteService(string serviceName)
            {
                // Find and remove the service
                availableServices.RemoveAll(s => s.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
            }

            public bool ServiceExists(string serviceName)
            {
                return availableServices.Any(s => s.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
            }
        }
    }