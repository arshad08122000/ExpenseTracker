using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using Transaction = ExpenseTracker.Models.Transaction;

namespace ExpenseTracker.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.transactions.Include(t => t.category);
            return View(await applicationDbContext.ToListAsync());
        }

        public IActionResult AddOrEdit(int id=0)
        {
            PopulateCategories();
            if (id == 0)
                return View(new Transaction());
            else
                return View(_context.transactions.Find(id));
        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("transactionid,categoryid,amount,note,date")] Transaction transaction)
        {
            if(ModelState.IsValid)
			{
                if(transaction.transactionid == 0)
                {
                    _context.Add(transaction);
                }
                else
                {
                    _context.Update(transaction);
                }
               
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
			}
            PopulateCategories();
            return View(transaction); 
        }
        
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if(_context.transactions==null)
            {
                return Problem("Entity set 'ApplicationDbContext.transactions' is null.");
            }
            var transaction = await _context.transactions.FindAsync(id);
            if(transaction!=null)
            {
                _context.transactions.Remove(transaction);  
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public void PopulateCategories()
        {
            var categoryCollection=_context.categories.ToList();
            Category DefaultCategory = new Category() { categoryid = 0, title = "Choose a category" };
            categoryCollection.Insert(0,DefaultCategory);
            ViewBag.categories = categoryCollection;
        }
    }

}
