/* Header
 * 
 * 24.01.2019, Jacqueline Kaefer, Luca Katzenberger
 * 
 * Dies ist die Mainklasse unseres Chatprogramms.
 * Sie beinhaltet die contactlist und die Methoden zum Nachrichten empfangen und darstellen.
 * Außerdem werden aus Ihr alle Dialoge und Funktionen, beispielsweise zum Speichern und Laden aufgerufen.
 * 
 */

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Chatprogramm_github
{
    public partial class MainWindow : Window
    {
        #region Global Variables
        delegate void AddMessage(Message receivedmessage);
        const int port = 54546;
        Sender messagesender;
        UdpClient messagereceiver;
        Thread receiverthread;
        User Mainuser = new User();        
        int row = 0;
        List<User> contactlist = new List<User>();
        #endregion

        public MainWindow() //Initialisierung
        {
            try
            {
                InitializeComponent();
            }
            catch
            {
                MessageBox.Show("Bei der Initialisierung des Programms ist ein Fehler aufgetreten. Bitte starten Sie das Programm neu.");
            }
            

            //Mainuser abfragen/initialisieren
            Mainuser = Load.LoadUsername();
            if(Mainuser == null || Mainuser.Username == null)
            {
                MessageBox.Show("Es ist ein Fehler mit Ihrem Usernamen aufgetreten. Bitte starten Sie das Programm neu.", "ERROR");
                Application.Current.Shutdown();
            }
            else
            {
                MessageBox.Show("Sie haben sich als \"" + Mainuser.Username + "\" angemeldet.", "Anmeldung erfolgreich");
            }            

            //contactlist abfragen
            contactlist = Load.LoadContacts();
            //Kontakte in Listbox darstellen
            DisplayContactlistinListbox();

            //Sender initialisieren
            messagesender = new Sender();
            try
            {
                //Empfänger initialisieren
                messagereceiver = new UdpClient(port);

                //Paralleler Thread für das Empfangen von Nachrichten anlegen
                ThreadStart start = new ThreadStart(Receiver);
                receiverthread = new Thread(start);
                receiverthread.IsBackground = true;
                receiverthread.Start();
            }
            catch
            {
                MessageBox.Show("Bei der Initialisierung des Thread zum Nachrichtenempfang ist ein Fehler aufgetreten. Bitte starten Sie das Programm neu.");
            }
            txt_Message.Focus();    //Fokus auf die Textbox zur Eingabe der Nachricht legen
        }

        #region Events
        
        private void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            //Nachricht wird bei Klick auf btn_Send gesendet
            string messagetext = txt_Message.Text;
            if (ListboxContacts.SelectedIndex >= 0)     //Ist ein Kontakt ausgewählt?
            {
                messagesender.Send(new Message(messagetext, Mainuser, contactlist[ListboxContacts.SelectedIndex], DateTime.Now, true));  //Könnte eine Exception werfen, wenn kein Kontakt ausgewählt ist                
                txt_Message.Text = "";
            }
            else
            {
                MessageBox.Show("Sie müssen einen Kontakt auswählen", "Kein Kontakt ausgewählt");
            }
        }

        private void txt_Nachricht_KeyDown(object sender, KeyEventArgs e)
        {
            //Drückt der Benutzer Enter, soll die eingegebene Nachricht abgeschickt werden.
            if (e.Key == Key.Enter)
            {
                if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    txt_Message.Text = txt_Message.Text.Substring(0, txt_Message.Text.Length - 2);  //Enter am Ende der Nachricht abschneiden, falls mit Enter geschickt wurde.
                    btn_Send_Click(sender, e);  //gibt trotzdem ein Enter aus
                }
            }
        }       
      
        private void btn_AddContact_Click(object sender, RoutedEventArgs e)  //Kontakt hinzufügen
        {
            Username_Dialog dlg = new Username_Dialog(false);
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                User newcontact = dlg.ReturnUser();   //Usernamen des Kontakts abfragen
                contactlist.Add(newcontact);
                DisplayContactlistinListbox();    //Zur contactlist hinzufügen
            }
        }

        private void ListboxContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayMessage();
        }

        private void btn_deletecontact_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Save.DeleteContact(contactlist[ListboxContacts.SelectedIndex]);
                contactlist.Remove(contactlist[ListboxContacts.SelectedIndex]);   //Kontakt löschen
                grid_Verlauf.Children.Clear();
            }
            catch (Exception)
            {
                MessageBox.Show("Bitte wählen Sie erst einen Kontakt aus", "Fehler");
            }

            ListboxContacts.Items.Clear();  //Listbox leeren
            foreach (User contact in contactlist)
            {
                ListboxContacts.Items.Add(contact); //Alle Kontakte in Listbox übertragen
            }
            ListboxContacts.UpdateLayout(); //Layout aktualisieren
        }
        #endregion

        #region Methods

        public void Receiver() //Könnte man in Klasse auslagern??
        {
            try
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port); //Any überwacht alle IP-Adressen des Netzwerks
                AddMessage messageDelegate = MessageReceived;
                while (true)
                {
                    byte[] data = messagereceiver.Receive(ref endpoint); // ref --> Verweis
                    Message receivedmessage = new Message();
                    receivedmessage = Message.DecodeMessage(data);
                    if (receivedmessage == null)
                    {
                        MessageBox.Show("Es ist ein Fehler beim Empfangen einer Nachricht aufgetreten. Bitte starten Sie das Programm neu.", "ERROR");
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        Dispatcher.Invoke(messageDelegate, receivedmessage);    //führt einen Delegaten aus
                    }

                }
            }
            catch
            {
                MessageBox.Show("Beim Empfang einer Nachricht ist ein Fehler aufgetreten.");
            }

        }

        public void MessageReceived(Message receivedmessage)
        {
            if (receivedmessage.Receiver.Username == Mainuser.Username) // Geht die Nachricht an den Mainuser?
            {
                //Bei empfangener Nachricht einen Signalton ausgeben
                System.Media.SystemSounds.Hand.Play();
                PopupContent.Text = "Neue Nachricht von " + receivedmessage.Sender.Username;
                PopupNewMessage.IsOpen = true;

                if (!(listcontains(contactlist, receivedmessage.Sender)))  //Ist der Absender in der contactlist?
                {
                    UnknownUserDialog unknownsenderdialog1 = new UnknownUserDialog(receivedmessage.Sender);
                    unknownsenderdialog1.ShowDialog();
                    if (unknownsenderdialog1.DialogResult == true)
                    {
                        if (unknownsenderdialog1.result == true)    //Benutzer soll als Kontakt gespeichert werden
                        {
                            User unknownuser = new User(receivedmessage.Sender.Username);
                            contactlist.Add(unknownuser);
                            DisplayContactlistinListbox();
                            Save.SaveData(Mainuser, receivedmessage); //Speichern des Kontaktes
                            DisplayMessage(); //Darstellen aller Nachrichten
                        }
                    }
                }
                else
                {
                    Save.SaveData(Mainuser, receivedmessage); //Wenn von bekanntem Kontakt kommt
                    DisplayMessage();
                }
            }
            else if (receivedmessage.Sender.Username == Mainuser.Username) //Wurde die Nachricht vom Mainuser gesendet?
            {
                Save.SaveData(Mainuser, receivedmessage);
                DisplayMessage();
            }
        }

        public void DisplayMessage()
        {
            if (!(ListboxContacts.SelectedIndex == -1))
            {
                List<Message> savedmessages = Load.LoadMessages(contactlist[ListboxContacts.SelectedIndex]);

                grid_Verlauf.Children.Clear();

                if (savedmessages != null)
                {
                    foreach (Message message in savedmessages)
                    {
                        TextBox Tb_nachricht = new TextBox();   //neue Textbox erstellen
                        Tb_nachricht = message.MessageinTextbox();
                        grid_Verlauf.Height = grid_Verlauf.Height + (Tb_nachricht.Height + 1) * Tb_nachricht.FontSize; //Einstellungen für eine schöne Darstellung
                        grid_Verlauf.RowDefinitions.Add(new RowDefinition()); //Neue Zeile wird hinzugefügt

                        if (message.Sender.Username == Mainuser.Username) //Bin ich Sender?
                        {
                            Grid.SetColumn(Tb_nachricht, 1);
                            Grid.SetRow(Tb_nachricht, row);
                            row++;
                            grid_Verlauf.Children.Add(Tb_nachricht);    //Anzeigen
                        }
                        else if (message.Receiver.Username == Mainuser.Username)   //Ist die Nachricht an mich adressiert
                        {
                            Grid.SetColumn(Tb_nachricht, 0);
                            Grid.SetRow(Tb_nachricht, row);
                            row++;
                            grid_Verlauf.Children.Add(Tb_nachricht);    //Anzeigen
                        }
                    }
                }
            }
            else
            {
                grid_Verlauf.Children.Clear();
            }
        }

        private void DisplayContactlistinListbox()
        {
            ListboxContacts.Items.Clear();  //Listbox leeren
            foreach (User contact in contactlist)
            {
                ListboxContacts.Items.Add(contact); //Alle Kontakte in Listbox übertragen
            }
            ListboxContacts.UpdateLayout(); //Layout aktualisieren
        }

        private bool listcontains(List<User> list, User contact)    //Hat mit .Contains nicht funktioniert
        {
            for (int i = 0; i < list.Count ; i++)
            {
                if (list[i].Username == contact.Username)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}