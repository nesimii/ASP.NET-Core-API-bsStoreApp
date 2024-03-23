using Entities.LogModel;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.ActionFilters;

public class LogFilterAttribute : ActionFilterAttribute
{
    private readonly ILoggerService _loggerService;

    public LogFilterAttribute(ILoggerService loggerService)
    {
        _loggerService = loggerService;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        _loggerService.LogInfo(Log("OnActionExecuting", context.RouteData));
    }

    private string Log(string modelName, RouteData routeData)
    {
        var logDetails = new LogDetails()
        {
            ModelName = modelName,
            Controller = routeData.Values["controller"],
            Action = routeData.Values["action"]
        };
        if (routeData.Values.Count > 2) logDetails.Id = routeData.Values["id"];
        return logDetails.ToString();
    }
}
