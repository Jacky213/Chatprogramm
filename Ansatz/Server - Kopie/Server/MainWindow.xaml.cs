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
            ServerStarten();
            InitializeTimer();
        }

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //AddresseFamily berzeichnet Format der Adresse SocketType datenübertragung Protokoll Tcp

        public void ServerStarten()
        {
            ////////////string inputS;
            ////////////int inputB, outputB;
            int port = 45678;
            ////////////////byte[] puffer = new byte[256];

            //Erstellung der Zieladresse hier Ausgangsadresse

            byte[] IP = new byte[] { 192, 168, 2, 123 };
            IPAddress Ich = new IPAddress(IP);
            IPEndPoint ep = new IPEndPoint(Ich, port);
            //IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, port);
            //global    Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //AddresseFamily berzeichnet Format der Adresse SocketType datenübertragung Protokoll Tcp
            //Erstellen des Knotenpunktes Socket ist quasi die Schnittstelle der Computer
            serverSocket.Bind(ep);
            //Wie viele Leute können sich verbinden
            serverSocket.Listen(5);

            try
            {
                txt_Verlauf.Text = ">>> Server läuft";
                Socket socket = serverSocket.Accept();
                txt_Verlauf.Text = "--> Client angemeldet";
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

                if ((inputBytes = serverSocket.Receive(puffer)) != 0)
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
            outputBytes = serverSocket.Send(puffer);    //gibt die ANzahl der bytes zurück, die gesendet wurden
            txt_Verlauf.Text += inputString;
            txt_Nachricht.Text = "";
        }
    }
}
