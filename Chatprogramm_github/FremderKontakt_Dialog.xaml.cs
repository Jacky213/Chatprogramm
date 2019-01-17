using System.Windows;

namespace Chatprogramm_github
{
    public partial class FremderKontakt_Dialog : Window
    {
        public FremderKontakt_Dialog()
        {
            InitializeComponent();
        }

        public bool Ergebnis;

        private void btn_Speichern_Click(object sender, RoutedEventArgs e)
        {
            Ergebnis = true;
            DialogResult = true;
        }

        private void btn_Ignorieren_Click(object sender, RoutedEventArgs e)
        {
            Ergebnis = false;
            DialogResult = true;
        }
    }
}