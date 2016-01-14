using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using okboba.Entities;

namespace okboba.Repository.EntityRepository
{
    public class EntityFavoriteRepository : IFavoriteRepository
    {
        #region Singelton
        private static EntityFavoriteRepository instance;
        private EntityFavoriteRepository() { }
        public static EntityFavoriteRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EntityFavoriteRepository();
                }
                return instance;
            }
        }
        #endregion

        public IList<Profile> GetFavorites(int profileId)
        {
            var db = new OkbDbContext();

            var query = from favorite in db.Favorites.AsNoTracking()
                        where favorite.ProfileId == profileId
                        orderby favorite.FavoriteDate descending
                        select favorite.FavoriteProfile;

            return query.ToList();
        }

        public bool IsFavorite(int me, int favoriteId)
        {
            var db = new OkbDbContext();
            var fav = db.Favorites.Find(me, favoriteId);
            return fav != null;
        }

        public void Remove(int me, int favoriteId)
        {
            var db = new OkbDbContext();
            var fav = db.Favorites.Find(me, favoriteId);
            db.Favorites.Remove(fav);
            db.SaveChanges();
        }

        public void Save(int me, int favoriteId)
        {
            var db = new OkbDbContext();
            db.Favorites.Add(new Favorite
            {
                ProfileId = me,
                FavoriteId = favoriteId,
                FavoriteDate = DateTime.Now
            });
            db.SaveChanges();
        }
    }
}
