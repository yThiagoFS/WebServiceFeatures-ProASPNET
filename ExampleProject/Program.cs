using ExampleProject.Contexts;
using ExampleProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("ProductConnection"));
    opts.EnableSensitiveDataLogging(true);
});

builder.Services.AddControllers();

builder.Services.Configure<JsonOptions>(opts => 
    opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

var app = builder.Build();

app.MapControllers();

#region Using individual endpoints
/*
 
const string BASEURL = "api/products";

app.MapGet($"{BASEURL}/{{id}}", async (HttpContext context, DataContext data) =>
{
    string? id = context.Request.RouteValues["id"] as string;

    if(id != null)
    {
        Product? p = data.Products.Find(long.Parse(id));

        if(p == null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        } else
        {
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(p));
        }
    }
});

app.MapGet(BASEURL, async (HttpContext context, DataContext data) =>
{
    context.Response.ContentType = "application/json";

    await context.Response.WriteAsync(JsonSerializer.Serialize(data.Products.ToList()));
});

app.MapPost(BASEURL, async (HttpContext context, DataContext data) =>
{
    Product? p = await
        JsonSerializer.DeserializeAsync<Product>(context.Request.Body);

    if(p != null)
    {
        await data.AddAsync(p);

        await data.SaveChangesAsync();

        context.Response.StatusCode = StatusCodes.Status200OK;
    }

});

 */
#endregion

var context = app.Services.CreateScope()
    .ServiceProvider.GetRequiredService<DataContext>();

SeedData.SeedDatabase(context);

app.Run();
