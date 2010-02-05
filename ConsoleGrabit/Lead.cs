using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace ConsoleGrabit
{
    public class Lead
    {
        /// <summary>
        /// must have debt
        /// must have last name or business name
        /// must have street address and zip or street address and city and state
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return ((!_last.IsEmpty() || !_businessname.IsEmpty()) && !_debt.IsEmpty() &&
                    (!_streetaddress.IsEmpty() && (!_zip.IsEmpty() || (!_state.IsEmpty() && !_city.IsEmpty()))));
        }

        public Lead()
        {
            _city = "";
            _zip = "";
            _streetaddress = "";
            _state = "";
            _recordeddate = "";
            _middle = "";
            _last = "";
            _first = "";
            _debt = "";
        }

        #region get/set

        public string First
        {
            get { return _first; }
            set { _first = value; }
        }

        public string Last
        {
            get { return _last; }
            set { _last = value; }
        }

        public string Middle
        {
            get { return _middle; }
            set { _middle = value; }
        }

        public string Businessname
        {
            get { return _businessname; }
            set { _businessname = value; }
        }

        public string Streetaddress
        {
            get { return _streetaddress; }
            set { _streetaddress = value; }
        }

        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        public string State
        {
            get { return _state; }
            set { _state = value; }
        }

        public string Zip
        {
            get { return _zip; }
            set { _zip = value; }
        }

        public string Debt
        {
            get { return _debt; }
            set { _debt = value; }
        }

        public string Recordeddate
        {
            get { return _recordeddate; }
            set { _recordeddate = value; }
        }

        #endregion

        #region members

        private string _first;
        private string _last;
        private string _middle;
        private string _businessname;
        private string _streetaddress;
        private string _city;
        private string _state;
        private string _zip;
        private string _debt;
        private string _recordeddate;

        #endregion

    }
}
