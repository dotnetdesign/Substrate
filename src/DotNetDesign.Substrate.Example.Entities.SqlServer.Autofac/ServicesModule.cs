using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetDesign.Substrate.Example.Entities.SqlServer.Autofac
{
    public class ServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserRepositoryService>().As<IUserRepositoryService>().InstancePerLifetimeScope();
        }
    }
}
