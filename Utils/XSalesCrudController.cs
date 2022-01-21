﻿using System.Linq;
using System.Threading.Tasks;
using AmbevWeb.Models;
using AmbevWeb.RulePipeline;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AmbevWeb.Utils;

namespace AmbevWeb.Controllers
{
    public abstract class XSalesCrudController<T> : XSalesController where T : class, new()
    {

        public XSalesCrudController(AmbevContext pContext, string pTitle, DbSet<T> pDbList)
            : base(pContext)
        {
            _Title = pTitle;
            _DbList = pDbList;
        }

        private readonly string _Title;
        private readonly DbSet<T> _DbList;

        protected abstract XSalesTask CreateCadrastarTask(int? id, T pCliente);
        protected abstract XSalesTask CreateExcluirTask(int id);

        public async Task<IActionResult> Index()
        {
            var clientes = await FContext.Clientes.OrderBy(x => x.Nome).AsNoTracking().ToListAsync();
            return View(clientes);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar(int? id)
        {
            if (!id.HasValue)
                return View(new T());
            var t = await _DbList.FindAsync(id) as T;
            if (t != null)
                return View(t);
            RegistraFalha($"{_Title} com ID=\"{id.Value}\" não encontrado.");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Cadastrar(int? id, [FromForm] T pEntity)
        {
            if (!ModelState.IsValid)
                return View(pEntity);
            XSalesTask t = CreateCadrastarTask(id, pEntity);
            return ProcessPipeline(nameof(Index), t);
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
            XSalesTask t = CreateExcluirTask(id);
            return ProcessPipeline(nameof(Index), t);
        }
    }
}