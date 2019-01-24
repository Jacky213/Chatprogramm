/*Haeder
 * 24.01.2019, Jacqueline Kaefer und Luca Katzenberger
 * Diese Datei enthält Funktionen, um Daten zu speichern und diese auch wieder zu laden.
 * Sie ist in mehrere Klasse unterteilt.
 * Die erste Klasse Speicherung enthält mehrere Funktionen zum Erstellen einer Sicherungsdatei und zum Speichern von Daten darin.
 * Die zweite Klasse Laden enthält Funktionen zum Laden von unterschiedlichen Daten aus der Speicherungsdatei.
 */


using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Windows;


namespace Chatprogramm_github
{

    class Speicherung
    {
        #region Constants
        //Pfad unter dem die Sicherungsdatei gespeichert wird
        const string pfad = @"Sicherungsdatei.xml";
        #endregion

        #region Methods
        public static void Speichern(User Mainuser, Nachricht empfangeneNachricht)
        {
            //Diese Methode speichert die mitgegebenene Nachricht in der Sicherungsdatei ab. 
            
            //Sicherungsdatei laden
            XmlDocument Sicherungsdatei = new XmlDocument();
            Sicherungsdatei.Load(pfad);
            
            //Unter welchem Kontakt soll die Nachricht gespeichert werden
            User Chatpartner = new User();
            if (empfangeneNachricht.Sender.Username == Mainuser.Username)
            {
                Chatpartner = empfangeneNachricht.Empfänger;
            }
            else
            {
                Chatpartner = empfangeneNachricht.Sender;
            }

            //Existiert Node schon?
            XmlNodeList Chatkontakte = Sicherungsdatei.SelectNodes("//mainuser/chats/chatkontakt"); //Alle ChatkontaktNodes laden

            bool gefunden = false;
            foreach (XmlNode Chatkontakt in Chatkontakte)   //Liste durchgehen, ob Chatkontakt schon gespeichert ist
            {
                if (Chatkontakt.Attributes["username"].Value == Chatpartner.Username) //True, wenn der Kontakt schon in der Sicherungsdatei enthalten ist.
                {
                    gefunden = true;

                    //Neue NachrichtNode erstellen mit all ihren Attributen
                    XmlNode Nachricht = Sicherungsdatei.CreateElement("nachricht");

                    XmlAttribute Sendername = Sicherungsdatei.CreateAttribute("sendername");
                    XmlAttribute Empfängername = Sicherungsdatei.CreateAttribute("empfängername");
                    XmlAttribute Zeitpunkt = Sicherungsdatei.CreateAttribute("zeitpunkt");
                    XmlAttribute Abgeschickt = Sicherungsdatei.CreateAttribute("abgeschickt");

                    //Den Attributen werden Values zugeordnet.
                    Sendername.Value = empfangeneNachricht.Sender.Username;
                    Empfängername.Value = empfangeneNachricht.Empfänger.Username;
                    Zeitpunkt.Value = empfangeneNachricht.Zeitpunkt.ToFileTime().ToString();
                    Abgeschickt.Value = empfangeneNachricht.Abgeschickt.ToString();//.ToString();
                    Nachricht.InnerText = empfangeneNachricht.Nachrichtentext;

                    //Die Attribute werden an die Node gehängt
                    Nachricht.Attributes.Append(Sendername);
                    Nachricht.Attributes.Append(Empfängername);
                    Nachricht.Attributes.Append(Zeitpunkt);
                    Nachricht.Attributes.Append(Abgeschickt);

                    //Und die Node an den Chatkontakt
                    Chatkontakt.AppendChild(Nachricht);
                }
            }

            if (gefunden == false)  //falls Chatkontakt nicht gefunden wurde
            {
                XmlNode Chats = Sicherungsdatei.SelectSingleNode("//mainuser/chats");

                //Erstellen einer neuen ChatkontaktNode mit ihrem Attribut
                XmlNode Chatkontakt = Sicherungsdatei.CreateElement("chatkontakt");
                XmlAttribute Username = Sicherungsdatei.CreateAttribute("username");
                Username.Value = Chatpartner.Username;
                Chatkontakt.Attributes.Append(Username);

                //Erstelle neue NachrichtNode
                XmlNode Nachricht = Sicherungsdatei.CreateElement("nachricht");

                XmlAttribute Sendername = Sicherungsdatei.CreateAttribute("sendername");
                XmlAttribute Empfängername = Sicherungsdatei.CreateAttribute("empfängername");
                XmlAttribute Zeitpunkt = Sicherungsdatei.CreateAttribute("zeitpunkt");
                XmlAttribute Abgeschickt = Sicherungsdatei.CreateAttribute("abgeschickt");
                Sendername.Value = empfangeneNachricht.Sender.Username;
                Empfängername.Value = empfangeneNachricht.Empfänger.Username;
                Zeitpunkt.Value = empfangeneNachricht.Zeitpunkt.ToFileTime().ToString();//ToString();
                Abgeschickt.Value = empfangeneNachricht.Abgeschickt.ToString();
                Nachricht.InnerText = empfangeneNachricht.Nachrichtentext;

                Nachricht.Attributes.Append(Sendername);
                Nachricht.Attributes.Append(Empfängername);
                Nachricht.Attributes.Append(Zeitpunkt);
                Nachricht.Attributes.Append(Abgeschickt);

                //Die neuen Nodes werden an die Sicherungsdatei gehängt.
                Chats.AppendChild(Chatkontakt);
                Chatkontakt.AppendChild(Nachricht);                
            }

            //Datei wieder speichern
            Sicherungsdatei.Save(pfad);
        }

