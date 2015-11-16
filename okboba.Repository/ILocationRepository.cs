using okboba.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okboba.Repository
{
    public interface ILocationRepository
    {
        List<Location> GetProvinceList();
        List<Location> GetDistrictList(int id);
        string GetLocationString(int locId1, int locId2);
    }
}
