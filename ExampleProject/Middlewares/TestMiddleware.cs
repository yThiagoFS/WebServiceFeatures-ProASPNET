using ExampleProject.Contexts;

namespace ExampleProject.Middlewares
{
    public class TestMiddleware
    {
        private RequestDelegate? _next;

        public TestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, DataContext dbContext)
        {
            if (context.Request.Path == "/teste")
            {
                await context.Response.WriteAsync($"Products count: {dbContext.Products.Count()}\n"
                    + $"Categories count: {dbContext.Categories.Count()}\n"
                    + $"Suppliers count: {dbContext.Suppliers.Count()}");
            }
            else
               await _next!(context);
        }
    }
}
