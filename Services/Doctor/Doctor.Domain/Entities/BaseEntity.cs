using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Doctor.Domain.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        [MaxLength(100)]
        public string? DeletedBy { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        // Soft Delete
        public virtual void SoftDelete(string? deletedBy = null)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy;
        }

        // Restore
        public virtual void Restore()
        {
            IsDeleted = false;
            DeletedAt = null;
            DeletedBy = null;
        }

        // Set Created Info
        public virtual void SetCreatedInfo(string? createdBy = null)
        {
            CreatedAt = DateTime.UtcNow;
            CreatedBy = createdBy ?? "System";
        }

        // Set Updated Info
        public virtual void SetUpdatedInfo(string? updatedBy = null)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy ?? "System";
        }
    }

    /// <summary>
    /// Base entity with ID of type T for custom ID types
    /// </summary>
    public abstract class BaseEntity<TId> : BaseEntity
    {
        public new TId Id { get; set; } = default!;
    }
}
