using DistDataAcquisition.Model.Filters;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DistDataAcquisition.DAO
{
    public abstract class AbstractDAO<T> where T : new()
    {
        protected Dictionary<string, string[]> Filterable = new Dictionary<string, string[]>();
        protected DatabaseHelper dbHelper;
        protected string basequery;
        protected string TableName;

        public AbstractDAO()
        {
            dbHelper = DatabaseHelper.Create();
            this.Filterable = this.GetFilter();
        }

        public virtual Dictionary<string, string[]> GetFilter()
        {
            foreach (PropertyInfo prop in this.GetProperties<T>())
            {
                Filterable.Add(prop.Name, new string[] { prop.Name, "=" });
            }

            return Filterable;
        }
        public abstract void Delete<K>(K id);
        public abstract Task<int> SaveAsync(T obj);
        public abstract int Save(T obj);
        public abstract void Update<k>(k id, T obj);
        public abstract T GetById<K>(K id);
        public abstract List<T> GetAll();
        public abstract IFilter ListByFilter(IPaginationFilter filter);


        public List<T> ListByFilter(params SqlParameter[] list)
        {
            string select = "SELECT * FROM " + this.TableName + " WHERE 1 = 1 ";

            foreach (SqlParameter c in list)
            {
                if (this.Filterable.ContainsKey(c.ParameterName))
                {
                    select += string.Format("AND {0} {1} @{0} ", Filterable[c.ParameterName][0], Filterable[c.ParameterName][1]);
                }
            }

            return dbHelper.GetList<T>(select, list);
        }
        public virtual T FindByFilter(params SqlParameter[] list)
        {
            string select = "SELECT TOP 1 * FROM " + this.TableName + " WHERE 1 = 1 ";

            foreach (SqlParameter c in list)
            {
                if (this.Filterable.ContainsKey(c.ParameterName))
                {
                    select += string.Format("AND {0} {1} @{0} ", Filterable[c.ParameterName][0], Filterable[c.ParameterName][1]);
                }
            }

            return dbHelper.GetList<T>(select, list).FirstOrDefault();
        }
        

        protected void Join(string jointable, string localkey = null, string joinoperator = "=", string remotekey = "ID")
        {
            localkey = string.IsNullOrEmpty(localkey) ? jointable + "ID" : localkey;
            this.basequery += "INNER JOIN " + jointable + " ON " + this.TableName + "." + localkey + " " + joinoperator + " " + jointable + "." + remotekey + " ";
        }
        protected void JoinTrough(string pivottable, string jointable, string localkey = null, string joinoperator = "=", string remotekey = "ID")
        {
            localkey = string.IsNullOrEmpty(localkey) ? jointable + "ID" : localkey;
            this.basequery += "INNER JOIN " + jointable + " ON " + pivottable + "." + localkey + " " + joinoperator + " " + jointable + "." + remotekey + " ";
        }
        protected void MountFilter(dynamic json)
        {
            if(json.filters != null)
            {
                basequery += "WHERE 1 = 1 ";

                foreach (var filter in json.filters)
                {
                    if (this.Filterable.ContainsKey(filter.key.Value))
                    {
                        if (this.Filterable[filter.key.Value][1].Equals("like"))
                        {
                            basequery += string.Format("AND {0} {1} '%{2}%' ", Filterable[filter.key.Value][0], Filterable[filter.key.Value][1], filter.value.Value);
                        }
                        else if (this.Filterable[filter.key.Value][1].Equals("IN"))
                        {
                            basequery += string.Format("AND {0} {1} ({2}) ", Filterable[filter.key.Value][0], Filterable[filter.key.Value][1], filter.value.Value);
                        }
                        else
                        {
                            basequery += string.Format("AND {0} {1} '{2}' ", Filterable[filter.key.Value][0], Filterable[filter.key.Value][1], filter.value.Value);
                        }
                    }
                }
            }  
        }
        protected void TotalOfPages(IFilter vo)
        {
            vo.total = dbHelper.GetInt("SELECT TOP(1) COUNT(*) OVER () " + basequery) ?? 0;
        }

        protected void Pagination(IPaginationFilter filter, IFilter vo)
        {
            int per_page = filter.per_page;
            int page = filter.page;

            decimal lastpage = Math.Ceiling((vo.total / per_page));

            vo.last_page = lastpage > 0 ? (int)Math.Ceiling(lastpage) : 1;

            if (page <= lastpage)
            {
                vo.from = 1;
                vo.to = per_page;
                if (page > 1)
                {
                    int offset = (page - 1) * per_page;
                    basequery += " OFFSET " + offset.ToString() + " ROWS ";
                    vo.to = per_page * page;
                    vo.from = (vo.to - per_page) + 1;
                }
                else
                {
                    basequery += " OFFSET 0 ROWS ";
                }

                basequery += "FETCH NEXT " + per_page.ToString() + " ROWS ONLY";

                if (vo.to > vo.total)
                {
                    vo.to = (int)vo.total;
                }

                vo.per_page = per_page;
                vo.current_page = page;
            }
        }
        protected PropertyInfo[] GetProperties<K>()
        {
            K obj = default(K);
            obj = Activator.CreateInstance<K>();
            return obj.GetType().GetProperties();
        }
}
}
