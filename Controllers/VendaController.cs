using System;
using System.Linq;
using System.Threading.Tasks;
using AmbevWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmbevWeb.Controllers
{
    public class VendaController : Controller
    {
        private readonly AmbevContext _context;

        public VendaController(AmbevContext context)
        {
            this._context = context;
        }

        public async Task<IActionResult> Index(int? cid)
        {
            if (cid.HasValue)
            {
                var cliente = await _context.Clientes.FindAsync(cid);
                if (cliente != null)
                {
                    var vendas = await _context.Vendas
                        .Where(p => p.IdCliente == cid)
                        .OrderByDescending(x => x.IdVenda)
                        .AsNoTracking().ToListAsync();

                    ViewBag.Cliente = cliente;
                    return View(vendas);
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Cliente não encontrado",
                        TipoMensagem.Erro);
                    return RedirectToAction("Index", "Cliente");
                }
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Cliente não informado",
                    TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar(int? cid)
        {
            if (cid.HasValue)
            {
                var cliente = await _context.Clientes.FindAsync(cid);
                if (cliente != null)
                {
                    _context.Entry(cliente).Collection(c => c.Vendas).Load();
                    VendaModel pedido = null;
                    if (_context.Vendas.Any(p => p.IdCliente == cid && !p.DataVenda.HasValue))
                    {
                        pedido = await _context.Vendas
                            .FirstOrDefaultAsync(p => p.IdCliente == cid && !p.DataVenda.HasValue);
                    }
                    else
                    {
                        pedido = new VendaModel { IdCliente = cid.Value, ValorTotal = 0, CashBack = 0 };
                        cliente.Vendas.Add(pedido);
                        await _context.SaveChangesAsync();
                    }
                    return RedirectToAction("Index", "ItemVenda", new { vend = pedido.IdVenda });
                }
                TempData["mensagem"] = MensagemModel.Serializar("Cliente não encontrado", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }
            TempData["mensagem"] = MensagemModel.Serializar("Cliente não informado", TipoMensagem.Erro);
            return RedirectToAction("Index", "Cliente");
        }

        private bool VendaExiste(int id)
        {
            return _context.Vendas.Any(x => x.IdVenda == id);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(int? id, [FromForm] VendaModel pedido)
        {
            if (ModelState.IsValid)
            {
                if (id.HasValue)
                {
                    if (VendaExiste(id.Value))
                    {
                        _context.Vendas.Update(pedido);
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Venda alterada com sucesso.");
                        }
                        else
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao alterar venda.", TipoMensagem.Erro);
                        }
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Pedido não encontrada.", TipoMensagem.Erro);
                    }
                }
                else
                {
                    _context.Vendas.Add(pedido);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Venda incluída com sucesso.");
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Erro ao cadastrar venda.", TipoMensagem.Erro);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(pedido);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Excluir(int? id)
        {
            if (!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Venda não informada.", TipoMensagem.Erro);
                return RedirectToAction("Index");
            }

            if (!VendaExiste(id.Value))
            {
                TempData["mensagem"] = MensagemModel.Serializar("Venda não encontrada.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }

            var venda = await _context.Vendas
                .Include(p => p.Cliente)
                .Include(p => p.ItensVenda)
                .ThenInclude(i => i.Cerveja)
                .FirstOrDefaultAsync(p => p.IdVenda == id);

            return View(venda);
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            VendaModel vendaModel = await _context.Vendas.FindAsync(id);
            var venda = vendaModel;
            if (venda != null)
            {
                _context.Vendas.Remove(venda);
                if (await _context.SaveChangesAsync() > 0)
                    TempData["mensagem"] = MensagemModel.Serializar("Venda excluída com sucesso.");
                else
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir a venda.", TipoMensagem.Erro);
                return RedirectToAction(nameof(Index), new { cid = venda.IdCliente });
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Venda não encontrada.", TipoMensagem.Erro);
                return RedirectToAction(nameof(Index), "Cliente");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Fechar(int? id)
        {
            if (!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Venda não informada.", TipoMensagem.Erro);
                return RedirectToAction("Index");
            }

            if (!VendaExiste(id.Value))
            {
                TempData["mensagem"] = MensagemModel.Serializar("Venda não encontrada.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }

            var venda = await _context.Vendas
                .Include(p => p.Cliente)
                .Include(p => p.ItensVenda)
                .ThenInclude(i => i.Cerveja)
                .FirstOrDefaultAsync(p => p.IdVenda == id);

            return View(venda);
        }

        [HttpPost]
        public async Task<IActionResult> Fechar(int id)
        {
            if (VendaExiste(id))
            {
                var venda = await _context.Vendas
                    .Include(p => p.Cliente)
                    .Include(p => p.ItensVenda)
                    .ThenInclude(i => i.Cerveja)
                    .FirstOrDefaultAsync(p => p.IdVenda == id);

                if (venda.ItensVenda.Count() > 0)
                {
                    venda.DataVenda = DateTime.Now;
                    foreach (var item in venda.ItensVenda)
                        item.Cerveja.Estoque -= item.Quantidade;
                    if (await _context.SaveChangesAsync() > 0)
                        TempData["mensagem"] = MensagemModel.Serializar("Venda fechada com sucesso.");
                    else
                        TempData["mensagem"] = MensagemModel.Serializar("Não foi possível fechar a venda.", TipoMensagem.Erro);
                    return RedirectToAction("Index", new { cid = venda.IdCliente });
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Não é possível fechar uma venda sem cerveja.", TipoMensagem.Erro);
                    return RedirectToAction("Index", new { cid = venda.IdCliente });
                }
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Venda não encontrada.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Entregar(int? id)
        {
            if (!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Venda não informada.", TipoMensagem.Erro);
                return RedirectToAction("Index");
            }

            if (!VendaExiste(id.Value))
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não encontrado.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }

            var pedido = await _context.Vendas
                .Include(p => p.Cliente)
                .Include(p => p.ItensVenda)
                .ThenInclude(i => i.Cerveja)
                .FirstOrDefaultAsync(p => p.IdVenda == id);

            return View(pedido);
        }

        [HttpPost]
        public async Task<IActionResult> Entregar(int idVenda)
        {
            if (!VendaExiste(idVenda))
            {
                TempData["mensagem"] = MensagemModel.Serializar("Venda não encontrado.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }
            var venda = await _context.Vendas.FindAsync(idVenda);
            venda.DataEntrega = DateTime.Now;
            if (await _context.SaveChangesAsync() > 0)
                TempData["mensagem"] = MensagemModel.Serializar("Entrega de venda registrada com sucesso.");
            else
                TempData["mensagem"] = MensagemModel.Serializar("Não foi possível registrar a entrega da venda.", TipoMensagem.Erro);
            return RedirectToAction("Index", new { cid = venda.IdCliente });
        }
    }
}