        public static void NeueSicherungsdateiErstellen(User Mainuser)
        {
            //Diese Methode erstellt eine völlig neue Sicherungsdatei
            XmlDocument Sicherungsdatei = new XmlDocument();

            XmlNode Mainusernode = Sicherungsdatei.CreateElement("mainuser");            
            XmlAttribute Username = Sicherungsdatei.CreateAttribute("username");
            Username.Value = Mainuser.Username;
            Mainusernode.Attributes.Append(Username);
            
            Sicherungsdatei.AppendChild(Mainusernode);

            XmlNode Chats = Sicherungsdatei.CreateElement("chats");           
            Mainusernode.AppendChild(Chats);

            Sicherungsdatei.Save(pfad); //Datei speichern
        }

        public static void Kontakt_löschen(User kontakt)
        {
            //Diese Methode löscht den mitgegebenen Kontakt und alle zugehörigen Nachrichten aus der Sicherungsdatei
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(pfad);

            //Es werden neue Nodes erstellt und an die Sicherungsdatei angehängt.
            XmlNode chats = xmlDoc.SelectSingleNode("//mainuser/chats");
            XmlNodeList chatkontakte = xmlDoc.SelectNodes("//mainuser/chats/chatkontakt");

            foreach(XmlNode chatkontakt in chatkontakte)
            {
                if(chatkontakt.Attributes["username"].Value==kontakt.Username)
                {
                    chats.RemoveChild(chatkontakt);
                    MessageBox.Show("Kontakt wurde gelöscht");
                }
                
            }
            //Nach der Bearbeitung wird die Datei wieder gespeichert.
            xmlDoc.Save(pfad);
        }
        #endregion
    }

    class Laden
    {
        #region Constants
        //Pfad unter dem die Sicherungsdatei abgelegt ist
        const string pfad = @"Sicherungsdatei.xml";
        #endregion

