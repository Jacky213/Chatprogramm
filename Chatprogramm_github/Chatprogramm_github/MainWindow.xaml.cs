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
        delegate void AddMessage(string message);
        const int port = 54546;
        const string broadcastaderss = "255.255.255.255";

        UdpClient nachrichtenempfänger;// = new UdpClient(new IPEndPoint(new IPAddress(broadcastadress), port));
        UdpClient nachrichtensender;// = new UdpClient(port);
        Thread empfängerThread;

        public MainWindow()
        {
          


            InitializeComponent();
           
            
            nachrichtensender = new UdpClient(broadcastaderss, port);
            
            nachrichtenempfänger = new UdpClient(port);

            nachrichtensender.EnableBroadcast = true;

            ThreadStart start = new ThreadStart(Receiver);
            empfängerThread = new Thread(start);
            empfängerThread.IsBackground = true;
            empfängerThread.Start();
            Send("Hallo");
            


        }
        
        public void Receiver()
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port); //ANy überwacht alle Ipadressend es Netzwerks
            AddMessage messageDelegate = MessageReceived;
            while(true)
            {
               byte[] data = nachrichtenempfänger.Receive(ref endpoint); // ref --> Verweis
                string message = Encoding.ASCII.GetString(data);
                //System.Windows.Invoke(messageDelegate, message); //führt einen Delegaten aus
                Dispatcher.Invoke(messageDelegate, message);
            }
        }
       
        public void MessageReceived(string message)
        {
            my_lbl.Content += message;
        }
        public void Send(string eingabe)
        {
            byte[] data = Encoding.ASCII.GetBytes(eingabe);
            nachrichtensender.Send(data, data.Length);
        }

        private void btn_senden_Click(object sender, RoutedEventArgs e)
        {
            Send("Jacqueline");
        }
    }
}
