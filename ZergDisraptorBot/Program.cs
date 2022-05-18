using SC2APIProtocol;
using Sharky._Common;
using Sharky.DefaultBot;
using Sharky.Setup;
using ZergDisraptorBot;

const Race myRace = Race.Zerg;

Console.WriteLine("Starting SharkyZergExampleBot");

var gameConnection = new GameConnection();
var defaultSharkyBot = new DefaultSharkyBot(gameConnection);

var zergBuildChoices = new ZergBuildChoices(defaultSharkyBot);
defaultSharkyBot.BuildChoices[myRace] = zergBuildChoices.BuildChoices;

var sharkyExampleBot = defaultSharkyBot.CreateBot(defaultSharkyBot.Managers, defaultSharkyBot.DebugService);

Func<Task> actionToRun = args.IsNullOrEmpty()
    ? () => gameConnection.RunSinglePlayer(sharkyExampleBot, @"BlackburnAIE.SC2Map", myRace, Race.Random, Difficulty.VeryHard, AIBuild.RandomBuild)
    : () => gameConnection.RunLadder(sharkyExampleBot, myRace, args);
    
await actionToRun();