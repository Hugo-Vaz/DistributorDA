using DistDataAcquisition.Model;
using DistDataAcquisition.Model.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistDataAcquisition.DAO
{
    public class SKUDAO : AbstractDAO<SKU>
    {
        public override void Delete<K>(K id)
        {
            throw new NotImplementedException();
        }

        public override List<SKU> GetAll()
        {
            string query = @"SELECT * FROM SKU";
            return dbHelper.GetList<SKU>(query);
        }

        public override SKU GetById<K>(K id)
        {
            string query = @"SELECT * FROM SKU where SKUID = @id";
            return dbHelper.GetList<SKU>(query, dbHelper.BuildParameter("id", id)).FirstOrDefault();
        }

        public SKU GetByPartNumber(string pn)
        {
            string query = @"SELECT * FROM SKU where PartNumber = @pn";
            return dbHelper.GetList<SKU>(query, dbHelper.BuildParameter("pn", pn)).FirstOrDefault();
        }

        public override IFilter ListByFilter(IPaginationFilter filter)
        {
            throw new NotImplementedException();
        }

        public override int Save(SKU obj)
        {
            return this.dbHelper.Save(obj);

        }

        public override Task<int> SaveAsync(SKU obj)
        {
            throw new NotImplementedException();
        }

        public override void Update<k>(k id, SKU obj)
        {
            throw new NotImplementedException();
        }
    }
}
