using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DATN.Data
{
    public class DapperHelper
    {
        private readonly IConfiguration _config;

        public DapperHelper(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection CreateConnection()
            => new SqlConnection(_config.GetConnectionString("DefaultConnection"));
    }
}
