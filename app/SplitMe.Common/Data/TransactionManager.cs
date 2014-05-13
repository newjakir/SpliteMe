using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace SplitMe.Common.Data
{
    public class TransactionManager
    {
        #region Fields =========================================================================================

		private string _dbconnectionstring = string.Empty;
		private SqlTransaction _transaction;
		private SqlConnection _connection;

		#endregion

		#region Properties =====================================================================================

		/// <summary>
		/// Obtains the current transactions.
		/// </summary>
		public SqlTransaction CurrentTransaction
		{
			get
			{
				if(_transaction == null)
				{
					// open new transaction
					CreateTransaction();
				}
				return _transaction;
			}
		}

		/// <summary>
		/// Obtain the current connection
		/// </summary>
		public IDbConnection CurrentConnection
		{
			get
			{
				return _connection;
			}
		}
		#endregion

		#region Constructor ====================================================================================
		public TransactionManager()
		{
			_dbconnectionstring = GetConnectionString();
			_transaction = null;
			_connection = null;
		}
		public TransactionManager(string connectionString)
		{
			_dbconnectionstring = connectionString;
			_transaction = null;
			_connection = null;
		}
		#endregion

		#region Methods ========================================================================================

		private static string GetConnectionString()
		{
			string dbconnstr = String.Empty;

			try
			{
				//                Console.WriteLine("getting regular db connection");
                ConnectionStringSettings testConn = ConfigurationManager.ConnectionStrings["DefaultConnection"];
				if(testConn != null)
					dbconnstr = testConn.ToString();
                //else
                //    dbconnstr = Configuration.Instance["Application Database"];
			}
			catch(Exception e)
			{
				//try import db
				//                Console.WriteLine("getting importer db connection");
				//dbconnstr = "Data Source=BS-011-PC\\MSSQL2008R2;Database=rlm_dev_new_new;UID=devuser;pwd=devuser;Connect Timeout=45;Min Pool Size=2;Max Pool Size=200;";
                dbconnstr = "Data Source=./SQLEXPRESS;Database=rlm_dev_new_new;UID=devuser;pwd=devuser;Connect Timeout=45;Min Pool Size=2;Max Pool Size=200;";
			}

			return dbconnstr;
		}

		private void CreateTransaction()
		{
			if(_connection == null)
				_connection = new SqlConnection(_dbconnectionstring);
			_connection.Open();
			_transaction = _connection.BeginTransaction();
		}

		/// <summary>
		/// Creates a sql command from the given parameters
		/// </summary>
		/// <param name="command">Sql string command will execute</param>
		/// <param name="commandType">Type of command to execute</param>
		/// <param name="tran">Transasction to execute command on, if null, one will be created</param>
		/// <returns></returns>
		protected SqlCommand PrepareCommand(string command, CommandType commandType, SqlTransaction tran)
		{
			SqlConnection conn = null;
			SqlCommand cmd = new SqlCommand(command)
			{
				CommandType = commandType,
				CommandTimeout = 300
			};

			if(tran != null)
			{
				conn = tran.Connection;
			}
			else
			{
				conn = new SqlConnection(_dbconnectionstring);
			}

			try
			{
				if(conn.State != ConnectionState.Open)
					conn.Open();
			}
			catch(TimeoutException to)
			{
				//Logger.Warn("Clearing Pool... TimeoutException", to);
				SqlConnection.ClearPool(conn);
				conn.Open();
			}
			catch(InvalidOperationException io)
			{
				//Logger.Warn("Clearing Pool... InvalidOperationException", io);
				SqlConnection.ClearPool(conn);
				conn.Open();
			}

			cmd.Connection = conn;

			if(tran != null)
				cmd.Transaction = tran;

			return cmd;
		}

		/// <summary>
		/// Executes a command from the given parameters
		/// </summary>
		/// <param name="command">Sql string command will execute</param>
		/// <param name="commandType">Type of command to execute</param>
		/// <param name="parms">List of parameters to pass into the command</param>
		public void ExecuteCommand(string command, CommandType commandType, IList<SqlParameter> parms)
		{

			try
			{
				using(SqlCommand cmd = PrepareCommand(command, commandType, CurrentTransaction))
				{

					if(parms != null && parms.Count > 0)
						cmd.Parameters.AddRange(parms.ToArray());

					LogSql(command, parms);
					string stCmd = cmd.CommandText;

					cmd.ExecuteNonQuery();
				}
			}
			catch(Exception ex)
			{
				Rollback();
				throw ex;
			}
		}

		/// <summary>
		/// Execute sql that returns a result
		/// </summary>
		/// <param name="command">Sql string command to execute</param>
		/// <param name="commandType">Type of the command</param>
		/// <param name="parms">Collection of paramters</param>
		/// <returns>Results collection from the command</returns>
		public DataRowCollection ExecuteQuery(string command, CommandType commandType, IList<SqlParameter> parms)
		{
			return ExecuteQuery(command, commandType, 60, parms);
		}

		/// <summary>
		/// Execute sql that returns a result
		/// </summary>
		/// <param name="command">Sql string command to execute</param>
		/// <param name="commandType">Type of the command</param>
		/// <param name="timeout">Command timeout in seconds</param>
		/// <param name="parms">Collection of paramters</param>
		/// <returns>Results collection from the command</returns>
		public DataRowCollection ExecuteQuery(string command, CommandType commandType, int timeout, IList<SqlParameter> parms)
		{
			DataSet ds = new DataSet();

			try
			{
				using(SqlCommand cmd = PrepareCommand(command, commandType, CurrentTransaction))
				{
					cmd.CommandTimeout = timeout;
					using(SqlDataAdapter adapter = new SqlDataAdapter(cmd))
					{

						if(parms != null && parms.Count > 0)
							cmd.Parameters.AddRange(parms.ToArray());
						LogSql(command, parms);
						adapter.Fill(ds);

						return ds.Tables[0].Rows;
					}
				}
			}
			catch(Exception ex)
			{
				Rollback();
				throw ex;
			}

		}

		/// <summary>
		/// Execute sql that returns a multiple result sets
		/// </summary>
		/// <param name="command">Sql string command to execute</param>
		/// <param name="commandType">Type of the command</param>
		/// <param name="parms">Collection of paramters</param>
		/// <returns>Result sets from the command</returns>
		public DataTableCollection ExecuteMultiQuery(string command, CommandType commandType, IList<SqlParameter> parms)
		{
			return ExecuteMultiQuery(command, commandType, 300, parms);
		}

		/// <summary>
		/// Execute sql that returns a result
		/// </summary>
		/// <param name="command">Sql string command to execute</param>
		/// <param name="commandType">Type of the command</param>
		/// <param name="timeout">Command timeout in seconds</param>
		/// <param name="parms">Collection of paramters</param>
		/// <returns>Results collection from the command</returns>
		public DataTableCollection ExecuteMultiQuery(string command, CommandType commandType, int timeout, IList<SqlParameter> parms)
		{
			DataSet ds = new DataSet();

			try
			{
				using(SqlCommand cmd = PrepareCommand(command, commandType, CurrentTransaction))
				{
					cmd.CommandTimeout = timeout;
					using(SqlDataAdapter adapter = new SqlDataAdapter(cmd))
					{

						if(parms != null && parms.Count > 0)
							cmd.Parameters.AddRange(parms.ToArray());
						LogSql(command, parms);
						adapter.Fill(ds);

						return ds.Tables;
					}
				}
			}
			catch(Exception ex)
			{
				Rollback();
				throw ex;
			}

		}

		/// <summary>
		/// Commit the transaction
		/// </summary>
		public void Commit()
		{
			if(_transaction != null)
				_transaction.Commit();
			if(_connection != null)
			{
				if(_connection.State != ConnectionState.Closed)
					_connection.Close();
				_connection.Dispose();
			}
			_transaction.Dispose();
		}

		/// <summary>
		/// Rollback the transaction
		/// </summary>
		public void Rollback()
		{
			try
			{
				if(_transaction != null)
				{
					_transaction.Rollback();
				}
				if(_connection != null)
				{
					if(_connection.State != ConnectionState.Closed)
						_connection.Close();
					_connection.Dispose();
				}
                if (_transaction != null)
                {
                    _transaction.Dispose();
                }
			}
			catch(InvalidOperationException ioe)
			{
				// do nothing, transaction already dumped
				//Logger.Debug(ioe);
			}
		}

		private void LogSql(string command, IList<SqlParameter> parms)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Executing SQL:");
			sb.AppendLine(command);
			sb.AppendLine("SQL PARMS:");
			if(parms != null)
			{
				foreach(SqlParameter p in parms)
				{
					sb.AppendLine(p.ParameterName + " = " + p.Value);
				}
			}
			//			Logger<TransactionManager>.Debug(sb.ToString());
			//           Logger.Debug(sb.ToString());
		}

		#endregion
    }
}
