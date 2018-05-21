using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Chronicity.Provider.EntityFramework.DataModels
{
    public class EventType
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
    }
}
