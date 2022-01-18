using System.Linq;
using System.Threading.Tasks;
using AmbevWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AmbevWeb.Controllers
{
    public class ItemPedidoController : Controller
    {
        private readonly AmbevContext _Context;

        public ItemPedidoController(AmbevContext context)
        {
            this._Context = context;
        }

        public async Task<IActionResult> Index(int? ped)
        {
            if (ped.HasValue)
            {
                if (_Context.Vendas.Any(p => p.IdVenda == ped))
                {
                    var pedido = await _Context.Vendas
                        .Include(p => p.Cliente)
                        .Include(p => p.ItensVenda.OrderBy(i => i.Cerveja.Nome))
                        .ThenInclude(i => i.Cerveja)
                        .FirstOrDefaultAsync(p => p.IdVenda == ped);

                    ViewBag.Pedido = pedido;
                    return View(pedido.ItensVenda);
                }
                TempData["mensagem"] = MensagemModel.Serializar("Venda não encontrada.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }
            TempData["mensagem"] = MensagemModel.Serializar("Venda não informada.", TipoMensagem.Erro);
            return RedirectToAction("Index", "Cliente");
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar(int? ped, int? prod)
        {
            if (ped.HasValue)
            {
                if (_Context.Vendas.Any(p => p.IdVenda == ped))
                {
                    var produtos = _Context.Cervejas
                        .OrderBy(x => x.Nome)
                        .Select(p => new { p.IdCerveja, NomePreco = $"{p.Nome} ({p.Preco:C})" })
                        .AsNoTracking().ToList();
                    var produtosSelectList = new SelectList(produtos, "IdCerveja", "NomePreco");
                    ViewBag.Produtos = produtosSelectList;

                    if (prod.HasValue && ItemPedidoExiste(ped.Value, prod.Value))
                    {
                        var itemPedido = await _Context.ItensPedidos
                            .Include(i => i.Cerveja)
                            .FirstOrDefaultAsync(i => i.IdVenda == ped && i.IdCerveja == prod);
                        return View(itemPedido);
                    }
                    else
                    {
                        return View(new ItemVendaModel()
                        { IdVenda = ped.Value, ValorUnitario = 0, Quantidade = 1 });
                    }
                }
                TempData["mensagem"] = MensagemModel.Serializar("Venda não encontrada.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }
            TempData["mensagem"] = MensagemModel.Serializar("Venda não informada.", TipoMensagem.Erro);
            return RedirectToAction("Index", "Cliente");
        }

        private bool ItemPedidoExiste(int ped, int cerv)
        {
            return _Context.ItensPedidos.Any(x => x.IdVenda == ped && x.IdCerveja == cerv);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromForm] ItemVendaModel itemPedido)
        {
            if (ModelState.IsValid)
            {
                if (itemPedido.IdVenda > 0)
                {
                    var produto = await _Context.Cervejas.FindAsync(itemPedido.IdVenda);
                    itemPedido.ValorUnitario = produto.Preco;
                    if (ItemPedidoExiste(itemPedido.IdVenda, itemPedido.IdVenda))
                    {
                        _Context.ItensPedidos.Update(itemPedido);
                        if (await _Context.SaveChangesAsync() > 0)
                            TempData["mensagem"] = MensagemModel.Serializar("Item de venda alterado com sucesso.");
                        else
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao alterar item de venda.", TipoMensagem.Erro);
                    }
                    else
                    {
                        _Context.ItensPedidos.Add(itemPedido);
                        if (await _Context.SaveChangesAsync() > 0)
                            TempData["mensagem"] = MensagemModel.Serializar("Item de vemda cadastrado com sucesso.");
                        else
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao cadastrar item de venda.", TipoMensagem.Erro);
                    }
                    var pedido = await _Context.Vendas.FindAsync(itemPedido.IdVenda);
                    pedido.ValorTotal = _Context.ItensPedidos
                        .Where(i => i.IdVenda == itemPedido.IdVenda)
                        .Sum(i => i.ValorUnitario * i.Quantidade);
                    await _Context.SaveChangesAsync();
                    return RedirectToAction("Index", new { ped = itemPedido.IdVenda });
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Pedido não informado.", TipoMensagem.Erro);
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
                ViewBag.Produtos = produtosSelectList;

                return View(itemPedido);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Excluir(int? ped, int? prod)
        {
            if (!ped.HasValue || !prod.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Item de pedido não informado.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }

            if (!ItemPedidoExiste(ped.Value, prod.Value))
            {
                TempData["mensagem"] = MensagemModel.Serializar("Item de pedido não encontrado.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }

            var itemPedido = await _Context.ItensPedidos.FindAsync(ped, prod);
            _Context.Entry(itemPedido).Reference(i => i.Cerveja).Load();
            return View(itemPedido);
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(int idPedido, int idProduto)
        {
            var itemPedido = await _Context.ItensPedidos.FindAsync(idPedido, idProduto);
            if (itemPedido != null)
            {
                _Context.ItensPedidos.Remove(itemPedido);
                if (await _Context.SaveChangesAsync() > 0)
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Item de venda excluído com sucesso.");
                    var pedido = await _Context.Vendas.FindAsync(itemPedido.IdVenda);
                    pedido.ValorTotal = _Context.ItensPedidos
                        .Where(i => i.IdVenda == itemPedido.IdVenda)
                        .Sum(i => i.ValorUnitario * i.Quantidade);
                    await _Context.SaveChangesAsync();
                }
                else
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir o item de venda.", TipoMensagem.Erro);
                return RedirectToAction("Index", new { ped = idPedido });
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Item de venda não encontrada.", TipoMensagem.Erro);
                return RedirectToAction("Index", new { ped = idPedido });
            }
        }
    }
}