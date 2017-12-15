using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core.Entities.AttributeFilters
{
    public class Equals : IAttributeFilter
    {
        private string _key;
        private string _value;

        public Equals(string key, string value)
        {
            _key = key;
            _value = value;
        }

        public bool Matches(IEnumerable<State> states)
        {
            foreach(var state in states)
            {
                return true;
            }

            return false;
        }
    }
}
