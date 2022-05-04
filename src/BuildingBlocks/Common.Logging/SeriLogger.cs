namespace Common.Logging
{
    #region USINGS
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Polly;
    using Polly.Extensions.Http;
    using Serilog;
    using Serilog.Sinks.Elasticsearch;
    #endregion

    public static class SeriLogger
    {
        public static Action<HostBuilderContext, LoggerConfiguration> Configure => (context, configuration) =>
        {
            var elasticUri = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");

            configuration
                 .Enrich.FromLogContext()
                 .Enrich.WithMachineName()
                 .WriteTo.Debug()
                 .WriteTo.Console()
                 .WriteTo.Elasticsearch(
                     new ElasticsearchSinkOptions(new Uri(elasticUri))
                     {
                         IndexFormat = $"applogs-{context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-")}-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                         AutoRegisterTemplate = true,
                         NumberOfShards = 2,
                         NumberOfReplicas = 1
                     })
                 .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                 .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                 .ReadFrom.Configuration(context.Configuration);
        };

        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(
                          retryCount: 5,
                          sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                          onRetry: (exception, retryCount, context) =>
                          {
                              Log.Error($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
                          }
                    );
        }

        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(
                            handledEventsAllowedBeforeBreaking: 5,
                            durationOfBreak: TimeSpan.FromSeconds(30)
                    );
        }
    }
}
