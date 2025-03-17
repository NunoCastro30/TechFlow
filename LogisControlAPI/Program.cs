using Microsoft.EntityFrameworkCore;
using LogisControlAPI.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//Configurar a conexão ao SQL Server
builder.Services.AddDbContext<LogisControlContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Ativar controladores para API REST
builder.Services.AddControllers();


//Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

//Ativar Swagger
app.UseSwagger();
app.UseSwaggerUI();

//Ativar autorização
app.UseAuthorization();

//Mapear controladores da API
app.MapControllers();
app.Run();


