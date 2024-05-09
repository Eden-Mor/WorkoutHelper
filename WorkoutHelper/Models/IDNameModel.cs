using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutHelper.Shared.Models;

namespace WorkoutHelper.Models
{
    public class IDNameModel : IIDProperty
    {
        public IDNameModel() { }

        public IDNameModel(int? id , string name)
        {
            Id = id;
            Name = name;
        }

        public int? Id { get; set; }
        public string? Name { get; set; }
    }
}
