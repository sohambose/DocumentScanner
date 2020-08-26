using DocumentScanner.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace DocumentScanner.Utilities
{
    public class StorageUtilities
    {
        private string ExternalRootFolderName = "ScannerAppFiles";
        private static StorageUtilities Instance = new StorageUtilities();

        /// <summary>
        /// Protected constructor for subclassing.
        /// </summary>
        protected StorageUtilities()
        {
        }

        /// <summary>
        /// Get the singleton instance.
        /// </summary>
        /// <returns>Single instance of CapitalFeeBO facade loaded in memory</returns>
        public static StorageUtilities GetInstance()
        {
            return Instance;
        }
        public string GetAppExternalStorageFolderName()
        {
            var ServicePlatform = DependencyService.Get<IPathService>();
            string PublicExternalFolderPath = ServicePlatform.PublicExternalFolder;
            string SavedFolderPath = Path.Combine(PublicExternalFolderPath, ExternalRootFolderName);
            return SavedFolderPath;
        }
    }
}
