using AltV.Net.Data;
using AltV.Streamers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _220223KCore.Models
{
    public class Prop
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public Position Position { get; set; }
        public Vector3 Rotation { get; set; }

        [NotMapped]
        public DynamicObject obj { get; set; }
    }
}
