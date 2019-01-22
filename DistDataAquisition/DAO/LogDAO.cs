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
            throw new NotImplementedException();
        }

        public override Log GetById<K>(K id)
        {
            throw new NotImplementedException();
        }

        public override IFilter ListByFilter(IPaginationFilter filter)
        {
            throw new NotImplementedException();
        }

        public override int Save(Log obj)
        {
            throw new NotImplementedException();
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
