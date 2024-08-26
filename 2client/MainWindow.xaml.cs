using System.IO;
using System.Net.Sockets;
using System.Net;
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

namespace _2client
{
    public partial class MainWindow : Window
    {
        private UdpClient udpClient;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SendQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var serverIp = ServerIpTextBox.Text;
                var products = ProductListTextBox.Text;
                var endpoint = new IPEndPoint(IPAddress.Parse(serverIp), 11000);

                using (udpClient = new UdpClient())
                {
                    byte[] queryBytes = Encoding.UTF8.GetBytes(products);
                    await udpClient.SendAsync(queryBytes, queryBytes.Length, endpoint);

                    var recipeResult = await udpClient.ReceiveAsync();
                    string recipe = Encoding.UTF8.GetString(recipeResult.Buffer);
                    RecipeTextBlock.Text = recipe;

                    var imageResult = await udpClient.ReceiveAsync();
                    using (var stream = new MemoryStream(imageResult.Buffer))
                    {
                        var image = new BitmapImage();
                        stream.Position = 0;
                        image.BeginInit();
                        image.StreamSource = stream;
                        image.EndInit();
                        RecipeImage.Source = image;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}