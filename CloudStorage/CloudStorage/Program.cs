﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure; // Namespace for Azure Configuration Manager
using Microsoft.WindowsAzure.Storage; // Namespace for Storage Client Library
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage
using Microsoft.WindowsAzure.Storage.File; // Namespace for File storage
using System.Runtime.InteropServices;

namespace CloudStorage
{
    class Program
    {
        [DllImport(@"E:\rtgcode\bin\BackupSystem.dll", EntryPoint = "GetCredit")]
        public static extern UInt64 GetCredit();

        //[DllImport("user32.dll", EntryPoint = "MessageBox", CharSet = Unicode)]
        //public static extern Int32 Test(Int32 hWnd, String* lpText, String* lpCaption,  UInt32 uType);

        public struct FILE_HEADER
        {
            public Int32 m_MagicNumId;          //!< Used to quickly identify a file as being a backup system file.
            public Int32 m_BufferSize;            //!< Because windows pads files, we need to store how big the original buffer was that got written.
            public Int32 m_VersionNumber;       //!< 
            public Int32 m_ChunkTableSize;        //!< 
        };
        


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
                      //  Test(0, S"Hello world!", S"Greetings", 0);

                        UInt64 nCredit = GetCredit();
                        Console.WriteLine("Credit=" + nCredit);

                        // Write the contents of the file to the console window.
                        long n = file.Properties.Length;
                        byte[] data = new byte[n];

                        Console.WriteLine("Start downloading " + n + " bytes...");

                        file.BeginDownloadToByteArray(data, 0, new AsyncCallback(HandleDownloadCallBack), data);

                        //Console.WriteLine(data[0]);// file.DownloadTextAsync().Result
                    }
                }

                
               int num = Convert.ToInt32(Console.ReadLine());
            }
        }

        private static void HandleDownloadCallBack(IAsyncResult ar)
        {
            byte[] data = (byte[])ar.AsyncState;

            FILE_HEADER header;
            header.m_MagicNumId = BitConverter.ToInt32(data, 0);
            header.m_BufferSize = BitConverter.ToInt32(data, 4);
            header.m_VersionNumber = BitConverter.ToInt32(data, 8);
            header.m_ChunkTableSize = BitConverter.ToInt32(data, 12);

            int ActualDataSize = 2984;
            int CRC_CHUNK_SIZE = 512;
            int ChunkCount = 6; // static_cast < unsigned int> ( ::ceil(static_cast<double>(ActualDataSize) / static_cast<double>(CRC_CHUNK_SIZE)));
            int SizeOfCRCChunk = 6;// 6, 8???
            int reserve = ChunkCount * SizeOfCRCChunk;
            //const uint32_t WriteSize = sizeof(FILE_HEADER) + (ChunkCount * sizeof(CRC_CHUNK)) + ActualDataSize;


            for (Int32 i = 12 + reserve; i < data.Length; i=i+4)
            {
                UInt32 n = BitConverter.ToUInt32(data, i);
                Console.Write(n + "    ");
            }
            // some code that notify users by sending email or other service
        }
    }
}
