using System.Windows;

namespace Chatprogramm_github
{
    public partial class Username_Dialog : Window
    {   
        private string Username; //Speichert die Eingabe
        private User Benutzer; //Ziel des Dialogs ist die Erstellung eines Users, des Nutzers dieses Programms; Soll später gespeichert werden

        // Standardkonstruktor
        public Username_Dialog()
        {
            InitializeComponent();
        }
        
        //Bei Klick auf den Button Ok des Fensters wird das ausgeführt
        private void btn_Ok_Username_Click(object sender, RoutedEventArgs e)
        {  
            //Die Eingabe wird ausgelesen und gespeichert
            this.Username = txt_Username.Text;
            //Der User wird mit der Eingabe als username erstellt
            this.Benutzer = new User(this.Username);
            //Entspricht die Eingabe den Anforderungen des Usernames wird der Dialog geschlossen.
            if(this.Benutzer.Username != null)
            {
                this.DialogResult = true;                
            }
            //Entspricht die Eingabe nicht den Anforderungen, wird eine Messagebox geöffnet und der Nutzer muss einen neuen Namen eingeben. Dialog geht von neuem los.
            else
            {
                MessageBox.Show("Die Länge des Usernames muss zwischen 3 und 20 Stellen lang sein. Bitte geben Sie einen gültigen Username ein.", "Error");
                txt_Username.Text = "";
            }
            
        }

        //Methode zur Ausgabe des kreeierten Nutzers.
        public User ReturnUser()
        {
            return this.Benutzer;
        }
    }
}
