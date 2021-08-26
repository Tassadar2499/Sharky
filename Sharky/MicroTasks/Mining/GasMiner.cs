﻿using Sharky.DefaultBot;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Sharky.MicroTasks.Mining
{
    public class GasMiner
    {
        BaseData BaseData;
        SharkyUnitData SharkyUnitData;

        public GasMiner(DefaultSharkyBot defaultSharkyBot)
        {
            BaseData = defaultSharkyBot.BaseData;
            SharkyUnitData = defaultSharkyBot.SharkyUnitData;
        }

        public GasMiner(BaseData baseData, SharkyUnitData sharkyUnitData)
        {
            BaseData = baseData;
            SharkyUnitData = sharkyUnitData;
        }

        public List<SC2APIProtocol.Action> MineGas(int frame)
        {
            var actions = new List<SC2APIProtocol.Action>();

            foreach (var selfBase in BaseData.SelfBases)
            {
                var baseVector = new Vector2(selfBase.ResourceCenter.Pos.X, selfBase.ResourceCenter.Pos.Y);
                foreach (var miningInfo in selfBase.GasMiningInfo)
                {
                    if (miningInfo.Workers.Count() > 0 && miningInfo.ResourceUnit.VespeneContents == 0)
                    {
                        foreach (var worker in miningInfo.Workers)
                        {
                            worker.UnitRole = UnitRole.None;
                        }
                        miningInfo.Workers.Clear();
                    }

                    var mineralVector = new Vector2(miningInfo.ResourceUnit.Pos.X, miningInfo.ResourceUnit.Pos.Y);
                    foreach (var worker in miningInfo.Workers.Where(w => w.UnitRole == UnitRole.Gas))
                    {
                        if (miningInfo.ResourceUnit.UnitType == (uint)UnitTypes.PROTOSS_ASSIMILATORRICH || miningInfo.ResourceUnit.UnitType == (uint)UnitTypes.TERRAN_REFINERYRICH || miningInfo.ResourceUnit.UnitType == (uint)UnitTypes.ZERG_EXTRACTORRICH)
                        {
                            if (worker.LastTargetTag != miningInfo.ResourceUnit.Tag && worker.LastAbility != Abilities.SMART)
                            {
                                var action = worker.Order(frame, Abilities.SMART, null, miningInfo.ResourceUnit.Tag, false);
                                if (action != null)
                                {
                                    actions.AddRange(action);
                                }
                            }
                            continue;
                        }
                        var workerVector = worker.UnitCalculation.Position;
                        if (worker.UnitCalculation.Unit.BuffIds.Any(b => SharkyUnitData.CarryingResourceBuffs.Contains((Buffs)b)))
                        {
                            var distanceSquared = Vector2.DistanceSquared(baseVector, workerVector);
                            if (distanceSquared > 25 || distanceSquared < 10)
                            {
                                var action = worker.Order(frame, Abilities.HARVEST_RETURN, null, 0, true);
                                if (action != null)
                                {
                                    actions.AddRange(action);
                                }
                            }
                            else
                            {
                                var action = worker.Order(frame, Abilities.MOVE, miningInfo.DropOffPoint, 0, false);
                                if (action != null)
                                {
                                    actions.AddRange(action);
                                }
                            }
                        }
                        else
                        {
                            var touchingWorker = worker.UnitCalculation.NearbyAllies.Any(w => Vector2.DistanceSquared(workerVector, w.Position) < .5);
                            if (touchingWorker || Vector2.DistanceSquared(mineralVector, workerVector) < 4)
                            {
                                var action = worker.Order(frame, Abilities.HARVEST_GATHER, null, miningInfo.ResourceUnit.Tag, false);
                                if (action != null)
                                {
                                    actions.AddRange(action);
                                }
                            }
                            else
                            {
                                var action = worker.Order(frame, Abilities.MOVE, miningInfo.HarvestPoint, 0, false);
                                if (action != null)
                                {
                                    actions.AddRange(action);
                                }
                            }
                        }
                    }
                }
            }

            return actions;
        }
    }
}
