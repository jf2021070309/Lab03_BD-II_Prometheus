using ClienteAPI.Data;
using Microsoft.EntityFrameworkCore;
using Prometheus;
// Referencias extras
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using System.Text;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor.
builder.Services.AddDbContext<BdClientesContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("ClienteDB")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Agregar servicios de Health Checks
var connectionString = builder.Configuration.GetConnectionString("ClienteDB");

if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddHealthChecks()
        .AddSqlServer(
            connectionString: connectionString, // Validado para evitar referencia nula
            name: "ClienteDB_HealthCheck", // Nombre del Health Check
            failureStatus: HealthStatus.Unhealthy);
}
else
{
    throw new InvalidOperationException("La cadena de conexión 'ClienteDB' no está configurada correctamente.");
}


var app = builder.Build();

// Middleware para Prometheus
app.UseMetricServer(); // Activar el servidor de métricas para Prometheus
app.UseHttpMetrics(); // Métricas HTTP para Prometheus
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();

// Contador para la métrica de Health Check
var healthCheckMetric = Metrics.CreateGauge("healthcheck_status", "Indicates the health status of the application. 1 for healthy, 0 for unhealthy.");


// Endpoint de Health Checks
app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        // Aquí se actualiza la métrica de salud
        healthCheckMetric.Set(report.Status == HealthStatus.Healthy ? 1 : 0);
        Console.WriteLine($"Health Check Status Updated: {report.Status}"); // Log para verificar la actualización

        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new {
                name = e.Key,
                status = e.Value.Status.ToString(),
                exception = e.Value.Exception?.Message ?? "none",
                duration = e.Value.Duration.ToString()
            })
        });

        await context.Response.WriteAsync(result);
    }
});



app.MapControllers();

app.Run();
