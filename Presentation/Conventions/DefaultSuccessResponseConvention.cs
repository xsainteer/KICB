using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Presentation.Conventions;

public sealed class DefaultSuccessResponseConvention : IActionModelConvention
{
    public void Apply(ActionModel action)
    {
        var declaresSuccess = action.Attributes
            .Concat(action.Controller.Attributes)
            .OfType<ProducesResponseTypeAttribute>()
            .Any(attribute => attribute.StatusCode is >= 200 and < 300);

        if (!declaresSuccess)
            action.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status200OK));
    }
}
