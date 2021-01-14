﻿using Sharky.Chat;
using System;
using System.Linq;
using System.Numerics;

namespace Sharky.EnemyStrategies
{
    public class Proxy : EnemyStrategy
    {
        TargetingData TargetingData;

        public Proxy(EnemyStrategyHistory enemyStrategyHistory, ChatService chatService, ActiveUnitData activeUnitData, SharkyOptions sharkyOptions, TargetingData targetingData, DebugService debugService, UnitCountService unitCountService)
        {
            EnemyStrategyHistory = enemyStrategyHistory;
            ChatService = chatService;
            ActiveUnitData = activeUnitData;
            SharkyOptions = sharkyOptions;
            TargetingData = targetingData;
            DebugService = debugService;
            UnitCountService = unitCountService;
        }

        protected override bool Detect(int frame)
        {
            if (frame < SharkyOptions.FramesPerSecond * 60 * 3)
            {
                if (ActiveUnitData.EnemyUnits.Values.Any(u => u.Attributes.Contains(SC2APIProtocol.Attribute.Structure) && u.Unit.UnitType != (uint)UnitTypes.TERRAN_KD8CHARGE && Vector2.DistanceSquared(new Vector2(TargetingData.EnemyMainBasePoint.X, TargetingData.EnemyMainBasePoint.Y), new Vector2(u.Unit.Pos.X, u.Unit.Pos.Y)) > (100 * 100)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
