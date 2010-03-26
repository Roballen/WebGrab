using System.Collections.Generic;
using Utilities;

namespace ConsoleGrabit.Models
{

public enum LeadStatus
        {
            Complete, MissingInformation, Error, Duplicate
        }

    public enum LeadType
    {
        State, Federal
    }

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
                    (!_address.Streetaddress1.IsEmpty() && (!_address.Zip.IsEmpty() || (!_address.State.IsEmpty() && !_address.City.IsEmpty()))));
        }

        public Lead()
        {
            _messages = new List<Message>();
            _document = new TaxDocument();
            _address = new Address();
            _recordeddate = "";
            _middle = "";
            _last = "";
            _first = "";
            _debt = "";
            _businessname = "";
            _leadtype = LeadType.Federal;
            _status = LeadStatus.MissingInformation;
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

        public Address Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public TaxDocument Document
        {
            get { return _document; }
            set { _document = value; }
        }

        public IList<Message> Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }
        public LeadStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public string Book
        {
            get { return _book; }
            set { _book = value; }
        }

        public string Page
        {
            get { return _page; }
            set { _page = value; }
        }

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public LeadType LeadType
        {
            get { return _leadtype; }
            set { _leadtype = value; }
        }
        #endregion
        #region members

        private LeadType _leadtype;
        private  string _book;
        private string _page;
        private string _id;
        private LeadStatus _status;
        private IList<Message> _messages;
        private TaxDocument _document;
        private Address _address;
        private string _first;
        private string _last;
        private string _middle;
        private string _businessname;
        private string _debt;
        private string _recordeddate;

        #endregion

    }
}


