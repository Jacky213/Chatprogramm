/*Haeder
 * 24.01.2019, Jacqueline Kaefer und Luca Katzenberger
 *Diese Klasse definiert das Objekt Nachricht mit all seinen Eigenschaften.
 * Objekte dieser Klassen enthalten unterschiedliche Eigenschaften, um wichtige Informationen zu einer Nachricht zusammenzuführen.
 * Die Klasse enthält ausserdem zwei Konstruktoren und mehrere Methoden, unter anderem zum Codieren und Decodieren von Nachrichten.
 */


using System;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chatprogramm_github
{
    public class Nachricht
    {
        #region Attributes
        public string Nachrichtentext;
        public User Sender;   //20 Zeichen
        public User Empfänger;    //20 Zeichen
        public DateTime Zeitpunkt;  //19 Zeichen
        public bool Abgeschickt;    //4 Zeichen
        #endregion


        #region Constructors
        public Nachricht(string nachrichtentext, User sender, User empfänger, DateTime zeitpunkt, bool abgeschickt)
        {
            this.Nachrichtentext = nachrichtentext;
            this.Sender = sender;
            this.Empfänger = empfänger;
            this.Zeitpunkt = zeitpunkt;
            this.Abgeschickt = abgeschickt;
        }

        public Nachricht()
        {
            this.Sender = new User();
            this.Empfänger = new User();
        }
        #endregion

        #region Methods
        public byte[] NachrichtCodieren()
        {
            //Diese Methode erzeugt aus einem Objekt Nachricht einen codierten String, der dann verschickt werden kann. Dieser String wird dann als Byte-Array zurückgegeben.

            //Protokoll: Sender,Empfänger,Zeitpunkt,Abgeschickt,Nachricht 

            string Codierung = "";
            string username = Sender.Username; 
            while (username.Length < 20)
            {
                username += " "; //Der Usernamen wird mit Leerzeichen auf 20 Zeichen verlängert.
            }
            Codierung += username  + "$%&"; //An den Usernamen wird das Trennzeichen zum Trennen der Informationen angehängt.
            string empfängername = Empfänger.Username;
            while (empfängername.Length < 20)
            {
                empfängername += " "; //Auch der Empfängername wird auf 20 Zeichen verlängert.
            }
            //Die anderen Informationen werden angehängt
            Codierung += empfängername + "$%&";
            Codierung += Zeitpunkt.ToString() + "$%&";
            Codierung += Abgeschickt.ToString() + "$%&";
            Codierung += Nachrichtentext; 

            //Der String wird zu einem Byte-Array konvertiert
            byte[] data = Encoding.Unicode.GetBytes(Codierung);

            return data; //Der Byte-Array wird zurückgegeben
        }
        

        public static Nachricht NachrichtDecodieren (string Codierung)
        {
            //Diese Nachricht erhält einen Byte-Array, der Empfangen worden ist und erstellt daraus ein Objekt Nachricht, das zurückgegeben wird. 
            Nachricht Empfang = new Nachricht();
            string[]  EmpfangeneDaten = new string[5];
            
            // Der Byte-Array wird in einen String konvertiert. Dieser String wird an den Trennzeichen aufgetrennt und die Substrings in einem String-Array gespeichert.
            //Die einzelnen Substrings werden dann in den Eigenschaften der Nachricht geschrieben.

            EmpfangeneDaten = Codierung.Split(new string[] { "$%&"},StringSplitOptions.None);
            Empfang.Sender.Username = EmpfangeneDaten[0].Trim();
            Empfang.Empfänger.Username = EmpfangeneDaten[1].Trim();
            Empfang.Zeitpunkt = Convert.ToDateTime(EmpfangeneDaten[2]);
            Empfang.Abgeschickt = Convert.ToBoolean(EmpfangeneDaten[3]);
            Empfang.Nachrichtentext = EmpfangeneDaten[4];

            return Empfang;
        } 

        public TextBox NachrichtinTextbox()
        {
            //Diese Funktion erstellt eine Textbox, die formatiert wird und den Nachrichtentext enthält. Diese Textbox wird zurückgegeben.

            TextBox mytxt = new TextBox();
            mytxt.Text = this.Nachrichtentext;
            mytxt.Width = 500;
            mytxt.Background = new SolidColorBrush(Colors.ForestGreen);
            mytxt.IsReadOnly = true;

            return mytxt;
        }
        #endregion
    }
}