using ClienteAPI.Data;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ClienteAPI.Services;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor.
builder.Services.AddDbContext<BdClientesContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("ClienteDB")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de Health Checks para verificar la salud de la aplicación
var connectionString = builder.Configuration.GetConnectionString("ClienteDB"); // Obtener la cadena de conexión

if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddHealthChecks() // Agregar Health Checks
        .AddSqlServer(
            connectionString: connectionString,
            name: "ClienteDB_HealthCheck", // Nombre del Health Check
            failureStatus: HealthStatus.Unhealthy);
}
else
{
    throw new InvalidOperationException("La cadena de conexión 'ClienteDB' no está configurada correctamente.");
}


var app = builder.Build();
// Configuración del middleware para la recolección de métricas con Prometheus
app.UseMetricServer(); // Activar el servidor de métricas para Prometheus
app.UseHttpMetrics(); // Métricas HTTP para Prometheus
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();

// Creación de una métrica para el estado de salud de la aplicación
var healthCheckMetric = Metrics.CreateGauge("healthcheck_status", "Indicates the health status of the application. 1 for healthy, 0 for unhealthy.");


// Configuración del endpoint para los Health Checks
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

// Simular la recolección de datos climáticos con datos Random
app.Lifetime.ApplicationStarted.Register(() =>
{
    var random = new Random();
    Task.Run(async () =>
    {
        while (true)
        {
            // Simular datos climáticos
            var temperatura = random.NextDouble() * (35 - 20) + 20; // Temperatura entre 20 y 35 grados
            MetricsService.ReportarDatosClimaticos(temperatura);
            await Task.Delay(5000); // Esperar 5 segundos
        }
    });
});


app.MapControllers(); // Mapea los controladores a la aplicación

app.Run(); // Ejecuta la aplicación y comienza a escuchar las solicitudes
