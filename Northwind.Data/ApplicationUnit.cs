using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Data.ViewModels;

namespace Northwind.Data
{
    public class ApplicationUnit : IDisposable, IApplicationUnit
    {
        private DbContext _context = new ProductsDB();
        private IRepository<Product> _products;
        private IRepository<Supplier> _suppliers;
        private IRepository<Category> _categories;

        public void Dispose()
        {
            if (this._context != null)
            {
                this._context.Dispose();
            }
        }

        public IRepository<Product> Products
        {
            get
            {
                if (_products == null)
                {
                    this._products = new ProductsRepository(this._context);
                }
                return this._products;
            }
        }

        public IRepository<Supplier> Suppliers
        {
            get
            {
                if (_suppliers == null)
                {
                    this._suppliers = new SuppliersRepository(this._context);
                }
                return this._suppliers;
            }
        }

        public IRepository<Category> Categories
        {
            get
            {
                if (_categories == null)
                {
                    this._categories = new CategoriesRepository(this._context);
                }
                return this._categories;
            }
        }

        public List<ProductsViewModel> GetProducts()
        {
            List<ProductsViewModel> productsView = new List<ProductsViewModel>();

            var query = (from p in this.Products.GetAll()
                         join s in this.Suppliers.GetAll() on p.SupplierID equals s.SupplierID
                         join c in this.Categories.GetAll() on p.CategoryID equals c.CategoryID
                         select p).Include("Supplier")
                           .Include("Category");

            foreach (var product in query)
            {
                productsView.Add(new ProductsViewModel
                {
                    ID = product.ProductID,
                    Name = product.ProductName,
                    Supplier = product.Supplier.CompanyName,
                    Category = product.Category.CategoryName,
                    UnitPrice = (decimal)product.UnitPrice,
                    UnitsInStock = product.UnitsInStock,
                    UnitsOnOrder = product.UnitsOnOrder
                });
            }

            return productsView;
        }

        public void SaveChanges()
        {
            this._context.SaveChanges();
        }
    }
}
