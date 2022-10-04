using System;
using Microsoft.EntityFrameworkCore;

namespace Likya.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TrackableAttribute : System.Attribute
    {
        public EntityState[] States { get; private set; }
        public bool Detailed { get; private set; } = true;
        public TrackableAttribute(params EntityState[] states) : base()
        {
            if (states != null && states.Length > 0)
                this.States = states;
            else
                this.States = new EntityState[] {
                    EntityState.Added,
                    EntityState.Modified,
                    EntityState.Deleted
                };
        }

        public TrackableAttribute(bool detailed, params EntityState[] states) : this(states)
        {
            this.Detailed = detailed;
        }
    }
}
