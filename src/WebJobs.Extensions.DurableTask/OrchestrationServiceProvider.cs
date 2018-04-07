using DurableTask.AzureStorage;
using DurableTask.Core;
using DurableTask.Core.Tracking;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.WebJobs.Extensions.DurableTask
{
    /// <summary>
    /// Provides the <see cref="IOrchestrationService"/> to be used
    /// </summary>
    public class OrchestrationServiceProvider
    {
  
        /// <summary>
        /// Gets or sets the name of the Azure Storage connection string used to manage the underlying Azure Storage resources.
        /// </summary>
        /// <remarks>
        /// If not specified, the default behavior is to use the standard `AzureWebJobsStorage` connection string for all storage usage.
        /// </remarks>
        /// <value>The name of a connection string that exists in the app's application settings.</value>
        public string AzureStorageConnectionStringName { get; set; }

        internal IOrchestrationService Create(DurableTaskExtension durableTaskExtension)
        {
           return this.CreateAzureStorageOrchestrationService(durableTaskExtension);
        }

        //private IOrchestrationService CreateServiceBusStorageOrchestrationService(DurableTaskExtension durableTaskExtension,
        //    string connectionNameOverride = null,
        //    string taskHubNameOverride = null)
        //{
        //    string connectionName = connectionNameOverride ?? this.AzureStorageConnectionStringName ?? ConnectionStringNames.Storage;
        //    string resolvedStorageConnectionString = AmbientConnectionStringProvider.Instance.GetConnectionString(connectionName);

        //    if (string.IsNullOrEmpty(resolvedStorageConnectionString))
        //    {
        //        throw new InvalidOperationException("Unable to find an Azure Storage connection string to use for this binding.");
        //    }

        //    var settings = new ServiceBusOrchestrationServiceSettings();
        //    var hubName = taskHubNameOverride ?? durableTaskExtension.HubName;

        //    IOrchestrationServiceInstanceStore instanceStore = null;
        //    IOrchestrationServiceBlobStore blobStore = null;
        //    return new ServiceBusOrchestrationService(resolvedStorageConnectionString, hubName, instanceStore, blobStore, settings);
        //}

        private IOrchestrationService CreateAzureStorageOrchestrationService(DurableTaskExtension durableTaskExtension)
        {
            var settings = this.GetAzureStorageOrchestrationServiceSettings(durableTaskExtension);
            return new AzureStorageOrchestrationService(settings);
        }


        internal AzureStorageOrchestrationServiceSettings GetAzureStorageOrchestrationServiceSettings(DurableTaskExtension durableTaskExtension,
            OrchestrationClientAttribute attribute)
        {
            return this.GetAzureStorageOrchestrationServiceSettings(
                durableTaskExtension,
                connectionNameOverride: attribute.ConnectionName,
                taskHubNameOverride: attribute.TaskHub);
        }

        internal AzureStorageOrchestrationServiceSettings GetAzureStorageOrchestrationServiceSettings(
            DurableTaskExtension durableTaskExtension,
            string connectionNameOverride = null,
            string taskHubNameOverride = null)
        {
            string connectionName = connectionNameOverride ?? this.AzureStorageConnectionStringName ?? ConnectionStringNames.Storage;
            string resolvedStorageConnectionString = AmbientConnectionStringProvider.Instance.GetConnectionString(connectionName);

            if (string.IsNullOrEmpty(resolvedStorageConnectionString))
            {
                throw new InvalidOperationException("Unable to find an Azure Storage connection string to use for this binding.");
            }

            return new AzureStorageOrchestrationServiceSettings
            {
                StorageConnectionString = resolvedStorageConnectionString,
                TaskHubName = taskHubNameOverride ?? durableTaskExtension.HubName,
                PartitionCount = durableTaskExtension.PartitionCount,
                ControlQueueVisibilityTimeout = durableTaskExtension.ControlQueueVisibilityTimeout,
                WorkItemQueueVisibilityTimeout = durableTaskExtension.WorkItemQueueVisibilityTimeout,
                MaxConcurrentTaskOrchestrationWorkItems = durableTaskExtension.MaxConcurrentTaskOrchestrationWorkItems,
                MaxConcurrentTaskActivityWorkItems = durableTaskExtension.MaxConcurrentTaskActivityWorkItems,
            };
        }

        internal IOrchestrationServiceClient CreateClient(DurableTaskExtension durableTaskExtension, OrchestrationClientAttribute attr)
        {
            var settings = this.GetAzureStorageOrchestrationServiceSettings(durableTaskExtension, attr);
            var client = new AzureStorageOrchestrationService(settings);
            return client;
        }
    }
}
