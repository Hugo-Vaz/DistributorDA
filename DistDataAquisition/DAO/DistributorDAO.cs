using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DistDataAquisition.Model;
using DistDataAquisition.Model.Filters;

namespace DistDataAquisition.DAO
{
    public class DistributorDAO : AbstractDAO<Model.Distibutor>
    {
        public override void Delete<K>(K id)
        {
            throw new System.NotImplementedException();
        }

        public override List<Distibutor> GetAll()
        {
            string query = @"SELECT * FROM Distributor";
            return dbHelper.GetList<Distibutor>(query);
        }

        public override Distibutor GetById<K>(K id)
        {
            string query = @"SELECT * FROM Distributor where DistributorID = @id";
            return dbHelper.GetList<Distibutor>(query, dbHelper.BuildParameter("id", id)).FirstOrDefault();
        }

        public Distibutor GetByName(string name)
        {
            string query = @"SELECT * FROM Distributor where Name = @name";
            return dbHelper.GetList<Distibutor>(query, dbHelper.BuildParameter("name", name)).FirstOrDefault();
        }

        public override IFilter ListByFilter(IPaginationFilter filter)
        {
            throw new System.NotImplementedException();
        }

        public override int Save(Distibutor obj)
        {
            throw new System.NotImplementedException();
        }

        public override Task<int> SaveAsync(Distibutor obj)
        {
            throw new System.NotImplementedException();
        }

        public override void Update<k>(k id, Distibutor obj)
        {
            throw new System.NotImplementedException();
        }
    }
}
