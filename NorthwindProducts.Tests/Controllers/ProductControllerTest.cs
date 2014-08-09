using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Northwind.Data;
using Northwind.Data.ViewModels;
using NorthwindProducts.Controllers;
using Moq;

namespace NorthwindProducts.Tests.Controllers
{
    [TestClass]
    public class ProductControllerTest
    {
        private Mock<IApplicationUnit> mock = null;
        List<ProductsViewModel> productsView = null;
        List<Supplier> suppliers = null;
        List<Category> categories = null;

        [TestInitialize]
        public void Init()
        {
            productsView = new List<ProductsViewModel>
            {
                new ProductsViewModel
                {
                    ID = 1,
                    Name = "Product A"
                },
                new ProductsViewModel
                {
                    ID = 2,
                    Name = "Product B"
                }
            };

            suppliers = new List<Supplier>
            {
               new Supplier
               {
                    SupplierID = 1,
                    CompanyName = "Supplier 1"
               },
               new Supplier
               {
                    SupplierID = 2,
                    CompanyName = "Supplier 2"
               }
            };

            categories = new List<Category>
            {
               new Category
               {
                  CategoryID = 1,
                  CategoryName = "Category 1"
               },
               new Category
               {
                  CategoryID = 2,
                  CategoryName = "Category 2"
               }
            };
            mock = new Mock<IApplicationUnit>();
        }

        [TestMethod]
        public void Index_should_display_default_view()
        {
            // Arrange
            ProductController controller = new ProductController(mock.Object);

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.AreEqual("",result.ViewName);
        }

        [TestMethod]
        public void Index_should_display_the_products_list()
        {
            // Arrange
            mock.Setup(m => m.GetProducts()).Returns(productsView);
            ProductController controller = new ProductController(mock.Object);

            // Act
            ViewResult result = controller.Index() as ViewResult;
            var viewModel = controller.ViewData.Model as IEnumerable<ProductsViewModel>;

            // Assert
            Assert.IsTrue(viewModel.Count() == 2);
        }

        [TestMethod]
        public void About_should_display_about_view()
        {
            // Arrange
            ProductController controller = new ProductController(mock.Object);

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("", result.ViewName);
        }

        [TestMethod]
        public void Contact_should_display_contact_view()
        {
            // Arrange
            ProductController controller = new ProductController(mock.Object);

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            // Assert
            Assert.AreEqual("", result.ViewName);
        }

        [TestMethod]
        public void GET_Create_should_display_create_view_with_suppliers_and_categories()
        {
            // Arrange
            mock.Setup(m => m.Suppliers.GetAll()).Returns(suppliers.AsQueryable<Supplier>());
            mock.Setup(m => m.Categories.GetAll()).Returns(categories.AsQueryable<Category>());
            ProductController controller = new ProductController(mock.Object);

            // Act
            ViewResult result = controller.Create() as ViewResult;
            var viewModel = controller.ViewData.Model as ProductsViewModel;

            // Assert
            Assert.IsTrue(viewModel.Suppliers.Count() == 2);
            Assert.IsTrue(viewModel.Categories.Count() == 2);
            Assert.AreEqual("", result.ViewName);
        }

