/*Haeder
 * 24.01.2019, Jacqueline Kaefer und Luca Katzenberger
 * Diese Datei enthält Funktionen, um Daten zu speichern und diese auch wieder zu laden.
 * Sie ist in mehrere Klasse unterteilt.
 * Die erste Klasse Speicherung enthält mehrere Funktionen zum Erstellen einer Backupfile und zum Speichern von Daten darin.
 * Die zweite Klasse Laden enthält Funktionen zum Laden von unterschiedlichen Daten aus der Speicherungsdatei.
 */


using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Windows;


namespace Chatprogramm_github
{

    class Save
    {
        #region Constants
        //Pfad unter dem die Backupfile gespeichert wird
        const string path = @"Backupfile.xml";
        #endregion

        #region Methods
        public static void SaveData(User Mainuser, Message receivedMessage)
        {
            //Diese Methode speichert die mitgegebenene message im Backupfile ab. 

            //Backupfile laden
            XmlDocument backupfile = new XmlDocument();
            backupfile.Load(path);
            
            //Unter welchem Kontakt soll die message gespeichert werden
            User chatpartner = new User();
            if (receivedMessage.Sender.Username == Mainuser.Username)
            {
                chatpartner = receivedMessage.Receiver;
            }
            else
            {
                chatpartner = receivedMessage.Sender;
            }

            //Existiert Node schon?
            XmlNodeList chatcontacts = backupfile.SelectNodes("//mainuser/chats/chatcontacts"); //Alle ChatkontaktNodes laden

            bool found = false;
            foreach (XmlNode chatcontact in chatcontacts)   //Liste durchgehen, ob chatcontact schon gespeichert ist
            {
                if (chatcontact.Attributes["username"].Value == chatpartner.Username) //True, wenn der Kontakt schon in der Backupfile enthalten ist.
                {
                    found = true;

                    //Neue NachrichtNode erstellen mit all ihren Attributen
                    XmlNode message = backupfile.CreateElement("message");

                    XmlAttribute sendername = backupfile.CreateAttribute("sendername");
                    XmlAttribute receivername = backupfile.CreateAttribute("receivername");
                    XmlAttribute timestamp = backupfile.CreateAttribute("timestamp");
                    XmlAttribute sent = backupfile.CreateAttribute("sent");

                    //Den Attributen werden Values zugeordnet.
                    sendername.Value = receivedMessage.Sender.Username;
                    receivername.Value = receivedMessage.Receiver.Username;
                    timestamp.Value = receivedMessage.Timestamp.ToFileTime().ToString();
                    sent.Value = receivedMessage.Sent.ToString();//.ToString();
                    message.InnerText = receivedMessage.Text;

                    //Die Attribute werden an die Node gehängt
                    message.Attributes.Append(sendername);
                    message.Attributes.Append(receivername);
                    message.Attributes.Append(timestamp);
                    message.Attributes.Append(sent);

                    //Und die Node an den chatcontact
                    chatcontact.AppendChild(message);
                }
            }

            if (found == false)  //falls chatcontact nicht gefunden wurde
            {
                XmlNode chats = backupfile.SelectSingleNode("//mainuser/chats");

                //Erstellen einer neuen ChatkontaktNode mit ihrem Attribut
                XmlNode chatcontact = backupfile.CreateElement("chatcontact");
                XmlAttribute username = backupfile.CreateAttribute("username");
                username.Value = chatpartner.Username;
                chatcontact.Attributes.Append(username);

                //Erstelle neue NachrichtNode
                XmlNode message = backupfile.CreateElement("message");

                XmlAttribute sendername = backupfile.CreateAttribute("sendername");
                XmlAttribute receivername = backupfile.CreateAttribute("receivername");
                XmlAttribute timestamp = backupfile.CreateAttribute("timestamp");
                XmlAttribute sent = backupfile.CreateAttribute("sent");
                sendername.Value = receivedMessage.Sender.Username;
                receivername.Value = receivedMessage.Receiver.Username;
                timestamp.Value = receivedMessage.Timestamp.ToString();
                sent.Value = receivedMessage.Sent.ToString();
                message.InnerText = receivedMessage.Text;

                message.Attributes.Append(sendername);
                message.Attributes.Append(receivername);
                message.Attributes.Append(timestamp);
                message.Attributes.Append(sent);

                //Die neuen Nodes werden an das Backupfile angehängt.
                chats.AppendChild(chatcontact);
                chatcontact.AppendChild(message);                
            }

            //Datei wieder speichern
            backupfile.Save(path);
        }

        public static void CreateNewBackupfile(User Mainuser)
        {
            //Diese Methode erstellt eine völlig neue Backupfile
            XmlDocument sicherungsdatei = new XmlDocument();

            XmlNode mainusernode = sicherungsdatei.CreateElement("mainuser");            
            XmlAttribute username = sicherungsdatei.CreateAttribute("username");
            username.Value = Mainuser.Username;
            mainusernode.Attributes.Append(username);
            
            sicherungsdatei.AppendChild(mainusernode);

            XmlNode chats = sicherungsdatei.CreateElement("chats");           
            mainusernode.AppendChild(chats);

            sicherungsdatei.Save(path); //Datei speichern
        }

