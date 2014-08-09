using Northwind.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Northwind.Data.ViewModels;
using System.Data.Entity.Infrastructure;
using System.Data;
using System.Net;

namespace NorthwindProducts.Controllers
{
    public class ProductController : Controller
    {
        private IApplicationUnit _unit;
        public ProductController(IApplicationUnit unit)
        {
            _unit = unit;
        }

        public ActionResult Index()
        {
            //display grid of products
            List<ProductsViewModel> productsView = this._unit.GetProducts();
            return View(productsView);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            //edit a particular product
            Product product = this._unit.Products.GetById(id);
            if (product == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            var productsViewModel = new ProductsViewModel();
            productsViewModel.ID = product.ProductID;
            productsViewModel.Name = product.ProductName;
            productsViewModel.UnitPrice = product.UnitPrice;
            productsViewModel.UnitsInStock = product.UnitsInStock;
            productsViewModel.UnitsOnOrder = product.UnitsOnOrder;
            productsViewModel.SelectedCategoryValue = product.CategoryID;
            productsViewModel.SelectedSupplierValue = product.SupplierID;

            productsViewModel.Suppliers = this._unit.Suppliers.GetAll().AsEnumerable<Supplier>();
            productsViewModel.Categories = this._unit.Categories.GetAll().AsEnumerable<Category>();

            return View(productsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductsViewModel productViewModel)
        {
            //get selected category
            var selectedCategory = this._unit.Categories.GetById(productViewModel.SelectedCategoryValue);
            //get selected supplier
            var selectedSupplier = this._unit.Suppliers.GetById(productViewModel.SelectedSupplierValue);

            //populate suppliers and categories in viewmodel in case validation fails
            productViewModel.Suppliers = this._unit.Suppliers.GetAll().AsEnumerable<Supplier>();
            productViewModel.Categories = this._unit.Categories.GetAll().AsEnumerable<Category>();

            //get product from database using the ID
            Product product = this._unit.Products.GetById(productViewModel.ID);

            //update values in product
            product.ProductName = productViewModel.Name;
            product.Supplier = selectedSupplier;
            product.Category = selectedCategory;
            product.UnitPrice = productViewModel.UnitPrice;
            product.UnitsInStock = productViewModel.UnitsInStock;
            product.UnitsOnOrder = productViewModel.UnitsOnOrder;

            try
            {
                if (ModelState.IsValid)
                {
                    this._unit.Products.Update(product);
                    this._unit.SaveChanges();
                    ViewBag.EditSuccess = true;
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ViewBag.EditError = true;
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(productViewModel);
        }

        public ActionResult Details(int id)
        {
            //list details for the speicific product
            List<ProductsViewModel> productsView = this._unit.GetProducts();
            ProductsViewModel productView = productsView.FirstOrDefault(p => p.ID == id);
            if (productView == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            return View(productView);
        }

        public ActionResult Create()
        {
            // display empty create page with dropdowns for suppliers and categories
            var productsViewModel = new ProductsViewModel();
            productsViewModel.Suppliers = this._unit.Suppliers.GetAll().AsEnumerable<Supplier>();
            productsViewModel.Categories = this._unit.Categories.GetAll().AsEnumerable<Category>();
            return View(productsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductsViewModel productViewModel)
        {
            //get selected category
            var selectedCategory = this._unit.Categories.GetById(productViewModel.SelectedCategoryValue);
            //get selected supplier
            var selectedSupplier = this._unit.Suppliers.GetById(productViewModel.SelectedSupplierValue);

            //populate suppliers and categories in viewmodel in case validation fails
            productViewModel.Suppliers = this._unit.Suppliers.GetAll().AsEnumerable<Supplier>();
            productViewModel.Categories = this._unit.Categories.GetAll().AsEnumerable<Category>();

            //create a new product object that contains what the user entered
            Product newProduct = new Product
            {
                ProductName = productViewModel.Name,
                Supplier = selectedSupplier,
                Category = selectedCategory,
                UnitPrice = productViewModel.UnitPrice,
                UnitsInStock = productViewModel.UnitsInStock,
                UnitsOnOrder = productViewModel.UnitsOnOrder
            };

            try
            {
                if (ModelState.IsValid)
                {
                    this._unit.Products.Add(newProduct);
                    this._unit.SaveChanges();
                    TempData["CreateSuccess"] = true;
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ViewBag.DeleteError = true;
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(productViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Product product = this._unit.Products.GetById(id);
                if (product == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                this._unit.Products.Delete(id);
                this._unit.SaveChanges();
                TempData["DeleteSuccess"] = true;
            }
            catch (DataException dex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                TempData["DeleteError"] = true;
                ModelState.AddModelError("",  "Delete failed. Try again, and if the problem persists see your system administrator");
            }
            return RedirectToAction("Index","Product");
        }

        public ActionResult About()
        {
            ViewBag.Message = "This is the Northwind Products page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "My contact page.";

            return View();
        }
    }
}