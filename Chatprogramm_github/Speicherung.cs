using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;


namespace Chatprogramm_github
{
    class Speicherung
    {
        const string pfad = @"Sicherungsdatei.xml";


        public static void Speichern(User mainuser, User aktueller_Chatpartner, Nachricht empfangeneNachricht)
        {
            //Entscheidung: 
            XmlDocument Sicherungsdatei = new XmlDocument();
            if (!File.Exists(pfad))
            {
                XmlNode Mainusernode = Create_Usernode("mainuser", mainuser.Username);
                Sicherungsdatei.AppendChild(Mainusernode);

                XmlNode Chats = Sicherungsdatei.CreateElement("Chats");
                Sicherungsdatei.AppendChild(Chats);

                XmlNode Chatkontakt = Create_Usernode("Chatkontakt", aktueller_Chatpartner.Username);
                Sicherungsdatei.AppendChild(Chatkontakt);


                XmlNode Create_Usernode(string nodename, string username)
                {
                    XmlNode user_Node = Sicherungsdatei.CreateElement(nodename);
                    XmlAttribute Username = Sicherungsdatei.CreateAttribute("username");
                    Username.Value = username;
                    return user_Node;
                }
            }
            else
            {
                //Load, Node suchen mit aktuellem Kontakt; neue Nachricht speichern unter aktuellem Kontakt
                //Diese Funktion muss von der Empfangenmethode aufgerufen werden

            }        

        }

    }

    class Laden
    {
        public static List <Nachricht> Nachrichten_Laden(User aktueller_Chatpartner)
        {
            
            //Dokument laden und aus den Nachrichten mit dem entsprechenden Chatpartner eine Liste mit Nachrichten erstellen und ausgeben

            //Die Nachrichten_Darstellen Funktion muss überarbeitet werden.
            return new List<Nachricht>();
        }
        
        public static User Username_Laden()
        {
            //Fehler werfen wenn dokument nicht vorhanden                       
            if (File.Exists(pfad))
            {
                XmlDocument Sicherungsdatei = new XmlDocument();
                Sicherungsdatei.Load(pfad);
            }
            
            return new User();
        }
    //FUnktion Überprüfen ob dok gibt und Usernamen auslesen! Nur Hier prüfen und in Speicherung dann nur Load des Doks. Dann Username zurückgeben
    }
}