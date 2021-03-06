﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace t_ag.Models
{
    public class Tour
    {
        public int id { set; get; }
        public String country { set; get; }
        public String type { set; get; }
        public int price { set; get; }
        public String description { set; get; }
        public int sale { set; get; }
        public DateTime saleDate { set; get; }
        public List<String> feedbacks { set; get; }
    }
}
