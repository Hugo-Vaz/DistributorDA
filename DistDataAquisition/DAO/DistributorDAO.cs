using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DistDataAcquisition.Model;
using DistDataAcquisition.Model.Filters;

namespace DistDataAcquisition.DAO
{
    public class DistributorDAO : AbstractDAO<Model.Distributor>
    {
        public override void Delete<K>(K id)
        {
            throw new System.NotImplementedException();
        }

        public override List<Distributor> GetAll()
        {
            string query = @"SELECT * FROM Distributor";
            return dbHelper.GetList<Distributor>(query);
        }

        public override Distributor GetById<K>(K id)
        {
            string query = @"SELECT * FROM Distributor where DistributorID = @id";
            return dbHelper.GetList<Distributor>(query, dbHelper.BuildParameter("id", id)).FirstOrDefault();
        }

        public Distributor GetByName(string name)
        {
            string query = @"SELECT * FROM Distributor where Name = @name";
            return dbHelper.GetList<Distributor>(query, dbHelper.BuildParameter("name", name)).FirstOrDefault();
        }

        public override IFilter ListByFilter(IPaginationFilter filter)
        {
            throw new System.NotImplementedException();
        }

        public override int Save(Distributor obj)
        {
            throw new System.NotImplementedException();
        }

        public override Task<int> SaveAsync(Distributor obj)
        {
            throw new System.NotImplementedException();
        }

        public override void Update<k>(k id, Distributor obj)
        {
            throw new System.NotImplementedException();
        }
    }
}
