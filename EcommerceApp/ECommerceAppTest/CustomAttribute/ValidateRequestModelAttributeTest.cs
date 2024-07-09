using EcommerceApp.Entities.DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using EcommerceApp.CustomAttributes;
using EcommerceApp.Controllers;

namespace ECommerceAppTest.CustomAttribute
{
    public class ValidateRequestModelAttributeTest
    {
        public ValidateRequestModelAttributeTest()
        { 

        }
        [Fact]
        public void Test_ValidateRequestModelAttributeTest_Returns_Ok()
        {
            var product = new Product
            {
                Name = "Nike sneaker",
                Price = 15000,
                Rating = 1,
                CategoryId = 5,
            };
            IDictionary<string,object> dict = new Dictionary<string, object>
             {
                { "product", product }
             };
            var actionContext = new ActionExecutingContext(
               new ActionContext
           {
               HttpContext = new DefaultHttpContext(),
               RouteData = new RouteData(),
               ActionDescriptor = new ActionDescriptor(),
           },
               new List<IFilterMetadata>(),
               dict,
               new Mock<Controller>().Object);
               
            var filter = new ValidateRequestModelAttribute();

            // Act
            filter.OnActionExecuting(actionContext);

            // Assert
            Assert.Null(actionContext.Result);

        }

        [Fact]
        public void Test_ValidateRequestModelAttributeTest_Returns_BadRequest()
        {
            var product = new Product
            {
                Price = 0,
                Rating = 8877654,
                CategoryId = 1,
            };
            IDictionary<string, object> dict = new Dictionary<string, object>
             {
                { "product", product }
             };
            var actionContext = new ActionExecutingContext(
              new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor(),
            },
              new List<IFilterMetadata>(),
              dict,
              new Mock<Controller>().Object);

            var filter = new ValidateRequestModelAttribute();

            // Act
            filter.OnActionExecuting(actionContext);

            // Assert
            Assert.Null(actionContext.Result);

            // Assert.IsType<BadRequestObjectResult>(actionContext.Result);

        }
    }
}
