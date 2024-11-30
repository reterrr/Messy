using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Messy.Helpers;

public class RoutePrefixConvention(string prefix) : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            foreach (var selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel != null)
                {
                    selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                        new AttributeRouteModel(new Microsoft.AspNetCore.Mvc.RouteAttribute(prefix)),
                        selector.AttributeRouteModel
                    );
                }
            }
        }
    }
}