using DocumentScanner.Interfaces;
using Plugin.ImageEdit;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public CaptureAndProcessImage(bool IsCameraMode = true)
        {
            InitializeComponent();

            if (IsCameraMode)
                HandleCamera();
            else
                HandleGallery();
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
                string NameInput = await DisplayPromptAsync("Name", "How would You like to identify this document?",
                    initialValue: DateTime.Now.ToFileTime().ToString(), keyboard: Keyboard.Default);

                if (string.IsNullOrEmpty(NameInput))
                    NameInput = DateTime.Now.ToFileTime().ToString();

                var ServicePlatform = DependencyService.Get<IPathService>();
                string PublicExternalFolderPath = ServicePlatform.PublicExternalFolder;

                string SaveFolder = Path.Combine(PublicExternalFolderPath, "ScannerAppFiles");
                if (!Directory.Exists(SaveFolder))
                    Directory.CreateDirectory(SaveFolder);

                string SaveFile = Path.Combine(SaveFolder, NameInput + ".jpg");
                File.Copy(_EditFile, SaveFile, true);

                await Navigation.PushAsync(new DocumentLandingPage());

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }

        }

        public async void HandleCamera()
        {
            if (CrossMedia.Current.IsTakePhotoSupported)
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
                    await Navigation.PushAsync(new DocumentLandingPage());  //Photo capture failure- Go to landing page
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


    }
}