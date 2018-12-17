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
using System.Windows.Shapes;

namespace Chatprogramm_github
{
    /// <summary>
    /// Interaktionslogik für Username_Dialog.xaml
    /// </summary>
    public partial class Username_Dialog : Window
    {   //Attribute sind beide Private
        
        string Username; //Speichert die Eingabe
        User Mainuser; //Ziel des Dialogs ist die Erstellung eines Users, des Nutzers dieses Programms, Soll später gespeichert werden

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
            this.Mainuser = new User(this.Username);
            //Entspricht die Eingabe den Anforderungen des Usernames wird der Dialog geschlossen.
            if(this.Mainuser.Username!=null)
            {
                this.DialogResult = true;
                
            }
            //Entspricht die Eingabe nicht den Anforderungen, wird eine Messagebox geöffnet und der Nutzer muss einen neuen Namen eingeben. Geht von neuem los.
            else
            {
                MessageBox.Show("Die Länge des Usernames muss zwischen 3 und 20 Stellen lang sein. Bitte geben Sie einen gültigen Username ein.", "Error");
                txt_Username.Text = "";
            }
            
        }

        //Methode zur Ausgabe des kreeierten Nutzers.
        public User ReturnUser()
        {
            return this.Mainuser;
        }
    }
}
