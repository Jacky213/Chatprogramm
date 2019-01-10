using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Chatprogramm_github
{
    public class Nachricht
    {
        //Eigenschaften
        public string Nachrichtentext;
        public User Sender;   //20 Zeichen
        public User Empfänger;    //20 Zeichen
        public DateTime Zeitpunkt;  //19 Zeichen
        public bool Abgeschickt;    //4 Zeichen

        //Konstruktoren
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

        //Methoden
        //bytearray in Methoden einfügen    !erledigt!
        public byte[] NachrichtCodieren()
        {
            //Protokoll: Sender,Empfänger,Zeitpunkt,Abgeschickt,Nachricht
            string Codierung = "";
            string username = Sender.Username; 
            while (username.Length < 20)
            {
                username += " ";
            }
            Codierung += username  + "$%&";
            string empfängername = Empfänger.Username;
            while (empfängername.Length < 20)
            {
                empfängername += " ";
            }
            Codierung += empfängername + "$%&";
            Codierung += Zeitpunkt.ToString() + "$%&";
            Codierung += Abgeschickt.ToString() + "$%&";
            Codierung += Nachrichtentext;

            byte[] data = Encoding.Unicode.GetBytes(Codierung);
            return data;
        }

        public static Nachricht NachrichtDecodieren (string Codierung)
        {
            Nachricht Empfang = new Nachricht();
            string[]  EmpfangeneDaten = new string[5];
            
            EmpfangeneDaten = Codierung.Split(new string[] { "$%&"},StringSplitOptions.None);
            Empfang.Sender.Username = EmpfangeneDaten[0].Trim();
            Empfang.Empfänger.Username = EmpfangeneDaten[1].Trim();
            Empfang.Zeitpunkt = Convert.ToDateTime(EmpfangeneDaten[2]);
            Empfang.Abgeschickt = Convert.ToBoolean(EmpfangeneDaten[3]);
            Empfang.Nachrichtentext = EmpfangeneDaten[4]; //Probelm wenn Nachricht ein Komma enthält!!

            return Empfang;
        } 


        public TextBox EmpfangeneNachrichtAusgabe()
        {
            TextBox mytxt = new TextBox();
            mytxt.Text = this.Nachrichtentext;
            mytxt.Width = 500;
            mytxt.Background = new SolidColorBrush(Colors.ForestGreen);
            mytxt.IsReadOnly = true;

            return mytxt;
        }
    }
}