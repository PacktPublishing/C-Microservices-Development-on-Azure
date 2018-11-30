using System;
using System.ComponentModel.DataAnnotations;

namespace ToDosService.Models
{
    public class BaseEntity
    {
        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
    }
}