/* 14.12.2018 von Luca Katzenberger und Jacqueline Kaefer
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        //Globale Variablen und Objekte
        delegate void AddMessage(Nachricht EmpfangeneNachricht);
        const int port = 54546;
        Sender nachrichtensender;
        UdpClient nachrichtenempfänger;
        Thread empfängerThread;
        User Mainuser = new User();        
        int row = 0;
        List<User> Kontaktliste = new List<User>();

        public MainWindow() //Initialisierung
        {
            InitializeComponent();

            //Mainuser abfragen/initialisieren
            Mainuser = Laden.Username_Laden();
            MessageBox.Show("Sie haben sich als \"" + Mainuser.Username + "\" angemeldet.", "Anmeldung erfolgreich");

            //Kontaktliste abfragen
            Kontaktliste = Laden.Kontakte_laden();
            KontaktlisteinListbox();

            //Sender initialisieren
            nachrichtensender = new Sender();
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
            AddMessage messageDelegate = NachrichtEmpfangen;
            while (true)
            {
                byte[] data = nachrichtenempfänger.Receive(ref endpoint); // ref --> Verweis
                string message = Encoding.Unicode.GetString(data);
                Nachricht EmpfangeneNachricht = new Nachricht();
                EmpfangeneNachricht = Nachricht.NachrichtDecodieren(message);
                Dispatcher.Invoke(messageDelegate, EmpfangeneNachricht);    //führt einen Delegaten aus
            }
        }

        public void NachrichtEmpfangen(Nachricht EmpfangeneNachricht)
        {
            if (EmpfangeneNachricht.Empfänger.Username==Mainuser.Username) // Geht die Nachricht an den Mainuser?
            {
                if (!(ListeBeinhaltet(Kontaktliste, EmpfangeneNachricht.Sender)))  //Ist der Absender in der Kontaktliste?
                {
                    FremderKontakt_Dialog fremderKontakt_Dialog1 = new FremderKontakt_Dialog();
                    fremderKontakt_Dialog1.ShowDialog();
                    if (fremderKontakt_Dialog1.DialogResult == true)
                    {
                        if (fremderKontakt_Dialog1.Ergebnis == true)    //Benutzer soll als Kontakt gespeichert werden
                        {
                            User FremderBenutzer = new User(EmpfangeneNachricht.Sender.Username);
                            Kontaktliste.Add(FremderBenutzer);
                            KontaktlisteinListbox();
                            Speicherung.Speichern(Mainuser, EmpfangeneNachricht); //Speichern des Kontaktes
                            NachrichtenDarstellen(); //Darstellen aller Nachrichten
                        }
                    }
                }
                else
                {
                    Speicherung.Speichern(Mainuser, EmpfangeneNachricht); //Wenn von bekanntem Kontakt kommt
                    NachrichtenDarstellen();
                }
            } 
            else if(EmpfangeneNachricht.Sender.Username==Mainuser.Username) //Wurde die Nachricht vom Mainuser gesendet?
            {
                Speicherung.Speichern(Mainuser, EmpfangeneNachricht);
                NachrichtenDarstellen();
            }             
        }

        
        public void NachrichtenDarstellen()   
        {
            if(!(ListboxKontakte.SelectedIndex==-1))
            {
                List<Nachricht> darzustellende_Nachrichten = Laden.Nachrichten_Laden(Kontaktliste[ListboxKontakte.SelectedIndex]);

                grid_Verlauf.Children.Clear();

                foreach (Nachricht nachricht in darzustellende_Nachrichten)
                {
                    TextBox Tb_nachricht = new TextBox();   //neue Textbox erstellen Tb_nachricht = nachricht.NachrichtinTextbox(); //Die Textbox erhält die Nachricht als Text ist ausgelagert in die Klasse Nachricht
                    Tb_nachricht = nachricht.NachrichtinTextbox();
                    grid_Verlauf.Height = grid_Verlauf.Height + (Tb_nachricht.Height + 1) * Tb_nachricht.FontSize; //Einstellungen für eine schöne Darstellung
                    grid_Verlauf.RowDefinitions.Add(new RowDefinition()); //Neue Zeile wird hinzugefügt

                    if (nachricht.Sender.Username == Mainuser.Username) //Bin ich Sender?
                    {
                        Grid.SetColumn(Tb_nachricht, 1);
                        Grid.SetRow(Tb_nachricht, row);
                        row++;
                        grid_Verlauf.Children.Add(Tb_nachricht);    //Anzeigen
                    }
                    else if (nachricht.Empfänger.Username == Mainuser.Username)   //Ist die Nachricht an mich adressiert
                    {
                        Grid.SetColumn(Tb_nachricht, 0);
                        Grid.SetRow(Tb_nachricht, row);
                        row++;
                        grid_Verlauf.Children.Add(Tb_nachricht);    //Anzeigen
                    }
                }
            }
            else
            {
                grid_Verlauf.Children.Clear();
            }
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
      
        private void btn_KontaktHinzufügen_Click(object sender, RoutedEventArgs e)  //Kontakt hinzufügen
        {
            Username_Dialog dlg = new Username_Dialog();
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                User NeuerKontakt = dlg.ReturnUser();   //Usernamen des Kontakts abfragen
                Kontaktliste.Add(NeuerKontakt);
                KontaktlisteinListbox();    //Zur Kontaktliste hinzufügen
            }
        }

        private void KontaktlisteinListbox()
        {
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
                Speicherung.Kontakt_löschen(Kontaktliste[ListboxKontakte.SelectedIndex]);
                Kontaktliste.Remove(Kontaktliste[ListboxKontakte.SelectedIndex]);   //Kontakt löschen
                grid_Verlauf.Children.Clear();
               
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

        private void ListboxKontakte_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NachrichtenDarstellen();
        }
    }
}