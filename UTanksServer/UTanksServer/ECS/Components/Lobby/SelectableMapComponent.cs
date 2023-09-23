using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types.Battle;
using UTanksServer.ECS.Types.Battle.AtomicType;
using UTanksServer.ECS.Types.Battle.Bonus;
using UTanksServer.ECS.Types.Lobby;

namespace UTanksServer.ECS.Components.Lobby
{
    [TypeUid(214771413291007170)]
    public class SelectableMapComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public SelectableMapsType selectableMaps { get; set; }

        public SelectableMapComponent() { }
        public SelectableMapComponent(string jsonData)
        {
            SelectableMapComponent userGarageDBComponent;
            using (StringReader reader = new StringReader(jsonData))
            {
                JsonTextReader jreader = new JsonTextReader(reader);
                userGarageDBComponent = (SelectableMapComponent)GlobalCachingSerialization.standartSerializer.Deserialize(jreader, typeof(SelectableMapComponent));
                selectableMaps = userGarageDBComponent.selectableMaps;
                foreach(var mapGroup in selectableMaps.GameMaps)
                {
                    foreach(var mapVariant in mapGroup.Maps)
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(Map));
                        using (XmlReader xmlreader = XmlReader.Create(AppDomain.CurrentDomain.BaseDirectory + mapVariant.Path.Replace("\\", GlobalProgramState.PathSeparator)))
                        {
                            mapVariant.map = (Map)ser.Deserialize(xmlreader);
                        }
                        foreach (var spawnPoint in mapVariant.map.Spawnpoints.Spawnpoint)
                        {
                            List <WorldPoint> Spawnpoints;
                            if (mapVariant.SpawnPoints.TryGetValue(spawnPoint.Type, out Spawnpoints))
                            {
                                Spawnpoints.Add(XMLOperations.XMLPosRotToWorldPoint(spawnPoint.Position, spawnPoint.Rotation));
                            }
                            else
                            {
                                mapVariant.SpawnPoints[spawnPoint.Type] = new List<WorldPoint> { XMLOperations.XMLPosRotToWorldPoint(spawnPoint.Position, spawnPoint.Rotation) };
                            }
                        }
                        foreach (var bonusRegion in mapVariant.map.Bonusregions.Bonusregion)
                        {
                            foreach (var bonusType in bonusRegion.Bonustype)
                            {
                                List<BonusDropRegion> Spawnpoints;
                                var worldPos = XMLOperations.XMLPosRotToWorldPoint(bonusRegion.Position, bonusRegion.Rotation);
                                #region dropPeriods
                                double droppingPeriod = 0;
                                if (BonusMatching.Bonuses[bonusType] == BonusType.Armor ||
                                    BonusMatching.Bonuses[bonusType] == BonusType.Damage ||
                                    BonusMatching.Bonuses[bonusType] == BonusType.Repair ||
                                    BonusMatching.Bonuses[bonusType] == BonusType.Speed)
                                {
                                    droppingPeriod = 40f * 1000f * mapVariant.SupplyDropFrequencyScaling;
                                }
                                else if (BonusMatching.Bonuses[bonusType] == BonusType.TestBox)
                                {
                                    droppingPeriod = 40f * 1000f * mapVariant.SupplyDropFrequencyScaling;
                                }
                                else if (BonusMatching.Bonuses[bonusType] == BonusType.Crystal)
                                {
                                    droppingPeriod = 20f * 1000f;
                                }
                                else if (BonusMatching.Bonuses[bonusType] == BonusType.Container)
                                {
                                    droppingPeriod = 20f * 1000f;
                                }
                                else if (BonusMatching.Bonuses[bonusType] == BonusType.Gold ||
                                    BonusMatching.Bonuses[bonusType] == BonusType.Ruby ||
                                    BonusMatching.Bonuses[bonusType] == BonusType.SuperContainer)
                                {
                                    droppingPeriod = 20f * 1000f;
                                }
                                #endregion
                                if (mapVariant.DropSpawnPoints.TryGetValue(bonusType, out Spawnpoints))
                                {
                                    Spawnpoints.Add(new BonusDropRegion {
                                        BonusType = bonusType,
                                        Max = XMLOperations.XMLPosToVector(bonusRegion.Max),
                                        Min = XMLOperations.XMLPosToVector(bonusRegion.Min),
                                        Name = bonusRegion.Name,
                                        Parachute = bonusRegion.Parachute,
                                        worldPosition = worldPos,
                                        DroppingPeriod = droppingPeriod
                                    });
                                }
                                else
                                {
                                    
                                    mapVariant.DropSpawnPoints[bonusType] = new List<BonusDropRegion> 
                                    {new BonusDropRegion {
                                        BonusType = bonusType,
                                        Max = XMLOperations.XMLPosToVector(bonusRegion.Max),
                                        Min = XMLOperations.XMLPosToVector(bonusRegion.Min),
                                        Name = bonusRegion.Name,
                                        Parachute = bonusRegion.Parachute,
                                        worldPosition = worldPos,
                                        DroppingPeriod = droppingPeriod
                                    }};
                                }
                            }
                        }
                        mapVariant.GoalPositionPoints["ctfFlag"] = new Dictionary<string, List<WorldPoint>>();

                        if(mapVariant.map.Ctfflags != null)
                        {
                            mapVariant.GoalPositionPoints["ctfFlag"]["Flagblue"] = new List<WorldPoint> { new WorldPoint { Position = XMLOperations.XMLPosToVector(mapVariant.map.Ctfflags.Flagblue) } };
                            mapVariant.GoalPositionPoints["ctfFlag"]["Flagred"] = new List<WorldPoint> { new WorldPoint { Position = XMLOperations.XMLPosToVector(mapVariant.map.Ctfflags.Flagred) } };
                        }
                        
                        if(mapVariant.map.Domkeypoints != null)
                        {
                            foreach (var domPoint in mapVariant.map.Domkeypoints.Domkeypoint)
                            {
                                Dictionary<string, List<WorldPoint>> GoalPoint;
                                if (mapVariant.GoalPositionPoints.TryGetValue("domPoint", out GoalPoint))
                                {
                                    GoalPoint[domPoint.Name] = new List<WorldPoint> { new WorldPoint { Position = XMLOperations.XMLPosToVector(domPoint.Position) } };
                                }
                                else
                                {
                                    mapVariant.GoalPositionPoints["domPoint"] = new Dictionary<string, List<WorldPoint>>();
                                    mapVariant.GoalPositionPoints["domPoint"][domPoint.Name] = new List<WorldPoint> { new WorldPoint { Position = XMLOperations.XMLPosToVector(domPoint.Position) } };
                                }
                            }
                        }
                        
                    }
                }
            }
            
        }
    }
}
