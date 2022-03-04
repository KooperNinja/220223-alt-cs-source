using _220223KCore.DataAccess;
using _220223KCore.Models;
using AltV.Net;
using AltV.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _220223KCore.Handlers
{
    public class PropHandler : IScript
    {
        public static List<Models.Prop> Props_ = new List<Models.Prop>();
        public static List<PropType> PropTypes_ = new List<PropType>();

        public static void Init()
        {
            using (var db = new PropContext())
            {
                Props_ = new List<Models.Prop>(db.Props);
            }

        }

    }
}
