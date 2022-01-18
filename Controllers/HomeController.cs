using System.Linq;
using System.Threading.Tasks;
using AmbevWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmbevWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly AmbevContext _context;

        public HomeController(AmbevContext context)
        {
            this._context = context;
        }

        public async Task<IActionResult> Index()
        {
            var pedidos = await _context.Vendas
                .Where(p => !p.DataVenda.HasValue)
                .Include(p => p.Cliente)
                .OrderByDescending(p => p.IdVenda)
                .AsNoTracking().ToListAsync();

            return View(pedidos);
        }
    }
}