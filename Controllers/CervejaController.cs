using System.Linq;
using System.Threading.Tasks;
using AmbevWeb.Models;
using AmbevWeb.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AmbevWeb.Controllers
{
    public class CervejaController : XSalesController
    {
        public CervejaController(AmbevContext pContext)
            : base(pContext)
        {
        }

        public async Task<IActionResult> Index()
        {
            var beers = await FContext.Cervejas.OrderBy(o => o.Nome).AsNoTracking().ToListAsync();
            return View(beers);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar(int? id)
        {
            if (id.HasValue)
                return View(new CervejaModel());
            var p = await FContext.Cervejas.FindAsync(id);
            if (p != null)
                return View(p);
            RegistraFalha("Cerveja não encontrada.");
            return RedirectToAction(nameof(Index));
        }

        private bool CervejaExiste(int pIdCerveja)
        {
            return FContext.Cervejas.Any(o => o.IdCerveja == pIdCerveja);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(int? id, [FromForm] CervejaModel cerveja)
        {
            if (ModelState.IsValid)
            {
                if (id.HasValue)
                {
                    if (CervejaExiste(id.Value))
                    {
                        FContext.Cervejas.Update(cerveja);
                        if (await FContext.SaveChangesAsync() > 0)
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Cerveja alterada com sucesso.");
                        }
                        else
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao alterar cerveja.", TipoMensagem.Erro);
                        }
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Cerveja não encontrada.", TipoMensagem.Erro);
                    }
                }
                else
                {
                    FContext.Cervejas.Add(cerveja);
                    if (await FContext.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Cerveja cadastrada com sucesso.");
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Erro ao cadastrar cerveja.", TipoMensagem.Erro);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(cerveja);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Excluir(int? id)
        {
            if (!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Cerveja não informada.", TipoMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            var produto = await FContext.Cervejas.FindAsync(id);
            if (produto == null)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Cerveja não informada.", TipoMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            return View(produto);
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            var produto = await FContext.Cervejas.FindAsync(id);
            if (produto != null)
            {
                FContext.Cervejas.Remove(produto);
                if (await FContext.SaveChangesAsync() > 0)
                    TempData["mensagem"] = MensagemModel.Serializar("Cerveja excluída com sucesso.");
                else
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir a cerveja.", TipoMensagem.Erro);
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Cerveja não encontrada.", TipoMensagem.Erro);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}