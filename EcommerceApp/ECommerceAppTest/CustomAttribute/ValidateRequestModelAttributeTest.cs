using EcommerceApp.CustomAttributes;
using EcommerceApp.Entities.DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace ECommerceAppTest.CustomAttribute
{
    public class ValidateRequestModelAttributeTest
    {
        [Fact]
        public void Test_ValidateRequestModelAttributeTest_Returns_BadRequest()
        {
           var product = new Product
            {
                Price = 15000,
                Rating = 1,
                CategoryId = 5,
            };

           var actContext = new ActionContext(
           Mock.Of<HttpContext>(),
           Mock.Of<RouteData>(),
           Mock.Of<ActionDescriptor>(),
           Mock.Of<ModelStateDictionary>()
           );
           actContext.ModelState.AddModelError("Name", "The name field is required.");

            var actionContext = new ActionExecutingContext(
               actContext,
               new List<IFilterMetadata>(),
               new Dictionary<string, object>(),
               new Mock<Controller>().Object
               );
               
            var filter = new ValidateRequestModelAttribute();
            // Act
            filter.OnActionExecuting(actionContext);

            // Assert
            Assert.IsType<BadRequestObjectResult>(actionContext.Result);
            Assert.NotNull(actionContext.Result);

            var badRequestResult = actionContext.Result as BadRequestObjectResult;
            Assert.IsType<SerializableError>(badRequestResult?.Value);

            var errorDictionary = badRequestResult.Value as SerializableError;
            Assert.True(errorDictionary?.ContainsKey("Name"));
            var errorMessages = errorDictionary?["Name"] as string[];
            Assert.Contains("The name field is required.", errorMessages);
        }

        [Fact]
        public void Test_ValidateRequestModelAttributeTest_Returns_OK()
        {
            var product = new Product
            {
                Name ="Adidas",
                Price = 15000,
                Rating = 1,
                CategoryId = 5,
            };
            var actContext = new ActionContext(
             Mock.Of<HttpContext>(),
             Mock.Of<RouteData>(),
             Mock.Of<ActionDescriptor>(),
             Mock.Of<ModelStateDictionary>()
             );

            //No modelstate error added.

            var actionContext = new ActionExecutingContext(
               actContext,
               new List<IFilterMetadata>(),
               new Dictionary<string, object>(),
               new Mock<Controller>().Object
               );

            var filter = new ValidateRequestModelAttribute();
            // Act
            filter.OnActionExecuting(actionContext);

            // Assert
            Assert.Null(actionContext.Result);
        }
    }
}
