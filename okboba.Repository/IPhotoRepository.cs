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
        void UploadPhoto(Stream upload, int leftThumb, int topThumb, int widthThumb, int profileId);
    }
}
