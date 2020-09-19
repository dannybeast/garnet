using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IcbcodeCMS.Areas.CMS.Models.Repositories
{
    public class ContentRepository : BaseRepository
    {
        public bool ExistsByWhere(string where, long domain_id)
        {
            bool exists = false;

            using (_db_connection)
            {
                _db_connection.Open();

                using (var result = SqlMapper.QueryMultiple(_db_connection,
                string.Format("select count(*) from contents where {0} and content_domain = @domain_id;", where),
                new { domain_id = domain_id }
                ))
                {
                    exists = result.Read<Int64>().Single() > 0;
                }

                _db_connection.Close();
            }

            return exists;
        }

        public void Create(long content_id, long? content_view, string content_name, long content_template, string content_url, long content_block, long? content_root, string sets, long domain_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                string.Format("insert into contents set content_name = @content_name, content_id = @content_id, content_view = @content_view, content_in_sitemap = true, content_url = @content_url, content_block = @content_block, content_h1 = @content_name, content_title = @content_name, content_root = @content_root, content_template = @content_template, content_link = @content_name, content_domain = @domain_id, content_order = 100000, {0};", sets),
                new { content_id = content_id, content_view = content_view, content_url = content_url, content_name = content_name, content_template = content_template, domain_id = domain_id, content_block = content_block, content_root = content_root }
                );

                _db_connection.Close();
            }
        }

        public void Update(string sets, string where)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                string.Format("update contents set {0} where {1};", sets, where)
                );

                _db_connection.Close();
            }
        }

        #region for api

        public List<dynamic> GetActive(string[] block_name, long[] content_root, string[] template_name, string order_fields, long page, long size, out long totals, string query, dynamic where, object domain)
        {
            List<dynamic> items; totals = 0;

            var parameters = new Dapper.DynamicParameters();
            parameters.AddDynamicParams(where);
            parameters.AddDynamicParams(new { block_name = block_name, content_root = content_root, template_name = template_name, limit = size, offset = GetOffset(page, size), domain_name = (domain is string ? domain : null), domain_id = (domain is string ? null : domain) });

            using (_db_connection)
            {
                _db_connection.Open();

                using (var result = SqlMapper.QueryMultiple(_db_connection,
                string.Format(@"select SQL_CALC_FOUND_ROWS contents.*, if(content_main = true, '/', content_url) as `content_url`, views.*, blocks.*, templates.*, domains.*
                from contents left join views on (content_view = view_id) join blocks on (block_id = content_block) join templates on (template_id = content_template)
                join domains on (domain_id = content_domain)
                where if(@domain_name is null, 1 = 1, find_in_set(@domain_name, domain_name) > 0) and if(@domain_id is null, 1 = 1, @domain_id = domain_id) and content_active = true and (content_publish is null or content_publish <= now())
                and {0} and {1} and {2} and {4} order by {3} limit @limit offset @offset; select FOUND_ROWS();",
                query == null ? " 1 = 1 " : query,
                block_name == null || block_name.Length == 0 ? " 1 = 1 " : "block_name in @block_name",
                template_name == null || template_name.Length == 0 ? " 1 = 1 " : "template_name in @template_name",
                order_fields ?? "content_order",
                content_root == null || content_root.Length == 0 ? " 1 = 1 " : "content_root in @content_root"
                ),
                parameters
                ))
                {
                    items = result.Read<dynamic>().ToList();

                    totals = GetTotalPages(result.Read<Int64>().Single(), size);
                }

                _db_connection.Close();
            }

            return items;
        }

        public dynamic GetActiveMain(string domain)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                @"select contents.*, if(content_main = true, '/', content_url) as `content_url`, views.*, blocks.*, templates.*, domains.* from contents left join views on (content_view = view_id) join blocks on (block_id = content_block) join templates on (template_id = content_template) join domains on (domain_id = content_domain) where if(@domain is null, 1 = 1, find_in_set(@domain, domain_name) > 0) and content_active = true and (content_publish is null or content_publish <= now()) and content_main = true limit 1;",
                new { domain = domain }
                );

                _db_connection.Close();
            }

            return items.Count() == 0 ? null : items.First<dynamic>();
        }

        public dynamic GetActiveByUrl(string content_url, string domain)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                @"select contents.*, if(content_main = true, '/', content_url) as `content_url`, views.*, blocks.*, templates.*, domains.* from contents left join views on (content_view = view_id) join blocks on (block_id = content_block) join templates on (template_id = content_template) join domains on (domain_id = content_domain) where if(@domain is null, 1 = 1, find_in_set(@domain, domain_name) > 0) and content_active = true and (content_publish is null or content_publish <= now()) and content_url = @content_url;",
                new { content_url = content_url, domain = domain }
                );

                _db_connection.Close();
            }

            return items.Count() == 0 ? null : items.First<dynamic>();
        }

        public dynamic GetActiveByID(long content_id)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                @"select contents.*, if(content_main = true, '/', content_url) as `content_url`, views.*, blocks.*, templates.*, domains.* from contents left join views on (content_view = view_id) join blocks on (block_id = content_block) join templates on (template_id = content_template) join domains on (domain_id = content_domain) where content_active = true and (content_publish is null or content_publish <= now()) and content_id = @id;",
                new { id = content_id }
                );

                _db_connection.Close();
            }

            return items.Count() == 0 ? null : items.First<dynamic>();
        }

        public List<dynamic> SearchActive(string terms, long[] content_root, string[] block_name, string[] template_name, long page, long size, out long totals, string query, dynamic where, string domain)
        {
            List<dynamic> items; totals = 0;

            var parameters = new Dapper.DynamicParameters();
            parameters.AddDynamicParams(where);
            parameters.AddDynamicParams(new { terms = terms, block_name = block_name, template_name = template_name, content_root = content_root, limit = size, offset = GetOffset(page, size), domain = domain });

            using (_db_connection)
            {
                _db_connection.Open();

                using (var result = SqlMapper.QueryMultiple(_db_connection,
                string.Format(@"select SQL_CALC_FOUND_ROWS contents.*, if(content_main = true, '/', content_url) as `content_url`, views.*, blocks.*, templates.*, domains.*, (MATCH (content_link, content_h1, content_text, content_short_text) AGAINST (@terms)) as score
                from contents left join views on (content_view = view_id) join blocks on (block_id = content_block) join templates on (template_id = content_template)
                join domains on (domain_id = content_domain)
                where if(@domain is null, 1 = 1, find_in_set(@domain, domain_name) > 0) and (MATCH (content_link, content_h1, content_text, content_short_text) AGAINST (@terms)) and
                content_active = true and (content_publish is null or content_publish <= now())
                and {0} and {1} and {2} and {3} order by score desc limit @limit offset @offset; select FOUND_ROWS();",
                query == null ? " 1 = 1 " : query,
                block_name == null || block_name.Length == 0 ? " 1 = 1 " : "block_name in @block_name",
                template_name == null || template_name.Length == 0 ? " 1 = 1 " : "template_name in @template_name",
                content_root == null || content_root.Length == 0 ? " 1 = 1 " : "content_root in @content_root"
                ),
                parameters
                ))
                {
                    items = result.Read<dynamic>().ToList();

                    totals = GetTotalPages(result.Read<Int64>().Single(), size);
                }

                _db_connection.Close();
            }

            return items;
        }

        #endregion

        #region for cms

        public List<dynamic> GetsByParent(long content_root)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                @"select * from contents where content_root = @content_root;",
                new { content_root = content_root }
                );

                _db_connection.Close();
            }

            return items.ToList<dynamic>();
        }

        public List<dynamic> Get(long domain_id, long block_id, long? content_root, string order, long page, long size, out long totals)
        {
            List<dynamic> items; totals = 0;

            using (_db_connection)
            {
                _db_connection.Open();

                using (var result = SqlMapper.QueryMultiple(_db_connection,
                @"select SQL_CALC_FOUND_ROWS * from contents join templates on (template_id = content_template) where content_domain = @domain_id and content_block = @block_id and if(@content_root is null, content_root is null, content_root = @content_root) order by " + order + " limit @limit offset @offset; select FOUND_ROWS();",
                new { domain_id = domain_id, block_id = block_id, content_root = content_root, limit = size, offset = GetOffset(page, size) }
                ))
                {

                    items = result.Read<dynamic>().ToList();

                    totals = GetTotalPages(result.Read<Int64>().Single(), size);
                }

                _db_connection.Close();
            }

            return items;
        }

        public List<dynamic> GetByBlock(long block_id)
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from contents join templates on (template_id = content_template) where content_block = @block_id;",
                new { block_id = block_id }
                ).ToList<dynamic>();

                _db_connection.Close();
            }

            return items;
        }

        public List<dynamic> GetByDomain(long domain_id)
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from contents join templates on (template_id = content_template) where content_domain = @domain_id;",
                new { domain_id = domain_id }
                ).ToList<dynamic>();

                _db_connection.Close();
            }

            return items;
        }


        public dynamic GetByID(long content_id)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                @"select * from contents join templates on (template_id = content_template) where content_id = @id;",
                new { id = content_id }
                );

                _db_connection.Close();
            }

            return items.Count() == 0 ? null : items.First<dynamic>();
        }

        public List<dynamic> GetByParent(long content_root, string[] template_name)
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                string.Format("select * from contents join templates on (template_id = content_template) where content_root = @content_root and {0};",
                template_name == null || template_name.Length == 0 ? " 1 = 1 " : "template_name in @template_name"
                ),
                new { content_root = content_root, template_name = template_name }
                ).ToList();

                _db_connection.Close();
            }

            return items;
        }

        public List<dynamic> GetByParent(long content_root)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                @"select * from contents where content_root = @content_root;",
                new { content_root = content_root }
                );

                _db_connection.Close();
            }

            return items.ToList();
        }

        public void UpdateOrder(long id, long order)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "update contents set content_order = @order where content_id = @id;",
                new { id = id, order = order }
                );

                _db_connection.Close();
            }
        }

        public void Remove(long content_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "delete from contents where content_id = @id;",
                new { id = content_id }
                );

                _db_connection.Close();
            }
        }

        public void RemoveColumn(string column_name)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                string.Format("ALTER TABLE contents DROP {0};", column_name)
                );

                _db_connection.Close();
            }
        }

        public void AddColumn(string column_name, string column_data_type)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                string.Format("ALTER TABLE contents ADD {0} {1};", column_name, column_data_type)
                );

                _db_connection.Close();
            }
        }

        public void EditColumn(string old_column_name, string new_column_name, string column_data_type)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                string.Format("ALTER TABLE contents CHANGE {0} {1} {2};", old_column_name, new_column_name, column_data_type)
                );

                _db_connection.Close();
            }
        }

        public bool Exists(string content_url, long domain_id)
        {
            bool exists = false;

            using (_db_connection)
            {
                _db_connection.Open();

                using (var result = SqlMapper.QueryMultiple(_db_connection,
                "select count(*) from contents where content_domain = @domain_id and content_url = @content_url;",
                new { content_url = content_url, domain_id = domain_id }
                ))
                {
                    exists = result.Read<Int64>().Single() > 0;
                }

                _db_connection.Close();
            }

            return exists;
        }

        public void Create(long content_id, bool content_in_sitemap, bool content_export_rss, bool content_active, bool content_main, bool content_allow_deleted, string content_name, long? content_view, string content_url, long content_block, string content_title, string content_description, string content_keywords, string content_h1, string content_h2, string content_short_text, string content_text, DateTime? content_publish, long? content_root, long content_template, bool? content_allow_redirect, bool? content_redirect_permanent, string content_redirect_url, string content_link, string content_export_rss_ids, string content_export_rss_title, long domain_id, string user_data, long content_order = 100000)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                string.Format("insert into contents set content_id = @content_id, content_in_sitemap = @content_in_sitemap, content_export_rss = @content_export_rss, content_active = @content_active, content_main = @content_main, content_allow_deleted = @content_allow_deleted, content_name = @content_name, content_view = @content_view, content_url = @content_url, content_block = @content_block, content_title = @content_title, content_description = @content_description, content_keywords = @content_keywords, content_h1 = @content_h1, content_h2 = @content_h2, content_short_text = @content_short_text, content_text = @content_text, content_publish = @content_publish, content_root = @content_root, content_template = @content_template, content_allow_redirect = @content_allow_redirect, content_redirect_permanent = @content_redirect_permanent, content_redirect_url = @content_redirect_url, content_link = @content_link, content_export_rss_ids = @content_export_rss_ids, content_export_rss_title = @content_export_rss_title, content_domain = @domain_id, content_order = @content_order {0};", user_data),
                new { content_id = content_id, content_in_sitemap = content_in_sitemap, content_export_rss = content_export_rss, content_active = content_active, content_main = content_main, content_allow_deleted = content_allow_deleted, content_name = content_name, content_view = content_view, content_url = content_url, content_block = content_block, content_title = content_title, content_description = content_description, content_keywords = content_keywords, content_h1 = content_h1, content_h2 = content_h2, content_short_text = content_short_text, content_text = content_text, content_publish = content_publish, content_root = content_root, content_template = content_template, content_allow_redirect = content_allow_redirect, content_redirect_permanent = content_redirect_permanent, content_redirect_url = content_redirect_url, content_link = content_link, content_export_rss_ids = content_export_rss_ids, content_export_rss_title = content_export_rss_title, domain_id = domain_id, content_order = content_order }
                );

                _db_connection.Close();
            }
        }


        public void UpdateRefsRoot(long content_id, long? content_root, string user_data)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                string.Format("update contents set content_root = @content_root {0} where content_id = @content_id;", user_data),
                new { content_id = content_id, content_root = content_root }
                );

                _db_connection.Close();
            }
        }

        public void Update(long content_id, bool content_in_sitemap, bool content_export_rss, bool content_active, bool content_main, bool content_allow_deleted, string content_name, long? content_view, long content_block, string content_title, string content_description, string content_keywords, string content_h1, string content_h2, string content_short_text, string content_text, DateTime? content_publish, long? content_root, long content_template, bool? content_allow_redirect, bool? content_redirect_permanent, string content_redirect_url, string content_link, string content_export_rss_ids, string content_export_rss_title, long domain_id, string user_data)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                string.Format("update contents set content_in_sitemap = @content_in_sitemap, content_export_rss = @content_export_rss, content_active = @content_active, content_main = @content_main, content_allow_deleted = @content_allow_deleted, content_name = @content_name, content_view = @content_view, content_block = @content_block, content_title = @content_title, content_description = @content_description, content_keywords = @content_keywords, content_h1 = @content_h1, content_h2 = @content_h2, content_short_text = @content_short_text, content_text = @content_text, content_publish = @content_publish, content_root = @content_root, content_template = @content_template, content_allow_redirect = @content_allow_redirect, content_redirect_permanent = @content_redirect_permanent, content_redirect_url = @content_redirect_url, content_link = @content_link, content_export_rss_ids = @content_export_rss_ids, content_export_rss_title = @content_export_rss_title, content_domain = @domain_id {0} where content_id = @content_id;", user_data),
                new { content_id = content_id, content_in_sitemap = content_in_sitemap, content_export_rss = content_export_rss, content_active = content_active, content_main = content_main, content_allow_deleted = content_allow_deleted, content_name = content_name, content_view = content_view, content_block = content_block, content_title = content_title, content_description = content_description, content_keywords = content_keywords, content_h1 = content_h1, content_h2 = content_h2, content_short_text = content_short_text, content_text = content_text, content_publish = content_publish, content_root = content_root, content_template = content_template, content_allow_redirect = content_allow_redirect, content_redirect_permanent = content_redirect_permanent, content_redirect_url = content_redirect_url, content_link = content_link, content_export_rss_ids = content_export_rss_ids, content_export_rss_title = content_export_rss_title, domain_id = domain_id }
                );

                _db_connection.Close();
            }
        }

        #endregion
    }
}