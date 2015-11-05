using okboba.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace okboba.Models.Validation
{
    public class LocalRequiredAttribute : RequiredAttribute
    {
        public LocalRequiredAttribute() 
        {
            this.ErrorMessage = "需要{0}";
        }
    }

}