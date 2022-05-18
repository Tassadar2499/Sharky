using SC2APIProtocol;
using Sharky.Builds;
using Sharky.Builds.Zerg;
using Sharky.DefaultBot;
using ZergDisraptorBot.Builds;
using ZergDisraptorBot.MicroTasks;

namespace ZergDisraptorBot;

public class ZergBuildChoices
{
    public BuildChoices BuildChoices { get; }

    public ZergBuildChoices(DefaultSharkyBot defaultSharkyBot)
    {
        var zerglingRush = new BasicZerglingRush(defaultSharkyBot);
        var mutaliskRush = new MutaliskRush(defaultSharkyBot);

        var builds = new Dictionary<string, ISharkyBuild>
        {
            [zerglingRush.Name()] = zerglingRush,
            [mutaliskRush.Name()] = mutaliskRush
        };

        var versusEverything = new List<List<string>>
        {
            new() { zerglingRush.Name(), mutaliskRush.Name() },
            new() { mutaliskRush.Name() },
        };

        var transitions = new List<List<string>>
        {
            new() { mutaliskRush.Name() },
        };

        var buildSequences = new Dictionary<string, List<List<string>>>
        {
            [Race.Terran.ToString()] = versusEverything,
            [Race.Zerg.ToString()] = versusEverything,
            [Race.Protoss.ToString()] = versusEverything,
            [Race.Random.ToString()] = versusEverything,
            ["Transition"] = transitions,
        };

        BuildChoices = new BuildChoices { Builds = builds, BuildSequences = buildSequences };

        AddZergTasks(defaultSharkyBot);
    }

    private static void AddZergTasks(DefaultSharkyBot defaultSharkyBot)
    {
        var overlordScoutTask = new OverlordScoutTask(defaultSharkyBot, 2);
        defaultSharkyBot.MicroTaskData.MicroTasks[overlordScoutTask.GetType().Name] = overlordScoutTask;
    }
}