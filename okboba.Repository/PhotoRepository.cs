using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.IO;
using ImageProcessor;
using System.Drawing;
using System.Drawing.Imaging;
using okboba.Entities;
using System.Data.Entity;

namespace okboba.Repository
{
    public class PhotoRepository
    {
        const int THUMB_WIDTH = 200;
        const int MAX_PHOTOS_PER_USER = 10;

        private static PhotoRepository instance;

        private PhotoRepository() {}

        public static PhotoRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PhotoRepository();
                }
                return instance;
            }
        }

        public string StorageConnectionString { get; set; }

        public bool AddPhotoToDb(string filename, int profileId)
        {
            var db = new OkbDbContext();
            var p = db.Profiles.Find(profileId);

            //Check if uploading more than max allowed photos
            var filenames = p.PhotosInternal.Split(';');

            if (filenames.Length >= MAX_PHOTOS_PER_USER)
            {
                //We've reached the maximum allowed photos per user. return error
                return false;
            }

            p.PhotosInternal += ';' + filename;

            return true;
        }

        public void UploadPhoto(Stream upload, int leftThumb, int topThumb, int widthThumb)
        {
            //Generate a random filename
            var fileName = Path.GetRandomFileName().Split('.')[0];

            using (var imgFactory = new ImageFactory())
            using (var outStream = new MemoryStream())
            {
                // Create thumbnail
                imgFactory
                    .Load(upload)
                    .Crop(new Rectangle(leftThumb, topThumb, widthThumb, widthThumb))
                    .Resize(new Size(THUMB_WIDTH,THUMB_WIDTH))
                    .Save(outStream);

                // Save to Azure                
                var f = fileName + "." + imgFactory.CurrentImageFormat.DefaultExtension;
                Console.WriteLine(f);
            }


            // Resize Image

            // Upload to Microsoft Azure

            // Update the database
            //var cont = CloudStorageAccount.Parse(StorageConnectionString)
            //    .CreateCloudBlobClient()
            //    .GetContainerReference("photos");

            //var storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
            //var blobClient = storageAccount.CreateCloudBlobClient();
            //var container = blobClient.GetContainerReference("photos");
            //var blob = container.GetBlockBlobReference("test.jpg");

            //// Create or overwrite the "myblob" blob with contents from a local file.
            //using (var fileStream = File.OpenRead(@"C:\Users\Public\Pictures\Sample Pictures\desert.jpg"))
            //{
            //    blob.UploadFromStream(fileStream);
            //}
        }
    }
}
