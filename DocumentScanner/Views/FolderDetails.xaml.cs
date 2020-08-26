using DocumentScanner.Interfaces;
using DocumentScanner.Models;
using DocumentScanner.Utilities;
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
    public partial class FolderDetails : ContentPage
    {
        private string _parentFolderPath = string.Empty;
        public FolderDetails(string ParentFolderPath = "")
        {
            InitializeComponent();
            _parentFolderPath = ParentFolderPath;
            BindFileList();
        }

        private async void btnOpenCamera_Clicked(object sender, EventArgs e)
        {
            bool IsCameraMode = true;
            await Navigation.PushAsync(new CaptureAndProcessImage(IsCameraMode, _parentFolderPath));
        }

        private async void btnOpenGallery_Clicked(object sender, EventArgs e)
        {
            bool IsCameraMode = false;
            await Navigation.PushAsync(new CaptureAndProcessImage(IsCameraMode, _parentFolderPath));
        }

        private async void BindFileList()
        {
            List<DocumentFile> lstDocFiles = new List<DocumentFile>();

            string SavedFolderPath = _parentFolderPath;
            PermissionStatus status = await PermissionUtilities.GetInstance().RequestExtrenalStorageReadPermission();

            if (Directory.Exists(SavedFolderPath) && status == PermissionStatus.Granted)
            {
                string[] filesList = Directory.GetFiles(SavedFolderPath);
                int slNo = 1;
                foreach (string file in filesList)
                {
                    DocumentFile docFile = new DocumentFile();

                    docFile.FullFilePath = file;
                    docFile.FileName = Path.GetFileName(file);
                    docFile.FileType = Path.GetExtension(file);
                    docFile.SlNo = slNo;

                    lstDocFiles.Add(docFile);
                    slNo++;
                }

                lstDocuments.ItemsSource = lstDocFiles;
            }
        }

        private void lstDocuments_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            DocumentFile selectedFile = e.SelectedItem as DocumentFile;
        }

    }
}