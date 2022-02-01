using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmbevWeb.Models;
using AmbevWeb.Pipeline;

namespace AmbevWeb.RulePipeline
{
    public class XConfirmationDeleteCustomerTask : XSalesTask
    {
        public XConfirmationDeleteCustomerTask(int? pCustomerID)
        {
            _CustomerID = pCustomerID;
        }

        private readonly int? _CustomerID;

        public override XSalesReturn Execute(AmbevContext pInput, ref bool pStop)
        {
            if (!_CustomerID.HasValue)
                return new XSalesReturn($"Cliente não informado.", XSalesResult.Error);
            var c = pInput.Clientes.Find(_CustomerID);
            if (c == null)
                return new XSalesReturn($"Cliente com ID=\"{_CustomerID}\" não encontrado.", XSalesResult.Error);
            return new XSalesReturn(string.Empty, XSalesResult.Sucess, c);
        }
    }
}