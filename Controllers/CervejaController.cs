using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmbevWeb.Models;
using AmbevWeb.RulePipeline;
using AmbevWeb.Utils;
using Microsoft.EntityFrameworkCore;

namespace AmbevWeb.Controllers
{
    public class CervejaController : XSalesCrudController<CervejaModel>
    {
        public CervejaController(AmbevContext pContext)
            : base(pContext, "Cerveja", pContext.Cervejas)
        {
        }

        protected override XSalesTask CreateCadrastarTask(int? id, CervejaModel pCerveja)
        {
            return new XCreateOrUpdateCervejaTask(id, pCerveja);
        }

        protected override XSalesTask CreateExcluirTask(int id)
        {
            return new XDeleteCustomerTask(id);
        }

        protected override XSalesTask CreateConfirmacaoExcluirTask(int? id)
        {
            return new XConfirmationDeleteCustomerTask(id);
        }

        protected override Task<List<CervejaModel>> GetIndexList()
        {
            return FContext.Cervejas.OrderBy(o => o.Nome).AsNoTracking().ToListAsync();
        }
    }
}