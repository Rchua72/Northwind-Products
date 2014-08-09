using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Data.ViewModels
{
    public interface IProductsViewModel
    {
        int ID { get; set; }

        string Name { get; set; }

        string Supplier { get; set; }

        string Category { get; set; }

        decimal? UnitPrice { get; set; }

        short? UnitsInStock { get; set; }

        short? UnitsOnOrder { get; set; }

        int? SelectedCategoryValue { get; set; }
        int? SelectedSupplierValue { get; set; }

        Category ProductCategory { get; set; }
        Supplier ProductSupplier { get; set; }

        IEnumerable<Category> Categories { get; set; }
        IEnumerable<Supplier> Suppliers { get; set; }
    }
}
