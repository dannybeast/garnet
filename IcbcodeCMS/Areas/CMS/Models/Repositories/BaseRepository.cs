using Dapper;
using System;
using System.Configuration;
using System.Data.Common;
using System.Linq;

namespace IcbcodeCMS.Areas.CMS.Models.Repositories
{
    public class BaseRepository : IDisposable
    {
        protected readonly DbConnection _db_connection;

        private readonly ConnectionStringSettings _connection_settings = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["ConnStrName"]];

        public BaseRepository()
        {
            DbProviderFactory provider_factory = DbProviderFactories.GetFactory(_connection_settings.ProviderName);

            _db_connection = provider_factory.CreateConnection();

            if (_db_connection != null)
                _db_connection.ConnectionString = _connection_settings.ConnectionString;
            else
                throw new Exception("Error while creating connetion to DB");
        }

        public void Dispose()
        {
            _db_connection.Dispose();
        }

        protected long GetTotalPages(long total_records, long page_size)
        {
            return (long)Math.Ceiling((double)total_records / page_size);
        }

        protected long GetOffset(long current_page, long page_size)
        {
            return (current_page - 1) * page_size;
        }

        public long CreateGlobalID()
        {
            long result = 0;

            using (_db_connection)
            {
                _db_connection.Open();

                using (var items = SqlMapper.QueryMultiple(_db_connection,
                @"LOCK TABLES refs WRITE; UPDATE refs SET ref_id=LAST_INSERT_ID(ref_id+1); select ref_id from refs; select LAST_INSERT_ID(); UNLOCK TABLES;"
                ))
                {
                    result = items.Read<Int64>().Single();
                }

                _db_connection.Close();
            }

            return result;
        }
    }
}