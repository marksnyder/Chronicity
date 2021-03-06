﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Chronicity.Provider.EntityFramework.DataModels
{
    public class TimeAndState
    {
        [Key]
        public int Id { get; set; }
        public DateTime On { get; set; }
        public string Entity { get; set; }
        public string Key { get; set; }
        public decimal? NumericValue { get; set; }
        public string Value { get; set; }
        public string PriorValue { get; set; }
        public decimal? NumericPriorValue { get; set; }
    }
}
