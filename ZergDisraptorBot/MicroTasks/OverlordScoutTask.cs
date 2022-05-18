using System.Collections.Concurrent;
using Sharky;
using Sharky.DefaultBot;
using Sharky.MicroTasks;

namespace ZergDisraptorBot.MicroTasks;

public class OverlordScoutTask : MicroTask
{
    private readonly BaseData _baseData;

    private readonly Random _random;

    public OverlordScoutTask(DefaultSharkyBot defaultSharkyBot, float priority, bool enabled = true)
    {
        _baseData = defaultSharkyBot.BaseData;
        _random = new Random();

        UnitCommanders = new List<UnitCommander>();
        Priority = priority;
        Enabled = enabled;
    }

    public override void ClaimUnits(ConcurrentDictionary<ulong, UnitCommander> commanders)
    {
        foreach (var commander in commanders.Where(c => !c.Value.Claimed && c.Value.UnitCalculation.Unit.UnitType == (uint)UnitTypes.ZERG_OVERLORD && c.Value.UnitCalculation.Unit.BuildProgress == 1))
        {
            commander.Value.UnitRole = UnitRole.Scout;
            commander.Value.Claimed = true;
            UnitCommanders.Add(commander.Value);
        }
    }

    public override IEnumerable<SC2APIProtocol.Action> PerformActions(int frame)
    {
        var actions = new List<SC2APIProtocol.Action>();

        actions.AddRange(ScoutRandomBases(frame));

        return actions;
    }

    private IEnumerable<SC2APIProtocol.Action> ScoutRandomBases(int frame)
    {
        var actions = new List<SC2APIProtocol.Action>();

        foreach (var commander in UnitCommanders)
        {
            if (commander.UnitCalculation.Unit.Orders.Any()) continue;
            var randomBase = _baseData.BaseLocations[_random.Next(_baseData.BaseLocations.Count)];

            var action = commander.Order(frame, Abilities.MOVE, randomBase.MineralLineLocation);
            if (action != null)
            {
                actions.AddRange(action);
            }
        }

        return actions;
    }
}