using okboba.Entities;
using okboba.Repository.Models;
using okboba.Resources;
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
        void EditProfileText(int profileId, string text, string whichQuestion);
        int GetProfileId(string userId);
        IDictionary<string, IList<ProfileDetailOption>> GetDetailOptions();
        void EditDetails(ProfileDetail details, OkbConstants.ProfileDetailSections section, int profileId);
        MatchCriteriaModel GetMatchCriteria(int profileId);
    }
}
