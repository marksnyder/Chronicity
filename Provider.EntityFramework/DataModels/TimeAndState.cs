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
    }
}