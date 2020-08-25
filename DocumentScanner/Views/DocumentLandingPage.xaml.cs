using DocumentScanner.Interfaces;
using DocumentScanner.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DocumentScanner.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentLandingPage : ContentPage
    {
        public DocumentLandingPage()
        {
            InitializeComponent();
            BindFileList();
        }
        private async void BindFileList()
        {
            List<DocumentFile> lstDocFiles = new List<DocumentFile>();


            var ServicePlatform = DependencyService.Get<IPathService>();
            string PublicExternalFolderPath = ServicePlatform.PublicExternalFolder;
            string SavedFolderPath = Path.Combine(PublicExternalFolderPath, "ScannerAppFiles");

            PermissionStatus status = await CheckAndRequestLocationPermission();
            if (Directory.Exists(SavedFolderPath) && status == PermissionStatus.Granted)
            {
                string[] filesList = Directory.GetFiles(SavedFolderPath);

                foreach (string file in filesList)
                {
                    DocumentFile docFile = new DocumentFile();

                    docFile.FullFilePath = file;
                    docFile.FileName = Path.GetFileName(file);
                    docFile.FileType = Path.GetExtension(file);

                    lstDocFiles.Add(docFile);
                }

                lstDocuments.ItemsSource = lstDocFiles;
            }
        }

        public async Task<PermissionStatus> CheckAndRequestLocationPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageRead>();
            }
            // Additionally could prompt the user to turn on in settings
            return status;
        }

        private async void btnOpenCamera_Clicked(object sender, EventArgs e)
        {
            bool IsCameraMode = true;
            await Navigation.PushAsync(new CaptureAndProcessImage(IsCameraMode));
        }

        private async void btnOpenGallery_Clicked(object sender, EventArgs e)
        {
            bool IsCameraMode = false;
            await Navigation.PushAsync(new CaptureAndProcessImage(IsCameraMode));
        }

        private void lstDocuments_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            DocumentFile selectedDoc = e.SelectedItem as DocumentFile;
        }
    }
}