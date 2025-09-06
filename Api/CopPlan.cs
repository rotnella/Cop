using System;
using System.Collections.Generic;

namespace BnEGames.Cop.Api
{
    public class CopPlan
    {
        public String Title { get; set; }
        public List<IOperation> Operations { get; set; }
        public Dictionary<string,Object> Parameters { get; set; }
    }
}

