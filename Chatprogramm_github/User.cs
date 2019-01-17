namespace Chatprogramm_github
{   //Klasse, die Users definiert. Alle Nutzer einschliesslich des Nutzers und der Kontakte sind Users
    public class User
    {
        //Attribute sind in Private und Public aufgeteilt, um get/Set-Methoden zu definieren
        private string username; 
        public string Username
        {
            get
            {
                return username; //Get gibt den usernamen zurück
            }
            set //Der Username muss zwischen 3 und 20 Stellen lang sein, damit er gespeichert wird. Ist dies nicht der fall, wird er auf null gesetzt
            {
                if ( value.Trim().Length <= 20 && value.Trim().Length >= 3)
                {
                    this.username = value;
                }
                else
                {
                    this.username = null; //Könnte hier auch eine Exception werfen...
                }
            }
        }

        //Konstruktoren
        public User(string name) 
        {
            this.Username = name;
        }
        public User()
        {

        }

        //Enthält bis jetzt noch keine Methoden
        
    }
}