        [TestMethod]
        public void POST_Create_should_save_the_record_and_redirect_to_index()
        {
            // Arrange
            Supplier selectedSupplier = new Supplier
            {
                 SupplierID = 99,
                 CompanyName = "Supplier 99"
            };
            Category selectedCategory = new Category
            {
                 CategoryID = 99,
                 CategoryName = "Category 99"
            };
            ProductsViewModel productViewModel = new ProductsViewModel
            {
                 Name = "Product Romeo",
                 UnitPrice = 20.00M,
                 UnitsInStock = 3,
                 UnitsOnOrder = 3,
                 SelectedCategoryValue = 99,
                 SelectedSupplierValue = 99
            };

            mock.Setup(m => m.Suppliers.GetById(It.IsAny<int>())).Returns(selectedSupplier);
            mock.Setup(m => m.Categories.GetById(It.IsAny<int>())).Returns(selectedCategory);
            mock.Setup(m => m.Suppliers.GetAll()).Returns(suppliers.AsQueryable<Supplier>());
            mock.Setup(m => m.Categories.GetAll()).Returns(categories.AsQueryable<Category>());

            mock.Setup(m => m.Products.Add(It.IsAny<Product>()));
            mock.Setup(m => m.SaveChanges());
           
            ProductController controller = new ProductController(mock.Object);

            // Act
            var result = controller.Create(productViewModel) as RedirectToRouteResult;
            var viewModel = controller.ViewData.Model as ProductsViewModel;

            // Assert
            // Check that each method was only called once.
            mock.Verify(x => x.Products.Add(It.IsAny<Product>()), Times.Once());
            mock.Verify(x => x.SaveChanges(), Times.Once());

            //check that redirec to index happened
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void Details_should_display_information_about_specific_product()
        {
            // Arrange
            mock.Setup(m => m.GetProducts()).Returns(productsView);
            ProductController controller = new ProductController(mock.Object);

            // Act
            ViewResult result = controller.Details(1) as ViewResult;
            var viewModel = controller.ViewData.Model as ProductsViewModel;

            // Assert
            Assert.IsTrue(viewModel.ID == 1);
            Assert.AreEqual("Product A", viewModel.Name);
            Assert.AreEqual("", result.ViewName);
        }

        [TestMethod]
        public void Delete_should_remove_the_product()
        {
            // Arrange
            Product product = new Product 
            {
                 ProductID = 1,
                 ProductName = "Product 1"
            };
            mock.Setup(m => m.Products.GetById(It.IsAny<int>())).Returns(product);
            mock.Setup(m => m.Products.Delete(It.IsAny<int>()));
            mock.Setup(m => m.SaveChanges());
            ProductController controller = new ProductController(mock.Object);

            // Act
            var result = controller.Delete(1) as RedirectToRouteResult;;
            var viewModel = controller.ViewData.Model as ProductsViewModel;

            // Assert
            // Check that each method was only called once.
            mock.Verify(x => x.Products.Delete(It.IsAny<int>()), Times.Once());
            mock.Verify(x => x.SaveChanges(), Times.Once());

            //check that redirec to index happened
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void Edit_should_allow_edit_for_a_specific_product()
        {
            // Arrange
            Product product = new Product
            {
                ProductID = 1,
                ProductName = "Product 1",
                UnitPrice = 10.00M,
                UnitsInStock = 1,
                UnitsOnOrder = 1,
                CategoryID = 1,
                SupplierID = 1
            };
            mock.Setup(m => m.Products.GetById(It.IsAny<int>())).Returns(product);
            Mock<IProductsViewModel> mockProductViewModel = new Mock<IProductsViewModel>();
            mock.Setup(m => m.Suppliers.GetAll()).Returns(suppliers.AsQueryable<Supplier>());
            mock.Setup(m => m.Categories.GetAll()).Returns(categories.AsQueryable<Category>());
            ProductController controller = new ProductController(mock.Object);
            
            //act
            ViewResult result = controller.Edit(1) as ViewResult;
            var viewModel = controller.ViewData.Model as ProductsViewModel;

            // Assert
            Assert.AreEqual(product.ProductID,viewModel.ID);
            Assert.AreEqual(product.ProductName, viewModel.Name);
            Assert.AreEqual(product.UnitPrice, viewModel.UnitPrice);
            Assert.AreEqual(product.UnitsInStock, viewModel.UnitsInStock);
            Assert.AreEqual(product.UnitsOnOrder, viewModel.UnitsOnOrder);
            Assert.AreEqual(product.CategoryID, viewModel.SelectedCategoryValue);
            Assert.AreEqual(product.SupplierID, viewModel.SelectedSupplierValue);
            Assert.AreEqual("", result.ViewName);
        }

        [TestMethod]
        public void POST_Edit_should_save_updated_record()
        {
            // Arrange
            Supplier selectedSupplier = new Supplier
            {
                SupplierID = 99,
                CompanyName = "Supplier 99"
            };
            Category selectedCategory = new Category
            {
                CategoryID = 99,
                CategoryName = "Category 99"
            };

            Product dbproduct = new Product
            {
                ProductID = 1,
                ProductName = "Product 1",
                UnitPrice = 10.00M,
                UnitsInStock = 1,
                UnitsOnOrder = 1,
                CategoryID = 1,
                SupplierID = 1,
                Supplier = new Supplier
                {
                    SupplierID = 1,
                    CompanyName = "Supplier 1"
                },
                Category = new Category
                {
                    CategoryID = 1,
                    CategoryName = "Category 1"
                }
            };

            ProductsViewModel productViewModel = new ProductsViewModel
            {
                ID = 1,
                Name = "Product Romeo",
                UnitPrice = 20.00M,
                UnitsInStock = 3,
                UnitsOnOrder = 3,
                SelectedCategoryValue = 99,
                SelectedSupplierValue = 99
            };

            mock.Setup(m => m.Suppliers.GetById(It.IsAny<int>())).Returns(selectedSupplier);
            mock.Setup(m => m.Categories.GetById(It.IsAny<int>())).Returns(selectedCategory);
            mock.Setup(m => m.Products.GetById(It.IsAny<int>())).Returns(dbproduct);
            mock.Setup(m => m.Suppliers.GetAll()).Returns(suppliers.AsQueryable<Supplier>());
            mock.Setup(m => m.Categories.GetAll()).Returns(categories.AsQueryable<Category>());
            mock.Setup(m => m.Products.Update(It.IsAny<Product>()));
            mock.Setup(m => m.SaveChanges());

            ProductController controller = new ProductController(mock.Object);

            //act
            var result = controller.Edit(productViewModel) as ViewResult;
            var viewModel = controller.ViewData.Model as ProductsViewModel;

            // Assert
            // Check that each method was only called once.
            mock.Verify(x => x.Products.Update(It.IsAny<Product>()), Times.Once());
            mock.Verify(x => x.SaveChanges(), Times.Once());
            Assert.AreEqual(dbproduct.ProductID, viewModel.ID);
            Assert.AreEqual(dbproduct.ProductName, viewModel.Name);
            Assert.AreEqual(dbproduct.UnitPrice, viewModel.UnitPrice);
            Assert.AreEqual(dbproduct.UnitsInStock, viewModel.UnitsInStock);
            Assert.AreEqual(dbproduct.UnitsOnOrder, viewModel.UnitsOnOrder);
            Assert.AreEqual(dbproduct.Supplier, selectedSupplier);
            Assert.AreEqual(dbproduct.Category, selectedCategory);
            Assert.AreEqual("", result.ViewName);
        }
    }
}
