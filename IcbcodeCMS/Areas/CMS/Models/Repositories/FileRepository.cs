using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IcbcodeCMS.Areas.CMS.Models.Repositories
{
    public class FileRepository : BaseRepository
    {
        public void Save(long file_id, string extension, string guid, string desc, int length, string field, DateTime? file_publish = null, int file_order = 100000, long? file_ref = null)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "insert into files set file_id = @file_id, file_extension = @extension, file_group = @group, file_desc = @desc, file_size = @length, file_field = @field, file_publish = @publish, file_order = @file_order, file_ref = @file_ref;",
                new { file_id = file_id, extension = extension, group = guid, desc = desc, length = length, field = field, publish = file_publish ?? DateTime.Now, file_order, file_ref = file_ref }
                );

                _db_connection.Close();
            }
        }

        public void Remove(long imageID)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "delete from files where file_id = @id;",
                new { id = imageID }
                );

                _db_connection.Close();
            }
        }

        public void RemoveByRef(long content_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                @"delete from files where file_ref = @id;",
                new { id = content_id }
                );

                _db_connection.Close();
            }
        }

        public void Update(long refID, string guid)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "update files set file_ref = @id, file_group = @id where file_group = @guid;",
                new { id = refID, guid = guid }
                );

                _db_connection.Close();
            }
        }

        public void UpdateDescr(long file_id, string file_desc)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "update files set file_desc = @file_desc where file_id = @file_id;",
                new { file_desc = file_desc, file_id = file_id }
                );

                _db_connection.Close();
            }
        }

        public List<dynamic> Get(long content_id, long page, long size, out long totals)
        {
            List<dynamic> items; totals = 0;

            using (_db_connection)
            {
                _db_connection.Open();

                using (var result = SqlMapper.QueryMultiple(_db_connection,
                "select SQL_CALC_FOUND_ROWS * from files where if(@guid = -1, 1 = 1, file_group = @guid) order by file_order limit @limit offset @offset; select FOUND_ROWS();",
                new { guid = content_id, limit = size, offset = GetOffset(page, size) }
                ))
                {
                    items = result.Read<dynamic>().ToList();

                    totals = GetTotalPages(result.Read<Int64>().Single(), size);
                }

                _db_connection.Close();
            }

            return items;
        }

        public List<dynamic> Get(string guid)
        {
            return Get(guid, string.Empty);
        }

        public List<dynamic> Get(string guid, string field)
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from files where file_group = @guid and file_field = @field order by file_order;",
                new { guid = guid, field = field }
                ).ToList<dynamic>();

                _db_connection.Close();
            }

            return items;
        }

        public List<dynamic> All(long domain_id)
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select files.* from files join contents on (file_group = content_id) and content_domain = @domain_id;",
                new { domain_id = domain_id }
                ).ToList<dynamic>();

                _db_connection.Close();
            }

            return items;
        }

        public dynamic Get(long file_id)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from files where file_id = @file_id;",
                new { file_id = file_id }
                );

                _db_connection.Close();
            }

            return items.Count() == 0 ? null : items.First<dynamic>();
        }

        public void UpdateOrder(long id, long order)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "update files set file_order = @order where file_id = @id;",
                new { id = id, order = order }
                );

                _db_connection.Close();
            }
        }

        public List<dynamic> Search(string terms, string[] block_name, string field, long? content_id, long page, long size, out long totals, string query, dynamic where)
        {
            List<dynamic> items; totals = 0;

            var parameters = new Dapper.DynamicParameters();
            parameters.AddDynamicParams(where);
            parameters.AddDynamicParams(new { terms = terms, block_name = block_name, field = field, icbcode_guid = content_id, content_id = content_id, limit = size, offset = GetOffset(page, size) });

            using (_db_connection)
            {
                _db_connection.Open();

                using (var result = SqlMapper.QueryMultiple(_db_connection,
                string.Format(@"select SQL_CALC_FOUND_ROWS files.*, (MATCH (file_desc) AGAINST (@terms)) as score
                from files join contents on (file_group = content_id) join blocks on (block_id = content_block)
                where (MATCH (file_desc) AGAINST (@terms)) and
                if(@icbcode_guid = -1, 1 = 1, file_group = @icbcode_guid) and
                {1}
                and file_field = @field
                and {0} order by score desc limit @limit offset @offset; select FOUND_ROWS();",
                query == null ? " 1 = 1 " : query,
                block_name == null || block_name.Length == 0 ? " 1 = 1 " : "block_name in @block_name"
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

        public List<dynamic> Get(long content_id, string[] fields, long page, long size, out long totals, string query, dynamic where)
        {
            List<dynamic> items; totals = 0;

            var parameters = new Dapper.DynamicParameters();
            parameters.AddDynamicParams(where);
            parameters.AddDynamicParams(new { fields = (fields == null || fields.Length == 0 ? new[] { string.Empty } : fields), guid = content_id, content_id = content_id, limit = size, offset = GetOffset(page, size) });

            using (_db_connection)
            {
                _db_connection.Open();

                using (var result = SqlMapper.QueryMultiple(_db_connection,
                string.Format(@"select SQL_CALC_FOUND_ROWS files.*
                from files
                where
                if(@guid = -1, 1 = 1, file_group = @guid)
                and file_field in @fields
                and {0} order by file_order limit @limit offset @offset; select FOUND_ROWS();",
                query == null ? " 1 = 1 " : query
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
    }
}