/* 14.12.2018 von Luca Katzenberger und Jacqueline Kaefer
 * 
 **/

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
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Chatprogramm_github
{    
   
    public partial class MainWindow : Window
    {   //Globale Variablen

        const int port = 54546;
        const string broadcastadress = "255.255.255.255";
        UdpClient nachrichtenempfänger = new UdpClient(broadcastadress, port);
        UdpClient nachrichtensender = new UdpClient(port);
        Thread empfängerThread;

        public MainWindow()
        {
            InitializeComponent();
            nachrichtensender.EnableBroadcast = true;

            ThreadStart start = new ThreadStart(Receiver);


        }
        
        public void Receiver()
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port); //ANy überwacht alle Ipadressend es Netzwerks
            
        }
        public void InitializeSender()
        {
            nachrichtenempfänger = new 
        }
    }
}
