using DocumentScanner.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DocumentScanner.Utilities;

namespace DocumentScanner.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentLandingPage : ContentPage
    {
        public DocumentLandingPage()
        {
            InitializeComponent();
            BindFolderList();
            //BindFileList();

        }
        private async void BindFolderList()
        {
            List<DocumentFolder> lstDocFolders = new List<DocumentFolder>();
            string RootFolderExternalStorage = StorageUtilities.GetInstance().GetAppExternalStorageFolderName();
            PermissionStatus readPermission = await PermissionUtilities.GetInstance().RequestExtrenalStorageReadPermission();

            if (Directory.Exists(RootFolderExternalStorage) && readPermission == PermissionStatus.Granted)
            {
                string[] folderNamesList = Directory.GetDirectories(RootFolderExternalStorage);
                foreach (string folderName in folderNamesList)
                {
                    DocumentFolder docFolder = new DocumentFolder();
                    DirectoryInfo dir = new DirectoryInfo(folderName);
                    docFolder.FolderName = dir.Name;
                    docFolder.FolderPath = dir.FullName;
                    lstDocFolders.Add(docFolder);
                }
                lstFolders.ItemsSource = lstDocFolders;
            }
        }

        private async void lstFolders_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            DocumentFolder selectedFolder = e.SelectedItem as DocumentFolder;
            await Navigation.PushAsync(new FolderDetails(selectedFolder.FolderPath));
        }

        private async void btnOpenCamera_Clicked(object sender, EventArgs e)
        {
            bool IsCameraMode = true;
            await Navigation.PushAsync(new CaptureAndProcessImage(IsCameraMode, string.Empty));
        }

        private async void btnOpenGallery_Clicked(object sender, EventArgs e)
        {
            bool IsCameraMode = false;
            await Navigation.PushAsync(new CaptureAndProcessImage(IsCameraMode, string.Empty));
        }


    }
}