using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Windows;


namespace Chatprogramm_github
{

    class Speicherung
    {
        //Pfad unter dem die SIcherungsdatei gespeichert wird
        const string pfad = @"Sicherungsdatei.xml";

        public static void Speichern(User Mainuser, Nachricht empfangeneNachricht)
        {
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
                if (Chatkontakt.Attributes["username"].Value == Chatpartner.Username)
                {
                    gefunden = true;

                    //NachrichtNode erstellen
                    XmlNode Nachricht = Sicherungsdatei.CreateElement("nachricht");

                    XmlAttribute Sendername = Sicherungsdatei.CreateAttribute("sendername");
                    XmlAttribute Empfängername = Sicherungsdatei.CreateAttribute("empfängername");
                    XmlAttribute Zeitpunkt = Sicherungsdatei.CreateAttribute("zeitpunkt");
                    XmlAttribute Abgeschickt = Sicherungsdatei.CreateAttribute("abgeschickt");
                    Sendername.Value = empfangeneNachricht.Sender.Username;
                    Empfängername.Value = empfangeneNachricht.Empfänger.Username;
                    Zeitpunkt.Value = empfangeneNachricht.Zeitpunkt.ToFileTime().ToString();
                    Abgeschickt.Value = empfangeneNachricht.Abgeschickt.ToString();//.ToString();
                    Nachricht.InnerText = empfangeneNachricht.Nachrichtentext;

                    Nachricht.Attributes.Append(Sendername);
                    Nachricht.Attributes.Append(Empfängername);
                    Nachricht.Attributes.Append(Zeitpunkt);
                    Nachricht.Attributes.Append(Abgeschickt);

                    Chatkontakt.AppendChild(Nachricht);
                }
            }

            if (gefunden == false)  //falls Chatkontakt nicht gefunden wurde
            {
                XmlNode Chats = Sicherungsdatei.SelectSingleNode("//mainuser/chats");

                //Erstelle ChatkontaktNode
                XmlNode Chatkontakt = Sicherungsdatei.CreateElement("chatkontakt");
                XmlAttribute Username = Sicherungsdatei.CreateAttribute("username");
                Username.Value = Chatpartner.Username;
                Chatkontakt.Attributes.Append(Username);

                //Erstelle NachrichtNode
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

                Chats.AppendChild(Chatkontakt);
                Chatkontakt.AppendChild(Nachricht);
                
            }

            //Datei wieder speichern
            Sicherungsdatei.Save(pfad);
        }

        public static void NeueSicherungsdateiErstellen(User Mainuser)
        {
            //Erstellt eine völlig neue Sicherungsdatei, falls das Programm zum ersten mal gestartet wird
            XmlDocument Sicherungsdatei = new XmlDocument();

            XmlNode Mainusernode = Sicherungsdatei.CreateElement("mainuser"); //Create_Usernode("mainuser", Mainuser.Username);
            //XmlNode user_Node = Sicherungsdatei.CreateElement("mainuser");
            XmlAttribute Username = Sicherungsdatei.CreateAttribute("username");
            Username.Value = Mainuser.Username;
            Mainusernode.Attributes.Append(Username);
            
            Sicherungsdatei.AppendChild(Mainusernode);

            XmlNode Chats = Sicherungsdatei.CreateElement("chats");           
            Mainusernode.AppendChild(Chats);

            //XmlNode Create_Usernode(string nodename, string username)   //evtl wieder löschen?!
            //{
            //    XmlNode user_Node = Sicherungsdatei.CreateElement(nodename);
            //    XmlAttribute Username = Sicherungsdatei.CreateAttribute("username");
            //    Username.Value = username;
            //    return user_Node;
            //}

            Sicherungsdatei.Save(pfad); //Datei speichern
        }

        public static void Kontakt_löschen(User kontakt)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(pfad);

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
            xmlDoc.Save(pfad);
        }
    }

    class Laden
    {
        //Pfad unter dem die Sicherungsdatei abgelegt ist
        const string pfad = @"Sicherungsdatei.xml";

        public static List <Nachricht> Nachrichten_Laden(User aktueller_Chatpartner)
        {
            XmlNode aktueller_Chatkontakt;
            XmlDocument Sicherungsdatei = new XmlDocument();
            Sicherungsdatei.Load(pfad);

            List<Nachricht> Nachrichten = new List<Nachricht>();
            Nachrichten.Clear();

            XmlNodeList Chatkontakte = Sicherungsdatei.SelectNodes("//mainuser/chats/chatkontakt"); //Alle ChatkontaktNodes laden

            foreach(XmlNode Chatkontakt in Chatkontakte)
            {
                if(Chatkontakt.Attributes["username"].Value == aktueller_Chatpartner.Username)
                {
                    aktueller_Chatkontakt = Chatkontakt;
                    XmlNodeList nachrichten = aktueller_Chatkontakt.ChildNodes;
                    foreach (XmlNode nachricht in aktueller_Chatkontakt)
                    {
                        string nachrichtentext = nachricht.InnerText;
                        User Sender = new User(nachricht.Attributes["sendername"].Value);
                        User Empfänger = new User(nachricht.Attributes["empfängername"].Value);
                        System.DateTime dateTime = System.DateTime.FromFileTime(System.Convert.ToInt64(nachricht.Attributes["zeitpunkt"].Value));
                        bool abgeschickt = System.Convert.ToBoolean(nachricht.Attributes["abgeschickt"].Value);

                        Nachricht hilfe = new Nachricht(nachrichtentext, Sender,Empfänger, dateTime,abgeschickt);
                        Nachrichten.Add(hilfe);
                    }
                }
            }
            return Nachrichten;

           


            //Dokument laden und aus den Nachrichten mit dem entsprechenden Chatpartner eine Liste mit Nachrichten erstellen und ausgeben

            //Die Nachrichten_Darstellen Funktion muss überarbeitet werden.
            //return new List<Nachricht>();
        }
        
        public static User Username_Laden() //Hier auch Kontakte laden
        {                     
            User Mainuser = new User();
            if (File.Exists(pfad))
            {
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
                //War die Erstellung des Users erfolgreich, wird eine Messagebox angezeigt
                //MessageBox.Show("Sie haben sich als \"" + Mainuser.Username + "\" angemeldet.", "Anmeldung erfolgreich");
                //Usernameeingabe war erfolgreich und wird ausgegeben.
                Speicherung.NeueSicherungsdateiErstellen(Mainuser);
            }
            return Mainuser;
        }

        public static List<User> Kontakte_laden()
        {
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
    }
}