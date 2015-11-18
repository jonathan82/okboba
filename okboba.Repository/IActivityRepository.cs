﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository
{
    public interface IActivityRepository
    {        
        void EditProfileTextActivity(int who, string what);
        void UploadPhotoActivity(int who, string what);
        void JoinedActivity(int who);
        void AnsweredQuestionActivity(int who, string what);
    }
}
