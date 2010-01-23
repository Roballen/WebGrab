using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IEAutomation;


namespace GrabIt
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var driver = new IEDriver();

            driver.Navigate("http://74.8.243.133/ALIS/WW400R.HTM?WSIQTP=LR09D&WSKYCD=E");
            //driver.ClickButton("");

            //start date W9FDTA
            //end date W9TDTA mmddyyyy

            string datetime = DateTime.Now.AddDays(-1).ToString("ddMMyyyyy");
            driver.SetInputStringValue(datetime,"W9FDTA");

            driver.ClickDefaultSubmit("");


        }
    }
}
