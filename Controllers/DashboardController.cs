using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ExpenseTracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
            DateTime startdate = DateTime.Today.AddDays(-2);
            DateTime enddate = DateTime.Today;

            List<Transaction> selectedtransactions = await _context.transactions
                .Include(x => x.category)
                .Where(y => y.date >= startdate && y.date <= enddate)
                .ToListAsync();

            int totalincome = selectedtransactions.Where(i => i.category.type == "income").Sum(j => j.amount);
            ViewBag.totalincome = totalincome.ToString("C0");

            int totalexpense = selectedtransactions.Where(i => i.category.type == "expense").Sum(j => j.amount);
            ViewBag.totalexpense = totalexpense.ToString("C0");

            int balance = totalincome - totalexpense;
            CultureInfo cul = CultureInfo.CreateSpecificCulture("en-US");
            cul.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.balance = String.Format(cul, "{0:C0}", balance);


            ViewBag.ChartData = selectedtransactions.Where(i => i.category.type == "expense").GroupBy(j => j.category.categoryid).Select(k => new { 
                    categorytitlewithicon=k.First().category.icon+" "+k.First().category.title,
                    amount = k.Sum(j => j.amount),
                    formattedamount = k.Sum(j => j.amount).ToString("C0"),
                }
                ).OrderByDescending(l=>l.amount).ToList();


            List<splinechhart> incomesummary = selectedtransactions
                .Where(i => i.category.type == "income")
                .GroupBy(j => j.date)
                .Select(k => new splinechhart()
                {
                    day = k.First().date.ToString("dd-MMM"),
                    income = k.Sum(l=>l.amount)
                }).ToList();

            List<splinechhart> expensesummary = selectedtransactions
             .Where(i => i.category.type == "expense")
             .GroupBy(j => j.date)
             .Select(k => new splinechhart()
             {
                 day = k.First().date.ToString("dd-MMM"),
                 expense = k.Sum(l => l.amount)
             }).ToList();

            string[] last7days = Enumerable.Range(0, 7)
                .Select(i => startdate.AddDays(i).ToString("dd-MMM")).ToArray();

            ViewBag.splinechartdata = from day in last7days
                                      join income in incomesummary on day equals income.day into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in expensesummary on day equals expense.day into dayExpenseJoined
                                      from expense in dayExpenseJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          income = income == null ? 0 : income.income,
                                          expense = expense == null ? 0 : expense.expense,

                                      };

            ViewBag.recenttransaction = await _context.transactions.Include(i => i.category).OrderByDescending(j => j.date).Take(5).ToListAsync();

            return View();

        }

        
    }

    public class splinechhart
    {
        public string day;
        public int income;
        public int expense;
    }


}
