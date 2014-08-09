using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Data.ViewModels;

namespace Northwind.Data
{
    public interface IApplicationUnit
    {
        IRepository<Product> Products { get; }
        IRepository<Supplier> Suppliers { get; }
        IRepository<Category> Categories { get; }
        List<ProductsViewModel> GetProducts();
        void SaveChanges();
    }
}
