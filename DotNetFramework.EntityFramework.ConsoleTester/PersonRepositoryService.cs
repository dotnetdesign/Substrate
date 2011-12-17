using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetFramework.EntityFramework.ConsoleTester
{

    public class PersonRepositoryService : IPersonRepositoryService
    {
        public PersonData GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public PersonData GetVersion(PersonData entityData, int version)
        {
            throw new NotImplementedException();
        }

        public PersonData GetPreviousVersion(PersonData entityData)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PersonData> GetByIds(IEnumerable<Guid> ids)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PersonData> GetAll()
        {
            throw new NotImplementedException();
        }

        public PersonData Save(PersonData entityData)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PersonData> SaveAll(IEnumerable<PersonData> entityData)
        {
            throw new NotImplementedException();
        }

        public void Delete(PersonData entityData)
        {
            throw new NotImplementedException();
        }

        public void DeleteAll(IEnumerable<PersonData> entityData)
        {
            throw new NotImplementedException();
        }
    }
}
