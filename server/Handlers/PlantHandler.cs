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
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Timers;

namespace _220223KCore.Handlers
{
    public class PlantHandler : IScript
    {
        public static List<Plant> Plants_ = new List<Plant>(); //Plants is DB list, Plants_ is List on Server
        public static List<PlantType> PlantTypes_ = new List<PlantType>();
        public static List<PlantTypeModel> PlantTypesModels_ = new List<PlantTypeModel>();

        public static void Init()
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
                UpdatePlantInfo(plant);
            }

            System.Timers.Timer plantTimer = new System.Timers.Timer(5*60*1000); //Every 5 Minutes
            plantTimer.Elapsed += PlantTimerElapsed;
            plantTimer.AutoReset = true;
            plantTimer.Enabled = true;
        }

        private static void PlantTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            foreach (Plant plant in Plants_.Where(x => x.State == (int)PlantStates.GROWING))
            {
                PlantType type = GetPlantType(plant.TypeId);

                plant.Water -= 1;
                plant.Growth += 5;

                if (plant.Water <= 0)
                {
                    plant.State = (int)PlantStates.DEAD;
                    UpdatePlantInfo(plant);
                    return;
                }
                if (plant.Growth >= (type.MaxGrowth*0.5) && plant.Model != type.Models.Model_Med)
                {
                    plant.Model = type.Models.Model_Med;
                }
                if (plant.Growth >= (type.MaxGrowth * 0.8) && plant.Model != type.Models.Model_Large)
                {
                    plant.Model = type.Models.Model_Large;
                }
                if (plant.Growth >= type.MaxGrowth)
                {
                    plant.State = (int)PlantStates.DONE;
                    UpdatePlantInfo(plant);
                    return;
                }
                using (var db = new PlantContext())
                {
                    db.Update(plant);
                    db.SaveChanges();
                }
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
                State = (int)PlantStates.GROWING,
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
        public static void UpdatePlantInfo(Plant plant)
        {            
            if (!(plant.State == (int)PlantStates.DEAD || plant.State == (int)PlantStates.DONE))
            {
                return;
            }
            string labelStr =
                $" Dev Info \n " +
                $" {plant.DisplayName} \n" +
                $" { Enum.GetName(typeof(PlantStates), plant.State)}";
            plant.label.Text = labelStr;

        }
        public static DynamicTextLabel CreatePlantInfo(Plant plant)
        {
            PlantType plantType = GetPlantType(plant.TypeId);
            TextInfo TI = new CultureInfo("en-US", false).TextInfo;

            string name = plantType.Name.ToLower().Replace("_", " ");
            plant.DisplayName = TI.ToTitleCase(name);

            DynamicTextLabel label = TextLabelStreamer.CreateDynamicTextLabel(
                $" Dev Info \n " +
                $" {plant.DisplayName} \n" +
                $" Wachstum: \n" +
                $"{plant.Growth} / {plantType.MaxGrowth}\n" +
                $" Water: \n" +
                $"{plant.Water}% / 100%",
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
        public static PlantType GetPlantType(int typeId)
        {   
            return PlantTypes_.ToList().FirstOrDefault(x => x.Id == typeId);
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
