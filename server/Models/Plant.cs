using AltV.Net.Data;
using AltV.Streamers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _220223KCore.Models
{
    public class Plant
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public int State { get; set; }
        public string Model { get; set; } = "";
        public Position Position { get; set; }
        public Vector3 Rotation { get; set; }
        public int Growth { get; set; }
        public int Water { get; set; }
        

        [NotMapped]
        public DynamicObject obj { get; set; }
        [NotMapped]
        public DynamicTextLabel label { get; set; }
        [NotMapped]
        public string DisplayName { get; set; } = "Not Set";

    }

    public enum PlantStates
    {
        DEAD = 0,
        GROWING = 1,
        DONE = 2,
    }
}
