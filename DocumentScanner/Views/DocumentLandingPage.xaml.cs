using DocumentScanner.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DocumentScanner.Utilities;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DocumentScanner.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentLandingPage : ContentPage
    {
        private ObservableCollection<DocumentFolder> _lstFolders;
        public DocumentLandingPage()
        {
            InitializeComponent();
            lstFolders.SelectedItem = null;
            SearchSavedFolders(string.Empty);
        }
        private async void SearchSavedFolders(string searchString = "")
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
                //--For Search:
                lstDocFolders = lstDocFolders.FindAll(d => d.FolderName.ToLower().Contains(searchString.ToLower()));

                _lstFolders = new ObservableCollection<DocumentFolder>(lstDocFolders);
                lstFolders.ItemsSource = _lstFolders;
                lstFolders.SelectedItem = null;
            }
        }

        private async void lstFolders_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            DocumentFolder selectedFolder = e.SelectedItem as DocumentFolder;
            GC.Collect();
            if (selectedFolder != null)
            {
                await Navigation.PushAsync(new FolderDetails(selectedFolder.FolderPath));
                lstFolders.SelectedItem = null;
            }
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

        private void srchFolders_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue;
            SearchSavedFolders(searchText);
        }

        private void lstFolders_Refreshing(object sender, EventArgs e)
        {
            srchFolders.Text = string.Empty;
            SearchSavedFolders(string.Empty);
            lstFolders.EndRefresh();
        }

        private async void cntxtbtnDelete_Clicked(object sender, EventArgs e)
        {
            bool delete_option = await DisplayAlert("Delete Folder", "Delete all scans under this? This will permanently delete the files", "Yes", "No");
            try
            {
                if (delete_option)
                {
                    var menuItemObj = sender as MenuItem;
                    DocumentFolder folderObjselected = menuItemObj.CommandParameter as DocumentFolder;
                    //---Code to actually delete the folder------
                    Directory.Delete(folderObjselected.FolderPath, true);
                    //-------------------------------------------
                    _lstFolders.Remove(folderObjselected);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}