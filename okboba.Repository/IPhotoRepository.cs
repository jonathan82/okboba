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
        Task UploadAsync(Stream upload, int leftThumb, int topThumb, int widthThumb, int profileId);
        Task EditThumbnailAsync(string photo, int topThumb, int leftThumb, int widthThumb, int screenWidth, int profileId);
        Task DeleteAsync(string photo, int profileId);
    }
}
