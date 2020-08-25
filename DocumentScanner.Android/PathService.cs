using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DocumentScanner.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(DocumentScanner.Droid.PathService))]
namespace DocumentScanner.Droid
{
    public class PathService : IPathService
    {
        //https://kimsereyblog.blogspot.com/2016/11/differences-between-internal-and.html
        public string InternalFolder
        {
            get
            {
                return Android.App.Application.Context.FilesDir.AbsolutePath;
            }
        }

        public string PublicExternalFolder
        {
            get
            {
                return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            }
        }

        public string PrivateExternalFolder
        {
            get
            {
                return Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath;
            }
        }
    }
}