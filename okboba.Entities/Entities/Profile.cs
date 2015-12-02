using okboba.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Entities
{
    public class Photo
    {
        public string Original { get; set; }
        public string Thumb { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Profile
    {
        public int Id { get; set; }

        [StringLength(20)]
        public string Nickname { get; set; }

        [Column(TypeName = "date")]
        public DateTime Birthdate { get; set; }

        public byte Gender { get; set; }
        public byte LookingForGender { get; set; }

        //public Int16 Height { get; set; }

        public Int16 LocationId1 { get; set; }
        public Int16 LocationId2 { get; set; }

        [StringLength(140)]
        public string PhotosInternal { get; set; } //list of semicolon separated filenames

        [ForeignKey("CurrentQuestion")]
        public Int16? CurrentQuestionId { get; set; }

        //Navigation properties
        public virtual Location Location { get; set; }
        public virtual ProfileText ProfileText { get; set; }
        public Question CurrentQuestion { get; set; }

        //Photo Helper methods        
        private IEnumerable<Photo> Photos(string suffix)
        {
            var photoList = new List<Photo>();
            if (string.IsNullOrEmpty(PhotosInternal)) return photoList;
            var photos = PhotosInternal.Split(';');
            for (int i = 0; i < photos.Length; i++)
            {
                var size = DecodeDimensions(photos[i].Substring(photos[i].IndexOf('_') + 1));
                photoList.Add(new Photo
                {
                    Original = photos[i],
                    Thumb = photos[i] + "_" + suffix,
                    Width = size.width,
                    Height = size.height
                });
            }

            return photoList;
        }
        
        public IEnumerable<Photo> GetHeadshots()
        {
            return Photos(OkbConstants.HEADSHOT_SUFFIX);
        }

        public IEnumerable<Photo> GetHeadshotsSmall()
        {
            return Photos(OkbConstants.HEADSHOT_SMALL_SUFFIX);
        }

        public IEnumerable<Photo> GetThumbnails()
        {
            return Photos(OkbConstants.THUMBNAIL_SUFFIX);
        }

        public string GetFirstHeadshot(bool small = false)
        {
            if (string.IsNullOrEmpty(PhotosInternal)) return "";
            var index = PhotosInternal.IndexOf(';');
            if (index < 0)
            {
                return PhotosInternal + '_' + (small ? OkbConstants.HEADSHOT_SMALL_SUFFIX : OkbConstants.HEADSHOT_SUFFIX);
            }

            return PhotosInternal.Substring(0, index) + '_' + (small ? OkbConstants.HEADSHOT_SMALL_SUFFIX : OkbConstants.HEADSHOT_SUFFIX);
        }
        

        ////////////////////////////// Base62 Functions ////////////////////////////////
        private const string Base62Codes = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static string EncodeDimensions(int width, int height)
        {            
            var code = new char[4];

            code[1] = Base62Codes[width % 62];
            width /= 62;
            code[0] = Base62Codes[width % 62];
            code[3] = Base62Codes[height % 62];
            height /= 62;
            code[2] = Base62Codes[height % 62];

            return new string(code);
        }

        struct Size
        {
            public int width;
            public int height;
        }

        private Size DecodeDimensions(string code)
        {
            var dim = new Size();

            dim.width += Base62Codes.IndexOf(code[0]) * 62;
            dim.width += Base62Codes.IndexOf(code[1]);
            dim.height += Base62Codes.IndexOf(code[2]) * 62;
            dim.height += Base62Codes.IndexOf(code[3]);

            return dim;
        }
        ////////////////////////////////////////////////////////////////////////////////

        //Helper methods
        //public string[] GetPhotos()
        //{
        //    if (string.IsNullOrEmpty(PhotosInternal)) return new string[0];
        //    return PhotosInternal.Split(';');
        //}

        //public string GetFirstPhoto()
        //{
        //    if (string.IsNullOrEmpty(PhotosInternal)) return "";
        //    var index = PhotosInternal.IndexOf(';');
        //    if (index == -1) index = PhotosInternal.Length;
        //    return PhotosInternal.Substring(0, index);
        //}

        public int GetAge()
        {
            DateTime today = DateTime.Today;
            int age = today.Year - Birthdate.Year;
            if (Birthdate > today.AddYears(-age)) age--;
            return age;
        }
    }
}
