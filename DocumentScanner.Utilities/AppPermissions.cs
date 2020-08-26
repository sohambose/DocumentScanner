using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace DocumentScanner.Utilities
{
    public class AppPermissions
    {
        private static AppPermissions Instance = new AppPermissions();

        protected AppPermissions()
        {
        }
        /// <summary>
        /// Get the singleton instance.
        /// </summary>
        /// <returns>Single instance of CapitalFeeBO facade loaded in memory</returns>
        public static AppPermissions GetInstance()
        {
            return Instance;
        }

        #region <------METHODS------>       
        public async Task<PermissionStatus> CheckExternalStorageReadPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            return status;
        }

        public async Task<PermissionStatus> RequestExtrenalStorageReadPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageRead>();
            }
            return status;
        }

        public async Task<PermissionStatus> CheckExternalStorageWritePermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            return status;
        }

        public async Task<PermissionStatus> RequestExtrenalStorageWritePermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            }
            return status;
        }

        public async Task<PermissionStatus> CheckCameraPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            return status;
        }

        public async Task<PermissionStatus> RequestCameraPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }
            return status;
        }

    }
    #endregion

}
}
