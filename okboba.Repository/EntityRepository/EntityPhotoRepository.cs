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
        public async Task<string> UploadAsync(Stream upload, int leftThumb, int topThumb, int widthThumb, string userId, int profileId)
        {
            string filename = "";

            using (var imgFactory = new ImageFactory())
            {
                Stream thumbStream, origStream;
                CloudBlockBlob thumbBlob, origBlob;

                //Setup
                var cont = GetBlobContainer(userId);
                var unique = GenerateUniqueFilename(cont);
                imgFactory.Load(upload);

                // Upload resized image if necessary
                if (imgFactory.Image.Height > MAX_IMAGE_HEIGHT)
                {
                    origStream = ResizeImage(imgFactory, MAX_IMAGE_HEIGHT);
                }
                else
                {
                    origStream = upload;
                }
                var code = Profile.EncodeDimensions(imgFactory.Image.Width, imgFactory.Image.Height);
                filename = unique + '_' + code;
                origBlob = cont.GetBlockBlobReference(filename);
                var t4 = origBlob.UploadFromStreamAsync(origStream);

                // Create and Upload the thumbnail
                imgFactory.Reset();
                thumbStream = CreateThumbnail(imgFactory, leftThumb, topThumb, widthThumb, OkbConstants.AVATAR_WIDTH);
                thumbBlob = cont.GetBlockBlobReference(filename + "_" + OkbConstants.HEADSHOT_SUFFIX);
                var t1 = thumbBlob.UploadFromStreamAsync(thumbStream);

                //create and upload small thumbnail
                imgFactory.Reset();
                thumbStream = CreateThumbnail(imgFactory, leftThumb, topThumb, widthThumb, OkbConstants.AVATAR_WIDTH_SMALL);
                thumbBlob = cont.GetBlockBlobReference(filename + "_" + OkbConstants.HEADSHOT_SMALL_SUFFIX);
                var t2 = thumbBlob.UploadFromStreamAsync(thumbStream);

                //create and upload full thumbnail
                imgFactory.Reset();
                thumbStream = ResizeImage(imgFactory, FULL_THUMBNAIL_HEIGHT);
                thumbBlob = cont.GetBlockBlobReference(filename + "_" + OkbConstants.THUMBNAIL_SUFFIX);
                var t3 = thumbBlob.UploadFromStreamAsync(thumbStream);                

                await t1; await t2; await t3; await t4;

                //Finally Save filename to DB
                await AddPhotoToDbAsync(filename, profileId);
            }

            return filename + "_" + OkbConstants.THUMBNAIL_SUFFIX;
        }


        /// <summary>
        /// Gets the Windows Azure Storage container
        /// </summary>
        private CloudBlobContainer GetBlobContainer(string userId)
        {
            var cont = CloudStorageAccount.Parse(StorageConnectionString)
                .CreateCloudBlobClient()
                .GetContainerReference(userId);

            cont.CreateIfNotExists(BlobContainerPublicAccessType.Blob);

            return cont;
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

        public async Task EditThumbnailAsync(string photo, int topThumb, int leftThumb, int widthThumb, int screenWidth, string userId)
        {
            var cont = GetBlobContainer(userId);
            var blob = cont.GetBlockBlobReference(photo);

            using (var imgFactory = new ImageFactory())
            {
                var stream = new MemoryStream();
                await blob.DownloadToStreamAsync(stream);
                imgFactory.Load(stream);

                //Calculate the scaled thumbnail dimensions
                float scale = (float)imgFactory.Image.Width / screenWidth;

                var thumbStream = CreateThumbnail(imgFactory, 
                    (int)(leftThumb * scale),
                    (int)(topThumb * scale),
                    (int)(widthThumb * scale), 
                    OkbConstants.AVATAR_WIDTH);

                //Upload the new thumbnail
                blob = cont.GetBlockBlobReference(photo + "_t");
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
            var cont = GetBlobContainer(userId);
            var t2 = cont.GetBlockBlobReference(photo).DeleteAsync();
            var t3 = cont.GetBlockBlobReference(photo + "_t").DeleteAsync();
            var t4 = cont.GetBlockBlobReference(photo + "_s").DeleteAsync();
            var t5 = cont.GetBlockBlobReference(photo + "_u").DeleteAsync();

            await t1; await t2; await t3; await t4; await t5;
        }
    }
}
