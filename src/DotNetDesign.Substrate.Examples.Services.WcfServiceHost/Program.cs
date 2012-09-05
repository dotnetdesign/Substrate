using Autofac;
using Autofac.Integration.Wcf;
using DotNetDesign.Substrate.Example.Entities;
using DotNetDesign.Substrate.Example.Entities.Autofac;
using DotNetDesign.Substrate.Example.Entities.SqlServer;
using DotNetDesign.Substrate.Example.Entities.SqlServer.Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;

namespace DotNetDesign.Substrate.Examples.Services.WcfServiceHost
{
    class Program
    {
        private const string ROOT_URI = "net.tcp://localhost:8080/";
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new EntitiesModule());
            builder.RegisterModule(new ServicesModule());

            var serviceInformationList =
                new[]
                    {
                        new
                            {
                                Address = "UserRepositoryService",
                                InterfaceType = typeof (IUserRepositoryService),
                                ImplementationType = typeof (UserRepositoryService)
                            }
                    };

            using (var container = builder.Build())
            {
                var serviceHosts = new List<ServiceHost>(serviceInformationList.Length);
                foreach (var serviceInfo in serviceInformationList)
                {
                    var serviceUri = new Uri(ROOT_URI + serviceInfo.Address);
                    var serviceHost = new ServiceHost(serviceInfo.ImplementationType, serviceUri);
                    var debugBehavior = serviceHost.Description.Behaviors.Single(x => x is ServiceDebugBehavior) as ServiceDebugBehavior;
                    debugBehavior.IncludeExceptionDetailInFaults = true;
                    serviceHost.AddServiceEndpoint(serviceInfo.InterfaceType, new NetTcpBinding(), string.Empty);
                    serviceHost.AddDependencyInjectionBehavior(serviceInfo.InterfaceType, container);
                    serviceHost.Open();
                    serviceHosts.Add(serviceHost);
                    Console.WriteLine("Hosted service of type {0} at endpoint {1}", serviceInfo.InterfaceType, serviceUri);
                }

                Console.WriteLine("The host has been opened.");
                Console.ReadLine();

                foreach (var serviceHost in serviceHosts)
                {
                    serviceHost.Close();
                }

                Environment.Exit(0);
            }
        }
    }
}
