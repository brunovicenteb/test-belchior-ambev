using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmbevWeb.Models;
using AmbevWeb.Pipeline;

namespace AmbevWeb.RulePipeline
{
    public abstract class XSalesTask : XTask<AmbevContext, XSalesReturn>
    {
    }
}