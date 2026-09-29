var builder = WebApplication.CreateBuilder(args);

// Habilita o uso de Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Liga as rotas definidas nos Controllers
app.MapControllers();

app.Run();
