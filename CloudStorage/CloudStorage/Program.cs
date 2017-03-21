using System;
using Microsoft.Azure; // Namespace for Azure Configuration Manager
using Microsoft.WindowsAzure.Storage; // Namespace for Storage Client Library
using Microsoft.WindowsAzure.Storage.File; // Namespace for File storage
using System.Runtime.InteropServices;
using System.IO;

namespace CloudStorage
{
    class Program
    {
        [DllImport(@"E:\rtgcode\bin\BackupSystem.dll", EntryPoint = "GetCredit")]
        public static extern UInt64 GetCredit(string sPath);

        [DllImport(@"E:\rtgcode\bin\BackupSystem.dll", EntryPoint = "GetTotalBet")]
        public static extern UInt64 GetTotalBet(string sPath);

        [DllImport(@"E:\rtgcode\bin\BackupSystem.dll", EntryPoint = "GetTotalWon")]
        public static extern UInt64 GetTotalWon(string sPath);

        [DllImport(@"E:\rtgcode\bin\BackupSystem.dll", EntryPoint = "GetGamePlayed")]
        public static extern UInt32 GetGamePlayed(string sPath);

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
                CloudFileDirectory sampleDir = rootDir.GetDirectoryReference("accounts");

                // Ensure that the directory exists.
                if (sampleDir.Exists())
                {
                    // Get a reference to the file we created previously.
                    CloudFile file = sampleDir.GetFileReference("d_accs_data_2way_0.bak");

                    // Ensure that the file exists.
                    if (file.Exists())
                    {
                        // Write the contents of the file to the console window.
                        long n = file.Properties.Length;
                        byte[] data = new byte[n];
                        Console.WriteLine("Start downloading " + n + " bytes...");
                        file.BeginDownloadToByteArray(data, 0, new AsyncCallback(HandleAccsDownloadCallBack), data);
                    }
                }

                // Get a reference to the directory we created previously.
                sampleDir = rootDir.GetDirectoryReference("stats");

                // Ensure that the directory exists.
                if (sampleDir.Exists())
                {
                    // Get a reference to the file we created previously.
                    CloudFile file = sampleDir.GetFileReference("d_mach_stats_2way_0.bak");

                    // Ensure that the file exists.
                    if (file.Exists())
                    {
                        // Write the contents of the file to the console window.
                        long n = file.Properties.Length;
                        byte[] data = new byte[n];
                        Console.WriteLine("Start downloading " + n + " bytes...");
                        file.BeginDownloadToByteArray(data, 0, new AsyncCallback(HandleStatsDownloadCallBack), data);
                    }
                }
            }
        }

        private static void HandleAccsDownloadCallBack(IAsyncResult ar)
        {
            byte[] data = (byte[])ar.AsyncState;
            string sDownloadFolder = @"C:\\Users\\David\\Source\\Repos\\CloudStorageAPI\\CloudStorage\\CloudStorage\\bin\\Download\\";
            File.WriteAllBytes(sDownloadFolder + "d_accs_data_2way_0.bak", data);

            UInt64 nCredit = GetCredit(sDownloadFolder);
            Console.WriteLine("Credit=" + nCredit);
        }

        private static void HandleStatsDownloadCallBack(IAsyncResult ar)
        {
            byte[] data = (byte[])ar.AsyncState;
            string sDownloadFolder = @"C:\\Users\\David\\Source\\Repos\\CloudStorageAPI\\CloudStorage\\CloudStorage\\bin\\Download\\";
            File.WriteAllBytes(sDownloadFolder + "d_mach_stats_2way_0.bak", data);

            UInt64 nBet = GetTotalBet(sDownloadFolder);
            UInt64 nWon = GetTotalBet(sDownloadFolder);
            UInt32 nGamePlayed = GetGamePlayed(sDownloadFolder);
            Console.WriteLine("GamePlayed=" + nGamePlayed + ", Bet=" + nBet + ", Won=" + nWon);
        }
    }
 }
