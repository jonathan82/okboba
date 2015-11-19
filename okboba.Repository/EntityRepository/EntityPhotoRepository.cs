using ImageProcessor;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository.EntityRepository
{
    public class EntityPhotoRepository : IPhotoRepository
    {
        #region Singelton
        private static EntityPhotoRepository instance;
        private EntityPhotoRepository() { }
        public static EntityPhotoRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EntityPhotoRepository();
                }
                return instance;
            }
        }
        #endregion

        ////////////////// Member variables ////////////////////////       
        const int MAX_IMAGE_HEIGHT = 1000;
        const int FULL_THUMBNAIL_HEIGHT = 250;
        const int MAX_FILENAME_RETRY = 3;

        public string StorageConnectionString { get; set; }

        ///////////////////////// Methods /////////////////////////////////
        /// <summary>
        /// Get the number of photos the user already has
        /// </summary>
        public int GetNumOfPhotos(int profileId)
        {
            var db = new OkbDbContext();
            var profile = db.Profiles.Find(profileId);
            return profile.GetPhotos().Length;
        }

        /// <summary>
        /// Uploads a photo to the database as well as Azure cloud.  Creates a thumbnail and r
        /// resizes image before doing so.  Also generates a random filename.
        /// </summary>
        public void UploadPhoto(Stream upload, int leftThumb, int topThumb, int widthThumb, int profileId)
        {
            using (var imgFactory = new ImageFactory())
            {
                Stream thumbStream, origStream;
                CloudBlockBlob thumbBlob, origBlob;

                //Setup
                var cont = GetBlobContainer(profileId);
                var filePrefix = GenerateUniqueFilename(cont);
                imgFactory.Load(upload);

                // Create and Upload the thumbnail
                thumbStream = CreateThumbnail(imgFactory, leftThumb, topThumb, widthThumb, OkbConstants.AVATAR_WIDTH);
                thumbBlob = cont.GetBlockBlobReference(filePrefix + "_t");
                thumbBlob.UploadFromStream(thumbStream);

                //create and upload small thumbnail
                imgFactory.Reset();
                thumbStream = CreateThumbnail(imgFactory, leftThumb, topThumb, widthThumb, OkbConstants.AVATAR_WIDTH_SMALL);
                thumbBlob = cont.GetBlockBlobReference(filePrefix + "_s");
                thumbBlob.UploadFromStream(thumbStream);

                //create and upload full thumbnail
                imgFactory.Reset();
                thumbStream = ResizeImage(imgFactory, FULL_THUMBNAIL_HEIGHT);
                thumbBlob = cont.GetBlockBlobReference(filePrefix + "_u");
                thumbBlob.UploadFromStream(thumbStream);

                // Upload resized image if necessary
                if (imgFactory.Image.Height > MAX_IMAGE_HEIGHT)
                {
                    imgFactory.Reset();
                    origStream = ResizeImage(imgFactory, MAX_IMAGE_HEIGHT);
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


        /// <summary>
        /// Gets the Windows Azure Storage container
        /// </summary>
        private CloudBlobContainer GetBlobContainer(int profileId)
        {
            var cont = CloudStorageAccount.Parse(StorageConnectionString)
                .CreateCloudBlobClient()
                .GetContainerReference(profileId.ToString());

            cont.CreateIfNotExists(BlobContainerPublicAccessType.Blob);

            return cont;
        }

        /// <summary>
        /// Adds a Photo to the database for the given user.  uses a semicolon delimited string to store
        /// photo names.  
        /// </summary>
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

        /// <summary>
        /// Creates a thumbnail from the given image
        /// </summary>
        private MemoryStream CreateThumbnail(ImageFactory imgFactory, int left, int top, int width, int finalWidth)
        {
            var outStream = new MemoryStream();

            imgFactory
                .Crop(new Rectangle(left, top, width, width))
                .Resize(new Size(finalWidth, finalWidth))
                .Save(outStream);

            return outStream;
        }

        /// <summary>
        /// Resizes image so the longest dimension is longestSide
        /// </summary>
        private MemoryStream ResizeImage(ImageFactory imgFactory, int height)
        {
            var outStream = new MemoryStream();

            //Maintain aspect ratio
            var width = (int)(((float)height / imgFactory.Image.Height) * imgFactory.Image.Width);
            
            //Maintain aspect ratio
            //int height = (int)(((float)width / imgFactory.Image.Width) * imgFactory.Image.Height);

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
    }
}
