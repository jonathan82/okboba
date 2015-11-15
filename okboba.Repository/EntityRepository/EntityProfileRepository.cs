using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository.EntityRepository
{
    public class EntityProfileRepository : IProfileRepository
    {
        #region Singleton
        private static EntityProfileRepository instance;
        private EntityProfileRepository() { }

        public static EntityProfileRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EntityProfileRepository();
                }
                return instance;
            }
        }
        #endregion

        //////////////////// Member variables ///////////////////////
        Dictionary<string, List<ProfileDetailOption>> _detailOptions; //cache the options in memory


        ////////////////////// Methods //////////////////
        public Profile GetProfile(int profileId)
        {
            var db = new OkbDbContext();
            var profile = db.Profiles.Find(profileId);
            return profile;
        }

        public ProfileDetail GetProfileDetail(int profileId)
        {
            var db = new OkbDbContext();
            var detail = db.ProfileDetails.Find(profileId);
            if (detail==null)
            {
                detail = new ProfileDetail();
            }
            return detail;
        }

        public ProfileText GetProfileText(int profileId)
        {
            var db = new OkbDbContext();
            var text = db.ProfileTexts.Find(profileId);
            if (text == null)
            {
                text = new ProfileText();
            }
            return text;
        }

        public string GetOptionValue(string colName, byte id)
        {
            if(_detailOptions==null)
            {
                //not cached, load from database
                LoadDetailOptions();
            }

            if (!_detailOptions.ContainsKey(colName))
            {
                //No detail option with this name, probably should log error
                return "";
            }

            foreach (var o in _detailOptions[colName])
            {
                if (o.Id == id) return o.Value;
            }

            //no option found for ID - probably should log error
            return "";
        }

        public List<ProfileDetailOption> GetOptionValues(string colName)
        {
            if (_detailOptions==null)
            {
                //not cached, load from database
                LoadDetailOptions();
            }

            if (!_detailOptions.ContainsKey(colName))
            {
                //return empty list, probably should log error
                return new List<ProfileDetailOption>();
            }

            return _detailOptions[colName];
        }

        private void LoadDetailOptions()
        {
            var db = new OkbDbContext();

            _detailOptions = new Dictionary<string, List<ProfileDetailOption>>();

            var query = from o in db.ProfileDetailOptions.AsNoTracking()
                        orderby o.ColName, o.Id ascending
                        select o;

            foreach (var option in query)
            {
                if (!_detailOptions.ContainsKey(option.ColName))
                {
                    _detailOptions.Add(option.ColName, new List<ProfileDetailOption>());
                }

                _detailOptions[option.ColName].Add(new ProfileDetailOption
                {
                    Id = option.Id,
                    Value = option.Value
                });
            }
        }
    }
}
