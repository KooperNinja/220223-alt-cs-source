using _220223KCore.Models;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _220223KCore.Handlers
{
    public class PlacingData
    {
        public bool active { get; set; }
        public string? type { get; set; }
        public string? animDict { get; set; }
        public string? animName { get; set; }
    }
    public class ObjectPlacingHandler : IScript
    {
        public static void SetPlayerObjectPlacing(IPlayer player, string type)
        {
            PlacingData plData = new PlacingData()
            {
                active = true, //turning placing mode on / keep it on
                type = type,
            };
            player.SetLocalMetaData("ObjectPlacing:Status", JsonConvert.SerializeObject(plData));
        }
        public static async void PlayerPlacingObject(IPlayer player, Position pos, int delay, string animDict, string animName)
        {
            float heading = VectorMath.HeadingToPosition(player.Position, pos);
            player.Rotation = new Rotation(player.Rotation.Roll, player.Rotation.Pitch, heading);

            PlacingData plData = new PlacingData()
            {
                active = true, //currently placing the object
                animDict = animDict,
                animName = animName,
            };

            player.SetLocalMetaData("ObjectPlacing:Planting", JsonConvert.SerializeObject(plData));

            await Task.Delay(delay);

            plData.active = false;
            player.SetLocalMetaData("ObjectPlacing:Planting", JsonConvert.SerializeObject(plData));
        }

        [ClientEvent("ObjectPlacing:Validate")]
        public static void onValidate(IPlayer player, string type, Position pos, Vector3 normal)
        {
            if (player.Position.Distance(pos) > 2 ||
                player.Position.Z < pos.Z ||
                Math.Abs(normal.Z - 1) > 0.1 ||
                Math.Abs(player.Position.Z - pos.Z) > 1.2
                )
            {
                Console.WriteLine("Val Failed");
                player.Emit("ObjectPlacing:ValidateFail"); 
                return;
            }
     
            PlacingData plData = new PlacingData()
            {
                active = false, //turning placing mode off
                type = "none", 
            };

            player.SetLocalMetaData("ObjectPlacing:Status", JsonConvert.SerializeObject(plData)); //Sending stop instructions to client

            if (type.ToLower().Contains("plant")) //if it's a plant type
            {
                int typeId = PlantHandler.GetPlantTypeId(type);
                if (typeId == 0)
                {
                    return;
                }
                Vector3 angles = VectorMath.NormalVectorToGroundRotAngles(normal);
                PlantHandler.StartPlayerPlant(player, typeId, pos, angles);
            }
            
        }
    }
}