        public static void DeleteContact(User contact)
        {
            //Diese Methode löscht den mitgegebenen Kontakt und alle zugehörigen Nachrichten aus der Backupfile
            XmlDocument Backupfile = new XmlDocument();
            Backupfile.Load(path);

            //Es werden neue Nodes erstellt und an die Backupfile angehängt.
            XmlNode chats = Backupfile.SelectSingleNode("//mainuser/chats");
            XmlNodeList chatcontacts = Backupfile.SelectNodes("//mainuser/chats/chatcontact");

            foreach(XmlNode chatcontact in chatcontacts)
            {
                if(chatcontact.Attributes["username"].Value==contact.Username)
                {
                    chats.RemoveChild(chatcontact);
                    MessageBox.Show("Kontakt wurde gelöscht");
                }
                
            }
            //Nach der Bearbeitung wird die Datei wieder gespeichert.
            Backupfile.Save(path);
        }
        #endregion
    }

    class Load
    {
        #region Constants
        //Pfad unter dem die Backupfile abgelegt ist
        const string path = @"Backupfile.xml";
        #endregion

        #region Methods
        public static List <Message> LoadMessages(User chatpartner)
        {
            //Diese Methode lädt die Backupfile und gibt alle Nachrichten in Zusamenhang mit dem mitgegebenen User in einer Liste zurück
           
            XmlDocument Sicherungsdatei = new XmlDocument();
            Sicherungsdatei.Load(path);

            List<Message> messages = new List<Message>();
            messages.Clear(); //Damit die Liste auf jeden Fall leer ist.

            XmlNodeList chatcontacts = Sicherungsdatei.SelectNodes("//mainuser/chats/chatcontact"); //Alle ChatkontaktNodes laden

            foreach(XmlNode chatcontact in chatcontacts)
            {
                if(chatcontact.Attributes["username"].Value == chatpartner.Username) //Suchen der Node, die zum mitgegebenen User gehört
                {
                    XmlNodeList messagenodelist = chatcontact.ChildNodes;
                    foreach (XmlNode message in messagenodelist)
                    {
                        //Aus dem Inhalt der Backupfile wird ein Objekt message erzeugt
                        string text = message.InnerText;
                        User sender = new User(message.Attributes["sendername"].Value);
                        User receiver = new User(message.Attributes["receivername"].Value);
                        System.DateTime dateTime = System.Convert.ToDateTime(message.Attributes["timestamp"].Value);
                        bool sent = System.Convert.ToBoolean(message.Attributes["sent"].Value);

                        Message tmp_message = new Message(text, sender,receiver, dateTime,sent);

                        //Die message wird der List hinzugefügt.
                        messages.Add(tmp_message);
                    }
                }
            }
            return messages; //Die List mit Messages wird zurückgegeben
        }
        
        public static User LoadUsername()
        {        
            //Diese Funktion prüft, ob bereits eine Backupfile existiert. Wenn ja, gibt sie den darin gespeicherten Mainuser zurück. Wenn nicht ruft sie eine Methode auf,
            //die eine neue Backupfile erstellt, nachdem sie den Username_Dialog zum Festlegen eines Mainusers aufgerufen hat.

            User mainuser = new User();

            if (File.Exists(path))//Existiert schon eine Backupfile?
            {
                //Wenn ja, wird der Mainuser geladen
                XmlDocument backupfile = new XmlDocument();
                backupfile.Load(path);
                XmlNode MainUserNode = backupfile.SelectSingleNode("//mainuser");
                mainuser.Username = MainUserNode.Attributes["username"].Value;
            }
            else
            {
                //Ein Dialog zur Eingabe des Usernamens wird erstellt und angezeigt

                Username_Dialog dlg = new Username_Dialog();
                dlg.ShowDialog();
                if (dlg.DialogResult == true)
                {
                    mainuser = dlg.ReturnUser();
                }
                //Danach wird eine neue Backupfile erstellt.
                Save.CreateNewBackupfile(mainuser);
            }
            return mainuser; //Der Mainuser wird zurückgegeben
        }

        public static List<User> LoadContacts()
        {
            //Diese Methode lädt alle Kotakte aus der Backupfile in eine Liste mit Usern und gibt diese zurück. Existiert keine Backupfile gibt diese Methode eine leere Liste zurück.
            List<User> contactlist = new List<User>();

            if (File.Exists(path))
            {
                XmlDocument backupfile = new XmlDocument();
                backupfile.Load(path);
                XmlNodeList contactnodes = backupfile.SelectNodes("//mainuser/chats/chatcontact");
               foreach(XmlNode contactnode in contactnodes)
               {
                    User contact = new User(contactnode.Attributes["username"].Value);
                    contactlist.Add(contact);
               }                
            }
            return contactlist;
        }
        #endregion
    }
}