using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalsAPI.Models
{
    public class Animal
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Species { get; set; }

        public string EatingHabit { get; set; }

        public int Legs { get; set; }

        [Required]
        public int ZooId { get; set; }

        public Zoo Zoo { get; set; }
    }
}
