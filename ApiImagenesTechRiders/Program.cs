using ApiImagenesTechRiders.Helpers;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Api de Imagenes TechRiders",
        Version = "v1",
        Description = "",
    });
});

string root = builder.Environment.ContentRootPath;
string urlServerHost = builder.Configuration.GetValue<string>("ServerId")!;
HelperFilesManager helperFiles = new HelperFilesManager(root, urlServerHost);
builder.Services.AddTransient<HelperFilesManager>(h => helperFiles);

HelperToken helper = new HelperToken(builder.Configuration);
//AÑADIMOS AUTENTIFICACION A NUESTRO SERVICIO
builder.Services.AddAuthentication(helper.GetAuthOptions())
    .AddJwtBearer(helper.GetJwtOptions());
builder.Services.AddTransient<HelperToken>(x => helper);
builder.Services.AddControllers();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
        url: "/swagger/v1/swagger.json", name: "Api v1");

    options.RoutePrefix = "";
});


if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
