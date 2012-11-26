using Autofac;
using Autofac.Integration.Wcf;
using DotNetDesign.Substrate.Example.Entities;
using DotNetDesign.Substrate.Example.Entities.Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace DotNetDesign.Substrate.Example.ConsoleClient
{
    class Program
    {
        private const string ROOT_URI = "net.tcp://localhost:8080/";

        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new EntitiesModule());
            RegisterNetTcpService<IUserRepositoryService>(builder, ROOT_URI + "UserRepositoryService");
            builder.RegisterType<DictionaryScopeManager>().As<IScopeManager>().SingleInstance();

            using (var container = builder.Build())
            {
                using (var lifetimeScope = container.BeginLifetimeScope())
                {
                    var userRepository = lifetimeScope.Resolve<IUserRepository>();
                    var users = userRepository.GetAll();
                    if (users.Count() > 0)
                    {
                        foreach (var user in users)
                        {
                            Console.WriteLine("First Name: {0} | Last Name: {1} | E-mail Address: {2}", user.FirstName, user.LastName, user.EmailAddress);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No users");

                        Console.WriteLine("Adding a user");

                        var user = userRepository.GetNew();
                        user.FirstName = "Adam";
                        user.LastName = "Carr";
                        user.EmailAddress = "adam.carr@gmail.com";
                        user.Save();

                        Console.WriteLine("Added user {0}", user);
                    }
                }
            }

            Console.Read();
        }

        private static void RegisterNetTcpService<TService>(ContainerBuilder builder, string address)
        {
            builder.Register(
                container =>
                new ChannelFactory<TService>(new NetTcpBinding()
                    {
                        SendTimeout = new TimeSpan(1, 0, 0),
                        ReceiveTimeout = new TimeSpan(0, 5, 0)
                    },
                                             new EndpointAddress(address))).SingleInstance();

            builder.Register(
                c => c.Resolve<ChannelFactory<TService>>().CreateChannel())
                   .UseWcfSafeRelease();
        }
    }
}
