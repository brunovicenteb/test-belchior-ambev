using System;
using System.Linq;
using AmbevWeb.Models;
using AmbevWeb.Pipeline;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AmbevWeb.RulePipeline
{
    public class XSalesPipeline : XPipeline<AmbevContext, XSalesReturn>
    {
    }
}