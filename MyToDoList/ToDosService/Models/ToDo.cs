using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ToDosService.Models
{
    public class ToDo : BaseEntity
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(255)]
        public string Body { get; set; }

        [Required]
        public bool IsCompleted { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
