using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmbevWeb.Models;
using AmbevWeb.Pipeline;

namespace AmbevWeb.RulePipeline
{
    public class XDeleteCustomerTask : XSalesTask
    {
        public XDeleteCustomerTask(int pCustomerID)
        {
            _CustomerID = pCustomerID;
        }

        private readonly int _CustomerID;

        public override XSalesReturn Execute(AmbevContext pInput, ref bool pStop)
        {
            var c = pInput.Clientes.Find(_CustomerID);
            if (c == null)
                return new XSalesReturn($"Cliente com ID=\"{_CustomerID}\" não encontrado.", XSalesResult.Error);
            pInput.Clientes.Remove(c);
            if (pInput.SaveChanges() == 0)
                return new XSalesReturn($"Não foi possível excluir o cliente \"{c.Nome}\".", XSalesResult.Error);
            return new XSalesReturn($"Cliente \"{c.Nome}\" excluído com sucesso.", XSalesResult.Sucess);
        }
    }
}