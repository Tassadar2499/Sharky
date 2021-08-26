﻿using SC2APIProtocol;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Sharky.MicroTasks
{
    public class DefenseService
    {
        ActiveUnitData ActiveUnitData;
        TargetPriorityService TargetPriorityService;

        public DefenseService(ActiveUnitData activeUnitData, TargetPriorityService targetPriorityService)
        {
            ActiveUnitData = activeUnitData;
            TargetPriorityService = targetPriorityService;
        }

        public List<UnitCommander> GetDefenseGroup(List<UnitCalculation> enemyGroup, List<UnitCommander> unitCommanders)
        {
            var position = enemyGroup.FirstOrDefault().Unit.Pos;
            var enemyGroupLocation = new Vector2(position.X, position.Y);

            var enemyHealth = enemyGroup.Sum(e => e.SimulatedHitpoints);
            var enemyDps = enemyGroup.Sum(e => e.SimulatedDamagePerSecond(new List<Attribute>(), true, true));
            var enemyHps = enemyGroup.Sum(e => e.SimulatedHealPerSecond);
            var enemyAttributes = enemyGroup.SelectMany(e => e.Attributes).Distinct();
            var hasGround = enemyGroup.Any(e => !e.Unit.IsFlying);
            var hasAir = enemyGroup.Any(e => e.Unit.IsFlying);
            var cloakable = enemyGroup.Any(e => e.UnitClassifications.Contains(UnitClassification.Cloakable));

            var counterGroup = new List<UnitCommander>();

            foreach (var commander in unitCommanders)
            {
                if ((hasGround && commander.UnitCalculation.DamageGround) || (hasAir && commander.UnitCalculation.DamageAir) || (cloakable && (commander.UnitCalculation.UnitClassifications.Contains(UnitClassification.Detector) || commander.UnitCalculation.UnitClassifications.Contains(UnitClassification.DetectionCaster))) || commander.UnitCalculation.Unit.UnitType == (uint)UnitTypes.PROTOSS_PHOENIX)
                {
                    counterGroup.Add(commander);

                    var targetPriority = TargetPriorityService.CalculateTargetPriority(counterGroup.Select(c => c.UnitCalculation), enemyGroup);
                    if (targetPriority.OverallWinnability > 1)
                    {
                        return counterGroup;
                    }
                }
            }

            return new List<UnitCommander>();
        }

        public List<List<UnitCalculation>> GetEnemyGroups(IEnumerable<UnitCalculation> enemies)
        {
            var enemyGroups = new List<List<UnitCalculation>>();
            foreach (var enemy in enemies)
            {
                if (!enemyGroups.Any(g => g.Any(e => e.Unit.Tag == enemy.Unit.Tag)))
                {
                    if (ActiveUnitData.EnemyUnits.ContainsKey(enemy.Unit.Tag))
                    {
                        var group = new List<UnitCalculation>();
                        group.Add(enemy);
                        foreach (var nearbyEnemy in ActiveUnitData.EnemyUnits[enemy.Unit.Tag].NearbyAllies)
                        {
                            if (!enemyGroups.Any(g => g.Any(e => e.Unit.Tag == nearbyEnemy.Unit.Tag)))
                            {
                                group.Add(nearbyEnemy);
                            }
                        }
                        enemyGroups.Add(group);
                    }
                }
            }
            return enemyGroups;
        }
    }
}
