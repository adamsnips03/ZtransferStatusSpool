using AdoNetCore.AseClient;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ZTransferStatusSpool.Models;

namespace ZTransferStatusSpool.Services
{
    public class RequestByStatus
    {
        SystemSettings _connection;
        private Logger _logger;
        public RequestByStatus(SystemSettings connection)
        {
            _connection = connection;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public List<RequestRef> GetAllRequest()
        {
            IDbCommand command;
            IDbConnection conn = new AseConnection(_connection.Sybase);


            var res = new List<RequestRef>();
            try
            {

                if (conn.State != ConnectionState.Open)
                    conn.Open();
                command = conn.CreateCommand();

                command.Connection = conn;
                command.CommandText = @"select reference from zenbase..zt_ztransfer_transactions as t 
                                        where trans_status = @trans_status and 
                                        (DATEDIFF(minute,t.create_dt , getdate())) >= @timeDiff";

                var param = command.CreateParameter();
                param.DbType = DbType.String;
                param.ParameterName = "@trans_status";
                param.Direction = ParameterDirection.InputOutput;
                param.Value = _connection.TransStatus;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.DbType = DbType.Int32;
                param.ParameterName = "@timeDiff";
                param.Direction = ParameterDirection.InputOutput;
                param.Value = _connection.TimeDiff;
                command.Parameters.Add(param);

                using (IDataReader reader = Task.FromResult(command.ExecuteReader(CommandBehavior.CloseConnection)).Result)
                {
                    while (reader != null && reader.Read())
                    {
                        var dd = new RequestRef()
                        {
                            OriginTracerNo = reader["reference"].ToString(),                          
                        };                      
                      
                        res.Add(dd);
                    }
                }              

                conn.Close();
            }
            catch (Exception ex)
            {
                _logger.Error("GetAllRequest: " + ex.Message);
            }

            return res;
        }       

    }
}
