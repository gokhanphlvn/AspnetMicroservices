using AspnetRunBasics.Services;
using Common.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseSerilog(Common.Logging.SeriLogger.Configure);

builder.Services.AddRazorPages();
builder.Services.AddTransient<LoggingDelegatingHandler>();
builder.Services.AddHttpClient<ICatalogService, CatalogService>(c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]))
                .AddHttpMessageHandler<LoggingDelegatingHandler>()
                .AddPolicyHandler(SeriLogger.GetRetryPolicy())
                .AddPolicyHandler(SeriLogger.GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IBasketService, BasketService>(c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]))
                .AddHttpMessageHandler<LoggingDelegatingHandler>()
                .AddPolicyHandler(SeriLogger.GetRetryPolicy())
                .AddPolicyHandler(SeriLogger.GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IOrderService, OrderService>(c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]))
                .AddHttpMessageHandler<LoggingDelegatingHandler>()
                .AddPolicyHandler(SeriLogger.GetRetryPolicy())
                .AddPolicyHandler(SeriLogger.GetCircuitBreakerPolicy());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
