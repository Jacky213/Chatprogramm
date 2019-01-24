/* Header
 * 
 * 24.01.2019, Jacqueline Kaefer, Luca Katzenberger
 * 
 * Diese Klasse definiert das Objekt User. Ein User hat nur die Eigenschaft "username". Alle Benutzer, einschließlich des Mainuser sind User. 
 * Beim Anlegen eines User wird über get/set überprüft, ob er die vorgegebene Zeichenlänge einhält.
 * 
 * Außerdem enthält er mehrere Konstruktoren, sodass direkt beim Aanlegen einer User ein Username mitgegeben werden kann.
 * 
 */

namespace Chatprogramm_github
{   
    public class User
    {
        #region Attributes
        private string username; 

        public string Username  //Getter/Setter
        {
            //Mit get/set soll bei der Eingabe eines Username überprüft werden, ob die erforderliche Zeichenlänge eingehalten wurde.
            //Ein Username muss zwischen 3 und 20 Zeichen lang sein.
            get
            {
                return username; //Get gibt den usernamen zurück
            }
            set 
            {
                if ( value.Trim().Length <= 20 && value.Trim().Length >= 3) //Hat der Username zwischen 3 und 20 Zeichen?
                {
                    this.username = value;
                }
                else
                {
                    this.username = null;   //Bei falscher Eingabe wird der Username auf null gesetzt
                }
            }
        }
        #endregion

        #region Konstruktoren
        public User(string name) 
        {
            //Dieser Konstruktor initialisiert einen User direkt beim Anlegen mit einem Username
            this.Username = name;
        }
        public User()
        {
            //Mit diesem Konstruktor kann ein User zunächst ohne Username angelegt werden.
        }
        #endregion
    }
}