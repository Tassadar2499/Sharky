using SC2APIProtocol;
using Sharky;
using Sharky.DefaultBot;
using System;
using Sharky.Setup;

namespace SharkyZergExampleBot
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Starting SharkyZergExampleBot");

            var gameConnection = new GameConnection();
            var defaultSharkyBot = new DefaultSharkyBot(gameConnection);

            var zergBuildChoices = new ZergBuildChoices(defaultSharkyBot);
            defaultSharkyBot.BuildChoices[Race.Zerg] = zergBuildChoices.BuildChoices;

            var sharkyExampleBot = defaultSharkyBot.CreateBot(defaultSharkyBot.Managers, defaultSharkyBot.DebugService);

            const Race myRace = Race.Zerg;
            if (args.Length == 0)
            {
                gameConnection.RunSinglePlayer(sharkyExampleBot, @"BlackburnAIE.SC2Map", myRace, Race.Random, Difficulty.VeryHard, AIBuild.RandomBuild).Wait();
            }
            else
            {
                gameConnection.RunLadder(sharkyExampleBot, myRace, args).Wait();
            }
        }
    }
}
