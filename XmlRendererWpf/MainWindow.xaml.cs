using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;
using XmlRendererWpf.Models;

namespace XmlRendererWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string _dbName = "Data Source=xmldatabase.db";
        static Database dbClient = new Database(_dbName);

        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists(_dbName))
            {
                dbClient.CreateDatabase(_dbName);
            }

            var items = dbClient.SelectItems();

            lstView.ItemsSource = items;

        }

        private void BtnLoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".xml",
                Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                InitialDirectory = Environment.CurrentDirectory

            };

            bool? file = fileDialog.ShowDialog();

            if (file == true)
            {
                txtInput.Clear();
                txtInput.Text = File.ReadAllText(fileDialog.FileName);
                string fileName = fileDialog.FileName;
            }
        }

        private void BtnSaveXml(object sender, RoutedEventArgs e)
        {
            var inputBox = new InputBox(dbClient, txtInput.Text);
            inputBox.ShowDialog();

            var items = dbClient.SelectItems();
            lstView.ItemsSource = items;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtInput.Clear();
            txtInput.Text = dbClient.SelectXmlItem(lstView.SelectedItem.ToString());

        }

        private void Button_Render(object sender, RoutedEventArgs e)
        {
            try
            {
                RenderZone.Children.Clear();


                var xmlDoc = new XmlDocument();

                string inputString = txtInput.Text;

                StringBuilder sb = new StringBuilder();
                string[] parts = inputString.Split(new char[] { ' ', '\n', '\t', '\r', '\f', '\v', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                int size = parts.Length;
                for (int i = 0; i < size; i++)
                    sb.AppendFormat("{0} ", parts[i]);

                TextReader xt = new StringReader(sb.ToString());

                xmlDoc.Load(xt);
  
                var root = xmlDoc.SelectNodes("/Screen");

                foreach (XmlNode nodes in root)
                {
                    foreach (XmlNode rows in nodes.ChildNodes)
                    {
                        foreach (XmlNode columns in rows.ChildNodes)
                        {
                            foreach (XmlNode item in columns.ChildNodes)
                            {
                                switch (item.Name)
                                {
                                    case "Textbox":
                                        RenderTextBox(item);
                                        break;
                                    case "Numericbox":
                                        RenderNumericBox(item);
                                        break;
                                    case "LabelElement":
                                        RenderLabel(item);
                                        break;
                                    case "Button":
                                        RenderButton(item);
                                        break;
                                    default:
                                        MessageBox.Show("Invalid element name!");
                                        RenderZone.Children.Clear();
                                        break;
                                }
                            }
                        }
                    }
                }

                RenderZone.IsHitTestVisible = false;

            }
            catch
            {
                MessageBox.Show("Something goes wrong, please check your XML file and try again!", "Error to render",  MessageBoxButton.OK, MessageBoxImage.Error);
            }
           
        }

        private class NumericBox : TextBox
        {
            protected override void OnPreviewTextInput(TextCompositionEventArgs e)
            {
                e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
                base.OnPreviewTextInput(e);
            }
        }

     
        #region Render Items
        private void RenderTextBox(XmlNode item)
        {
            var txtLabel = new Label
            {
                Content = item.Attributes["label"].Value
            };

            var txtBox = new TextBox
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1, 1, 1, 1)
            };

            if (Convert.ToInt32(item.Attributes["length"].Value) > 0)
            {
                txtBox.MaxLength = Convert.ToInt32(item.Attributes["length"].Value);
            }
            else
            {
                txtBox.MaxLength = 20;
            }

            txtBox.Width = Convert.ToDouble(item.Attributes["width"].Value);
            txtBox.Height = Convert.ToDouble(item.Attributes["height"].Value);
            Canvas.SetTop(txtBox, Convert.ToInt32(item.Attributes["top"].Value) + 23);
            Canvas.SetLeft(txtBox, Convert.ToInt32(item.Attributes["left"].Value));
            Canvas.SetTop(txtLabel, Convert.ToInt32(item.Attributes["top"].Value));
            Canvas.SetLeft(txtLabel, Convert.ToInt32(item.Attributes["left"].Value));

            RenderZone.Children.Add(txtBox);
            RenderZone.Children.Add(txtLabel);


        }

        private void RenderNumericBox(XmlNode item)
        {
            var txtLabel = new Label();
            txtLabel.Content = item.Attributes["label"].Value;

            var numBox = new NumericBox();
            numBox.BorderBrush = Brushes.Black;
            numBox.BorderThickness = new Thickness(1, 1, 1, 1);
            numBox.MaxLength = Convert.ToInt32(item.Attributes["length"].Value);

            Canvas.SetTop(txtLabel, Convert.ToDouble(item.Attributes["top"].Value));
            Canvas.SetLeft(txtLabel, Convert.ToDouble(item.Attributes["left"].Value));
            numBox.Width = Convert.ToDouble(item.Attributes["width"].Value);
            numBox.Height = Convert.ToDouble(item.Attributes["height"].Value);
            Canvas.SetTop(numBox, Convert.ToInt32(item.Attributes["top"].Value) + 23);
            Canvas.SetLeft(numBox, Convert.ToInt32(item.Attributes["left"].Value));


            RenderZone.Children.Add(numBox);
            RenderZone.Children.Add(txtLabel);
        }

        private void RenderLabel(XmlNode item)
        {
            var txtLabel = new Label();
            txtLabel.Content = item.Attributes["label"].Value;

            switch(item.Attributes["type"].Value)
            {
                case ("small"):
                    txtLabel.FontSize = 8;
                    break;
                case ("medium"):
                    txtLabel.FontSize = 12;
                    break;
                case ("big"):
                    txtLabel.FontSize = 16;
                    break;
                default:
                    break;
            }

           Canvas.SetTop(txtLabel, Convert.ToDouble(item.Attributes["top"].Value));
           Canvas.SetLeft(txtLabel, Convert.ToDouble(item.Attributes["left"].Value));

           RenderZone.Children.Add(txtLabel);
        }

        private void RenderButton(XmlNode item)
        {
            var btn = new Button();
            btn.Content = item.Attributes["label"].Value;
            btn.Width = Convert.ToDouble(item.Attributes["width"].Value);
            btn.Height = Convert.ToDouble(item.Attributes["height"].Value);
            Canvas.SetTop(btn, Convert.ToDouble(item.Attributes["top"].Value));
            Canvas.SetLeft(btn, Convert.ToDouble(item.Attributes["left"].Value));
            RenderZone.Children.Add(btn);
        }
        #endregion

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RenderZone.IsHitTestVisible = (bool)chkInteractive.IsChecked;
        }
    }
}


