using System.Linq;
using System.Threading.Tasks;
using AmbevWeb.Models;
using AmbevWeb.RulePipeline;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AmbevWeb.Utils;
using System.Collections.Generic;

namespace AmbevWeb.Controllers
{
    public class ClienteController : XSalesCrudController<ClienteModel>
    {

        public ClienteController(AmbevContext pContext)
            : base(pContext, "Cliente", pContext.Clientes)
        {
        }

        protected override XSalesTask CreateCadrastarTask(int? id, ClienteModel pCliente)
        {
            return new XCreateOrUpdateCustomerTask(id, pCliente);
        }

        protected override XSalesTask CreateExcluirTask(int id)
        {
            return new XDeleteCustomerTask(id);
        }

        protected override XSalesTask CreateConfirmacaoExcluirTask(int? id)
        {
            return new XConfirmationDeleteCustomerTask(id);
        }

        protected override Task<List<ClienteModel>> GetIndexList()
        {
            return FContext.Clientes.OrderBy(o => o.Nome).AsNoTracking().ToListAsync();
        }
    }
}