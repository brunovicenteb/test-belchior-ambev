using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmbevWeb.Models;
using AmbevWeb.Pipeline;

namespace AmbevWeb.RulePipeline
{
    public class XCustomerTask : XSalesTask
    {
        public XCustomerTask(int? pClienteID, ClienteModel pCliente)
        {
            _ClienteID = pClienteID;
            _Cliente = pCliente;
        }

        private readonly int? _ClienteID;
        private readonly ClienteModel _Cliente;

        private bool ClienteExiste(AmbevContext pContext, int pClienteID)
        {
            return pContext.Clientes.Any(o => o.IdUsuario == _ClienteID.Value);
        }

        public override XSalesReturn Execute(AmbevContext pInput, ref bool pStop)
        {
            if (!_ClienteID.HasValue)
            {
                pInput.Clientes.Add(_Cliente);
                if (pInput.SaveChanges() > 0)
                    return new XSalesReturn($"Cliente \"{_Cliente.Nome}\" incluído com sucesso.", XSalesResult.Sucess);
                else
                    return new XSalesReturn($"Erro ao incluir cliente \"{_Cliente.Nome}\".", XSalesResult.Error);
            }
            if (!ClienteExiste(pInput, _ClienteID.Value))
                return new XSalesReturn($"Cliente com ID=\"{_ClienteID.Value}\" não encontrado.", XSalesResult.Error);
            pInput.Clientes.Update(_Cliente);
            pInput.Entry(_Cliente).Property(o => o.Senha).IsModified = false;
            if (pInput.SaveChanges() > 0)
                return new XSalesReturn($"Cliente \"{_Cliente.Nome}\" alterado com sucesso.", XSalesResult.Sucess);
            else
                return new XSalesReturn($"Erro ao alterar cliente \"{_Cliente.Nome}\".", XSalesResult.Error);
        }
    }
}