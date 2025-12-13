using System.Windows;
using ShoeApp.Views;

namespace ShoeApp
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
        }
    }
}
