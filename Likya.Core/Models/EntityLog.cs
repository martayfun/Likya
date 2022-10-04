using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Likya.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Likya.Core.Models
{
    public class EntityLog : BaseKeyedEntity<long>
    {
        [Required]
        [StringLength(250)]
        [Column(TypeName = "NVARCHAR(250)")]
        public string TableName { get; set; }

        [Required]
        [StringLength(36)]
        [Column(TypeName = "NVARCHAR(36)")]
        public string RecordId { get; set; }

        [Required]
        public EntityState State { get; set; }

        [Column(TypeName = "NVARCHAR(max)")]
        public string Data { get; set; }

        public Guid? UserId { get; set; }
        [NotMapped]
        public string UserName { get; set; }
    }
}
