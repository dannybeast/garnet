using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IcbcodeCMS.Areas.CMS.Models.Repositories
{
    public class VariableRepository : BaseRepository
    {
        public List<dynamic> All(long domain_id)
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from vars where var_domain = @domain_id order by var_group;",
                new { domain_id = domain_id }
                ).ToList<dynamic>();

                _db_connection.Close();
            }

            return items;
        }

        public List<dynamic> All(string domain)
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from vars join domains on (var_domain = domain_id) where find_in_set(@domain, domain_name) > 0 order by var_group;",
                new { domain = domain }
                ).ToList<dynamic>();

                _db_connection.Close();
            }

            return items;
        }

        public List<string> AllGroups(long domain_id)
        {
            List<string> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query<string>(_db_connection,
                "select var_group from vars where var_domain = @domain_id group by var_group order by var_group;",
                new { domain_id = domain_id }
                ).ToList<string>();

                _db_connection.Close();
            }

            return items;
        }

        public dynamic GetByID(long var_id)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                @"select * from vars where var_id = @var_id;",
                new { var_id = var_id }
                );

                _db_connection.Close();
            }

            return items.Count() == 0 ? null : items.First<dynamic>();
        }

        public dynamic GetByName(string var_name)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                @"select * from vars where var_name = @var_name;",
                new { var_name = var_name }
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
                "delete from vars where var_domain = @domain_id;",
                new { domain_id = domain_id }
                );

                _db_connection.Close();
            }
        }

        public void Remove(long var_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "delete from vars where var_id = @id;",
                new { id = var_id }
                );

                _db_connection.Close();
            }
        }

        public void Update(long var_id, string var_name, string var_friendly_name, string var_value, string var_comment, string var_group, long domain_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "update vars set var_name = @var_name, var_friendly_name = @var_friendly_name, var_value = @var_value, var_comment = @var_comment, var_group = @var_group, var_domain = @domain_id where var_id = @var_id;",
                new { var_id = var_id, var_name = var_name, var_friendly_name = var_friendly_name, var_value = var_value, var_comment = var_comment, var_group = var_group, domain_id = domain_id }
                );

                _db_connection.Close();
            }
        }

        public void Create(long var_id, string var_name, string var_friendly_name, string var_value, string var_type, string var_comment, string var_group, long domain_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "insert into vars set var_id = @var_id, var_name = @var_name, var_friendly_name = @var_friendly_name, var_value = @var_value, var_type = @var_type, var_comment = @var_comment, var_group = @var_group, var_domain = @domain_id;",
                new { var_id = var_id, var_name = var_name, var_friendly_name = var_friendly_name, var_value = var_value, var_type = var_type, var_comment = var_comment, var_group = var_group, domain_id = domain_id }
                );

                _db_connection.Close();
            }
        }

        public bool Exists(string var_name, long domain_id)
        {
            bool exists = false;

            using (_db_connection)
            {
                _db_connection.Open();

                using (var result = SqlMapper.QueryMultiple(_db_connection,
                "select count(*) from vars where var_name = @var_name and var_domain = @domain_id;",
                new { var_name = var_name, domain_id = domain_id }
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