using static ShoeApp.ShoeEntity; 
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace ShoeApp.Views
{
    public partial class OrdersWindow : Window
    {
        private ShoeEntity _context;
        private Users _currentUser;

        public OrdersWindow(Users user)
        {
            InitializeComponent();

            _currentUser = user;
            _context = new ShoeEntity();

            CheckAccess();
            LoadOrders();
        }

        private void CheckUserAccess()
        {
            if (_currentUser == null)
            {
                Close();
                return;
            }

            string role = _currentUser.Roles.Name;

            // Клиент — только просмотр
            if (role == "Клиент")
            {
                BtnAdd.Visibility = Visibility.Collapsed;
                BtnEdit.Visibility = Visibility.Collapsed;
                BtnDelete.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadOrders()
        {
            OrdersGrid.ItemsSource = _context.Orders
                .Include(o => o.Users)
                .Include(o => o.OrderStatus)
                .Include(o => o.PickupPoints)
                .ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Добавление заказа — следующий шаг");
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem == null)
                return;

            MessageBox.Show("Редактирование заказа — следующий шаг");
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is Orders order)
            {
                if (MessageBox.Show("Удалить заказ?", "Подтверждение",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _context.Orders.Remove(order);
                    _context.SaveChanges();
                    LoadOrders();
                }
            }
        }
    }
}
