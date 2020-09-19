using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IcbcodeCMS.Areas.CMS.Models.Repositories
{
    public class UserRepository : BaseRepository
    {
        public List<dynamic> All()
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from users join user_roles on (user_role = user_role_id) join domains on (user_domain = domain_id) order by user_id desc;"
                ).ToList<dynamic>();

                _db_connection.Close();
            }

            return items;
        }

        public List<dynamic> AllUserRoles()
        {
            List<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from user_roles;"
                ).ToList<dynamic>();

                _db_connection.Close();
            }

            return items;
        }

        public void Remove(long user_id)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "delete from users where user_id = @user_id;",
                new { user_id = user_id }
                );

                _db_connection.Close();
            }
        }

        public dynamic GetByID(long user_id)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from users where user_id = @user_id;",
                new { user_id = user_id }
                );

                _db_connection.Close();
            }

            return items.Count() == 0 ? null : items.First<dynamic>();
        }

        public dynamic GetByLogin(string user_login)
        {
            IEnumerable<dynamic> items;

            using (_db_connection)
            {
                _db_connection.Open();

                items = SqlMapper.Query(_db_connection,
                "select * from users join user_roles on (user_role = user_role_id) join domains on (user_domain = domain_id) where user_login = @user_login limit 1;",
                new { user_login = user_login }
                );

                _db_connection.Close();
            }

            return items.Count() == 0 ? null : items.First<dynamic>();
        }

        public void Update(long user_id, long user_role, string user_name, string user_login, string user_password, string user_blocks, string user_var_groups, long user_domain)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "update users set user_role = @user_role, user_name = @user_name, user_login = @user_login, user_password = @user_password, user_blocks = @user_blocks, user_var_groups = @user_var_groups, user_domain = @user_domain where user_id = @user_id;",
                new { user_id = user_id, user_role = user_role, user_name = user_name, user_login = user_login, user_password = user_password, user_blocks = user_blocks, user_var_groups = user_var_groups, user_domain = user_domain }
                );

                _db_connection.Close();
            }
        }

        public void Create(long user_id, long user_role, string user_name, string user_login, string user_password, string user_blocks, string user_var_groups, long user_domain)
        {
            using (_db_connection)
            {
                _db_connection.Open();

                SqlMapper.Execute(_db_connection,
                "insert into users set user_id = @user_id, user_role = @user_role, user_name = @user_name, user_login = @user_login, user_password = @user_password, user_blocks = @user_blocks, user_var_groups = @user_var_groups, user_domain = @user_domain;",
                new { user_id = user_id, user_role = user_role, user_name = user_name, user_login = user_login, user_password = user_password, user_blocks = user_blocks, user_var_groups = user_var_groups, user_domain = user_domain }
                );

                _db_connection.Close();
            }
        }

        public bool Exists(string user_login)
        {
            bool exists = false;

            using (_db_connection)
            {
                _db_connection.Open();

                using (var result = SqlMapper.QueryMultiple(_db_connection,
                "select count(*) from users where user_login = @user_login;",
                new { user_login = user_login }
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