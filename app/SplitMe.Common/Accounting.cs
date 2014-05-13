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

namespace SplitMe.Common
{
    public class Accounting
    {
        public static Dictionary<string, double> DoTransaction(string debtPhoneNo, string creditPhones)
        {
            TransactionManager manager = null;
            bool myManager = false;
            if (manager == null)
            {
                manager = new TransactionManager();
                myManager = true;
            }

            Dictionary<string, double> cisWithDeviceTokens = new Dictionary<string,double>();
            Dictionary<string, double> creditInformation = StringToMap(creditPhones);

            string sql = string.Empty;
            IList<SqlParameter> parms = new List<SqlParameter>();
            parms.Add(new SqlParameter("@FromPhone", debtPhoneNo));
            parms.Add(new SqlParameter("@CreditInfo", ToXml(creditInformation)));

            sql = "dbo.prd_Process_Transaction";

            try
            {
                DataRowCollection tdc = manager.ExecuteQuery(sql, CommandType.StoredProcedure, parms);

                if (myManager)
                    manager.Commit();

                foreach (DataRow dr in tdc)
                {
                    cisWithDeviceTokens.Add(dr["DeviceToken"].ToString().Trim(), Convert.ToDouble(dr["Amount"].ToString().Trim()));
                }

                return cisWithDeviceTokens;
            }
            catch (SqlException se)
            {
                if (se.Message.StartsWith("Violation of PRIMARY KEY constraint") ||
                    se.Message.StartsWith("Violation of UNIQUE KEY constraint"))
                {
                    throw se;
                }
                throw se;
            }
        }

        private static Dictionary<string, double> StringToMap(string creditPhones)
        {
            Dictionary<string, double> ci = null;
            string[] phonesWithCr = creditPhones.Trim().Split(',');

            if (phonesWithCr.Length > 0)
            {
                ci = new Dictionary<string, double>();
                foreach (string pwc in phonesWithCr)
                {
                    string[] tmp = pwc.Trim().Split('-');
                    if (!ci.ContainsKey(tmp[0]))
                        ci.Add(tmp[0], Convert.ToDouble(tmp[1].Trim()));
                    else
                        ci[tmp[0]] += Convert.ToDouble(tmp[1].Trim());
                }
            }
            return ci;
        }

        private static string ToXml(Dictionary<string, double> creditInformation)
        {
            if (creditInformation == null) return string.Empty;

            StringBuilder xml = new StringBuilder();
            xml.Append("<crs>");
            foreach (string key in creditInformation.Keys)
            {
                xml.AppendFormat("<cr p=\"{0}\" a=\"{1}\"", key, creditInformation[key]);
                xml.Append("/>");
            }
            xml.Append("</crs>");

            return xml.ToString();
        }
    }
}
