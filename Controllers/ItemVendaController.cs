using System.Linq;
using System.Threading.Tasks;
using AmbevWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AmbevWeb.Controllers
{
    public class ItemVendaController : Controller
    {
        private readonly AmbevContext _Context;

        public ItemVendaController(AmbevContext context)
        {
            this._Context = context;
        }

        public async Task<IActionResult> Index(int? vend)
        {
            if (vend.HasValue)
            {
                if (_Context.Vendas.Any(p => p.IdVenda == vend))
                {
                    var venda = await _Context.Vendas
                        .Include(p => p.Cliente)
                        .Include(p => p.ItensVenda.OrderBy(i => i.Cerveja.Nome))
                        .ThenInclude(i => i.Cerveja)
                        .FirstOrDefaultAsync(p => p.IdVenda == vend);

                    ViewBag.Pedido = venda;
                    return View(venda.ItensVenda);
                }
                TempData["mensagem"] = MensagemModel.Serializar("Venda não encontrada.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }
            TempData["mensagem"] = MensagemModel.Serializar("Venda não informada.", TipoMensagem.Erro);
            return RedirectToAction("Index", "Cliente");
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar(int? vend, int? prod)
        {
            if (vend.HasValue)
            {
                if (_Context.Vendas.Any(p => p.IdVenda == vend))
                {
                    var produtos = _Context.Cervejas
                        .OrderBy(x => x.Nome)
                        .Select(p => new { p.IdCerveja, NomePreco = $"{p.Nome} ({p.Preco:C})" })
                        .AsNoTracking().ToList();
                    var produtosSelectList = new SelectList(produtos, "IdCerveja", "NomePreco");
                    ViewBag.Cervejas = produtosSelectList;

                    if (prod.HasValue && ItemVendaExiste(vend.Value, prod.Value))
                    {
                        var itemVenda = await _Context.ItensVendas
                            .Include(i => i.Cerveja)
                            .FirstOrDefaultAsync(i => i.IdVenda == vend && i.IdCerveja == prod);
                        return View(itemVenda);
                    }
                    else
                    {
                        return View(new ItemVendaModel() { IdVenda = vend.Value, ValorUnitario = 0, Quantidade = 1 });
                    }
                }
                TempData["mensagem"] = MensagemModel.Serializar("Venda não encontrada.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }
            TempData["mensagem"] = MensagemModel.Serializar("Venda não informada.", TipoMensagem.Erro);
            return RedirectToAction("Index", "Cliente");
        }

        private bool ItemVendaExiste(int vend, int cerv)
        {
            return _Context.ItensVendas.Any(x => x.IdVenda == vend && x.IdCerveja == cerv);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromForm] ItemVendaModel itemVenda)
        {
            if (ModelState.IsValid)
            {
                if (itemVenda.IdVenda > 0)
                {
                    var produto = await _Context.Cervejas.FindAsync(itemVenda.IdVenda);
                    itemVenda.ValorUnitario = produto.Preco;
                    if (ItemVendaExiste(itemVenda.IdVenda, itemVenda.IdVenda))
                    {
                        _Context.ItensVendas.Update(itemVenda);
                        if (await _Context.SaveChangesAsync() > 0)
                            TempData["mensagem"] = MensagemModel.Serializar("Item de venda alterado com sucesso.");
                        else
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao alterar item de venda.", TipoMensagem.Erro);
                    }
                    else
                    {
                        _Context.ItensVendas.Add(itemVenda);
                        if (await _Context.SaveChangesAsync() > 0)
                            TempData["mensagem"] = MensagemModel.Serializar("Item de vemda cadastrado com sucesso.");
                        else
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao cadastrar item de venda.", TipoMensagem.Erro);
                    }
                    var pedido = await _Context.Vendas.FindAsync(itemVenda.IdVenda);
                    pedido.ValorTotal = _Context.ItensVendas
                        .Where(i => i.IdVenda == itemVenda.IdVenda)
                        .Sum(i => i.ValorUnitario * i.Quantidade);
                    await _Context.SaveChangesAsync();
                    return RedirectToAction("Index", new { ped = itemVenda.IdVenda });
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Venda não informada.", TipoMensagem.Erro);
                    return RedirectToAction("Index", "Cliente");
                }
            }
            else
            {
                var produtos = _Context.Cervejas
                        .OrderBy(x => x.Nome)
                        .Select(p => new { p.IdCerveja, NomePreco = $"{p.Nome} ({p.Preco:C})" })
                        .AsNoTracking().ToList();
                var produtosSelectList = new SelectList(produtos, "IdCerveja", "NomePreco");
                ViewBag.Cervejas = produtosSelectList;

                return View(itemVenda);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Excluir(int? vend, int? prod)
        {
            if (!vend.HasValue || !prod.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Item de venda não informado.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }

            if (!ItemVendaExiste(vend.Value, prod.Value))
            {
                TempData["mensagem"] = MensagemModel.Serializar("Item de venda não encontrado.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }

            var itemVenda = await _Context.ItensVendas.FindAsync(vend, prod);
            _Context.Entry(itemVenda).Reference(i => i.Cerveja).Load();
            return View(itemVenda);
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(int idVenda, int idProduto)
        {
            var itemPedido = await _Context.ItensVendas.FindAsync(idVenda, idProduto);
            if (itemPedido != null)
            {
                _Context.ItensVendas.Remove(itemPedido);
                if (await _Context.SaveChangesAsync() > 0)
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Item de venda excluído com sucesso.");
                    var pedido = await _Context.Vendas.FindAsync(itemPedido.IdVenda);
                    pedido.ValorTotal = _Context.ItensVendas
                        .Where(i => i.IdVenda == itemPedido.IdVenda)
                        .Sum(i => i.ValorUnitario * i.Quantidade);
                    await _Context.SaveChangesAsync();
                }
                else
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir o item de venda.", TipoMensagem.Erro);
                return RedirectToAction("Index", new { vend = idVenda });
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Item de venda não encontrada.", TipoMensagem.Erro);
                return RedirectToAction("Index", new { vend = idVenda });
            }
        }
    }
}