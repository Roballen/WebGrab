namespace ConsoleGrabit.Models
{
    public class TaxDocument
    {
        public TaxDocument()
        {
            _url = "";
            _type = "";
            _pages = 0;
            _disklocation = "";
            
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public int Pages
        {
            get { return _pages; }
            set { _pages = value; }
        }

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string Disklocation
        {
            get { return _disklocation; }
            set { _disklocation = value; }
        }

        private string _disklocation;
        private string _url;
        private int _pages;
        private string _type;
    }
}


