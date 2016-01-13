using ImageProcessor;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using okboba.Entities;
using okboba.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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
            return profile.GetThumbnails().Count();
        }

        /// <summary>
        /// Uploads a photo to the database as well as Azure cloud.  Creates four versions of the photo:
        /// 
        ///   - a 200x200 headshot
        ///   - a 90x90 smaller headshot
        ///   - a thumbnail of the full image with a height of 250 px
        ///   - the original photo (max height of 1000 px)
        /// 
        /// The photo is stored in Azure Storage with the UserId as the foldername. A random filename is
        /// generated for each photo using RNGCrypto based filename generator. The dimensions of the original
        /// photo are encoded in Base62 and stored in the filenames.
        /// 
        /// Returns the filename of the thumbnail generated. Used by the Activity Feed to show additional info.
        /// </summary>
        public async Task<string> UploadAsync(Stream upload, int leftThumb, int topThumb, int widthThumb, int screenWidth, int profileId, string userId)
        {
            string filename = "";

            using (var imgFactory = new ImageFactory())
            {
                Stream thumbStream, origStream;
                CloudBlockBlob thumbBlob, origBlob;

                //Setup
                var dir = GetPhotoDirectory(userId);
                var unique = GenerateUniqueFilename(dir);
                imgFactory.Load(upload);

                //Calculate the scaled thumbnail dimensions
                float scale = (float)imgFactory.Image.Width / screenWidth;

                //Resize original image if necessary
                if (imgFactory.Image.Height > MAX_IMAGE_HEIGHT)
                {
                    origStream = ResizeImage(imgFactory, MAX_IMAGE_HEIGHT);
                }
                else
                {
                    origStream = upload;
                }

                //Generate filename with size code
                var code = Profile.EncodeDimensions(imgFactory.Image.Width, imgFactory.Image.Height);
                filename = unique + '_' + code;

                //Upload original
                origBlob = dir.GetBlockBlobReference(filename);
                var t4 = origBlob.UploadFromStreamAsync(origStream);

                //Crop and upload headshot
                imgFactory.Reset();
                thumbStream = CreateThumbnail(imgFactory, leftThumb, topThumb, widthThumb, screenWidth, 
                    OkbConstants.AVATAR_WIDTH);
                thumbBlob = dir.GetBlockBlobReference(filename + OkbConstants.HEADSHOT_SUFFIX);
                var t1 = thumbBlob.UploadFromStreamAsync(thumbStream);

                //Crop and upload small headshot
                imgFactory.Reset();
                thumbStream = CreateThumbnail(imgFactory, leftThumb, topThumb, widthThumb, screenWidth, 
                    OkbConstants.AVATAR_WIDTH_SMALL);
                thumbBlob = dir.GetBlockBlobReference(filename + OkbConstants.HEADSHOT_SMALL_SUFFIX);
                var t2 = thumbBlob.UploadFromStreamAsync(thumbStream);

                //Resize and upload full thumbnail
                imgFactory.Reset();
                thumbStream = ResizeImage(imgFactory, FULL_THUMBNAIL_HEIGHT);
                thumbBlob = dir.GetBlockBlobReference(filename + OkbConstants.THUMBNAIL_SUFFIX);
                var t3 = thumbBlob.UploadFromStreamAsync(thumbStream);

                await Task.WhenAll(t1, t2, t3, t4);
                //await t1; await t2; await t3; await t4;

                //Finally Save filename to DB
                await AddPhotoToDbAsync(filename, profileId);
            }

            //Return the thumbnail
            return filename + OkbConstants.THUMBNAIL_SUFFIX;
        }


        /// <summary>
        /// Gets the virtual directory for a user's photos. Creates the photo container if
        /// it doesn't exist.
        /// </summary>
        private CloudBlobDirectory GetPhotoDirectory(string userId)
        {
            var cont = CloudStorageAccount.Parse(StorageConnectionString)
           .CreateCloudBlobClient()
           .GetContainerReference(OkbConstants.PHOTO_CONTAINER);

            cont.CreateIfNotExists(BlobContainerPublicAccessType.Blob);

            return cont.GetDirectoryReference(userId);
        }

        /// <summary>
        /// Adds a Photo to the database for the given user.  uses a semicolon delimited string to store
        /// photo names.  
        /// </summary>
        private async Task AddPhotoToDbAsync(string filename, int profileId)
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

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Creates a thumbnail from the given image
        /// </summary>
        private MemoryStream CreateThumbnail(ImageFactory imgFactory, int left, int top, int width, int screenWidth, int finalWidth)
        {
            var outStream = new MemoryStream();

            //Calculate the scaled thumbnail dimensions
            float scale = (float)imgFactory.Image.Width / screenWidth;

            imgFactory
                .Crop(new Rectangle((int)(left*scale), (int)(top*scale), (int)(width*scale), (int)(width *scale)))
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

        private string GenerateUniqueFilename(CloudBlobDirectory dir)
        {
            for (int i = 0; i < MAX_FILENAME_RETRY; i++)
            {
                var filename = Path.GetRandomFileName().Split('.')[0];
                if (!dir.GetBlockBlobReference(filename).Exists())
                {
                    return filename;
                }
            }

            //Got here, couldn't generate filename
            throw new Exception("Couldn't generate unique filename");
        }

        public async Task EditThumbnailAsync(string photo, int topThumb, int leftThumb, int widthThumb, int screenWidth, string userId)
        {
            var dir = GetPhotoDirectory(userId);
            var blob = dir.GetBlockBlobReference(photo);

            using (var imgFactory = new ImageFactory())
            {
                //Download original image to crop
                var stream = new MemoryStream();
                await blob.DownloadToStreamAsync(stream);
                imgFactory.Load(stream);

                //Calculate the scaled thumbnail dimensions
                float scale = (float)imgFactory.Image.Width / screenWidth;

                //Create and upload Headshot
                var thumbStream = CreateThumbnail(imgFactory, leftThumb, topThumb, widthThumb, screenWidth, 
                    OkbConstants.AVATAR_WIDTH);

                blob = dir.GetBlockBlobReference(photo + OkbConstants.HEADSHOT_SUFFIX);

                await blob.UploadFromStreamAsync(thumbStream);

                //Create and upload small headshot
                imgFactory.Reset();
                thumbStream = CreateThumbnail(imgFactory,leftThumb, topThumb, widthThumb, screenWidth,
                    OkbConstants.AVATAR_WIDTH_SMALL);

                blob = dir.GetBlockBlobReference(photo + OkbConstants.HEADSHOT_SMALL_SUFFIX);
                await blob.UploadFromStreamAsync(thumbStream);
            }
        }

        public async Task DeleteAsync(string photo, int profileId, string userId)
        {
            //first delete the photo in the database
            var db = new OkbDbContext();
            var profile = db.Profiles.Find(profileId);

            profile.PhotosInternal = profile.PhotosInternal.Replace(photo, "");

            //cleanup
            profile.PhotosInternal = profile.PhotosInternal.Trim(';');
            profile.PhotosInternal = profile.PhotosInternal.Replace(";;", ";");

            var t1 = db.SaveChangesAsync();

            //delete from storage
            var dir = GetPhotoDirectory(userId);
            var t2 = dir.GetBlockBlobReference(photo).DeleteAsync();
            var t3 = dir.GetBlockBlobReference(photo + OkbConstants.HEADSHOT_SUFFIX).DeleteAsync();
            var t4 = dir.GetBlockBlobReference(photo + OkbConstants.HEADSHOT_SMALL_SUFFIX).DeleteAsync();
            var t5 = dir.GetBlockBlobReference(photo + OkbConstants.THUMBNAIL_SUFFIX).DeleteAsync();

            //await t1; await t2; await t3; await t4; await t5;
            await Task.WhenAll(t1, t2, t3, t4, t5);
        }
    }
}
