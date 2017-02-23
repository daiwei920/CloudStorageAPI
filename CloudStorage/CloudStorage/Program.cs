using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure; // Namespace for Azure Configuration Manager
using Microsoft.WindowsAzure.Storage; // Namespace for Storage Client Library
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage
using Microsoft.WindowsAzure.Storage.File; // Namespace for File storage

namespace CloudStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create a CloudFileClient object for credentialed access to File storage.
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

            // Get a reference to the file share we created previously.
            CloudFileShare share = fileClient.GetShareReference("backupsystem");

            // Ensure that the share exists.
            if (share.Exists())
            {
                /*
                // Check current usage stats for the share.
                // Note that the ShareStats object is part of the protocol layer for the File service.
                Microsoft.WindowsAzure.Storage.File.Protocol.ShareStats stats = share.GetStats();
                Console.WriteLine("Current share usage: {0} GB", stats.Usage.ToString());

                // Specify the maximum size of the share, in GB.
                // This line sets the quota to be 10 GB greater than the current usage of the share.
                share.Properties.Quota = 10 + stats.Usage;
                share.SetProperties();

                // Now check the quota for the share. Call FetchAttributes() to populate the share's properties.
                share.FetchAttributes();
                Console.WriteLine("Current share quota: {0} GB", share.Properties.Quota);
                */


                
                // Get a reference to the root directory for the share.
                CloudFileDirectory rootDir = share.GetRootDirectoryReference();

                // Get a reference to the directory we created previously.
                CloudFileDirectory sampleDir = rootDir.GetDirectoryReference("TestDirectory");

                // Ensure that the directory exists.
                if (sampleDir.Exists())
                {
                    // Get a reference to the file we created previously.
                    CloudFile file = sampleDir.GetFileReference("LocalAu.txt");

                    // Ensure that the file exists.
                    if (file.Exists())
                    {
                        // Write the contents of the file to the console window.
                        Console.WriteLine(file.DownloadTextAsync().Result);
                    }
                }
            }
        }
    }
}
