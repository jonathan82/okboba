using MySql.Data.Entity;
using System.Data.Entity;

namespace okboba.Entities
{
    public class MySqlConfiguration : MySqlEFConfiguration
    {
        public MySqlConfiguration()
        {
            SetHistoryContext("MySql.Data.MySqlClient", (conn, schema) => new MySqlHistoryContext(conn, schema));
        }
    }
}