using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IcbcodeCMS.Areas.CMS.Models.Repositories
{
    public class ImageRepository : BaseRepository
    {
        public List<dynamic> All(long domain_id)
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select images.* from images join contents on (image_group = content_id) and content_domain = @domain_id;",
                new { domain_id = domain_id }
                ).ToList<dynamic>();

                _db_connection.Close();
            }

            return items;
        }

        public void Save(long image_id, string extension, string guid, string field, string image_desc = "", int image_order = 100000, long? image_ref = null)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "insert into images set image_id = @image_id, image_extension = @extension, image_group = @group, image_field = @field, image_desc = @image_desc, image_order = @image_order, image_ref = @image_ref;",
                new { image_id = image_id, extension = extension, group = guid, field = field ?? string.Empty, image_desc = image_desc, image_order = image_order, image_ref = image_ref }
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
                "delete from images where image_id = @id;",
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
                @"delete from images where image_ref = @id;",
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
                "update images set image_ref = @id, image_group = @id where image_group = @guid;",
                new { id = refID, guid = guid }
                );

                _db_connection.Close();
            }
        }

        public void UpdateDescr(long imageID, string description)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "update images set image_desc = @image_desc where image_id = @image_id;",
                new { image_desc = description, image_id = imageID }
                );

                _db_connection.Close();
            }
        }


        //
        public List<dynamic> Search(string terms, string[] block_name, string field, long? content_id, long page, long size, out long totals, string query, dynamic where)
        {
            List<dynamic> items; totals = 0;

            var parameters = new Dapper.DynamicParameters();
            parameters.AddDynamicParams(where);
            parameters.AddDynamicParams(new { terms = terms, block_name = block_name, field = field, guid = content_id, content_id = content_id, limit = size, offset = GetOffset(page, size) });

            using (_db_connection)
            {
                _db_connection.Open();

                using (var result = SqlMapper.QueryMultiple(_db_connection,
                string.Format(@"select SQL_CALC_FOUND_ROWS images.*, (MATCH (image_desc) AGAINST (@terms)) as score
                from images join contents on (image_group = content_id) join blocks on (block_id = content_block)
                where (MATCH (image_desc) AGAINST (@terms)) and
                if(@guid = -1, 1 = 1, image_group = @guid) and
                {1}
                and image_field = @field
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
            parameters.AddDynamicParams(new { fields = (fields == null || fields.Length == 0 ? new string[] { string.Empty } : fields), guid = content_id, content_id = content_id, limit = size, offset = GetOffset(page, size) });

            using (_db_connection)
            {
                _db_connection.Open();

                using (var result = SqlMapper.QueryMultiple(_db_connection,
                string.Format(@"select SQL_CALC_FOUND_ROWS images.*
                from images
                where
                if(@guid = -1, 1 = 1, image_group = @guid)
                and image_field in @fields
                and {0} order by image_order limit @limit offset @offset; select FOUND_ROWS();",
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

        public List<dynamic> Get(long content_id, string field, long page, long size, out long totals)
        {
            List<dynamic> items; totals = 0;

            using (_db_connection)
            {
                _db_connection.Open();

                using (var result = SqlMapper.QueryMultiple(_db_connection,
                "select SQL_CALC_FOUND_ROWS * from images where image_group = @guid and image_field = @field order by image_order limit @limit offset @offset; select FOUND_ROWS();",
                new { guid = content_id, field = field, limit = size, offset = GetOffset(page, size) }
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
                "select * from images where image_group = @guid and image_field = @field order by image_order;",
                new { guid = guid, field = field }
                ).ToList<dynamic>();

                _db_connection.Close();
            }

            return items;
        }

        public dynamic Get(long image_id)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from images where image_id = @image_id;",
                new { image_id = image_id }
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
                "update images set image_order = @order where image_id = @id;",
                new { id = id, order = order }
                );

                _db_connection.Close();
            }
        }

        public void UpdateMode(long id, bool crop)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "update images set image_crop = @crop where image_id = @id;",
                new { id = id, crop = crop }
                );

                _db_connection.Close();
            }
        }
    }
}