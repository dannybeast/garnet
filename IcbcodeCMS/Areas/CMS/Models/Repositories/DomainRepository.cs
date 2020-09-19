using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IcbcodeCMS.Areas.CMS.Models.Repositories
{
    public class DomainRepository : BaseRepository
    {
        public List<dynamic> All()
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from domains;"
                ).ToList<dynamic>();

                _db_connection.Close();
            }

            return items;
        }

        public void Remove(long domain_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                @"delete from domains where domain_id = @domain_id;",
                new { domain_id = domain_id }
                );

                _db_connection.Close();
            }
        }

        public dynamic GetByID(long domain_id)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from domains where domain_id = @domain_id;",
                new { domain_id = domain_id }
                );

                _db_connection.Close();
            }

            return items.Count() == 0 ? null : items.First<dynamic>();
        }

        public dynamic GetByName(string block_name)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from blocks where block_name = @block_name;",
                new { block_name = block_name }
                );

                _db_connection.Close();
            }

            return items.Count() == 0 ? null : items.First<dynamic>();
        }

        public bool Exists(string block_name)
        {
            bool exists = false;

            using (_db_connection)
            {
                _db_connection.Open();

                using (var result = SqlMapper.QueryMultiple(_db_connection,
                "select count(*) from blocks where block_name = @block_name;",
                new { block_name = block_name }
                ))
                {
                    exists = result.Read<Int64>().Single() > 0;
                }

                _db_connection.Close();
            }

            return exists;
        }

        public void Create(long domain_id, string domain_name, string comment)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                @"insert into domains set domain_id = @domain_id, domain_name = @domain_name, domain_comment = @comment;",
                new { domain_id = domain_id, domain_name = domain_name, comment = comment }
                );

                _db_connection.Close();
            }
        }

        public void Update(long domain_id, string domain_name, string comment)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                @"update domains set domain_id = @domain_id, domain_name = @domain_name, domain_comment = @comment where domain_id = @domain_id;",
                new { domain_id = domain_id, domain_name = domain_name, comment = comment }
                );

                _db_connection.Close();
            }
        }
    }
}