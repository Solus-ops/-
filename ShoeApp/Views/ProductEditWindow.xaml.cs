using System.Linq;
using System.Windows;
using ShoeApp;
using ShoeApp.Data;

namespace ShoeApp.Views
{
    public partial class ProductEditWindow : Window
    {
        private Products _product;
        private bool _isNew;

        public ProductEditWindow(int? productId = null)
        {
            InitializeComponent();
            LoadLookups();

            if (productId == null)
            {
                _isNew = true;
                _product = new Products();
            }
            else
            {
                _isNew = false;
                _product = Db.Context.Products.FirstOrDefault(p => p.Id == productId.Value);
                if (_product != null)
                    FillForm();
            }
        }

        private void LoadLookups()
        {
            cbCategory.ItemsSource = Db.Context.Categories.ToList();
            cbManufacturer.ItemsSource = Db.Context.Manufacturers.ToList();
            cbSupplier.ItemsSource = Db.Context.Suppliers.ToList();
        }

        private void FillForm()
        {
            tbArticle.Text = _product.Article;
            tbName.Text = _product.Name;
            tbPrice.Text = _product.Price.ToString();
            tbDiscount.Text = _product.Discount.ToString();
            tbStock.Text = _product.StockQty.ToString();
            tbDescription.Text = _product.Description;

            cbCategory.SelectedItem =
                Db.Context.Categories.FirstOrDefault(c => c.Id == _product.CategoryId);
            cbManufacturer.SelectedItem =
                Db.Context.Manufacturers.FirstOrDefault(m => m.Id == _product.ManufacturerId);
            cbSupplier.SelectedItem =
                Db.Context.Suppliers.FirstOrDefault(s => s.Id == _product.SupplierId);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            decimal price;
            int discount;
            int stock;

            decimal.TryParse(tbPrice.Text.Trim(), out price);
            int.TryParse(tbDiscount.Text.Trim(), out discount);
            int.TryParse(tbStock.Text.Trim(), out stock);

            _product.Article = tbArticle.Text.Trim();
            _product.Name = tbName.Text.Trim();
            _product.Price = price;
            _product.Discount = discount;
            _product.StockQty = stock;
            _product.Description = tbDescription.Text.Trim();

            _product.CategoryId = (cbCategory.SelectedItem as Categories)?.Id;
            _product.ManufacturerId = (cbManufacturer.SelectedItem as Manufacturers)?.Id;
            _product.SupplierId = (cbSupplier.SelectedItem as Suppliers)?.Id;

            if (_isNew)
                Db.Context.Products.Add(_product);

            Db.Save();
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
