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
    }
}
