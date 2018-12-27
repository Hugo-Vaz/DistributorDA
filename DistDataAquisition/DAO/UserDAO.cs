using DistDataAquisition.Model;
using DistDataAquisition.Model.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistDataAquisition.DAO
{
    public class UserDAO : AbstractDAO<User>
    {
        public override void Delete<K>(K id)
        {
            throw new NotImplementedException();
        }

        public override List<User> GetAll()
        {
            string query = "SELECT * FROM User";
            return dbHelper.GetList<User>(query);
        }

        public override User GetById<K>(K id)
        {
            string query = "SELECT * FROM User WHERE USerID = @id";
            return dbHelper.GetList<User>(query,dbHelper.BuildParameter("id",id)).FirstOrDefault();
        }

        public List<User> GetUsersForReport()
        {
            string query = "SELECT * FROM User WHERE IsActive = 1 AND ReceiveEmail = 1";
            return dbHelper.GetList<User>(query);
        }

        public override IFilter ListByFilter(IPaginationFilter filter)
        {
            throw new NotImplementedException();
        }

        public override int Save(User obj)
        {
            throw new NotImplementedException();
        }

        public override Task<int> SaveAsync(User obj)
        {
            throw new NotImplementedException();
        }

        public override void Update<k>(k id, User obj)
        {
            throw new NotImplementedException();
        }
    }
}
