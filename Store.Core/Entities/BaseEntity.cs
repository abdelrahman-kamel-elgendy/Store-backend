using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Core.Entities
{
    /// <summary>Provides the integer key and common lifecycle fields for persisted entities.</summary>
    /// <typeparam name="T">The entity key type.</typeparam>
    public class BaseEntity<T>
    {
        /// <summary>Gets or sets the entity's primary key.</summary>
        public T Id { get; set; }

        /// <summary>Gets or sets when the entity was created.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Gets or sets when the entity was last updated.</summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>Gets or sets whether the entity is logically deleted.</summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>Gets or sets when the entity was deleted.</summary>
        public DateTime DeletedAt { get; set; }
    }
}
