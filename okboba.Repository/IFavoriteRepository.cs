using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository
{
    public interface IFavoriteRepository
    {
        IList<Profile> GetFavorites(int profileId);
        void Save(int me, int favoriteId);
        void Remove(int me, int favoriteId);
        bool IsFavorite(int me, int favoriteId);
    }
}
