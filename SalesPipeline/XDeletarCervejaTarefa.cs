using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmbevWeb.Models;
using AmbevWeb.Pipeline;

namespace AmbevWeb.RulePipeline
{
    public class XDeletarCervejaTarefa : XSalesTask
    {
        public XDeletarCervejaTarefa(int pCervejaID)
        {
            _CervejaID = pCervejaID;
        }

        private readonly int _CervejaID;

        public override XSalesReturn Execute(AmbevContext pInput, ref bool pStop)
        {
            var c = pInput.Cervejas.Find(_CervejaID);
            if (c == null)
                return new XSalesReturn($"Cerveja com ID=\"{_CervejaID}\" não encontrada.", XSalesResult.Error);
            pInput.Cervejas.Remove(c);
            if (pInput.SaveChanges() == 0)
                return new XSalesReturn($"Não foi possível excluir a cerveja \"{c.Nome}\".", XSalesResult.Error);
            return new XSalesReturn($"Cerveja \"{c.Nome}\" excluída com sucesso.", XSalesResult.Sucess);
        }
    }
}