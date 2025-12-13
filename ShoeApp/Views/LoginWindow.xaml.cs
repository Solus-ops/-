using System.Linq;
using System.Windows;
using ShoeApp.Data;
using ShoeApp.Core;

namespace ShoeApp.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = pwd.Password.Trim();

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            var user = Db.Context.Users
                .Include("Roles")
                .FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль");
                return;
            }

            Session.CurrentUser = user;

            var wnd = new ProductsWindow();
            wnd.Show();
            this.Close();
        }

        private void BtnGuest_Click(object sender, RoutedEventArgs e)
        {
            Session.CurrentUser = null;

            var wnd = new ProductsWindow();
            wnd.Show();
            this.Close();
        }
    }
}
