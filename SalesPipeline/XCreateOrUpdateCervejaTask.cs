using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmbevWeb.Models;
using AmbevWeb.Pipeline;

namespace AmbevWeb.RulePipeline
{
    public class XCreateOrUpdateCervejaTask : XSalesTask
    {
        public XCreateOrUpdateCervejaTask(int? pCustomerID, CervejaModel pCerveja)
        {
            _CervejaID = pCustomerID;
            _Cerveja = pCerveja;
        }

        private readonly int? _CervejaID;
        private readonly CervejaModel _Cerveja;

        public override XSalesReturn Execute(AmbevContext pInput, ref bool pStop)
        {
            if (!_CervejaID.HasValue)
            {
                pInput.Cervejas.Add(_Cerveja);
                if (pInput.SaveChanges() > 0)
                    return new XSalesReturn($"Cerveja \"{_Cerveja.Nome}\" incluída com sucesso.", XSalesResult.Sucess);
                else
                    return new XSalesReturn($"Erro ao incluir cerveja \"{_Cerveja.Nome}\".", XSalesResult.Error);
            }
            if (!pInput.Clientes.Any(o => o.IdUsuario == _CervejaID.Value))
                return new XSalesReturn($"Cerveja com ID=\"{_CervejaID.Value}\" não encontrada.", XSalesResult.Error);
            pInput.Cervejas.Update(_Cerveja);
            if (pInput.SaveChanges() == 0)
                return new XSalesReturn($"Erro ao alterar cerveja \"{_Cerveja.Nome}\".", XSalesResult.Error);
            return new XSalesReturn($"Cerveja \"{_Cerveja.Nome}\" alterada com sucesso.", XSalesResult.Sucess);
        }
    }
}