using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetDesign.Common;
using System.Data;

namespace DotNetDesign.Substrate.Example.Entities.SqlServer
{
    public class UserRepositoryService : BaseRepositoryService, IUserRepositoryService
    {
        public UserRepositoryService()
            : base()
        {

        }

        public void Delete(Guid id, Dictionary<string, string> scopeContext)
        {
            using (TraceTopics.SqlServer.Scope())
            {
                var usersToDelete = DbContext.Users.Where(x => x.Id == id);
                foreach (var userData in usersToDelete)
                {
                    DbContext.Users.Remove(userData);
                }
                DbContext.SaveChanges();
            }
        }

        public void DeleteAll(IEnumerable<Guid> ids, Dictionary<string, string> scopeContext)
        {
            using (TraceTopics.SqlServer.Scope())
            {
                foreach (var userId in ids)
                {
                    Delete(userId, scopeContext);
                }
            }
        }

        public IEnumerable<UserData> GetAll(Dictionary<string, string> scopeContext)
        {
            using (TraceTopics.SqlServer.Scope())
            {
                return from user in DbContext.Users
                       select user;
            }
        }

        public UserData GetById(Guid id, Dictionary<string, string> scopeContext)
        {
            using (TraceTopics.SqlServer.Scope())
            {
                return (from user in DbContext.Users
                        where user.Id == id
                        select user).FirstOrDefault();
            }
        }

        public IEnumerable<UserData> GetByIds(IEnumerable<Guid> ids, Dictionary<string, string> scopeContext)
        {
            using (TraceTopics.SqlServer.Scope())
            {
                return from user in DbContext.Users
                       where ids.Contains(user.Id)
                       select user;
            }
        }

        public UserData GetVersion(Guid id, int version, Dictionary<string, string> scopeContext)
        {
            throw new NotImplementedException();
        }

        public UserData Save(UserData entityData, Dictionary<string, string> scopeContext)
        {
            using (TraceTopics.SqlServer.Scope())
            {
                AttachIfNotAttached(entityData, (entityData.Version == 1) ? EntityState.Added : EntityState.Modified);
                DbContext.SaveChanges();
                return entityData;
            }
        }

        public IEnumerable<UserData> SaveAll(IEnumerable<UserData> entityData, Dictionary<string, string> scopeContext)
        {
            using (TraceTopics.SqlServer.Scope())
            {
                return entityData.Select(x => Save(x, scopeContext));
            }
        }
    }
}
