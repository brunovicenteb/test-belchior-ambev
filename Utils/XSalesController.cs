using AmbevWeb.Models;
using AmbevWeb.RulePipeline;
using Microsoft.AspNetCore.Mvc;

namespace AmbevWeb.Utils
{
    public abstract class XSalesController : XUiController
    {
        protected readonly AmbevContext FContext;

        public XSalesController(AmbevContext pContext)
        {
            FContext = pContext;
        }

        protected XSalesReturn ProcessPipeline(params XSalesTask[] pTasks)
        {
            XSalesPipeline p = new XSalesPipeline();
            foreach (var t in pTasks)
                p.Register(t);
            return p.Process(FContext);
        }

        protected IActionResult ProcessPipeline(string pDefaultAction, params XSalesTask[] pTasks)
        {
            XSalesReturn sr = ProcessPipeline(pTasks);
            switch (sr.Result)
            {
                case XSalesResult.Sucess:
                    RegistraSucesso(sr.Message);
                    break;
                default:
                    RegistraFalha(sr.Message);
                    break;
            }
            return RedirectToAction(pDefaultAction);
        }
    }
}