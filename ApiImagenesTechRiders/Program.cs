using ApiImagenesTechRiders.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
