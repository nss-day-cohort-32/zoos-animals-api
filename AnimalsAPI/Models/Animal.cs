using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalsAPI.Models
{
    public class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public string EatingHabit { get; set; }
        public int Legs { get; set; }
        public int ZooId { get; set; }
        public Zoo Zoo { get; set; }
    }
}
