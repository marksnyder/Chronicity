using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Chronicity.Provider.EntityFramework.DataModels
{
    public class TimeAndStateEntry
    {
        [Key]
        public int Id { get; set; }
        public int TimeAndStateEntryId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
