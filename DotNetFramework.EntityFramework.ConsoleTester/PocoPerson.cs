using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetDesign.EntityFramework;

namespace DotNetFramework.EntityFramework.ConsoleTester
{
    public class PocoPerson : BaseLogger<PocoPerson>
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public uint Age { get; set; }

        public PocoPerson()
        {
            using (Logger.Scope())
            {
                Id = Guid.NewGuid();
                Version = 1;
                CreatedAt = DateTime.Now;
            }
        }
    }
}
