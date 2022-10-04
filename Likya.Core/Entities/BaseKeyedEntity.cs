using System;

namespace Likya.Core.Entities
{
    public class BaseKeyedEntity<T> : BaseEntity
    {
        public T Id { get; set; }
        public BaseKeyedEntity()
        {
            if (typeof(T).Equals(typeof(Guid)))
            {
                this.Id = (T)(object)Guid.NewGuid();
            }
        }
    }
}
