using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _220223KCore.Models
{
    public class PlantType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PlantTypeModel Models { get; set; }
        public int MaxGrowth { get; set; }
        public int PlantDelay { get; set; }
        public int MinRecive { get; set; }
        public int MaxRecive { get; set; }
    }
    public class PlantTypeModel
    {
        public int Id { get; set; }
        public string Model_Small { get; set; }     
        public string Model_Med { get; set; }       
        public string Model_Large { get; set; }     
        public string AnimDict { get; set; }        
        public string AnimName { get; set; }        

    }
    public enum PlantTypeIds 
    {
        PURPLE_HAZE = 1,
        HOLY_KUSH = 2,
        LEMON_HAZE = 3,
        WHITE_WIDDOW = 4,
    }
}
