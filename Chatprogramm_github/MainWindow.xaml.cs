/* 14.12.2018 von Luca Katzenberger und Jacqueline Kaefer
 * 
 */

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
    // J. Codierte Nachrichten verschicken mit Kontakten abgleichen in Main und in Klasse hinzufügen
    // send and receive in eine Klasse auslagern
    // J. Nachrichten schön darstellen

    // L. Kontakte als Liste und darstellen und zwischen Chats wechseln 
    //(Einmal Kontakthinzufügenbutton und Kontakte abgleichen bei neuer Nachricht und Mitteilung an User ob zu Kontakten hinzufügen. 
    //(neuer Dialog zur Entscheidung) Button Kontakt löschen)
    
    //Speicherung in XML-Format zu zweit nach Weihnachten
   
    public partial class MainWindow : Window
    {  
        //Globale Variablen
        delegate void AddMessage(string message);
        const int port = 54546;
        const string broadcastaderss = "255.255.255.255";
        UdpClient nachrichtenempfänger;// = new UdpClient(new IPEndPoint(new IPAddress(broadcastadress), port));
        UdpClient nachrichtensender;// = new UdpClient(port);
        Thread empfängerThread;
        User Mainuser = new User();
        Nachricht ZumSenden;
        Nachricht EmpfangeneNachricht;
        int row = 0;
        List<User> Kontaktliste = new List<User>();

        User hilfe = new User("Michael"); //Zum Testen!!


        public MainWindow()
        {
            InitializeComponent();

            Usernamen_festlegen();          
            
            //Sender initialisieren
            nachrichtensender = new UdpClient(broadcastaderss, port);
            nachrichtensender.EnableBroadcast = true;

            //Empfänger initialisieren
            nachrichtenempfänger = new UdpClient(port);            

            //Parallerer Thread für das Empfangen von Nachrichten anlegen
            ThreadStart start = new ThreadStart(Receiver);
            empfängerThread = new Thread(start);
            empfängerThread.IsBackground = true;
            empfängerThread.Start();
            //grid_Verlauf.Height = 80;
        }
        
        public void Receiver() //Könnte man in Klasse auslagern??
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port); //Any überwacht alle IP-Adressen des Netzwerks
            AddMessage messageDelegate = MessageReceived;
            while(true)
            {
                byte[] data = nachrichtenempfänger.Receive(ref endpoint); // ref --> Verweis
                string message = Encoding.Unicode.GetString(data);
                Dispatcher.Invoke(messageDelegate, message);    //führt einen Delegaten aus
            }
        }
       
        public void MessageReceived(string message) //Anzeigen der Message
        {                        
            EmpfangeneNachricht = Nachricht.NachrichtDecodieren(message);
            TextBox Tb_nachricht = new TextBox();
            Tb_nachricht = EmpfangeneNachricht.EmpfangeneNachrichtAusgabe();           
            grid_Verlauf.Height = grid_Verlauf.Height + (Tb_nachricht.Height + 1) * Tb_nachricht.FontSize;
            grid_Verlauf.RowDefinitions.Add(new RowDefinition());
            if (EmpfangeneNachricht.Sender.Username == Mainuser.Username)
            {
                Grid.SetColumn(Tb_nachricht, 1);
                Grid.SetRow(Tb_nachricht, row);
                row++;
            }
            else
            {
                Grid.SetColumn(Tb_nachricht, 0);
                Grid.SetRow(Tb_nachricht, row);
                row++;
            }
            if (row>=9)
            {
                row = 0;
            }
            grid_Verlauf.Children.Add(Tb_nachricht);            
        }

        //Sendet einen String
        public void Send(string eingabe) //Könnten wir auch auslagern
        {
            ZumSenden = new Nachricht(eingabe, Mainuser, hilfe, DateTime.Now, true);
            byte[] data = ZumSenden.NachrichtCodieren();            
            nachrichtensender.Send(data, data.Length);
        }      

        //Nachricht wird bei Klick auf Sendenbtn gesendet
        private void btn_Senden_Click(object sender, RoutedEventArgs e)
        {
            string nachricht = txt_Nachricht.Text;
            Send(nachricht);
            txt_Nachricht.Text = "";
        }

        //Methode, die den Usernamen festlegt
        private void Usernamen_festlegen()      //in Klasse auslagern?!
        {
            //Ein Dialog zur Eingabe des Usernamens wird erstellt und angezeigt            
            Username_Dialog dlg = new Username_Dialog();
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                Mainuser = dlg.ReturnUser();
            }
            //War die Erstellung des Users erfolgreich, wird eine Messagebox angezeigt
            MessageBox.Show("Sie haben sich als " + Mainuser.Username + " angemeldet.", "Anmeldung erfolgreich");
            //Usernameeingabe war erfolgreich wird auch ausgegeben.
        }

        private void btn_KontaktHinzufügen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Username_Dialog dlg = new Username_Dialog();
                dlg.ShowDialog();
                if (dlg.DialogResult == true)
                {
                    User NeuerKontakt = dlg.ReturnUser();   //Usernamen des Kontakts abfragen
                    Kontaktliste.Add(NeuerKontakt);     //Kontakt zur Kontakrliste hinzufügen
                    ListboxKontakte.ItemsSource = Kontaktliste;
                    MessageBox.Show("Der Kontakt " + NeuerKontakt.Username + " wurde erfolgreich zur Kontaktliste hinzugefügt.", "Kontakt hinzugefügt");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }
    }
}