using System;

namespace Chatprogramm_github
{
    class Nachricht
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
        }

        //Methoden
        //bytearray in Methoden einfügen
        public string NachrichtCodieren()
        {
            //Protokoll: Sender,Empfänger,Zeitpunkt,Abgeschickt,Nachricht
            string Codierung = "";
            string username = Sender.Username; 
            while (username.Length < 20)
            {
                username += " ";
            }
            Codierung += username  + ",";
            string empfängername = Empfänger.Username;
            while (empfängername.Length < 20)
            {
                empfängername += " ";
            }
            Codierung += empfängername + ",";
            Codierung += Zeitpunkt.ToString() + ",";
            Codierung += Abgeschickt.ToString() + ",";
            Codierung += Nachrichtentext;

            return Codierung;
        }

        public static Nachricht NachrichtDecodieren (string Codierung)
        {
            Nachricht Empfang = new Nachricht();
            string[]  EmpfangeneDaten = new string[5];

            EmpfangeneDaten = Codierung.Split(',');
            Empfang.Sender.Username = EmpfangeneDaten[0].Trim();
            Empfang.Empfänger.Username = EmpfangeneDaten[1].Trim();
            Empfang.Zeitpunkt = Convert.ToDateTime(EmpfangeneDaten[2]);
            Empfang.Abgeschickt = Convert.ToBoolean(EmpfangeneDaten[3]);
            Empfang.Nachrichtentext = EmpfangeneDaten[4];

            return Empfang;
        }        
    }
}