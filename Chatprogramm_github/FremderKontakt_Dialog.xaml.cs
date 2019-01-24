/* Header
 * 
 * 24.01.2019, Jacqueline Kaefer, Luca Katzenberger
 * 
 * Dieses Fenster wird angezeigt, wenn der Mainuser eine Message von einem User erhalten hat, der nicht in der Kontaktliste enthalten ist.
 * Er wird dazu aufgefordert zu entscheiden, ob die Message, und damit auch der User, gespeichert werden sollen.
 * Als Ergebnis wird ein Bool-Wert zurückgegeben.
 * 
 */

using System.Windows;

namespace Chatprogramm_github
{
    public partial class FremderKontakt_Dialog : Window
    {
        public FremderKontakt_Dialog()
        {
            InitializeComponent();  //Die Elemente des Fensters werden initialisiert
        }

        #region Attributes
        public bool Ergebnis;   //Message & User sollen gespeichert werden = true | Message & User sollen verworfen werden = false
        #endregion

        #region Methods
        private void btn_Speichern_Click(object sender, RoutedEventArgs e)
        {
            Ergebnis = true;    //Message & User sollen gespeichert werden
            DialogResult = true;    //Dialog war erfolgreich
        }

        private void btn_Ignorieren_Click(object sender, RoutedEventArgs e)
        {
            Ergebnis = false;   //Message & User sollen verworfen werden
            DialogResult = true;    //Dialog war erfolgreich
        }
        #endregion
    }
}