        #region Methods
        public static List <Nachricht> Nachrichten_Laden(User aktueller_Chatpartner)
        {
            //Diese Methode lädt die Sicherungsdatei und gibt alle Nachrichten in Zusamenhang mit dem mitgegebenen User in einer Liste zurück
           
            XmlNode aktueller_Chatkontakt;
            XmlDocument Sicherungsdatei = new XmlDocument();
            Sicherungsdatei.Load(pfad);

            List<Nachricht> Nachrichten = new List<Nachricht>();
            Nachrichten.Clear(); //Damit die Liste auf jeden Fall leer ist.

            XmlNodeList Chatkontakte = Sicherungsdatei.SelectNodes("//mainuser/chats/chatkontakt"); //Alle ChatkontaktNodes laden

            foreach(XmlNode Chatkontakt in Chatkontakte)
            {
                if(Chatkontakt.Attributes["username"].Value == aktueller_Chatpartner.Username) //Suchen der Node, die zum mitgegebenen User gehört
                {
                    aktueller_Chatkontakt = Chatkontakt;
                    XmlNodeList nachrichten = aktueller_Chatkontakt.ChildNodes;
                    foreach (XmlNode nachricht in aktueller_Chatkontakt)
                    {
                        //Aus dem Inhalt der Sicherungsdatei wird ein Objekt Nachricht erzeugt
                        string nachrichtentext = nachricht.InnerText;
                        User Sender = new User(nachricht.Attributes["sendername"].Value);
                        User Empfänger = new User(nachricht.Attributes["empfängername"].Value);
                        System.DateTime dateTime = System.DateTime.FromFileTime(System.Convert.ToInt64(nachricht.Attributes["zeitpunkt"].Value));
                        bool abgeschickt = System.Convert.ToBoolean(nachricht.Attributes["abgeschickt"].Value);

                        Nachricht hilfe = new Nachricht(nachrichtentext, Sender,Empfänger, dateTime,abgeschickt);

                        //Die Nachricht wird der List hinzugefügt.
                        Nachrichten.Add(hilfe);
                    }
                }
            }
            return Nachrichten; //Die Liste mit Nachrichten wird zurückgegeben
        }
        
        public static User Username_Laden()
        {        
            //Diese Funktion prüft, ob bereits eine Sicherungsdatei existiert. Wenn ja, gibt sie den darin gespeicherten Mainuser zurück. Wenn nicht ruft sie eine Methode auf,
            //die eine neue Sicherungsdatei erstellt, nachdem sie den Username_Dialog zum Festlegen eines Mainusers aufgerufen hat.

            User Mainuser = new User();

            if (File.Exists(pfad))//Existiert schon eine Sicherungsdatei?
            {
                //Wenn ja, wird der Mainuser geladen
                XmlDocument Sicherungsdatei = new XmlDocument();
                Sicherungsdatei.Load(pfad);
                XmlNode MainUserNode = Sicherungsdatei.SelectSingleNode("//mainuser");
                Mainuser.Username = MainUserNode.Attributes["username"].Value;
            }
            else
            {
                //Ein Dialog zur Eingabe des Usernamens wird erstellt und angezeigt

                Username_Dialog dlg = new Username_Dialog();
                dlg.ShowDialog();
                if (dlg.DialogResult == true)
                {
                    Mainuser = dlg.ReturnUser();
                }
                //Danach wird eine neue Sicherungsdatei erstellt.
                Speicherung.NeueSicherungsdateiErstellen(Mainuser);
            }
            return Mainuser; //Der Mainuser wird zurückgegeben
        }

        public static List<User> Kontakte_laden()
        {
            //Diese Methode lädt alle Kotakte aus der Sicherungsdatei in eine Liste mit Usern und gibt diese zurück. Existiert keine Sicherungsdatei gibt diese Methode eine leere Liste zurück.
            List<User> Kontaktliste = new List<User>();

            if (File.Exists(pfad))
            {
                XmlDocument Sicherungsdatei = new XmlDocument();
                Sicherungsdatei.Load(pfad);
                XmlNodeList Kontakte = Sicherungsdatei.SelectNodes("//mainuser/chats/chatkontakt");
               foreach(XmlNode Kontakt in Kontakte)
               {
                    User kontakt = new User(Kontakt.Attributes["username"].Value);
                    Kontaktliste.Add(kontakt);
               }                
            }
            return Kontaktliste;
        }
        #endregion
    }
}