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
using System.Windows;

namespace Chatprogramm_github
{
    public class Message
    {
        #region Attributes
        public string Text;
        public User Sender;   //20 Zeichen
        public User Receiver;    //20 Zeichen
        public DateTime Timestamp;  //19 Zeichen
        public bool Sent;    //4 Zeichen
        #endregion

        #region Constructors
        public Message(string text, User sender, User receiver, DateTime timestamp, bool sent)
        {
            this.Text = text;
            this.Sender = sender;
            this.Receiver = receiver;
            this.Timestamp = timestamp;
            this.Sent = sent;
        }

        public Message()
        {
            this.Sender = new User();
            this.Receiver = new User();
        }
        #endregion

        #region Methods
        public byte[] EncodeMessage()
        {
            try
            {
                //Diese Methode erzeugt aus einem Objekt Nachricht einen codierten String, der dann verschickt werden kann. Dieser String wird dann als Byte-Array zurückgegeben.

                //Protokoll: Sender,Empfänger,Zeitpunkt,Abgeschickt,Nachricht 

                string code = "";
                string username = Sender.Username;
                while (username.Length < 20)
                {
                    username += " "; //Der Usernamen wird mit Leerzeichen auf 20 Zeichen verlängert.
                }
                code += username + "$%&"; //An den Usernamen wird das Trennzeichen zum Trennen der Informationen angehängt.
                string receivername = Receiver.Username;
                while (receivername.Length < 20)
                {
                    receivername += " "; //Auch der Empfängername wird auf 20 Zeichen verlängert.
                }
                //Die anderen Informationen werden angehängt
                code += receivername + "$%&";
                code += Timestamp.ToString() + "$%&";
                code += Sent.ToString() + "$%&";
                code += Text;

                //Der String wird zu einem Byte-Array konvertiert
                byte[] data = Encoding.Unicode.GetBytes(code);

                return data; //Der Byte-Array wird zurückgegeben
            }
            catch
            {
                MessageBox.Show("Es ist ein beim Senden der Nachricht aufgetreten, bitte versuchen Sie es erneut.");
                return null;
            }
         
        }
        

        public static Message DecodeMessage (string Codierung)
        {
            try
            {
                //Diese Nachricht erhält einen Byte-Array, der Empfangen worden ist und erstellt daraus ein Objekt Nachricht, das zurückgegeben wird. 
                Message receivedmessage = new Message();
                string[] receiveddata = new string[5];

                // Der Byte-Array wird in einen String konvertiert. Dieser String wird an den Trennzeichen aufgetrennt und die Substrings in einem String-Array gespeichert.
                //Die einzelnen Substrings werden dann in den Eigenschaften der Nachricht geschrieben.

                receiveddata = Codierung.Split(new string[] { "$%&" }, StringSplitOptions.None);
                receivedmessage.Sender.Username = receiveddata[0].Trim();
                receivedmessage.Receiver.Username = receiveddata[1].Trim();
                receivedmessage.Timestamp = Convert.ToDateTime(receiveddata[2]);
                receivedmessage.Sent = Convert.ToBoolean(receiveddata[3]);
                receivedmessage.Text = receiveddata[4];

                return receivedmessage;
            }
            catch
            {
                MessageBox.Show("Beim Empfang einer Nachricht ist ein Fehler aufgetreten.");
                return null;
            }
     
        } 

        public TextBox MessageinTextbox()
        {
            //Diese Funktion erstellt eine Textbox, die formatiert wird und den Nachrichtentext enthält. Diese Textbox wird zurückgegeben.

            TextBox mytxtbox = new TextBox();
            mytxtbox.Text = this.Text;
            mytxtbox.Width = 500;
            mytxtbox.Background = new SolidColorBrush(Colors.ForestGreen);
            mytxtbox.IsReadOnly = true;

            return mytxtbox;
        }
        #endregion
    }
}