//Client

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

namespace Chatprogramm
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ClientStarten();
            InitializeTimer();
        }

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//AdressFamily.InterNetwork -> Format der Adresse, SocketType -> Datenübertragung

        public void ClientStarten()
        {
            ///globale Variablen
            int port = 45678;
            //////////////////////int outputB, inputB;
            //////////////////////string inputS;
            //////////////////////byte[] puffer = new byte[256];

            //Erstellung der Zieladresse
            //Zugriff auf Knotenpunkt(Server IP)
            byte[] IP = new byte[] { 192, 168, 2, 123 };
            IPAddress Jac = new IPAddress(IP);
            IPEndPoint ep = new IPEndPoint(Jac, port);
            //IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, port);
            //Console.WriteLine(IPAddress.Loopback.ToString());
            //Socket = Schnittstelle der PCs
            // global Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  //AdressFamily.InterNetwork -> Format der Adresse, SocketType -> Datenübertragung
            System.Threading.Thread.Sleep(3000);    //Warten (Grund?)

            try
            {
                clientSocket.Connect(ep);
                Console.WriteLine(">>> Verbindung zu Server " + clientSocket.RemoteEndPoint.ToString() + " hergestellt");
            }
            catch (Exception ex)
            {
                txt_Verlauf.Text = ex.Message;
            }
        }

        private void InitializeTimer()
        {
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            string inputString = "";
            int inputBytes = 0;
            byte[] puffer = new byte[256];

            try
            {

                if ((inputBytes = clientSocket.Receive(puffer)) != 0)
                {
                    inputString = Encoding.ASCII.GetString(puffer, 0, inputBytes);
                    //Console.Write(">>> {0} Bytes empfangen: ", inputB);
                    txt_Verlauf.Text += ("     " + inputString); //Alles empfangene ausgegeben
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }

        private void btn_Senden_Click(object sender, RoutedEventArgs e)
        {
            string inputString = "";
            int outputBytes = 0;
            byte[] puffer = new byte[256];

            inputString = txt_Nachricht.Text;    //eingabe
            puffer = Encoding.ASCII.GetBytes(inputString);   //zu bytes
            outputBytes = clientSocket.Send(puffer);    //gibt die ANzahl der bytes zurück, die gesendet wurden
            txt_Verlauf.Text += inputString;
            txt_Nachricht.Text = "";
        }
    }
}