/* Header
 * 
 * 24.01.2019, Jacqueline Kaefer, Luca Katzenberger
 * 
 * Dieses Fenster wird angezeigt, wenn der Mainuser eine Message von einem User erhalten hat, der nicht in der contactlist enthalten ist.
 * Er wird dazu aufgefordert zu entscheiden, ob die Message, und damit auch der User, gespeichert werden sollen.
 * Als Ergebnis wird ein Bool-Wert zurückgegeben.
 * 
 */

using System.Windows;

namespace Chatprogramm_github
{
    public partial class UnknownUserDialog : Window
    {
        public UnknownUserDialog(User sender)
        {
            try
            {
                InitializeComponent();  //Die Elemente des Fensters werden initialisiert
                                        //Zeigt den Usernamen des unbekannten User an.
                Tb_display.Text = "Sie haben eine Nachricht von " + sender.Username + " erhalten. Wollen Sie diesen Nutzer als neuen Kontakt speichern oder ignorieren?";
            }
            catch
            {
                MessageBox.Show("Es ist ein Fehler aufgetreten.");
            }
       
        }

        #region Attributes
        public bool result;   //Message & User sollen gespeichert werden = true | Message & User sollen verworfen werden = false
        #endregion

        #region Methods
        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            result = true;    //Message & User sollen gespeichert werden
            DialogResult = true;    //Dialog war erfolgreich
        }

        private void btn_Ignore_Click(object sender, RoutedEventArgs e)
        {
            result = false;   //Message & User sollen verworfen werden
            DialogResult = true;    //Dialog war erfolgreich
        }
        //
        #endregion
    }
}