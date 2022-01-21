using System.Linq;
using System.Threading.Tasks;
using AmbevWeb.Models;
using AmbevWeb.RulePipeline;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AmbevWeb.Utils;

namespace AmbevWeb.Controllers
{
    public class ClienteController : XSalesController
    {

        public ClienteController(AmbevContext pContext)
            : base(pContext)
        {
        }

        public async Task<IActionResult> Index()
        {
            var clientes = await FContext.Clientes.OrderBy(x => x.Nome).AsNoTracking().ToListAsync();
            return View(clientes);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar(int? id)
        {
            if (!id.HasValue)
                return View(new ClienteModel());
            var c = await FContext.Clientes.FindAsync(id);
            if (c != null)
                return View(c);
            RegistraFalha($"Cliente com ID=\"{id.Value}\" não encontrado.");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Cadastrar(int? id, [FromForm] ClienteModel pCliente)
        {
            if (!ModelState.IsValid)
                return View(pCliente);
            return ProcessPipeline(nameof(Index), new XCreateOrUpdateCustomerTask(id, pCliente));
        }

        [HttpGet]
        public async Task<IActionResult> Excluir(int? id)
        {
            if (!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Cliente não informado.", TipoMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            var cliente = await FContext.Clientes.FindAsync(id);
            if (cliente == null)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Cliente não encontrado.", TipoMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        [HttpPost]
        public IActionResult Excluir(int id)
        {
            return ProcessPipeline(nameof(Index), new XDeleteCustomerTask(id));
        }
    }
}