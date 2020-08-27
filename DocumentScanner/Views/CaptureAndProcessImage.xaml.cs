using DocumentScanner.Interfaces;
using DocumentScanner.Utilities;
using Plugin.ImageEdit;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DocumentScanner.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CaptureAndProcessImage : ContentPage
    {
        private string _CapturedFile;
        private string _EditFile;
        private bool IsEdited = false;
        private string _ParentFolderName = string.Empty;

        public CaptureAndProcessImage(bool IsCameraMode = true, string ParentFolderName = "")
        {
            InitializeComponent();

            _ParentFolderName = ParentFolderName;
            if (IsCameraMode)
                HandleCamera();
            else
                HandleGallery();

        }

        public async void HandleCamera()
        {
            PermissionStatus status = await PermissionUtilities.GetInstance().RequestCameraPermission();
            if (CrossMedia.Current.IsTakePhotoSupported && status == PermissionStatus.Granted)
            {
                MediaFile capturedImagefile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    AllowCropping = true,
                    Directory = "temp",
                    Name = DateTime.Now.ToFileTime().ToString()
                });

                if (capturedImagefile != null)  //Photo capture success.....
                {
                    _CapturedFile = capturedImagefile.Path;   ///storage/emulated/0/Android/data/com.companyname.healthcare.mobile/files/Pictures/temp/132409169326373120.jpg

                    //-----Copy Captured file for Edit Purpose
                    _EditFile = Path.Combine(Path.GetDirectoryName(_CapturedFile), "currentedit.jpg");
                    File.Copy(_CapturedFile, _EditFile, true);


                    //--Set Image Source for display
                    imgCaptured.Source = ImageSource.FromStream(() =>
                    {
                        return capturedImagefile.GetStream();
                    });
                }
                else
                {
                    GC.Collect();
                    await Navigation.PushAsync(new DocumentLandingPage());  //Photo capture failure- Go to landing page
                }
                    
            }
        }

        public async void HandleGallery()
        {
            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                MediaFile capturedImagefile = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Medium
                });
                if (capturedImagefile != null)
                {
                    await DisplayAlert("File Path-", capturedImagefile.Path, "OK");
                    _CapturedFile = capturedImagefile.Path;
                    //-----Copy Captured file for Edit Purpose
                    _EditFile = Path.Combine(Path.GetDirectoryName(_CapturedFile), "currentedit.jpg");
                    File.Copy(_CapturedFile, _EditFile, true);
                    imgCaptured.Source = ImageSource.FromStream(() =>
                    {
                        return capturedImagefile.GetStream();
                    });
                }
                else
                {
                    await Navigation.PushAsync(new DocumentLandingPage());
                }
            }
        }

        private async Task<byte[]> RotateImageByDegree(Stream imageStream, int degree)
        {
            var image = await CrossImageEdit.Current.CreateImageAsync(imageStream);
            image.Rotate(degree);

            var jpgBytes = image.ToJpeg(100);
            return jpgBytes;
        }

        private async void btnRotateRight_Clicked(object sender, EventArgs e)
        {
            spnrProcessingImage.IsVisible = spnrProcessingImage.IsRunning = true;
            Stream ImageFileStream = File.OpenRead(_EditFile);
            byte[] arrJpeg = await RotateImageByDegree(ImageFileStream, 90);

            imgCaptured.Source = ImageSource.FromStream(() =>   //set display
            {
                return new MemoryStream(arrJpeg);
            });
            //spnrProcessingImage.IsVisible = spnrProcessingImage.IsRunning = false;

            File.WriteAllBytes(_EditFile, arrJpeg);
            IsEdited = true;
        }

        private async void btnRotateLeft_Clicked(object sender, EventArgs e)
        {
            //spnrProcessingImage.IsVisible = spnrProcessingImage.IsRunning = true;
            Stream ImageFileStream = File.OpenRead(_EditFile);
            byte[] arrJpeg = await RotateImageByDegree(ImageFileStream, -90);

            imgCaptured.Source = ImageSource.FromStream(() =>   //set display
            {
                return new MemoryStream(arrJpeg); ;
            });

            //spnrProcessingImage.IsVisible = spnrProcessingImage.IsRunning = false;
            File.WriteAllBytes(_EditFile, arrJpeg);
            IsEdited = true;
        }

        private async void btnCrop_Clicked(object sender, EventArgs e)
        {
            //spnrProcessingImage.IsVisible = spnrProcessingImage.IsRunning = true;
            Stream ImageFileStream = File.OpenRead(_EditFile);
            var image = await CrossImageEdit.Current.CreateImageAsync(ImageFileStream);
            image.Crop(10, 10, 500, 500);

            var jpgBytes = image.ToJpeg(100);

            imgCaptured.Source = ImageSource.FromStream(() =>   //set display
            {
                return new MemoryStream(jpgBytes); ;
            });
            //spnrProcessingImage.IsVisible = spnrProcessingImage.IsRunning = false;
            File.WriteAllBytes(_EditFile, jpgBytes);
            IsEdited = true;
        }

        private async void btnGrayscaleConvert_Clicked(object sender, EventArgs e)
        {
            //spnrProcessingImage.IsVisible = spnrProcessingImage.IsRunning = true;
            Stream ImageFileStream = File.OpenRead(_EditFile);
            var image = await CrossImageEdit.Current.CreateImageAsync(ImageFileStream);
            image.ToMonochrome();

            var jpgBytes = image.ToJpeg(100);

            imgCaptured.Source = ImageSource.FromStream(() =>   //set display
            {
                return new MemoryStream(jpgBytes); ;
            });
            //spnrProcessingImage.IsVisible = spnrProcessingImage.IsRunning = false;
            File.WriteAllBytes(_EditFile, jpgBytes);
            IsEdited = true;
        }

        private async void btnDeleteImage_Clicked(object sender, EventArgs e)
        {
            bool delete_option = await DisplayAlert("Delete Image", "Delete this Image?", "Yes", "No");
            try
            {
                if (delete_option)
                {
                    File.Delete(_CapturedFile);
                    File.Delete(_EditFile);
                    await Navigation.PushAsync(new DocumentLandingPage());
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void btnUndo_Clicked(object sender, EventArgs e)
        {

            bool undo_option = await DisplayAlert("Undo Changes", "Revert to Original?", "Yes", "No");

            if (undo_option && IsEdited)
            {

                Stream ImageFileStream = File.OpenRead(_CapturedFile);
                var image = await CrossImageEdit.Current.CreateImageAsync(ImageFileStream);
                var jpgBytes = image.ToJpeg(100);

                imgCaptured.Source = ImageSource.FromStream(() =>   //set display
                {
                    return new MemoryStream(jpgBytes); ;
                });

                File.WriteAllBytes(_EditFile, jpgBytes);
            }
        }

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            try
            {
                PermissionStatus status = await PermissionUtilities.GetInstance().RequestExtrenalStorageWritePermission();
                if (status == PermissionStatus.Granted)
                {
                    string FolderNameInput = string.Empty;
                    if (string.IsNullOrEmpty(_ParentFolderName))    //Direct camera on from Landing Page
                    {
                        FolderNameInput = await DisplayPromptAsync("Name", "What Document is this?",
                                               initialValue: "SampleDoc", keyboard: Keyboard.Default);
                        if (string.IsNullOrEmpty(FolderNameInput))
                            FolderNameInput = DateTime.Now.ToFileTime().ToString();

                        _ParentFolderName = FolderNameInput;
                    }


                    string PublicExternalFolderPath = StorageUtilities.GetInstance().GetAppExternalStorageFolderName();
                    //1st time case
                    if (!Directory.Exists(PublicExternalFolderPath))
                        Directory.CreateDirectory(PublicExternalFolderPath);

                    string SaveDirectory = Path.Combine(PublicExternalFolderPath, _ParentFolderName);
                    if (!Directory.Exists(SaveDirectory))
                        Directory.CreateDirectory(SaveDirectory);

                    string saveFileName = DateTime.Now.ToFileTime().ToString();
                    string SaveFilePath = Path.Combine(SaveDirectory, saveFileName + ".jpg");
                    File.Copy(_EditFile, SaveFilePath, true);

                    GC.Collect();
                    await Navigation.PushAsync(new DocumentLandingPage());
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }

        }
    }
}