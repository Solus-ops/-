using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data.Entity;
using ShoeApp;
using ShoeApp.Data;
using ShoeApp.Core;

namespace ShoeApp.Views
{
    public partial class ProductsWindow : Window
    {
        public ProductsWindow()
        {
            InitializeComponent();
            SetupUiByRole();
            LoadProducts();
        }

        private void SetupUiByRole()
        {
            bool isAdmin = Session.IsInRole("Администратор");
            bool isManager = Session.IsInRole("Менеджер");

            btnAdd.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            btnEdit.Visibility = (isAdmin || isManager) ? Visibility.Visible : Visibility.Collapsed;
            btnDelete.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;

            if (!Session.IsAuthenticated || Session.IsInRole("Авторизованный клиент"))
            {
                txtSearch.IsEnabled = false;
                btnRefresh.IsEnabled = false;
                btnAdd.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadProducts()
        {
            // подгружаем навигационные свойства с именами как в ShoeEntity (Categories, Manufacturers, Suppliers)
            Db.Context.Products
                .Include(p => p.Categories)
                .Include(p => p.Manufacturers)
                .Include(p => p.Suppliers)
                .Load();

            dgProducts.ItemsSource = Db.Context.Products.Local.ToBindingList();

            dgProducts.LoadingRow += (s, e) =>
            {
                var item = e.Row.Item as Products;
                if (item == null) return;
                int disc = item.Discount;
                if (disc > 15)
                    e.Row.Background = (Brush)(new BrushConverter().ConvertFrom("#2E8B57"));
                else
                    e.Row.Background = Brushes.White;
            };
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Session.IsAuthenticated || Session.IsInRole("Авторизованный клиент")) return;

            string q = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(q))
            {
                dgProducts.ItemsSource = Db.Context.Products.Local.ToBindingList();
                return;
            }

            var list = Db.Context.Products.Local.Where(p =>
                (p.Article ?? "").ToLower().Contains(q) ||
                (p.Name ?? "").ToLower().Contains(q) ||
                (p.Description ?? "").ToLower().Contains(q)).ToList();

            dgProducts.ItemsSource = list;
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Db.Context.Products.Load();
            dgProducts.ItemsSource = Db.Context.Products.Local.ToBindingList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new ProductEditWindow();
            if (wnd.ShowDialog() == true)
            {
                Db.Save();
                BtnRefresh_Click(null, null);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var sel = dgProducts.SelectedItem as Products;
            if (sel == null) { MessageBox.Show("Выберите товар."); return; }
            var wnd = new ProductEditWindow(sel.Id);
            if (wnd.ShowDialog() == true)
            {
                Db.Save();
                BtnRefresh_Click(null, null);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var sel = dgProducts.SelectedItem as Products;
            if (sel == null) { MessageBox.Show("Выберите товар."); return; }
            if (MessageBox.Show($"Удалить товар {sel.Name} ?", "Подтвердите", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            Db.Context.Products.Remove(sel);
            Db.Save();
            BtnRefresh_Click(null, null);
        }
    }
}
