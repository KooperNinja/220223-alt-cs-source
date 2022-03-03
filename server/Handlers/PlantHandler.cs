using _220223KCore.DataAccess;
using _220223KCore.Models;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Resources.Chat.Api;
using AltV.Streamers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _220223KCore.Handlers
{
    public class PlantHandler : IScript
    {
        public static List<Plant> Plants_ = new List<Plant>(); //Plants is DB list, Plants_ is List on Server
        public static List<PlantType> PlantTypes_ = new List<PlantType>();
        public static List<PlantTypeModel> PlantTypesModels_ = new List<PlantTypeModel>();

        public static void LoadAllPlants()
        {
            using (var db = new PlantContext())
            {
                Plants_ = new List<Plant>(db.Plants);
                Alt.Log($"{Plants_.Count} Plants were loaded");
                PlantTypes_ = new List<PlantType>(db.PlantTypes);
                Alt.Log($"{PlantTypes_.Count} Plant Types were loaded");
                PlantTypesModels_ = new List<PlantTypeModel>(db.PlantTypeModels);
            }

            foreach (Plant plant in Plants_)
            {
                plant.obj = ObjectStreamer.CreateDynamicObject(
                    plant.Model,
                    plant.Position,
                    plant.Rotation,
                    frozen: true
                    );
                plant.label = CreatePlantInfo(plant);

            }
        }


        public static void CreateNewPlant(int typeID, Position pos, Vector3 angles)
        {
            PlantType type = PlantTypes_.ToList().FirstOrDefault(x => x.Id == typeID);
            if (type == null)
            {
                Alt.Log("Type not valid"); 
                return;
            }
            

            Plant plant = new Plant
            {
                TypeId = type.Id,
                Model = type.Models.Model_Small,
                Position = pos,
                Rotation = angles,
                Growth = 0,
                Water = 50,
            };

            plant.obj = ObjectStreamer.CreateDynamicObject(
                plant.Model,
                plant.Position,
                plant.Rotation,
                frozen: true
                );
            plant.label = CreatePlantInfo(plant); 

            Plants_.Add(plant);
            using (var db = new PlantContext())
            {
                db.Plants.Add(plant);
                db.SaveChanges();
            }
        }
        public static DynamicTextLabel CreatePlantInfo(Plant plant)
        {
            DynamicTextLabel label = TextLabelStreamer.CreateDynamicTextLabel(
                $" Dev Info \n {plant.Growth}/30",
                plant.Position + new Position(0, 0, 1),
                streamRange: 10
                );
            return label;
        }

        public static void TryPlayerPlacingPlant(IPlayer player, string plant) //plant may be smth like PuRpleHaze or purple_HAZE OR plant_Purple_Haze
        {
            string plantStr = plant.ToLower().Replace(" ", String.Empty);
            
            if (!plantStr.StartsWith("plant_"))
            {
                plantStr = $"plant_{plantStr}";
            }

            ObjectPlacingHandler.SetPlayerObjectPlacing(player, plantStr);
        }

        public static int GetPlantTypeId(string type)
        {
            string typeStr = type.ToUpper();
            typeStr = typeStr.Replace(" ", String.Empty)
                             .Replace("_", String.Empty)
                             .Replace("PLANT", String.Empty); //Remove Plant identifier
            PlantType plantType = PlantTypes_.ToList().FirstOrDefault(
                x => x.Name.Replace("_", String.Empty).Contains(typeStr));
            if (plantType == null)
            {
                Console.WriteLine($"Plant Type: {typeStr} could not been found");
                return 0;
            }
            
            return plantType.Id;
        }


        public static async void StartPlayerPlant(IPlayer player, int typeId, Position pos, Vector3 angles)
        {
            PlantType plantType = PlantTypes_.ToList().FirstOrDefault(x => x.Id == typeId);
            if (plantType == null) return;

            int delay = plantType.PlantDelay;
            string animDict = plantType.Models.AnimDict;
            string animName = plantType.Models.AnimName;

            ObjectPlacingHandler.PlayerPlacingObject(player, pos, delay, animDict, animName);

            await Task.Delay(delay);

            CreateNewPlant(typeId, pos, angles);
            
        }
    }
}
