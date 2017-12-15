using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core.Entities.AttributeFilters
{
    public interface IAttributeFilter
    {
         bool Matches(IEnumerable<State> states);
    }
}
