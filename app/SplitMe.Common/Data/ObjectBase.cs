using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using SplitMe.Common;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace SplitMe.Common.Data
{
    public class ObjectBase
    {
        /// <summary>
        /// Collection of validation errors 
        /// </summary>
        public IDictionary<string, string> Validation { get; protected set; }

        /// <summary>
        /// Determination if the object is valid or not
        /// </summary>

        protected ObjectBase()
        {
            Validation = new Dictionary<string, string>();
        }

        /// <summary>
        /// Create factory method
        /// </summary>
        /// <returns>new ObjectBase</returns>
        public static ObjectBase New()
        {
            return new ObjectBase();
        }

        protected bool OverWireSafe(string value)
        {
            return Regex.IsMatch(value, @"^[a-zA-Z0-9\s\._\-\/]*(\#{1})?([a-zA-Z0-9\s\._\-\/]+)?$");
        }

        /// <summary>
        /// Tests that a string is a valid decimal value
        /// </summary>
        /// <param name="value">String to test</param>
        /// <returns>result of the test</returns>
        protected bool ValidateDecimal(string value)
        {
            return Regex.IsMatch(value, @"^[0-9]+(\.[0-9]+)?$");
        }

        /// <summary>
        /// Tests that a string is a valid integer value
        /// </summary>
        /// <param name="value">String to test</param>
        /// <returns>result of the test</returns>
        protected bool ValidateInt(string value)
        {
            return Regex.IsMatch(value, @"^[0-9]{1,9}$");
        }

        /// <summary>
        /// Does a simple string replace to prevent sql injection attacks
        /// </summary>
        /// <param name="input">String to scrub</param>
        /// <returns>Clean string value</returns>
        protected string ScrubForSql(string input)
        {
            return input.Replace("'", "''");
        }

        private bool _isDirty;
        private bool _isNew = true;

        /// <summary>
        /// Determines if the object has been changed
        /// </summary>
        public virtual bool IsDirty
        {
            get { return _isDirty; }
        }

        /// <summary>
        /// Determins if the object is new
        /// </summary>
        public bool IsNew
        {
            get { return _isNew; }
            set { _isNew = value; }
        }

        /// <summary>
        /// Marks the object as being changed
        /// </summary>
        protected void MarkDirty()
        {
            _isDirty = true;
        }

        /// <summary>
        /// Marks the object as being not new and not changed
        /// </summary>
        protected void MarkClean()
        {
            _isDirty = false;
        }

        /// <summary>
        /// Marks the object as being new
        /// </summary>
        protected void MarkAsNew()
        {
            _isNew = true;
        }

        /// <summary>
        /// Marks the object as being not old but not changed
        /// </summary>
        protected void MarkAsOld()
        {
            _isNew = false;
            _isDirty = false;
        }
    }
}
