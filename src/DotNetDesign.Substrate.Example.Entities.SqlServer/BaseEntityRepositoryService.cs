using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using DotNetDesign.Common;

namespace DotNetDesign.Substrate.Example.Entities.SqlServer
{
    public class BaseRepositoryService : IDisposable
    {
        protected BaseRepositoryService()
        {
            using (TraceTopics.SqlServer.Scope())
            {
                try
                {
                    DbContext = new ExampleDb();
                    DbContext.Database.Initialize(false);
                }
                catch (Exception ex)
                {
                    TraceTopics.SqlServer.Error(ex.Message);
                    throw;
                }
            }
        }


        protected ExampleDb DbContext { get; set; }


        public void Dispose()
        {
            if (DbContext != null)
            {
                DbContext.Dispose();
            }
        }


        public void AttachIfNotAttached<TDataImplementation>(TDataImplementation entity, EntityState entityState)
            where TDataImplementation : class
        {
            if (DbContext.Entry(entity).State == EntityState.Detached)
            {
                DbContext.Set<TDataImplementation>().Attach(entity);
            }


            DbContext.Entry(entity).State = entityState;
        }
    }

}
