using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IcbcodeCMS.Areas.CMS.Models.Repositories
{
    public class FieldRepository : BaseRepository
    {
        public List<dynamic> AllAll()
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from fields order by field_group, field_id desc;"
                ).ToList<dynamic>();

                _db_connection.Close();
            }

            return items;
        }

        public List<dynamic> GetFilterFields(long content_id)
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                List<string> ids = SqlMapper.Query<string>(_db_connection,
                "select templates.template_fields from contents join templates on (content_template = template_id) where content_id = @content_id",
                new { content_id = content_id }
                ).ToList<string>();

                items = SqlMapper.Query(_db_connection,
                "select fields.* from fields where field_hidden = false and field_is_filter = true and field_id in (" + string.Join(",", ids) + ") order by field_group, field_id desc;",
                new { content_id = content_id }
                ).ToList<dynamic>();

                _db_connection.Close();
            }

            return items;
        }

        public List<dynamic> All()
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from fields where field_hidden = false order by field_group, field_id desc;"
                ).ToList<dynamic>();

                _db_connection.Close();
            }

            return items;
        }

        public List<dynamic> All(long template_id)
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select fields.* from fields join templates on (template_id = @template_id AND find_in_set(field_id, template_fields)) where field_hidden = FALSE AND field_is_filter = true order by field_group, field_id desc;",
                new { template_id = template_id }
                ).ToList<dynamic>();

                _db_connection.Close();
            }

            return items;
        }

        public void Remove(long field_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "delete from fields where field_id = @field_id and field_system = false;",
                new { field_id = field_id }
                );

                _db_connection.Close();
            }
        }

        public dynamic GetByID(long field_id)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from fields where field_id = @field_id;",
                new { field_id = field_id }
                ).ToList();

                _db_connection.Close();
            }

            return items.Count() == 0 ? null : items.First<dynamic>();
        }

        public List<dynamic> GetByIDs(string field_ids)
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                    string.Format("select * from fields where field_id in ({0});", string.IsNullOrWhiteSpace(field_ids) ? "''" : field_ids)
                ).ToList();

                _db_connection.Close();
            }

            return items;
        }

        public void Create(string field_name, string field_friendly_name, string field_type, string field_description, string field_group, long? filed_content_root, string filed_template_name, bool field_is_filter, bool field_inside_domain, long? filed_tags_limit, string field_rel_field)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                @"insert into fields set field_name = @field_name, field_friendly_name = @field_friendly_name, field_type = @field_type, field_description = @field_description, field_group = @field_group, filed_content_root = @filed_content_root, filed_template_name = @filed_template_name, field_is_filter = @field_is_filter, field_inside_domain = @field_inside_domain, filed_tags_limit = @filed_tags_limit, field_rel_field = @field_rel_field;",
                new { field_name = field_name, field_friendly_name = field_friendly_name, field_type = field_type, field_description = field_description, field_group = field_group, filed_content_root = filed_content_root, filed_template_name = filed_template_name, field_is_filter = field_is_filter, field_inside_domain = field_inside_domain, filed_tags_limit = filed_tags_limit, field_rel_field = field_rel_field }
                );
                _db_connection.Close();
            }
        }

        public void Update(long? field_id, string field_name, string field_friendly_name, string field_type, string field_description, long? filed_content_root, string filed_template_name, bool field_is_filter, bool field_inside_domain, long? filed_tags_limit, string field_rel_field)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                @"update fields set field_id = @field_id, field_name = @field_name, field_friendly_name = @field_friendly_name, field_type = @field_type, field_description = @field_description, filed_content_root = @filed_content_root, filed_template_name = @filed_template_name, field_is_filter = @field_is_filter, field_inside_domain = @field_inside_domain, filed_tags_limit = @filed_tags_limit, field_rel_field = @field_rel_field where field_id = @field_id;",
                new { field_id = field_id, field_name = field_name, field_friendly_name = field_friendly_name, field_type = field_type, field_description = field_description, filed_content_root = filed_content_root, filed_template_name = filed_template_name, field_is_filter = field_is_filter, field_inside_domain = field_inside_domain, filed_tags_limit = filed_tags_limit, field_rel_field = field_rel_field }
                );

                _db_connection.Close();
            }
        }

        public bool Exists(string field_name)
        {
            bool exists = false;

            using (_db_connection)
            {
                _db_connection.Open();

                using (var result = SqlMapper.QueryMultiple(_db_connection,
                "select count(*) from fields where field_name = @field_name;",
                new { field_name = field_name }
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