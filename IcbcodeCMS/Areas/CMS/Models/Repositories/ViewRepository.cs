using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IcbcodeCMS.Areas.CMS.Models.Repositories
{
    public class ViewRepository : BaseRepository
    {
        public List<dynamic> All(long domain_id)
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from views where view_domain = @domain_id order by view_order;",
                new { domain_id = domain_id }
                ).ToList<dynamic>();

                _db_connection.Close();
            }

            return items;
        }

        public List<dynamic> GetByIDs(string content_view_ids)
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                    string.Format(@"select * from views where view_id in ({0}) order by view_order;", string.IsNullOrWhiteSpace(content_view_ids) ? "''" : content_view_ids)
                ).ToList();

                _db_connection.Close();
            }

            return items;
        }

        public dynamic GetByID(long view_id)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                @"select * from views where view_id = @view_id;",
                new { view_id = view_id }
                );

                _db_connection.Close();
            }

            return items.Count() == 0 ? null : items.First<dynamic>();
        }

        public void RemoveByDomain(long domain_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "delete from views where view_domain = @domain_id;",
                new { domain_id = domain_id }
                );

                _db_connection.Close();
            }
        }

        public void Remove(long view_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "delete from views where view_id = @id;",
                new { id = view_id }
                );

                _db_connection.Close();
            }
        }

        public void Update(long view_id, string view_friendly_name, string view_name, string view_path, long domain_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "update views set view_name = @view_name, view_friendly_name = @view_friendly_name, view_path = @view_path, view_domain = @domain_id where view_id = @view_id;",
                new { view_id = view_id, view_name = view_name, view_friendly_name = view_friendly_name, view_path = view_path, domain_id = domain_id }
                );

                _db_connection.Close();
            }
        }

        public void Create(long view_id, string view_friendly_name, string view_name, string view_path, long domain_id, long view_order = 100000)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "insert into views set view_id = @view_id, view_name = @view_name, view_friendly_name = @view_friendly_name, view_path = @view_path, view_order = @view_order, view_domain = @domain_id;",
                new { view_id = view_id, view_name = view_name, view_friendly_name = view_friendly_name, view_path = view_path, domain_id = domain_id, view_order = view_order }
                );

                _db_connection.Close();
            }
        }

        public void UpdateOrder(long id, long order)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                @"update views set view_order = @order where view_id = @id;",
                new { id = id, order = order }
                );

                _db_connection.Close();
            }
        }

        public bool Exists(string view_name, long domain_id)
        {
            bool exists = false;

            using (_db_connection)
            {
                _db_connection.Open();

                using (var result = SqlMapper.QueryMultiple(_db_connection,
                "select count(*) from views where view_name = @view_name and view_domain = @domain_id;",
                new { view_name = view_name, domain_id = domain_id }
                ))
                {
                    exists = result.Read<Int64>().Single() > 0;
                }

                _db_connection.Close();
            }

            return exists;
        }
    }
}