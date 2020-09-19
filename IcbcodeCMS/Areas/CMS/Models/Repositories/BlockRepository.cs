using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IcbcodeCMS.Areas.CMS.Models.Repositories
{
    public class BlockRepository : BaseRepository
    {
        public List<dynamic> All(long domain_id)
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from blocks where block_domain = @domain_id order by block_order;",
                new { domain_id = domain_id }
                ).ToList<dynamic>();

                _db_connection.Close();
            }

            return items;
        }

        public void UpdateOrder(long id, long order)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                @"update blocks set block_order = @order where block_id = @id;",
                new { id = id, order = order }
                );

                _db_connection.Close();
            }
        }

        public void RemoveByDomain(long domain_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                @"delete from blocks where block_domain = @domain_id;",
                new { domain_id = domain_id }
                );

                _db_connection.Close();
            }
        }

        public void Remove(long block_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                @"delete from blocks where block_id = @block_id;",
                new { block_id = block_id }
                );

                _db_connection.Close();
            }
        }

        public dynamic GetByID(long block_id)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from blocks where block_id = @block_id;",
                new { block_id = block_id }
                );

                _db_connection.Close();
            }

            return items.Count() == 0 ? null : items.First<dynamic>();
        }

        public dynamic GetByName(string block_name, string domain_name)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from blocks join domains on (block_domain = domain_id) where block_name = @block_name and if(@domain_name is null, 1 = 1, find_in_set(@domain_name, domain_name) > 0);",
                new { block_name = block_name, domain_name = domain_name }
                );

                _db_connection.Close();
            }

            return items.Count() == 0 ? null : items.First<dynamic>();
        }

        public bool Exists(string block_name, long domain_id)
        {
            bool exists = false;

            using (_db_connection)
            {
                _db_connection.Open();

                using (var result = SqlMapper.QueryMultiple(_db_connection,
                "select count(*) from blocks where block_name = @block_name and block_domain = @domain_id;",
                new { block_name = block_name, domain_id = domain_id }
                ))
                {
                    exists = result.Read<Int64>().Single() > 0;
                }

                _db_connection.Close();
            }

            return exists;
        }

        public void Create(long block_id, string block_name, string block_friendly_name, string block_templates, long? block_content_root, string block_area, string block_controller, string block_action, bool block_allow_sort, string block_order_fields, bool block_separated, long domain_id, long block_order = 100000)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                @"insert into blocks set block_id = @block_id, block_name = @block_name, block_friendly_name = @block_friendly_name, block_templates = @block_templates, block_content_root = @block_content_root, block_area = @block_area, block_controller = @block_controller, block_action = @block_action, block_allow_sort = @block_allow_sort, block_order_fields = @block_order_fields, block_separated = @block_separated, block_domain = @domain_id, block_order = @block_order;",
                new { block_id = block_id, block_name = block_name, block_friendly_name = block_friendly_name, block_templates = block_templates, block_content_root = block_content_root, block_area = block_area, block_controller = block_controller, block_action = block_action, block_allow_sort = block_allow_sort, block_order_fields = block_order_fields, block_separated = block_separated, domain_id = domain_id, block_order = block_order }
                );

                _db_connection.Close();
            }
        }

        public void UpdateRefsRoot(long block_id, long? block_content_root)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                @"update blocks set block_id = @block_id, block_content_root = @block_content_root where block_id = @block_id;",
                new { block_id = block_id, block_content_root = block_content_root }
                );

                _db_connection.Close();
            }
        }

        public void Update(long block_id, string block_name, string block_friendly_name, string block_templates, long? block_content_root, string block_area, string block_controller, string block_action, bool block_allow_sort, string block_order_fields, long domain_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                @"update blocks set block_id = @block_id, block_name = @block_name, block_friendly_name = @block_friendly_name, block_templates = @block_templates, block_content_root = @block_content_root, block_area = @block_area, block_controller = @block_controller, block_action = @block_action, block_allow_sort = @block_allow_sort, block_order_fields = @block_order_fields, block_domain = @domain_id where block_id = @block_id;",
                new { block_id = block_id, block_name = block_name, block_friendly_name = block_friendly_name, block_templates = block_templates, block_content_root = block_content_root, block_area = block_area, block_controller = block_controller, block_action = block_action, block_allow_sort = block_allow_sort, block_order_fields = block_order_fields, domain_id = domain_id }
                );

                _db_connection.Close();
            }
        }
    }
}