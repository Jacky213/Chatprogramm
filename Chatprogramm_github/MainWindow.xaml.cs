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
    // Usernamen bei unbekanntem Absender anzeigen

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
        Sender nachrichtensender; 
        delegate void AddMessage(Nachricht EmpfangeneNachricht);
        const int port = 54546;
        UdpClient nachrichtenempfänger;            

        Thread empfängerThread;
        User Mainuser = new User();
        
        int row = 0;
        List<User> Kontaktliste = new List<User>();

        public MainWindow()
        {
            InitializeComponent();

            Usernamen_festlegen();

            ////Sender initialisieren
            nachrichtensender = new Sender();
            ////Empfänger initialisieren
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
            AddMessage messageDelegate = NachrichtDarstellen;
            while (true)
            {
                byte[] data = nachrichtenempfänger.Receive(ref endpoint); // ref --> Verweis
                string message = Encoding.Unicode.GetString(data);
                Nachricht EmpfangeneNachricht = new Nachricht();
                EmpfangeneNachricht = Nachricht.NachrichtDecodieren(message);
                Dispatcher.Invoke(messageDelegate, EmpfangeneNachricht);    //führt einen Delegaten aus
            }
        }

        //Braucht eine neue Funktion Nachricht_empfangen, die die neue Nachricht als delegate empfängt, die Speichern-Funktion und anschliessend die NachrichtenDarstellenfunktion aufruft.

        public void NachrichtDarstellen(Nachricht EmpfangeneNachricht)   // Soll nur die Nachrichten der Datei darstellen muss also den Kontakt kennen der dargestellt wird. Listbox.SelectedItemIndex der Kontaktliste
        {                                                               //Hat keine Argumente mehr. Soll dann die Ladenfunktion aufrufen über eine foreach-Schleife alle Nachrichten darstellen.
            //Nachricht EmpfangeneNachricht = Nachricht.NachrichtDecodieren(nachricht);
            TextBox Tb_nachricht = new TextBox();   //neue Textbox erstellen
            Tb_nachricht = EmpfangeneNachricht.NachrichtinTextbox();
            grid_Verlauf.Height = grid_Verlauf.Height + (Tb_nachricht.Height + 1) * Tb_nachricht.FontSize;
            grid_Verlauf.RowDefinitions.Add(new RowDefinition());

            if (EmpfangeneNachricht.Sender.Username == Mainuser.Username) //Bin ich Sender?
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

                if (ListeBeinhaltet(Kontaktliste, EmpfangeneNachricht.Sender))  //Ist der Absender in der Kontaktliste?
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

            //if (row >= 9)   //Wieder oben anfangen
            //{
            //    row = 0;
            //}
            //grid_Verlauf.Children.Add(Tb_nachricht);
        }
    

        //Nachricht wird bei Klick auf Sendenbtn gesendet
        private void btn_Senden_Click(object sender, RoutedEventArgs e)
        {
            string nachrichtentext = txt_Nachricht.Text;
            if (ListboxKontakte.SelectedIndex >= 0)     //Ist ein Kontakt ausgewählt?
            {
                nachrichtensender.send(new Nachricht(nachrichtentext, Mainuser, Kontaktliste[ListboxKontakte.SelectedIndex], DateTime.Now, true));  //Könnte eine Exception werfen, wenn kein Kontakt ausgewählt ist                
            }
            else
            {
                MessageBox.Show("Sie müssen einen Kontakt auswählen", "Kein Kontakt ausgewählt");
            }
            txt_Nachricht.Text = "";
        }

        //Senden mit Enter Funktioniert noch nicht
        private void txt_Nachricht_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {                
                btn_Senden_Click(sender, e);               
            }
        }

        //Methode, die den Usernamen festlegt
        private void Usernamen_festlegen()
        {//Username Laden Funktion aufrufen und schauen ob was zurück gibt oder nicht.  Wenn nicht Dialog aufrufen
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
                KontaktInKontaktliste(NeuerKontakt);    //Zur Kontaktliste hinzufügen
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

        private bool ListeBeinhaltet(List<User> Liste, User Kontakt)    //Hat mit .Contains nicht funktioniert
        {
            for (int i = 0; i < Liste.Count ; i++)
            {
                if (Liste[i].Username == Kontakt.Username)
                {
                    return true;
                }
            }
            return false;
        }

        private void btn_KontaktLöschen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Kontaktliste.Remove(Kontaktliste[ListboxKontakte.SelectedIndex]);   //Kontakt löschen
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bitte wählen Sie erst einen Kontakt aus", "Fehler");
            }

            ListboxKontakte.Items.Clear();  //Listbox leeren
            foreach (User Kontakt in Kontaktliste)
            {
                ListboxKontakte.Items.Add(Kontakt); //Alle Kontakte in Listbox übertragen
            }
            ListboxKontakte.UpdateLayout(); //Layout aktualisieren
        }
    }
}
 