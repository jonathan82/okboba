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
        //////////////////////////////////////// Private variables ////////////////////////
        const int THUMB_WIDTH = 200;
        const int MAX_PHOTOS_PER_USER = 10;
        const int MAX_IMAGE_WIDTH = 900;
        const int MAX_FILENAME_RETRY = 3;
        private static PhotoRepository instance;
        private PhotoRepository() { }


        ///////////////////////////////////// Public Properties ///////////////////////////
        public string StorageConnectionString { get; set; }
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


        /////////////////////////////////////// Private Methods ////////////////////////

        /// <summary>
        /// Gets the Windows Azure Storage container
        /// </summary>
        /// <returns></returns>
        private CloudBlobContainer GetBlobContainer()
        {
            var cont = CloudStorageAccount.Parse(StorageConnectionString)
                .CreateCloudBlobClient()
                .GetContainerReference("photos");

            return cont;
        }

        /// <summary>
        /// Adds a Photo to the database
        /// </summary>
        /// <param name="filename">The filename of the photo</param>
        /// <param name="profileId">The profild ID of the user this photo belongs to</param>
        /// <returns>An integer indicating how many photos were added.  1 or 0</returns>
        private int AddPhotoToDb(string filename, int profileId)
        {
            var db = new OkbDbContext();
            var p = db.Profiles.Find(profileId);

            if (p.PhotosInternal == "" || p.PhotosInternal == null)
            {
                p.PhotosInternal = filename;
            }
            else
            {
                p.PhotosInternal += ';' + filename;
            }
            
            db.SaveChanges();

            return 1;
        }

        private MemoryStream CreateThumbnail(ImageFactory imgFactory, int left, int top, int width)
        {
            var outStream = new MemoryStream();

            imgFactory
                .Crop(new Rectangle(left, top, width, width))
                .Resize(new Size(THUMB_WIDTH, THUMB_WIDTH))
                .Save(outStream);

            return outStream;
        }

        private MemoryStream ResizeImage(ImageFactory imgFactory, int width)
        {
            var outStream = new MemoryStream();

            //Maintain aspect ratio
            int height = (int)(((float)width / (float)imgFactory.Image.Width) * (float)imgFactory.Image.Height);

            imgFactory
                .Resize(new Size(width, height))
                .Save(outStream);

            return outStream;
        }

        private string GenerateUniqueFilename(CloudBlobContainer cont)
        {
            for (int i = 0; i < MAX_FILENAME_RETRY; i++)
            {
                var filename = Path.GetRandomFileName().Split('.')[0];
                if (!cont.GetBlockBlobReference(filename).Exists())
                {
                    return filename;
                }
            }

            //Got here, couldn't generate filename
            throw new Exception("Couldn't generate unique filename");
        }


        //////////////////////////////////////// Public Methods ///////////////////////

        /// <summary>
        /// Get the number of photos the user already has
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns></returns>
        public int GetNumOfPhotos(int profileId)
        {
            var db = new OkbDbContext();
            var profile = db.Profiles.Find(profileId);
            int numOfPhotos = 0;

            if(profile.PhotosInternal == "" || profile.PhotosInternal == null)
            {
                numOfPhotos = 0;
            }
            else
            {
                numOfPhotos = profile.PhotosInternal.Split(';').Count();
            }
            
            return numOfPhotos;
        }

        /// <summary>
        /// Uploads a photo to the database as well as Azure cloud.  Creates a thumbnail and r
        /// resizes image before doing so.  Also generates a random filename.
        /// </summary>
        /// <param name="upload"></param>
        /// <param name="leftThumb"></param>
        /// <param name="topThumb"></param>
        /// <param name="widthThumb"></param>
        public void UploadPhoto(Stream upload, int leftThumb, int topThumb, int widthThumb, int profileId)
        {
            using (var imgFactory = new ImageFactory())
            {
                Stream thumbStream, origStream;
                CloudBlockBlob thumbBlob, origBlob;

                //Setup
                var cont = GetBlobContainer();
                var filePrefix = GenerateUniqueFilename(cont);
                imgFactory.Load(upload);

                // Create and Upload the thumbnail
                thumbStream = CreateThumbnail(imgFactory, leftThumb, topThumb, widthThumb);
                thumbBlob = cont.GetBlockBlobReference(filePrefix + "_t");
                thumbBlob.UploadFromStream(thumbStream);

                // Upload resized image if necessary
                if (imgFactory.Image.Width > MAX_IMAGE_WIDTH)
                {
                    imgFactory.Reset();
                    origStream = ResizeImage(imgFactory, MAX_IMAGE_WIDTH);
                }
                else
                {
                    origStream = upload;
                }
                origBlob = cont.GetBlockBlobReference(filePrefix);
                origBlob.UploadFromStream(origStream);

                //Finally Save filename to DB
                AddPhotoToDb(filePrefix, profileId);
            }
        }
    }
}
