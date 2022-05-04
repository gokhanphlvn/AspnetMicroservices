using Common.Logging;
using Serilog;
using Shopping.Aggregator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog(SeriLogger.Configure);

builder.Services.AddTransient<LoggingDelegatingHandler>();

builder.Services.AddHttpClient<ICatalogService, CatalogService>(c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:CatalogUrl"]))
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler(SeriLogger.GetRetryPolicy())
    .AddPolicyHandler(SeriLogger.GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IBasketService, BasketService>(c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BasketUrl"]))
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    //.AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)))
    //.AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)))
    .AddPolicyHandler(SeriLogger.GetRetryPolicy())
    .AddPolicyHandler(SeriLogger.GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IOrderService, OrderService>(c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:OrderingUrl"]))
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler(SeriLogger.GetRetryPolicy())
    .AddPolicyHandler(SeriLogger.GetCircuitBreakerPolicy());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
