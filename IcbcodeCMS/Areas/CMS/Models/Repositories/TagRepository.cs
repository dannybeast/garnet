using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace IcbcodeCMS.Areas.CMS.Models.Repositories
{
    public class TagRepository : BaseRepository
    {
        public dynamic GetByID(long tag_id)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                @"select * from tags where tag_id = @tag_id;",
                new { tag_id = tag_id }
                );

                _db_connection.Close();
            }

            return items.Count() == 0 ? null : items.First<dynamic>();
        }

        public List<dynamic> Get(string tag_field_name, long? tag_domain)
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                    "select * from fields left join tags on (tag_field_name = field_name) where field_type = 'tags' and if(@tag_field_name is null, 1 = 1, field_name = @tag_field_name ) and if(@tag_domain is null or tag_domain is null, 1 = 1, tag_domain = @tag_domain);",
                    new { tag_field_name = tag_field_name, tag_domain = tag_domain }
                ).ToList();

                _db_connection.Close();
            }

            return items;
        }

        public void RemoveByDomain(long domain_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "delete from tags where tag_domain = @domain_id;",
                new { domain_id = domain_id }
                );

                _db_connection.Close();
            }
        }

        public void Remove(long tag_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "delete from tags where tag_id = @id;",
                new { id = tag_id }
                );

                _db_connection.Close();
            }
        }

        public void Update(long tag_id, string tag_name, string tag_field_name, long domain_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "update tags set tag_name = @tag_name, tag_domain = @domain_id, tag_field_name= @tag_field_name where tag_id = @tag_id;",
                new { tag_id = tag_id, tag_name = tag_name, tag_field_name = tag_field_name, domain_id = domain_id }
                );

                _db_connection.Close();
            }
        }

        public void Create(long tag_id, string tag_name, string tag_field_name, long domain_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "insert into tags set tag_id = @tag_id, tag_name = @tag_name, tag_domain = @domain_id, tag_field_name = @tag_field_name;",
                new { tag_id = tag_id, tag_name = tag_name, tag_field_name = tag_field_name, domain_id = domain_id }
                );

                _db_connection.Close();
            }
        }
    }
}