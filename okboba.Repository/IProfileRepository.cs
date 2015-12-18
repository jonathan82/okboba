using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository
{
    public interface IProfileRepository
    {
        Profile GetProfile(int profileId);
        ProfileText GetProfileText(int profileId);
        ProfileDetail GetProfileDetail(int profileId);
        string GetOptionValue(string colName, byte id);
        List<ProfileDetailOption> GetOptionValues(string colName);
        void EditProfileText(int profileId, string text, string whichQuestion);
        int GetProfileId(string userId);
    }
}
