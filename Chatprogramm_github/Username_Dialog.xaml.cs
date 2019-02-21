/* Header
 * 
 * 24.01.2019, Jacqueline Kaefer, Luca Katzenberger
 * 
 * Das Ziel des Dialogs ist die Erstellung eines Users
 * 
 * Dieses Fenster wird angezeigt, wenn es erforderlich ist, dass der Benutzer einen Username eingibt.
 * Dies ist beim ersten Starten des Programms erforderlich, da es noch keine Sicherungsdatei gibt.
 * Außerdem wird es beim Hinzufügen eines neuen Kontakts zur contactlist angezeigt.
 * 
 */

using System.Windows;
using System.Windows.Input;

namespace Chatprogramm_github
{
    public partial class Username_Dialog : Window
    {
        public Username_Dialog(bool firstcall)
        {
            try
            {
                this.Firstcall = firstcall;
                InitializeComponent();  //Initialisert das Dialogfenster
                txt_Username.Focus();   //Fokus auf Textbox legen, sodass der Benutzer direkt tippen kann
            }
            catch
            {
                MessageBox.Show("Es ist ein Fehler aufgetreten.");                
            }
        }

        #region Attributes

        private User User;
        private bool Firstcall;
        #endregion

        #region Methods
        private void btn_Ok_Username_Click(object sender, RoutedEventArgs e)
        {
            //Der User wird mit der Eingabe des username erstellt
            this.User = new User(txt_Username.Text);
            //Entspricht die Eingabe den Anforderungen des Usernames wird der Dialog geschlossen.
            if (this.User.Username != null)
            {
                this.DialogResult = true;
            }
            //Entspricht die Eingabe nicht den Anforderungen, wird eine Messagebox angezeigt und der Benutzer muss einen neuen Username eingeben.
            else
            {
                //Fehlermeldung, dass die Eingabe des Benutzers nicht den Anforderungen eines Username entspricht.
                MessageBox.Show("Die Länge des Usernames muss zwischen 3 und 20 Stellen lang sein. Bitte geben Sie einen gültigen Username ein.", "Error");
            }
        }
        
        public User ReturnUser()
        {
            //Diese Methode gibt den kreeierten User aus
            return this.User;
        }        

        private void Txt_Username_PreviewKeyUp(object sender, KeyEventArgs e)
        {            
            //Drückt der Benutzer Enter, wird der eingegebene Benutzername übernommen.
            if (e.Key == Key.Enter)
            {
                btn_Ok_Username_Click(sender, e);
            }            
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            if (this.Firstcall == true && DialogResult == false)
            {
                MessageBox.Show("Sie haben das Programm beendet.");
                System.Environment.Exit(0);
            }
        }
        #endregion
    }
}