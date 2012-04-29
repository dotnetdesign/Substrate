using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerformanceTestConsole
{
    public class PersonRepositoryService : IPersonRepositoryService
    {
        public void Delete(Guid id, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public void DeleteAll(IEnumerable<Guid> ids, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PersonData> GetAll(Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public PersonData GetById(Guid id, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PersonData> GetByIds(IEnumerable<Guid> ids, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public PersonData GetVersion(Guid id, int version, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public PersonData Save(PersonData entityData, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PersonData> SaveAll(IEnumerable<PersonData> entityData, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }
    }
}
