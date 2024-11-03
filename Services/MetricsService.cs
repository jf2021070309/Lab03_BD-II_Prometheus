using Prometheus;

namespace ClienteAPI.Services
{
    public static class MetricsService
    {
        private static readonly Gauge TemperaturaMedia = Metrics.CreateGauge("datos_climaticos_temperatura_media", "Temperatura promedio registrada");
        private static readonly Gauge TemperaturaMaxima = Metrics.CreateGauge("datos_climaticos_temperatura_maxima", "Temperatura máxima registrada");
        private static readonly Gauge TemperaturaMinima = Metrics.CreateGauge("datos_climaticos_temperatura_minima", "Temperatura mínima registrada");
        private static readonly Counter TotalLecturas = Metrics.CreateCounter("datos_climaticos_total_lecturas", "Total de lecturas de temperatura");

        public static void ReportarDatosClimaticos(double temperatura)
        {
            TemperaturaMedia.Set(temperatura);
            TemperaturaMaxima.Set(Math.Max(temperatura, TemperaturaMaxima.Value));
            TemperaturaMinima.Set(Math.Min(temperatura, TemperaturaMinima.Value));
            TotalLecturas.Inc();
        }
    }
}
