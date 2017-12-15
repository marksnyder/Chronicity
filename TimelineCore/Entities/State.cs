using System;
using System.Collections.Generic;
using System.Text;

namespace Chronicity.Core.Entities
{
    public class State
    {
        public IEnumerable<Attribute> AttributeChanges { get; set; }
        public IEnumerable<Relationship> RelationshipChanges { get; set; }
    }
}
