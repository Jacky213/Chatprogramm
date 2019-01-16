using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Chatprogramm_github
{
    class Sender
    {       
        const int port = 54546;
        const string broadcastadress = "255.255.255.255";       
        UdpClient nachrichtensender;

        public Sender()
        {
            //Sender initialisieren
            nachrichtensender = new UdpClient(broadcastadress, port);
            nachrichtensender.EnableBroadcast = true;      
        }

        public void send(Nachricht nachricht)
        {
            byte[] data = nachricht.NachrichtCodieren();
            nachrichtensender.Send(data, data.Length);
        }
       
    }
    
    
}
