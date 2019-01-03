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
    /// <To-Do>

    // J. Codierte Nachrichten verschicken mit Kontakten abgleichen in Main und in Klasse hinzufügen
    // send and receive in eine Klasse auslagern
    // J. Nachrichten schön darstellen

    // L. Kontakte als Liste und darstellen     !erledigt!
    // zwischen Chats wechseln -> Erst speichern der Nachrichten 
    // Kontakthinzufügenbutton  !erledigt!
    // Kontakte abgleichen bei neuer Nachricht und Mitteilung an User ob zu Kontakten hinzufügen 
    // neuer Dialog zur Entscheidung    !erledigt!
    // Button Kontakt löschen    !erledigt!

    // Speicherung in XML-Format zu zweit nach Weihnachten

    /// </To-Do>

    public partial class MainWindow : Window
    {  
        //Globale Variablen
        delegate void AddMessage(string message);
        const int port = 54546;
        const string broadcastadress = "255.255.255.255";
        UdpClient nachrichtenempfänger;// = new UdpClient(new IPEndPoint(new IPAddress(broadcastadress), port));
        UdpClient nachrichtensender;// = new UdpClient(port);
        Thread empfängerThread;
        User Mainuser = new User();
        Nachricht ZumSenden;
        Nachricht EmpfangeneNachricht;
        int row = 0;
        List<User> Kontaktliste = new List<User>();

        public MainWindow()
        {
            InitializeComponent();

            Usernamen_festlegen();          
            
            //Sender initialisieren
            nachrichtensender = new UdpClient(broadcastadress, port);
            nachrichtensender.EnableBroadcast = true;

            //Empfänger initialisieren
            nachrichtenempfänger = new UdpClient(port);            

            //Paralleler Thread für das Empfangen von Nachrichten anlegen
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

            NachrichtDarstellen();
        }

        public void NachrichtDarstellen()
        {
            TextBox Tb_nachricht = new TextBox();   //neue Textbox erstellen
            Tb_nachricht = EmpfangeneNachricht.EmpfangeneNachrichtAusgabe();
            grid_Verlauf.Height = grid_Verlauf.Height + (Tb_nachricht.Height + 1) * Tb_nachricht.FontSize;
            grid_Verlauf.RowDefinitions.Add(new RowDefinition());
            if (EmpfangeneNachricht.Sender.Username == Mainuser.Username)
            {
                Grid.SetColumn(Tb_nachricht, 1);
                Grid.SetRow(Tb_nachricht, row);
                row++;
                grid_Verlauf.Children.Add(Tb_nachricht);    //Anzeigen
            }
            else if (EmpfangeneNachricht.Empfänger.Username == Mainuser.Username)   //Ist die Nachricht an mich adressiert
            {
                Grid.SetColumn(Tb_nachricht, 0);
                Grid.SetRow(Tb_nachricht, row);
                row++;

                if (Kontaktliste.Contains(EmpfangeneNachricht.Sender))  //Ist der Absender in der Kontaktliste?
                {
                    grid_Verlauf.Children.Add(Tb_nachricht);    //Anzeigen
                }
                else    //Absender nicht in Kontaktliste
                {
                    FremderKontakt_Dialog fremderKontakt_Dialog1 = new FremderKontakt_Dialog();
                    fremderKontakt_Dialog1.ShowDialog();
                    if (fremderKontakt_Dialog1.DialogResult == true)
                    {
                        if (fremderKontakt_Dialog1.Ergebnis == true)    //Benutzer soll als Kontakt gespeichert werden
                        {
                            User FremderBenutzer = new User(EmpfangeneNachricht.Sender.Username);
                            KontaktInKontaktliste(FremderBenutzer);
                            grid_Verlauf.Children.Add(Tb_nachricht);
                        }
                    }
                }
            }

            if (row >= 9)   //Wieder oben anfangen
            {
                row = 0;
            }
            //grid_Verlauf.Children.Add(Tb_nachricht);
        }

        //Sendet einen String
        public void Send(string eingabe) //Könnten wir auch auslagern
        {
            if (ListboxKontakte.SelectedIndex >= 0)     //Ist ein Kontakt ausgewählt?
            {
                ZumSenden = new Nachricht(eingabe, Mainuser, Kontaktliste[ListboxKontakte.SelectedIndex], DateTime.Now, true);  //Könnte eine Exception werfen, wenn kein Kontakt ausgewählt ist
                byte[] data = ZumSenden.NachrichtCodieren();
                nachrichtensender.Send(data, data.Length);
            }
            else
            {
                MessageBox.Show("Sie müssen einen Kontakt auswählen", "Kein Kontakt ausgewählt");
            }
        }      

        //Nachricht wird bei Klick auf Sendenbtn gesendet
        private void btn_Senden_Click(object sender, RoutedEventArgs e)
        {
            string nachricht = txt_Nachricht.Text;
            Send(nachricht);
            txt_Nachricht.Text = "";
        }

        //Senden mit Enter
        private void txt_Nachricht_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string nachricht = txt_Nachricht.Text;
                Send(nachricht);
                txt_Nachricht.Text = "";
            }
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
            MessageBox.Show("Sie haben sich als \"" + Mainuser.Username + "\" angemeldet.", "Anmeldung erfolgreich");
            //Usernameeingabe war erfolgreich wird auch ausgegeben.
        }

        private void btn_KontaktHinzufügen_Click(object sender, RoutedEventArgs e)  //Kontakt hinzufügen
        {
            Username_Dialog dlg = new Username_Dialog();
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                User NeuerKontakt = dlg.ReturnUser();   //Usernamen des Kontakts abfragen
                KontaktInKontaktliste(NeuerKontakt);
            }
        }

        private void KontaktInKontaktliste(User NeuerKontakt)
        {
            Kontaktliste.Add(NeuerKontakt);     //Kontakt zur Kontaktliste hinzufügen

            //ListboxKontakte.ItemsSource = Kontaktliste;   //hat immer nur den ersten Kontakt angezeigt

            ListboxKontakte.Items.Clear();  //Listbox leeren
            foreach (User Kontakt in Kontaktliste)
            {
                ListboxKontakte.Items.Add(Kontakt); //Alle Kontakte in Listbox übertragen
            }
            ListboxKontakte.UpdateLayout(); //Layout aktualisieren
        }
    }
}
 