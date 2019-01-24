/* Header
 * 
 * 24.01.2019, Jacqueline Kaefer, Luca Katzenberger
 * 
 * Die Klasse Sender wird verwendet, um in einem byte-Array codierte Nachrichten zu verschicken.
 * Sie beinhaltet die Konstanten für IP-Adresse und Port, sowie die Methode zum Senden einer Message.
 * 
 * In der Mainklasse muss ein Objekt dieser Klasse angelegt werden. Dabei wird der UdpClient initialisiert.
 * 
 */

using System.Net.Sockets;

namespace Chatprogramm_github
{
    class Sender
    {
        #region Atttributes_and_Constants
        //Constants
        private const int port = 54546;
        private const string broadcastadress = "255.255.255.255";  //Diese Adresse sendet an alle Geräte, die sich im gleichen Netzwerk befinden. 
        
        //Attributes
        private UdpClient messagesender;
        #endregion

        #region Constructors        
        public Sender()
        {
            //Beim Anlegen eines Sender-Objkets wird dieser Konstruktor aufgerufen.
            //Er initialisert den Sender mit IP-Adresse, Port und den gewünschten Einstellungen.
            messagesender = new UdpClient(broadcastadress, port);
            messagesender.EnableBroadcast = true;      
        }
        #endregion

        #region Methods
        public void send(Message message)
        {
            //Diese Methode soll ein byte Array über den UdpClient an alle Geräte im Netzwerk senden
            byte[] data = message.EncodeMessage();    //Die übergebene Message wird in ein byte-Array Codiert
            messagesender.Send(data, data.Length);      //Die Methode Send schickt die Nachricht ab . Sie benötigt das zu versendene byte-Array und die Anzahl der bytes 
        }
        #endregion
    }
}