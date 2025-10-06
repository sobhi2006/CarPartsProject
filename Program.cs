using System.Text;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseCors("any");
app.UseRouting();



app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.UseResponseCompression();

app.UseStatusCodePages();

app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger(options =>
    {
        options.RouteTemplate = "openapi/{documentName}.json";
    });
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "CarParts API V1");
        options.RoutePrefix = "swagger";
        options.EnableDeepLinking();
        options.DisplayRequestDuration();
        options.EnableFilter();
    });
}

app.MapControllers();

//app.MapGet("/debug/api-descriptions", (IApiDescriptionGroupCollectionProvider provider) =>
//{
//    var sb = new StringBuilder();
//    sb.AppendLine("API Descriptions Discovered:");
//    sb.AppendLine("============================");

//    foreach (var group in provider.ApiDescriptionGroups.Items)
//    {
//        foreach (var description in group.Items)
//        {
//            sb.AppendLine($"Action: {description.ActionDescriptor.DisplayName}");
//            sb.AppendLine($"Route: {description.RelativePath}");
//            sb.AppendLine($"Method: {description.HttpMethod}");
//            sb.AppendLine("---");
//        }
//    }

//    return Results.Text(sb.ToString(), "text/plain");
//});

app.Run();
