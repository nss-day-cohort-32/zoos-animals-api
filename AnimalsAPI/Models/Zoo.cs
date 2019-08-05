using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalsAPI.Models
{
    public class Zoo
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }

        public float Acres { get; set; }

        public List<Animal> Animals { get; set; } = new List<Animal>();
    }
}
