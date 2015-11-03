using okboba.Models.Validation;
using okboba.Repository;
using okboba.Repository.EntityRepository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;

namespace okboba.App_Start
{
    public class OkbobaConfig
    {
        public static void Init()
        {
            // Setup the Photo Repository
            var str = ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
            EntityPhotoRepository.Instance.StorageConnectionString = str;

            //Custom attributes
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(LocalRequiredAttribute), typeof(RequiredAttributeAdapter));
        }
    }
}