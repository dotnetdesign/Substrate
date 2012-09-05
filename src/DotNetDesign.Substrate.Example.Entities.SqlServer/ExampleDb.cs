using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace DotNetDesign.Substrate.Example.Entities.SqlServer
{
    public class ExampleDb : DbContext
    {
        public DbSet<UserData> Users { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ExampleDb, ExampleDbConfiguration>());
        }
    }

}
