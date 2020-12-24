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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Web;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private List<ViewModelComboBox1> devisesSources { get; set; }
        private List<ViewModelComboBox1> devisesDestinations { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            devisesSources = new List<ViewModelComboBox1>()
            {
                new ViewModelComboBox1("Bitcoin", "btc-bitcoin"),
                new ViewModelComboBox1(){Name = "Euro", Id = "eur-euro-token" },
                new ViewModelComboBox1(){Name = "Neurochain", Id = "ncc-neurochain" }
            };
            comboDeviseSource.ItemsSource = devisesSources;
            comboDeviseSource.DisplayMemberPath = "Name";
            comboDeviseSource.SelectedValuePath = "Id";


            devisesDestinations = new List<ViewModelComboBox1>()
            {
                new ViewModelComboBox1("USD", "usd-us-dollars"),
                new ViewModelComboBox1(){Name = "Ethereum", Id = "eth-ethereum" },
                new ViewModelComboBox1(){Name = "xrp", Id = "xrp-xrp" }
            };

            DataContext = this;
            comboDeviseDestination.ItemsSource = devisesDestinations;
            comboDeviseDestination.DisplayMemberPath = "Name";
            comboDeviseDestination.SelectedValuePath = "Id";
        }
        private double convertCurrency(string baseCurrency, string quoteCurrency, double amount)
        {
            WebRequest request = null;
            WebResponse response = null;
            StreamReader streamReader = null;
            double convertedValue = 0;
            string requestUrl = String.Format("https://api.coinpaprika.com/v1/price-converter?base_currency_id={0}&quote_currency_id={1}&amount={2}", baseCurrency, quoteCurrency, amount);
            Console.WriteLine(requestUrl);
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 |
                                                   SecurityProtocolType.Tls12;
            request = WebRequest.Create(requestUrl);
            try
            {
                response = (WebResponse)request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    streamReader = new StreamReader(stream);
                    var responseValues = JObject.Parse(streamReader.ReadToEnd());
                    convertedValue = Convert.ToDouble(responseValues["price"]);
                }

            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return convertedValue;
        }
        private void txtAmount_GotFocus(object sender, RoutedEventArgs e)
        {
            txtAmount.Clear();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtAmount.Text == "")
                MessageBox.Show("Saisir un montant numerique a convertir !");
            else if (comboDeviseSource.SelectedValue == null)
                MessageBox.Show("Veuillez choisir une source devise");
            else if (comboDeviseDestination.SelectedValue == null)
                MessageBox.Show("Veuillez choisir une destination devise");
            else
            {
                try
                {
                    double result = convertCurrency(comboDeviseSource.SelectedValue.ToString(), comboDeviseDestination.SelectedValue.ToString(), int.Parse(txtAmount.Text));
                    lblResult.Content = result.ToString() + " " + comboDeviseDestination.Text;
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error : " + ex.Message);
                }
            }
        }
    }
}
