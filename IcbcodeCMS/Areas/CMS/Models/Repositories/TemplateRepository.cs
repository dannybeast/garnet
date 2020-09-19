using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IcbcodeCMS.Areas.CMS.Models.Repositories
{
    public class TemplateRepository : BaseRepository
    {
        public dynamic GetByName(string template_name, long domain_id)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from templates where template_name = @template_name and template_domain = @domain_id;",
                new { template_name = template_name, domain_id = domain_id }
                );

                _db_connection.Close();
            }

            return items.Count() == 0 ? null : items.First<dynamic>();
        }

        public List<dynamic> All(long domain_id)
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from templates where template_domain = @domain_id order by template_order;",
                new { domain_id = domain_id }
                ).ToList<dynamic>();

                _db_connection.Close();
            }

            return items;
        }

        public List<dynamic> GetByIDs(string template_ids)
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                    string.Format("select * from templates where template_id in ({0}) order by template_order;", string.IsNullOrWhiteSpace(template_ids) ? "''" : template_ids)
                ).ToList();

                _db_connection.Close();
            }

            return items;
        }

        public dynamic GetByID(long template_id)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                @"select * from templates where template_id = @template_id;",
                new { template_id = template_id }
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
                "delete from templates where template_domain = @domain_id;",
                new { domain_id = domain_id }
                );

                _db_connection.Close();
            }
        }

        public void Remove(long template_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "delete from templates where template_id = @id;",
                new { id = template_id }
                );

                _db_connection.Close();
            }
        }

        public void UpdateRefsTemplates(long template_id, string template_templates)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "update templates set template_templates = @template_templates where template_id = @template_id;",
                new { template_id = template_id, template_templates = template_templates }
                );

                _db_connection.Close();
            }
        }

        public void Update(long template_id, string template_friendly_name, string template_name, bool template_allow_subpartitions, string template_fields, string template_views, string template_templates, bool template_allow_sort, string template_order_fields, bool template_create_child, long domain_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "update templates set template_name = @template_name, template_friendly_name = @template_friendly_name, template_allow_subpartitions = @template_allow_subpartitions, template_fields = @template_fields, template_views = @template_views, template_templates = @template_templates, template_allow_sort = @template_allow_sort, template_order_fields = @template_order_fields, template_create_child = @template_create_child, template_domain = @domain_id where template_id = @template_id;",
                new { template_id = template_id, template_name = template_name, template_friendly_name = template_friendly_name, template_allow_subpartitions = template_allow_subpartitions, template_fields = template_fields, template_views = template_views, template_templates = template_templates, template_allow_sort = template_allow_sort, template_order_fields = template_order_fields, template_create_child = template_create_child, domain_id = domain_id }
                );

                _db_connection.Close();
            }
        }

        public void Create(long template_id, string template_friendly_name, string template_name, bool template_allow_subpartitions, string template_fields, string template_views, string template_templates, bool template_allow_sort, string template_order_fields, bool template_create_child, long domain_id, long template_order = 100000)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "insert into templates set template_id = @template_id, template_name = @template_name, template_friendly_name = @template_friendly_name, template_allow_subpartitions = @template_allow_subpartitions, template_fields = @template_fields, template_views = @template_views, template_templates = @template_templates, template_allow_sort = @template_allow_sort, template_order_fields = @template_order_fields, template_create_child = @template_create_child, template_domain = @domain_id, template_order = @template_order;",
                new { template_id = template_id, template_name = template_name, template_friendly_name = template_friendly_name, template_allow_subpartitions = template_allow_subpartitions, template_fields = template_fields, template_views = template_views, template_templates = template_templates, template_allow_sort = template_allow_sort, template_order_fields = template_order_fields, template_create_child = template_create_child, domain_id = domain_id, template_order = template_order }
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
                @"update templates set template_order = @order where template_id = @id;",
                new { id = id, order = order }
                );

                _db_connection.Close();
            }
        }

        public bool Exists(string template_name, long domain_id)
        {
            bool exists = false;

            using (_db_connection)
            {
                _db_connection.Open();

                using (var result = SqlMapper.QueryMultiple(_db_connection,
                "select count(*) from templates where template_name = @template_name and template_domain = @domain_id;",
                new { template_name = template_name, domain_id = domain_id }
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