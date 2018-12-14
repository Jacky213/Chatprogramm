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
    {
        public string Eingabe; //Kann hier noch über Get/Set die Länge des Namens begrenzen, wenn gewollt.

        public Username_Dialog()
        {
            InitializeComponent();
        }

        private void btn_Ok_Username_Click(object sender, RoutedEventArgs e)
        {
            this.Eingabe = txt_Username.Text;
            this.DialogResult = true;
        }
    }
}
