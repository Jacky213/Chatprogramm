using System;

namespace Chatprogramm_github
{
    class Nachricht
    {
        //Eigenschaften
        public string Nachrichtentext;
        public string Sendername;   //20 Zeichen
        public string Empfängername;    //20 Zeichen
        public DateTime Zeitpunkt;  //19 Zeichen
        public bool Abgeschickt;    //4Zeichen

        //Konstruktoren
        public Nachricht(string nachrichtentext, string sendername, string empfängername, DateTime zeitpunkt, bool abgeschickt)
        {
            Nachrichtentext = nachrichtentext;
            Sendername = sendername;
            Empfängername = empfängername;
            Zeitpunkt = zeitpunkt;
            Abgeschickt = abgeschickt;
        }

        public Nachricht()
        {
        }

        //Methoden
        public string NachrichtCodieren()
        {
            //Protokoll: Sender,Empfänger,Zeitpunkt,Abgeschickt,Nachricht
            string Codierung = "";

            while (Sendername.Length < 20)
            {
                Sendername += " ";
            }
            Codierung += Sendername  + ",";
            while (Empfängername.Length < 20)
            {
                Empfängername += " ";
            }
            Codierung += Empfängername + ",";
            Codierung += Zeitpunkt.ToString() + ",";
            Codierung += Abgeschickt.ToString() + ",";
            Codierung += Nachrichtentext + ",";

            return Codierung;
        }

        public static Nachricht NachrichtDecodieren (string Codierung)
        {
            Nachricht Empfang = new Nachricht();
            string[]  EmpfangeneDaten = new string[5];

            EmpfangeneDaten = Codierung.Split(',');
            Empfang.Sendername = EmpfangeneDaten[0].Trim();
            Empfang.Empfängername = EmpfangeneDaten[1].Trim();
            Empfang.Zeitpunkt = Convert.ToDateTime(EmpfangeneDaten[2]);
            Empfang.Abgeschickt = Convert.ToBoolean(EmpfangeneDaten[3]);
            Empfang.Nachrichtentext = EmpfangeneDaten[4];

            return Empfang;
        }        
    }
}