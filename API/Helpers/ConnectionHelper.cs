using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Data;
using System;
using System.Data.Odbc;
using CCSec;
namespace UrbanBank.Helpers
{

    internal static class ConnectionHelper
    {
        
        public static dynamic Open(OdbcConnection conn)
        {
            try
            {
                //var connectionString = "Dsn=CCBank; uid =" + secData.GetUID() + "; pwd =" + secData.GetPwdApp();
                // var connectionString = Regex.Replace(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString, @"\\\\+", @"\");
                var connectionString = "Dsn=CCBank; uid =dba; pwd =sql";
                conn = new OdbcConnection(connectionString);
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Open();
                return conn;
            }
            catch(Exception ex)
            {

                return null;
            }
        }

        public static void Reset(OdbcConnection conn)
        {
            conn.Close();
            conn.Open();
        }
        public static void close_conn(OdbcConnection conn)
        {
            conn.Close();
        }
    }

}