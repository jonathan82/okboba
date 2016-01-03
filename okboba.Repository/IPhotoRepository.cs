using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace okboba.Repository
{
    public interface IPhotoRepository
    {
        int GetNumOfPhotos(int profileId);
        Task<string> UploadAsync(Stream upload, int leftThumb, int topThumb, int widthThumb, int profileId, string userId);
        Task EditThumbnailAsync(string photo, int topThumb, int leftThumb, int widthThumb, int screenWidth, string userId);
        Task DeleteAsync(string photo, int profileId, string userId);
    }
}
