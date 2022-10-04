using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Likya.Core.Entities
{
    public class BaseEntity
    {
        [JsonIgnore]
        [Column(Order = 100)]
        public DateTime CreateAt { get; set; }

        [JsonIgnore]
        [Column(Order = 200)]
        public DateTime? UpdateAt { get; set; }
    }
}
