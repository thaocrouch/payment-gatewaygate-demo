using Microsoft.Data.SqlClient;

namespace Infrastructure.Persistence;

public class DbConnected
{
    private readonly string _connectString;

    public DbConnected(string connectString)
    {
        _connectString = connectString;
    }

    public SqlConnection Db => new SqlConnection(_connectString);
}