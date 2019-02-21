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
             
                //Die Informationen werden angehängt
                code += Sender.Username + "$%&"; //An den Usernamen wird das Trennzeichen zum Trennen der Informationen angehängt.
                code += Receiver.Username + "$%&";
                code += Timestamp.ToString() + "$%&";
                code += Sent.ToString() + "$%&";
                code += Text;

                //Der String wird zu einem Byte-Array konvertiert
                byte[] data = Encoding.Unicode.GetBytes(code);

                return data; //Der Byte-Array wird zurückgegeben
            }
            catch
            {
                MessageBox.Show("Es ist ein Fehler beim Senden der Nachricht aufgetreten, bitte versuchen Sie es erneut.");
                return null;
            }         
        }
        
        public static Message DecodeMessage (byte[] data)
        {
            try
            {
                //Diese Nachricht erhält einen Byte-Array, der Empfangen worden ist und erstellt daraus ein Objekt Nachricht, das zurückgegeben wird. 
                Message receivedmessage = new Message();
                string encodedmessage = Encoding.Unicode.GetString(data);
                string[] receiveddata = new string[5];

                // Der Byte-Array wird in einen String konvertiert. Dieser String wird an den Trennzeichen aufgetrennt und die Substrings in einem String-Array gespeichert.
                //Die einzelnen Substrings werden dann in den Eigenschaften der Nachricht geschrieben.
                receiveddata = encodedmessage.Split(new string[] { "$%&" }, StringSplitOptions.None);
                receivedmessage.Sender.Username = receiveddata[0];
                receivedmessage.Receiver.Username = receiveddata[1];
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
            mytxtbox.Width = 250;
            mytxtbox.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(150,175, 238, 238));
            mytxtbox.IsReadOnly = true;
            mytxtbox.TextWrapping = TextWrapping.Wrap;
            mytxtbox.BorderBrush = System.Windows.Media.Brushes.Black;

            return mytxtbox;
        }
        #endregion
    }
}