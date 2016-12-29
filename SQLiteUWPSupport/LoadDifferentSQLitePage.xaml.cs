using SQLite.Net;
using SQLiteUWPSupport.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SQLiteUWPSupport
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoadDifferentSQLitePage : Page
    {
        public LoadDifferentSQLitePage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var folder = await StorageDataFolder();
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".dha");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            await file.CopyAsync(folder, file.Name, NameCollisionOption.ReplaceExisting);
            TryChange(file.Name);
        }
        public async static Task<StorageFolder> StorageDataFolder()
        {
            var folder1 = Windows.Storage.ApplicationData.Current.LocalFolder;
            var subfolder = await folder1.CreateFolderAsync("Data", CreationCollisionOption.OpenIfExists);
            return subfolder;
        }
        public async static Task<SQLiteConnection> DBConnection(string dbname)
        {
            var folder = await StorageDataFolder();
            string path = Path.Combine(folder.Path, dbname);

           var conn = new SQLite.Net.SQLiteConnection(new
            SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            return conn;
        }
        public async void TryChange(string dbName)
        {
            image1.Source = null;
            using (var db = await DBConnection(dbName))
            {
                var t = db.CreateTable<PictureModel>();
                var c = db.GetMapping<PictureModel>();
                var image = (from d in db.Table<PictureModel>()
                             where d.Id == 1
                             select d).FirstOrDefault();
                //StorageFile file = await image.Picture.AsStorageFileAsync("te.jpg");

                //var strem = await file.OpenAsync(FileAccessMode.Read);
                BitmapImage bmp = new BitmapImage(new Uri("ms-appx://SQLiteUWPSupport/Assets/Pictures.png"));
                //bmp.SetSource(strem);

                image1.Source = bmp;
                //the imagesource does not change...
            }
        }
    }
}
