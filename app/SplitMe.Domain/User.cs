using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using SplitMe.Common;
using SplitMe.Common.Data;
using SplitMe.Common.Search;
using System.Data;
using System.Web.Security;
using System.Web.Mvc;
namespace SplitMe.Domain
{
    public class User
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string AgeRange { get; set; }
        public string PhoneNo { get; set; }
        public string DeviceToken { get; set; }
        public string Gender { get; set; }

        private static readonly MembershipProvider _provider = Membership.Provider;
        private static MembershipUser _user = null;

        public static User New()
        {
            return new User();
        }

        public void Register(TransactionManager manager)
        {
            bool myManager = false;
            if (manager == null)
            {
                manager = new TransactionManager();
                myManager = true;
            }

            string sql = string.Empty;
            IList<SqlParameter> parms = new List<SqlParameter>();
            parms.Add(new SqlParameter("@Email", Email));
            parms.Add(new SqlParameter("@FirstName", FirstName));
            parms.Add(new SqlParameter("@Surname", Surname));
            parms.Add(new SqlParameter("@PhoneNo", PhoneNo));
            parms.Add(new SqlParameter("@DeviceToken", DeviceToken));
            parms.Add(new SqlParameter("@AgeRange", AgeRange));
            parms.Add(new SqlParameter("@Gender", Gender));

            sql = "dbo.prd_User_Register";

            MembershipCreateStatus status;
            try
            {
                _provider.CreateUser(Email, Password, Email, "Question", "Answer", true, Guid.NewGuid(), out status);

                if (!status.Equals(MembershipCreateStatus.Success)) throw new Exception("Registration failed!!");
                manager.ExecuteCommand(sql, CommandType.StoredProcedure, parms);

                if (myManager)
                    manager.Commit();
            }
            catch (SqlException se)
            {
                if (se.Message.StartsWith("Violation of PRIMARY KEY constraint") ||
                    se.Message.StartsWith("Violation of UNIQUE KEY constraint"))
                {
                    //throw new SplitMe.Common.DuplicateException("An Ingredient for this site already exists with that name [" + Name + "]!", se);
                    throw se;
                }
                throw se;
            }
        }

        public static User Login(string email, string pass, string deviceToken)
        {
            bool myManager = false;
            TransactionManager manager = null;
            if (manager == null)
            {
                myManager = true;
                manager = new TransactionManager();
            }

            if (!_provider.ValidateUser(email, pass)) throw new Exception("Invalid Username or Password!");

            IList<SqlParameter> parms = new List<SqlParameter>{
				new SqlParameter("@Email", email),
                new SqlParameter("@DeviceToken", deviceToken)
			};

            string sql = "dbo.prd_User_Login";
            User user = null;

            DataRowCollection tdc = manager.ExecuteQuery(sql, CommandType.StoredProcedure, parms); //manager.ExecuteMultiQuery(sql, CommandType.StoredProcedure, parms);

            foreach (DataRow dr in tdc)
            {
                user = User.New();
                user.Email = dr["Email"].ToString().Trim();
                user.FirstName = dr["FirstName"].ToString().Trim();
                user.Surname = dr["Surname"].ToString().Trim();
                user.PhoneNo = dr["PhoneNo"].ToString().Trim();
                user.DeviceToken = dr["DeviceToken"].ToString().Trim();
                user.AgeRange = dr["AgeRange"].ToString().Trim();
                user.Gender = dr["Gender"].ToString().Trim();
            }

            if (myManager)
                manager.Commit();

            return user;
        }

        public static void ResetPassword(string email)
        {
            try
            {
                string password = _provider.ResetPassword(email, "Answer");
                Utility.SendPassword(email, password);
            }
            catch
            {
                throw new Exception("Invalid email!");
            }
        }

        //public void Register()
        //{
        //    string conString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //    string query = "INSERT INTO .[dbo].[Users]([Username],[Password],[FirstName],[Surname],[AgeRange],[Gender])";
        //    query += "VALUES('" + UserName + "','" + Password + "','" + FirstName + "','" + Surname + "','" + AgeRange + "','" + Gender + "')";
        //    SqlConnection con = new SqlConnection(conString);
        //    SqlCommand com = new SqlCommand(query, con);
        //    com.CommandType = System.Data.CommandType.Text;
        //    try
        //    {
        //        con.Open();
        //        com.ExecuteNonQuery();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        con.Close();
        //    }
        //}

        //public static User Login(string un, string pass)
        //{
        //    string conString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //    string query = "SELECT [Username],[Password],[FirstName],[Surname],[AgeRange],[Gender]  FROM [dbo].[Users]";
        //    query += "WHERE Username='" + un + "' AND Password='" + pass + "'";
        //    SqlConnection con = new SqlConnection(conString);
        //    SqlDataAdapter da = new SqlDataAdapter(query, con);
        //    DataSet ds = new DataSet();
        //    try
        //    {
        //        con.Open();
        //        da.Fill(ds);
        //        User user = null;
        //        foreach (DataRow dr in ds.Tables[0].Rows)
        //        {
        //            user = new RegisterModel();
        //            user.UserName = dr["Username"].ToString();
        //            user.FirstName = dr["FirstName"].ToString();
        //            user.Surname = dr["Surname"].ToString();
        //            user.AgeRange = dr["AgeRange"].ToString();
        //            user.Gender = dr["Gender"].ToString();
        //        }
        //        return user;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        con.Close();
        //    }
        //}
    }
}
