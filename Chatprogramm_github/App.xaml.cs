using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows.Threading;
using System.Windows;

namespace Chatprogramm_github
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        //private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        //{
        //    MessageBox.Show("Es ist ein Fehler aufgetreten.");
        //    e.Handled = true;
        //}
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Process unhandled exception
            MessageBox.Show("Es ist ein Fehler aufgetreten.");
            // Prevent default unhandled exception processing
            e.Handled = true;
        }

    }
}
