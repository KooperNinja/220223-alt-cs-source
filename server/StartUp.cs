using _220223KCore.Handlers;
using AltV.Net;
using AltV.Streamers;
using System;



namespace _220223KCore
{
    public class StartUp : Resource
    {
        public override void OnStart()
        {
            AltStreamers.Init();
            PlantHandler.Init();

        }
        public override void OnStop()
        {
            Console.WriteLine("Test Stopped");
        }
    }
}