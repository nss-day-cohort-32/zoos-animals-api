using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalsAPI.Models
{
    public class Zoo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public float Acres { get; set; }
        public List<Animal> Animals { get; set; } = new List<Animal>();
    }
}
