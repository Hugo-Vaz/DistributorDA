using DistDataAcquisition.Model;
using DistDataAcquisition.Model.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistDataAcquisition.DAO
{
    public class DistributorReportDAO : AbstractDAO<DistributorReport>
    {
        public override void Delete<K>(K id)
        {
            throw new NotImplementedException();
        }

        public override List<DistributorReport> GetAll()
        {
            string query = @"SELECT * FROM DistributorReport";
            return dbHelper.GetList<DistributorReport>(query);
        }

        public override DistributorReport GetById<K>(K id)
        {
            string query = @"SELECT * FROM DistributorReport WHERE DistributorReportID = @id";
            return dbHelper.GetCompleteList<DistributorReport>(query, dbHelper.BuildParameter("id", id)).FirstOrDefault();
        }

        public List<DistributorReport> ListByTimestamp(DateTime startDate,DateTime endDate)
        {
            string query = @"SELECT * FROM DistributorReport WHERE Timestamp BETWEEN @start AND @end";
            return dbHelper.GetList<DistributorReport>(query, dbHelper.BuildParameter("start", startDate),dbHelper.BuildParameter("end",endDate));
        }

        public override IFilter ListByFilter(IPaginationFilter filter)
        {
            throw new NotImplementedException();
        }

        public void Save(List<DistributorReport> reports)
        {
            foreach(var current in reports)
            {
                this.Save(current);
            }
        }

        public override int Save(DistributorReport obj)
        {
            return dbHelper.Save(obj);
        }

        public override Task<int> SaveAsync(DistributorReport obj)
        {
            throw new NotImplementedException();
        }

        public override void Update<k>(k id, DistributorReport obj)
        {
            throw new NotImplementedException();
        }
    }
}
