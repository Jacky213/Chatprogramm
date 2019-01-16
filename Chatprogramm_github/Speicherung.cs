using System.Collections.Generic;
using System.Xml;
using System.IO;


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
            if (empfangeneNachricht.Sender == Mainuser)
            {
                Chatpartner = empfangeneNachricht.Empfänger;
            }
            else
            {
                Chatpartner = empfangeneNachricht.Sender;
            }

            //Existiert Node schon?
            XmlNodeList Chatkontakte = Sicherungsdatei.SelectNodes("//mainuser/chats/Chatkontakt"); //Alle ChatkontaktNodes laden

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
                    Zeitpunkt.Value = empfangeneNachricht.Zeitpunkt.ToString();
                    Abgeschickt.Value = empfangeneNachricht.Abgeschickt.ToString();
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
                Chatkontakt.Attributes.Append(Username);

                //Erstelle NachrichtNode
                XmlNode Nachricht = Sicherungsdatei.CreateElement("nachricht");

                XmlAttribute Sendername = Sicherungsdatei.CreateAttribute("sendername");
                XmlAttribute Empfängername = Sicherungsdatei.CreateAttribute("empfängername");
                XmlAttribute Zeitpunkt = Sicherungsdatei.CreateAttribute("zeitpunkt");
                XmlAttribute Abgeschickt = Sicherungsdatei.CreateAttribute("abgeschickt");
                Sendername.Value = empfangeneNachricht.Sender.Username;
                Empfängername.Value = empfangeneNachricht.Empfänger.Username;
                Zeitpunkt.Value = empfangeneNachricht.Zeitpunkt.ToString();
                Abgeschickt.Value = empfangeneNachricht.Abgeschickt.ToString();
                Nachricht.InnerText = empfangeneNachricht.Nachrichtentext;

                Nachricht.Attributes.Append(Sendername);
                Nachricht.Attributes.Append(Empfängername);
                Nachricht.Attributes.Append(Zeitpunkt);
                Nachricht.Attributes.Append(Abgeschickt);

                Chatkontakt.AppendChild(Nachricht);
                Chatkontakt.AppendChild(Chatkontakt);
            }

            //Datei wieder speichern
            Sicherungsdatei.Save(pfad);
        }

        public static void NeueSicherungsdateiErstellen(User Mainuser)
        {
            //Erstellt eine völlig neue Sicherungsdatei, falls das Programm zum ersten mal gestartet wird
            XmlDocument Sicherungsdatei = new XmlDocument();

            XmlNode Mainusernode = Create_Usernode("mainuser", Mainuser.Username);
            Sicherungsdatei.AppendChild(Mainusernode);

            XmlNode Chats = Sicherungsdatei.CreateElement("chats");
            Sicherungsdatei.AppendChild(Chats);

            XmlNode Create_Usernode(string nodename, string username)   //evtl wieder löschen?!
            {
                XmlNode user_Node = Sicherungsdatei.CreateElement(nodename);
                XmlAttribute Username = Sicherungsdatei.CreateAttribute("username");
                Username.Value = username;
                return user_Node;
            }

            Sicherungsdatei.Save(pfad); //Datei speichern
        }
    }

    class Laden
    {
        //Pfad unter dem die Sicherungsdatei abgelegt ist
        const string pfad = @"Sicherungsdatei.xml";

        public static List <Nachricht> Nachrichten_Laden(User aktueller_Chatpartner)
        {
            
            //Dokument laden und aus den Nachrichten mit dem entsprechenden Chatpartner eine Liste mit Nachrichten erstellen und ausgeben

            //Die Nachrichten_Darstellen Funktion muss überarbeitet werden.
            return new List<Nachricht>();
        }
        
        public static User Username_Laden()
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
    }
}