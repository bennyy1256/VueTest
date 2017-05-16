using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VueWebApplication.Interface;

namespace VueWebApplication.Models
{
    public class VueTestModel : IRedisList
    {
        public long RedisIndex { get; set; }

        //----------------------------------------------------------------------------------------------------

        public string Name { get; set; }

        public int Age { get; set; }

        public string Birthday { get; set; }

        public bool Changed { get; set; }

        
    }
}