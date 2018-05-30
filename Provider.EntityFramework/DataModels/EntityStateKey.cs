using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Chronicity.Provider.EntityFramework.DataModels
{
    public class EntityStateKey
    {
        [Key]
        public int Id { get; set; }
        public string Entity { get; set; }
        public string Key { get; set; }
    }
}
