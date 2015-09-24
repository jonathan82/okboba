using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace okboba.Entities
{
    public class UserModel
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
    }
}
