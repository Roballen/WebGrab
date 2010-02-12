namespace ConsoleGrabit.Models
{
    public class Address
    {
        public Address()
        {
            _zip = "";
            _valid = false;
            _streetaddress2 = "";
            _streetaddress1 = "";
            _state = "";
            _deliverable = false;
            _county = "";
            _city = "";
        }

        public string Streetaddress1
        {
            get { return _streetaddress1; }
            set { _streetaddress1 = value; }
        }

        public string Streetaddress2
        {
            get { return _streetaddress2; }
            set { _streetaddress2 = value; }
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

        public string County
        {
            get { return _county; }
            set { _county = value; }
        }

        public bool Deliverable
        {
            get { return _deliverable; }
            set { _deliverable = value; }
        }

        public bool Valid
        {
            get { return _valid; }
            set { _valid = value; }
        }

        private string _streetaddress1;
        private string _streetaddress2;
        private string _city;
        private string _state;
        private string _zip;
        private string _county;
        private bool _deliverable;
        private bool _valid;


    }
}


