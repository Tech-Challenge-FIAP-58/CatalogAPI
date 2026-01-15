//using FIAP.FGC.USER.WebApi.Middlewares;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using FCG.Application.Producers;
using FCG.Application.Services;
using FCG.Infra.Context;
using FCG.Infra.Mapping;
using FCG.Infra.Repository;
using FCG.WebApi.Settings;



//using FIAP.FGC.USER.Infra.Mapping;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAutoMapper(
    cfg => { },
    AppDomain.CurrentDomain.GetAssemblies()
);

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<CatalogProfile>();
});



builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Core"));
    options.UseLazyLoadingProxies();
}, ServiceLifetime.Scoped);

builder.InitilizeRetrySettings();
builder.AddMassTransitSettings();

// repositório de banco de dados
builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// serviço de apoio para as iterações com o banco
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// producer da fila OrderPlacedEvent
builder.Services.AddScoped<IRabbitMQServiceProducer, RabbitMQServiceProducer>();
builder.Services.AddScoped<IOrderPlacedEventProducer, OrderPlacedEventProducer>();

// consumidor da fila PaymentProcessedEvent
builder.Services.AddHostedService<PaymentProcessedConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
