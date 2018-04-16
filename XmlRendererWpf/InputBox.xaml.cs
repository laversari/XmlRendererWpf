using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using XmlRendererWpf.Models;

namespace XmlRendererWpf
{
    /// <summary>
    /// Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBox : Window
    {
        private Database _dbClient;
        private string _inputText;

        public InputBox(Database db, string xmlText)
        {
            InitializeComponent();
            _dbClient = db;
            _inputText = xmlText;
            txtName.Focus();
        }

        private void btnOk(object sender, RoutedEventArgs e)
        {
            var lst = new List<InsertData>();

            lst.Add(new InsertData("Name", txtName.Text));
            lst.Add(new InsertData("Value", _inputText));
            _dbClient.Insert("xml", lst);
            this.Close();
        }

        private void btnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
