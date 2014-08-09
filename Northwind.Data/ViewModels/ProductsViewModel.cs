using DataAnnotationsExtensions;
using Northwind.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Northwind.Data.ViewModels
{
    public class ProductsViewModel : IProductsViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage="required")]
        [DisplayName("Product Name")]
        [StringLength(40)]
        public string Name { get; set; }

        [DisplayName("Supplier")]
        public string Supplier { get; set; }

        [DisplayName("Category")]
        public string Category { get; set; }

        [DisplayName("Unit Price")]
        [Numeric]
        public decimal? UnitPrice { get; set; }

        [DisplayName("Units In Stock")]
        [Numeric]
        public short? UnitsInStock { get; set; }

        [DisplayName("Units On Order")]
        [Numeric]
        public short? UnitsOnOrder { get; set; }

        public int? SelectedCategoryValue  { get; set; }
        public int? SelectedSupplierValue { get; set; }

        public virtual Category ProductCategory { get; set; }
        public virtual Supplier ProductSupplier { get; set; }

        public virtual IEnumerable<Category> Categories { get; set; }
        public virtual IEnumerable<Supplier> Suppliers { get; set; }
    }
}