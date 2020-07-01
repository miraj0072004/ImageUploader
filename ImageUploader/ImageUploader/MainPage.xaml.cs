using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace ImageUploader
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Error","This isn't supported in your device","Ok");
                return;
            }

            var mediaOptions = new PickMediaOptions()
            {
                PhotoSize = PhotoSize.Medium
            };

            var selectedImageFile = await CrossMedia.Current.PickPhotoAsync(mediaOptions);

            if (selectedImageFile == null)
            {
                await DisplayAlert("Error", "There was an error when trying to access your image", "Ok");
                return;
            }

            SelectedImage.Source = ImageSource.FromStream(() => selectedImageFile.GetStream());

            UploadImage(selectedImageFile.GetStream());
        }

        private async void UploadImage(Stream stream)
        {
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=imagestoragemiraj;AccountKey=E2PaXcbuG9KyHwPR7ASVDczFeOeBxLyU0N/mcoXUhKzMzY9ugJkK24tr7Em8tNYef301tW4JMh+tGPJG51I3iQ==;EndpointSuffix=core.windows.net");
            // Create the container and return a container client object
            
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("imagecontainermiraj");
            //to avoid authentication issues  go into your storage account > IAM > Add role and add the special permission for this type of request, STORAGE BLOB DATA CONTRIBUTOR. Then copied the new access keys over to here, and retried. Worked !!

            var name = Guid.NewGuid().ToString();
            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient($"{name}.jpg");
            await blobClient.UploadAsync(stream, true);

            var url = blobClient.Uri.OriginalString;
        }
    }
}
