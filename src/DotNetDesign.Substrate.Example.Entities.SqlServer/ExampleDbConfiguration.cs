using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;

namespace DotNetDesign.Substrate.Example.Entities.SqlServer
{
    class ExampleDbConfiguration : DbMigrationsConfiguration<ExampleDb>
    {
        public ExampleDbConfiguration()
        {
            AutomaticMigrationsEnabled = true;
        }
    }

}
