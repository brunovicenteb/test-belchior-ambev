using AmbevWeb.Models;
using AmbevWeb.RulePipeline;
using Microsoft.AspNetCore.Mvc;
using System;

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

        protected IActionResult ProcessPipeline(string pRedirect, params XSalesTask[] pTasks)
        {
            return ProcessPipeline(pRedirect, pRedirect, pTasks);
        }

        protected IActionResult ProcessPipeline(string pSucessRedirect, string pErrorRedirect, params XSalesTask[] pTasks)
        {
            Func<XSalesReturn, IActionResult> sucess = (o) => RedirectToAction(pSucessRedirect);
            Func<XSalesReturn, IActionResult> error = (o) => RedirectToAction(pErrorRedirect);
            return ProcessPipeline(sucess, error, pTasks);
        }

        protected IActionResult ProcessPipeline(Func<XSalesReturn, IActionResult> pSucessAction, Func<XSalesReturn, IActionResult> pErrorAction, params XSalesTask[] pTasks)
        {
            XSalesReturn sr = ProcessPipeline(pTasks);
            switch (sr.Result)
            {
                case XSalesResult.Sucess:
                    RegistraSucesso(sr.Message);
                    return pSucessAction(sr);
                default:
                    RegistraFalha(sr.Message);
                    return pErrorAction(sr);
            }
        }
    }
}