using System;
using System.Collections.Generic;
using System.Text;

namespace CountriesCitiesManagement.Domain.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


    }
}
