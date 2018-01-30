using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace IdentityServer4.MicroService.Services
{
    public class SqlService
    {
       string  connectionString { get; set; }

       SqlConnection conn
        {
            get
            {
                return new SqlConnection(connectionString);
            }
        }

        public SqlService(IOptions<ConnectionStrings> Configuration)
        {
            connectionString = Configuration.Value.DefaultConnection;
        }

        public int ExecuteNonQuery(string cmdText, params SqlParameter[] cmdParms)
        {
            using (conn)
            {
                SqlCommand cmd = conn.CreateCommand();
                PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, cmdParms);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public int ExecuteNonQuery(CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            using (conn)
            {
                SqlCommand cmd = conn.CreateCommand();
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public SqlDataReader ExecuteReader(string cmdText, CommandType cmdType = CommandType.Text, params SqlParameter[] cmdParms)
        {
            using (conn)
            {
                SqlCommand cmd = conn.CreateCommand();
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return rdr;
            }
        }

        public object ExecuteScalar(string cmdText, CommandType cmdType = CommandType.Text, params SqlParameter[] cmdParms)
        {
            using (conn)
            {
                SqlCommand cmd = conn.CreateCommand();
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] commandParameters)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandType = cmdType;
            //attach the command parameters if they are provided
            if (commandParameters != null && commandParameters.Length > 0)
            {
                AttachParameters(cmd, commandParameters);
            }
        }

        void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            foreach (SqlParameter p in commandParameters)
            {
                //check for derived output value with no value assigned
                if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }
                command.Parameters.Add(p);
            }
        }
    }
}
