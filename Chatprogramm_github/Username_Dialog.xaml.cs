/* Header
 * 
 * 24.01.2019, Jacqueline Kaefer, Luca Katzenberger
 * 
 * Das Ziel des Dialogs ist die Erstellung eines Users
 * 
 * Dieses Fenster wird angezeigt, wenn es erforderlich ist, dass der Benutzer einen Username eingibt.
 * Dies ist beim ersten Starten des Programms erforderlich, da es noch keine Sicherungsdatei gibt.
 * Außerdem wird es beim Hinzufügen eines neuen Kontakts zur Kontaktliste angezeigt.
 * 
 */

using System.Windows;

namespace Chatprogramm_github
{
    public partial class Username_Dialog : Window
    {
        public Username_Dialog()
        {
            InitializeComponent();  //Initialisert das Dialogfenster
        }

        #region Attributes

        private User Benutzer;
        #endregion

        #region Methods
        private void btn_Ok_Username_Click(object sender, RoutedEventArgs e)
        {
            //Der User wird mit der Eingabe des username erstellt
            this.Benutzer = new User(txt_Username.Text);
            //Entspricht die Eingabe den Anforderungen des Usernames wird der Dialog geschlossen.
            if (this.Benutzer.Username != null)
            {
                this.DialogResult = true;
            }
            //Entspricht die Eingabe nicht den Anforderungen, wird eine Messagebox angezeigt und der Benutzer muss einen neuen Username eingeben.
            else
            {
                //Fehlermeldung, dass die Eingabe des Benutzers nicht den Anforderungen eines Username entspricht.
                MessageBox.Show("Die Länge des Usernames muss zwischen 3 und 20 Stellen lang sein. Bitte geben Sie einen gültigen Username ein.", "Error");
                txt_Username.Text = ""; //Textbox wird geleert
            }
        }
        
        public User ReturnUser()
        {
            //Diese Methode gibt den kreeierten User aus
            return this.Benutzer;
        }
        #endregion
    }
}