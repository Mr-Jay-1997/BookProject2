using BLLBookProject;
using BookProject.Models;
using DALBookProject.Database;
using DALBookProject.Database.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SharedLibrary.Models;
using System.Globalization;

namespace BookProject.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        BLLBook bllbook = new BLLBook();
        public IActionResult Index()
        {
            var bookList = bllbook.GetBookList();
            return View(bookList);
        }

        public IActionResult Details(int id)
        {
            var book = bllbook.GetBook(id);
            return PartialView("_DetailsPartialView", book);
        }

        public IActionResult Create()
        {
            using (var db = new BookDbContext(BookDbContext.ops.dbOptions))
            {
                var bookModel = new BookModel();
                BLLCategory bLLCategory = new BLLCategory();
                ViewBag.Categories = GetCategories();
                return View(bookModel);
            }
        }

        [HttpPost]
        public IActionResult Create(BookModel bookModel)
        {
            try
            {
                bookModel = bllbook.CreateBook(bookModel);
                TempData["SuccessMessage"] = "Book " + bookModel.BookName + " Created Successfully";
                return RedirectToAction(nameof(Index));

            }
            catch
            {
                TempData["ErrorMessage"] = "Book " + bookModel.BookName + " failed to create";

            }
            return View(bookModel);

        }

        public IActionResult Edit(int id)
        {
            BLLCategory bLLCategory = new BLLCategory();
            ViewBag.Categories = GetCategories();
            var updatedBook = new BookModel();
            updatedBook = bllbook.GetBook(id);

            return View(updatedBook);
        }

        [HttpPost]
        public IActionResult Edit(int bookId, BookModel updatedBook)
        {
            try
            {
                updatedBook = bllbook.UpdateBook(bookId, updatedBook);
                TempData["SuccessMessage"] = "Book " + updatedBook.BookName + " Saved Successfully";

            }
            catch
            {
                TempData["ErrorMessage"] = "Book " + updatedBook.BookName + " not Updated";

            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var bookModel = new BookModel();
            bookModel = bllbook.GetBook(id);
            return PartialView("_DeletePartialView", bookModel);
        }

        [HttpPost]
        public IActionResult Delete(BookModel bookModel)
        {
            try
            {
                //ModelState.Remove("BookName");
                //ModelState.Remove("CategoryName");
                //ModelState.Remove("Author");
                //ModelState.Remove("Publisher");
                //ModelState.Remove("Price");
                if (ModelState.IsValid)
                {
                    var result = bllbook.DeleteBook(bookModel);
                    if (result == true)
                    {
                        TempData["SuccessMessage"] = "Book " + bookModel.BookName + " Deleted Successfully";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Book " + bookModel.BookName + " Delete Failed";
                    }

                }
                else
                {
                    TempData["ErrorMessage"] = "Book " + bookModel.BookName + " Delete Failed";
                }
                
            }
            catch
            {
                // Handle any exceptions
            }
            return RedirectToAction(nameof(Index));
        }

        private List<SelectListItem> GetCategories()
        {
            var lstcategories = new List<SelectListItem>();
            BLLCategory bLLCategory= new BLLCategory();
            List<CategoryModel> categories = bLLCategory.GetCategoryList();
            lstcategories = categories.Select(ut=> new SelectListItem()
            {
                Value = ut.CategoryId.ToString(),
                Text = ut.CategoryName
            }).ToList();

            var defItem = new SelectListItem()
            {
                Value = "",
                Text = "---Select Category---"
            };
            lstcategories.Insert(0, defItem);
            return lstcategories;
        }
    }
}
