﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionTrack
{
    public class Product
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
        public string Size { get; set; }
        public string Gender { get; set; }
        public int Qty { get; set; }
        public decimal price { get; set; }
    }
}