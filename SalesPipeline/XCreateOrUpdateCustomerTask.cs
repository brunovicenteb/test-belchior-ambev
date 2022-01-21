using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmbevWeb.Models;
using AmbevWeb.Pipeline;

namespace AmbevWeb.RulePipeline
{
    public class XCreateOrUpdateCustomerTask : XSalesTask
    {
        public XCreateOrUpdateCustomerTask(int? pCustomerID, ClienteModel pCustomer)
        {
            _CustomerID = pCustomerID;
            _Customer = pCustomer;
        }

        private readonly int? _CustomerID;
        private readonly ClienteModel _Customer;

        public override XSalesReturn Execute(AmbevContext pInput, ref bool pStop)
        {
            if (!_CustomerID.HasValue)
            {
                pInput.Clientes.Add(_Customer);
                if (pInput.SaveChanges() > 0)
                    return new XSalesReturn($"Cliente \"{_Customer.Nome}\" incluído com sucesso.", XSalesResult.Sucess);
                else
                    return new XSalesReturn($"Erro ao incluir cliente \"{_Customer.Nome}\".", XSalesResult.Error);
            }
            if (!pInput.Clientes.Any(o => o.IdUsuario == _CustomerID.Value))
                return new XSalesReturn($"Cliente com ID=\"{_CustomerID.Value}\" não encontrado.", XSalesResult.Error);
            pInput.Clientes.Update(_Customer);
            pInput.Entry(_Customer).Property(o => o.Senha).IsModified = false;
            if (pInput.SaveChanges() == 0)
                return new XSalesReturn($"Erro ao alterar cliente \"{_Customer.Nome}\".", XSalesResult.Error);
            return new XSalesReturn($"Cliente \"{_Customer.Nome}\" alterado com sucesso.", XSalesResult.Sucess);
        }
    }
}