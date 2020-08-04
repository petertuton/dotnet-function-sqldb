using System;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Logging;

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System.Linq;

namespace dotnet_function_sqldb
{
    public class ProcessDeviceEvents
    {
        private readonly TelemetryClient telemetryClient;
        private readonly SQLDbContext sqlDbContext; 

        public ProcessDeviceEvents(SQLDbContext sqlDbContext, TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient;
            this.sqlDbContext = sqlDbContext;
        }

        [FunctionName(nameof(ProcessMessages))]
        public async Task ProcessMessages(
            [EventHubTrigger(@"%EventHubName%", Connection = @"EventHubConnection")] EventData[] deviceEvents,
            ILogger log)
        {
            // Initiate latency metric
            double totalLatency = 0.0;

            // Process the messages
            foreach (EventData deviceEvent in deviceEvents)
            {
                // Perform some logic on the event...

                // Including performing lookups...
                bool found1 = QueryItems("1234", log);
                // bool found2 = await QueryItemsAsync("9012", log);

                // Write the data
                DeviceData deviceData = new DeviceData { Id = Guid.NewGuid().ToString(), Message = Encoding.UTF8.GetString(deviceEvent.Body) };
                // await documents.AddAsync(deviceData);
                await sqlDbContext.deviceData.AddAsync(deviceData);

                // Calculate telemetry
                DateTime nowTimeUTC = DateTime.UtcNow;
                DateTime enqueuedTimeUtc = deviceEvent.SystemProperties.EnqueuedTimeUtc;
                totalLatency += (nowTimeUTC - enqueuedTimeUtc).TotalMilliseconds;
            }

            // Save all changes to the database
            await sqlDbContext.SaveChangesAsync();

            // Report telemetry
            telemetryClient.TrackMetric(new MetricTelemetry("batchSize", deviceEvents.Length));
            telemetryClient.TrackMetric(new MetricTelemetry("batchAverageLatency", (totalLatency / deviceEvents.Length) / 1000.0));
        }

        private bool QueryItems(string id, ILogger log)
        {
            LookupData lookupResult = sqlDbContext.lookupData.Where(lookup => lookup.id == "1234").FirstOrDefault();
            return lookupResult != null;
        }
   }
}