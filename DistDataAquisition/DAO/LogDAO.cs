using DistDataAcquisition.DAO;
using DistDataAcquisition.Model.Filters;
using DistDataAquisition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistDataAquisition.DAO
{
    public class LogDAO : AbstractDAO<Log>
    {
        public override void Delete<K>(K id)
        {
            throw new NotImplementedException();
        }

        public override List<Log> GetAll()
        {
            string query = @"SELECT * FROM Log";
            return dbHelper.GetList<Log>(query);
        }

        public override Log GetById<K>(K id)
        {
            string query = @"SELECT * FROM Log WHERE LogID = @id";
            return dbHelper.GetCompleteList<Log>(query, dbHelper.BuildParameter("id", id)).FirstOrDefault();
        }

        public override IFilter ListByFilter(IPaginationFilter filter)
        {
            throw new NotImplementedException();
        }

        public override int Save(Log obj)
        {
            return dbHelper.Save(obj);
        }


        public void Save(List<Log> logs)
        {
            foreach (var current in logs)
            {
                this.Save(current);
            }
        }

        public override Task<int> SaveAsync(Log obj)
        {
            throw new NotImplementedException();
        }

        public override void Update<k>(k id, Log obj)
        {
            throw new NotImplementedException();
        }
    }
}
