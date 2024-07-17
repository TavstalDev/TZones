using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Compatibility.Database;
using Tavstal.TLibrary.Compatibility.Interfaces;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Managers;
using Tavstal.TZones.Models.Core;
using Tavstal.TZones.Utils.Constants;
using Tavstal.TZones.Models.Enums;

namespace Tavstal.TZones.Utils.Managers
{
    public class DatabaseManager : DatabaseManagerBase
    {
        private static TZonesConfig _pluginConfig => TZones.Instance.Config;

        public DatabaseManager(IPlugin plugin, IConfigurationBase config) : base(plugin, config)
        {

        }

        /// <summary>
        /// Checks the schema of the database, creates or modifies the tables if needed
        /// <br/>PS. If you change the Primary Key then you must delete the table.
        /// </summary>
        protected override async void CheckSchema()
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    if (!await connection.OpenSafeAsync())
                        TZones.IsConnectionAuthFailed = true;
                    if (connection.State != System.Data.ConnectionState.Open)
                        throw new Exception("# Failed to connect to the database. Please check the plugin's config file.");

                    // Flags
                    if (await connection.DoesTableExistAsync<Flag>(_pluginConfig.Database.TableFlags))
                        await connection.CheckTableAsync<Flag>(_pluginConfig.Database.TableFlags);
                    else
                    {
                        await connection.CreateTableAsync<Flag>(_pluginConfig.Database.TableFlags);

                        await connection.AddTableRowsAsync(_pluginConfig.Database.TableFlags, new List<Flag>() {
                            new Flag(Flags.NoDamage, "Prevents barricade and structure damage.", "TZones"),
                            new Flag(Flags.NoVehicleDamage, "Prevents vehicle damage.", "TZones"),
                            new Flag(Flags.NoTireDamage, "Prevents tire damage.", "TZones"),
                            new Flag(Flags.NoPlayerDamage, "Prevents player damage.", "TZones"),
                            new Flag(Flags.NoAnimalDamage, "Prevents animal damage.", "TZones"),
                            new Flag(Flags.NoZombieDamage, "Prevents zombie damage.", "TZones"),
                            new Flag(Flags.NoLockpick, "Prevents lockpicking.", "TZones"),
                            new Flag(Flags.NoBarricades, "Prevents placing barricades.", "TZones"),
                            new Flag(Flags.NoStructures, "Prevents placing structures.", "TZones"),
                            new Flag(Flags.NoItemEquip, "Prevents equiping items.", "TZones"),
                            new Flag(Flags.NoItemUnequip, "Prevents unequipping items.", "TZones"),
                            new Flag(Flags.NoItemDrop, "Prevents dropping items.", "TZones"),
                            new Flag(Flags.NoEnter, "Prevents entering the zone.", "TZones"),
                            new Flag(Flags.NoLeave, "Prevents leaving the zone.", "TZones"),
                            new Flag(Flags.NoZombie, "Prevents zombie spawning.", "TZones"),
                            new Flag(Flags.InfiniteGenerator, "Refuels generators.", "TZones"),
                            new Flag(Flags.NoVehicleCarjack, "Prevents carjacking vehicles", "TZones"),
                            new Flag(Flags.NoVehicleSiphoning, "Prevents siphoning vehicles", "TZones"),
                            new Flag(Flags.NoPvP, "Prevents PvP.", "TZones")
                        });
                    }


                    // Zones
                    if (await connection.DoesTableExistAsync<Zone>(_pluginConfig.Database.TableZones))
                        await connection.CheckTableAsync<Zone>(_pluginConfig.Database.TableZones);
                    else
                        await connection.CreateTableAsync<Zone>(_pluginConfig.Database.TableZones);

                    // Nodes
                    if (await connection.DoesTableExistAsync<Node>(_pluginConfig.Database.TableZoneNodes))
                        await connection.CheckTableAsync<Node>(_pluginConfig.Database.TableZoneNodes);
                    else
                        await connection.CreateTableAsync<Node>(_pluginConfig.Database.TableZoneNodes);

                    // Zone Flags
                    if (await connection.DoesTableExistAsync<ZoneFlag>(_pluginConfig.Database.TableZoneFlags))
                        await connection.CheckTableAsync<ZoneFlag>(_pluginConfig.Database.TableZoneFlags);
                    else
                        await connection.CreateTableAsync<ZoneFlag>(_pluginConfig.Database.TableZoneFlags);

                    // Zone Events
                    if (await connection.DoesTableExistAsync<ZoneEvent>(_pluginConfig.Database.TableZoneEvents))
                        await connection.CheckTableAsync<ZoneEvent>(_pluginConfig.Database.TableZoneEvents);
                    else
                        await connection.CreateTableAsync<ZoneEvent>(_pluginConfig.Database.TableZoneEvents);

                    // Block List
                    if (await connection.DoesTableExistAsync<Block>(_pluginConfig.Database.TableZoneBlocklist))
                        await connection.CheckTableAsync<Block>(_pluginConfig.Database.TableZoneBlocklist);
                    else
                        await connection.CreateTableAsync<Block>(_pluginConfig.Database.TableZoneBlocklist);

                    if (connection.State != System.Data.ConnectionState.Closed)
                        connection.Close();
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.LogException("Error in checkSchema:");
                TZones.Logger.LogError(ex);
            }
        }

        #region Flags
        public async Task AddFlagAsync(string name, string description, string flagRegister)
        {
            using (var connection = CreateConnection()) {
                try 
                {
                    await connection.AddTableRowAsync(_pluginConfig.Database.TableFlags, 
                        new Flag(name, description, flagRegister));
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in AddFlagAsync(): {ex}");    
                }
            }
        }

        public async Task RemoveFlagAsync(ulong id)
        {
            using (var connection = CreateConnection()) {
                try 
                {
                    await connection.RemoveTableRowAsync<Flag>(_pluginConfig.Database.TableFlags, $"Id=`{id}`", null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in RemoveFlagAsync(): {ex}");    
                }
            }
        }

        public async Task<List<Flag>> GetFlagsAsync(string whereClause)
        {
            List<Flag> flag = null;

            using (var connection = CreateConnection()) {
                try 
                {
                    flag = await connection.GetTableRowsAsync<Flag>(_pluginConfig.Database.TableFlags, whereClause, null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in GetFlagsAsync(): {ex}");    
                }
            }

            return flag;
        }

        public async Task<Flag> FindFlagAsync(string name)
        {
            Flag flag = null;

            using (var connection = CreateConnection()) {
                try 
                {
                    flag = await connection.GetTableRowAsync<Flag>(_pluginConfig.Database.TableFlags, $"Name=`{name}`", null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in FindFlagAsync(): {ex}");    
                }
            }

            return flag;
        }
        #endregion

        #region Zone Table
        public async Task AddZoneAsync(string name, string description, ulong creatorId)
        {
            using (var connection = CreateConnection()) {
                try 
                {
                    await connection.AddTableRowAsync(_pluginConfig.Database.TableZones, 
                        new Zone(name, description, creatorId, DateTime.Now));
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in AddZoneAsync(): {ex}");    
                }
            }
        }

        public async Task RemoveZoneAsync(ulong zoneId)
        {
            using (var connection = CreateConnection()) {
                try 
                {
                    await connection.RemoveTableRowAsync<Zone>(_pluginConfig.Database.TableZones, $"Id=`{zoneId}`", null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in RemoveZoneAsync(): {ex}");    
                }
            }
        }

        public async Task UpdateZoneAsync(ulong zoneId, string description)
        {
            using (var connection = CreateConnection()) 
            {
                try 
                {
                    await connection.UpdateTableRowAsync<Zone>(_pluginConfig.Database.TableZones, $"Id=`{zoneId}`", SqlParameter.Get<Zone>(x=> x.Description, description));
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in UpdateZoneAsync(): {ex}");    
                }
            }
        }

        public async Task<List<Zone>> GetZonesAsync(string whereClause)
        {
            List<Zone> zones = null;

            using (var connection = CreateConnection()) {
                try 
                {
                    zones = await connection.GetTableRowsAsync<Zone>(_pluginConfig.Database.TableZones, whereClause, null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in GetZonesAsync(): {ex}");    
                }
            }

            return zones;
        }

        public async Task<Zone> FindZoneAsync(string whereClause)
        {
            Zone zone = null;

            using (var connection = CreateConnection()) {
                try 
                {
                    zone = await connection.GetTableRowAsync<Zone>(_pluginConfig.Database.TableZones, whereClause, null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in FindZoneAsync(): {ex}");    
                }
            }

            return zone;
        }
        #endregion
    
        #region Zone Nodes
        public async Task AddNodeAsync(ulong zoneId, float x, float y, float z, bool isUpper)
        {
            using (var connection = CreateConnection()) {
                try 
                {
                    await connection.AddTableRowAsync(_pluginConfig.Database.TableZoneNodes, 
                        new Node(zoneId, x, y, z, isUpper));
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in AddNodeAsync(): {ex}");    
                }
            }
        }

        public async Task RemoveNodeAsync(Node node)
        {
            using (var connection = CreateConnection()) {
                try 
                {
                    await connection.RemoveTableRowAsync<Node>(_pluginConfig.Database.TableZoneNodes, $"ZoneId=`{node.ZoneId}` AND X=`{node.X}` AND Y=`{node.Y}` AND Z=`{node.Z}` AND IsUpper=`{node.IsUpper}`", null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in RemoveNodeAsync(): {ex}");    
                }
            }
        }

        public async Task<List<Node>> GetNodesAsync(string whereClause)
        {
            List<Node> nodes= null;

            using (var connection = CreateConnection()) {
                try 
                {
                    nodes = await connection.GetTableRowsAsync<Node>(_pluginConfig.Database.TableZoneNodes, whereClause, null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in GetNodesAsync(): {ex}");    
                }
            }

            return nodes;
        }

        public async Task<Node> FindNodeAsync(string whereClause)
        {
            Node node = null;

            using (var connection = CreateConnection()) {
                try 
                {
                    node = await connection.GetTableRowAsync<Node>(_pluginConfig.Database.TableZoneNodes, whereClause, null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in FindNodeAsync(): {ex}");    
                }
            }

            return node;
        }
        #endregion

        #region  Zone Flags
        public async Task AddZoneFlagAsync(ulong zoneId, ulong flagId)
        {
            using (var connection = CreateConnection()) {
                try 
                {
                    await connection.AddTableRowAsync(_pluginConfig.Database.TableZoneFlags, 
                        new ZoneFlag(zoneId, flagId));
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in AddZoneFlagAsync(): {ex}");    
                }
            }
        }

        public async Task RemoveZoneFlagAsync(ulong zoneId, ulong flagId)
        {
            using (var connection = CreateConnection()) {
                try 
                {
                    await connection.RemoveTableRowAsync<ZoneFlag>(_pluginConfig.Database.TableZoneFlags, $"ZoneId=`{zoneId}` AND FlagId=`{flagId}`", null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in RemoveZoneFlagAsync(): {ex}");    
                }
            }
        }

        public async Task<List<ZoneFlag>> GetZoneFlagsAsync(string whereClause)
        {
            List<ZoneFlag> zflags = null;

            using (var connection = CreateConnection()) {
                try 
                {
                    zflags = await connection.GetTableRowsAsync<ZoneFlag>(_pluginConfig.Database.TableZoneFlags, whereClause, null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in GetZoneFlagsAsync(): {ex}");    
                }
            }

            return zflags;
        }

        public async Task<ZoneFlag> FindZoneFlagAsync(string whereClause)
        {
            ZoneFlag zflag = null;

            using (var connection = CreateConnection()) {
                try 
                {
                    zflag = await connection.GetTableRowAsync<ZoneFlag>(_pluginConfig.Database.TableZoneFlags, whereClause, null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in FindZoneFlagAsync(): {ex}");    
                }
            }

            return zflag;
        }
        #endregion

        #region Zone Events
        public async Task AddZoneEventAsync(ulong zoneId, EEventType type, string value)
        {
            using (var connection = CreateConnection()) {
                try 
                {
                    await connection.AddTableRowAsync(_pluginConfig.Database.TableZoneEvents, 
                        new ZoneEvent(zoneId, type, value));
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in AddZoneEventAsync(): {ex}");    
                }
            }
        }

        public async Task RemoveZoneEventAsync(ulong zoneId, EEventType eventType)
        {
            using (var connection = CreateConnection()) {
                try 
                {
                    await connection.RemoveTableRowAsync<ZoneEvent>(_pluginConfig.Database.TableZoneEvents, $"ZoneId=`{zoneId}` AND Type=`{eventType}`", null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in RemoveZoneEventAsync(): {ex}");    
                }
            }
        }

        public async Task UpdateZoneEventAsync(ulong zoneId, EEventType eventType, string newValue)
        {
            using (var connection = CreateConnection()) 
            {
                try 
                {
                    await connection.UpdateTableRowAsync<ZoneEvent>(_pluginConfig.Database.TableZoneEvents, $"ZoneId=`{zoneId}` AND Type=`{eventType}`", 
                    SqlParameter.Get<ZoneEvent>(x => x.Value, newValue));
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in UpdateZoneEventAsync(): {ex}");    
                }
            }
        }

        public async Task<List<ZoneEvent>> GetZoneEventsAsync(string whereClause)
        {
            List<ZoneEvent> events = null;

            using (var connection = CreateConnection()) {
                try 
                {
                    events = await connection.GetTableRowsAsync<ZoneEvent>(_pluginConfig.Database.TableZoneEvents, whereClause, null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in GetZoneEventsAsync(): {ex}");    
                }
            }

            return events;
        }

        public async Task<ZoneEvent> FindZoneEventAsync(string whereClause)
        {
            ZoneEvent events = null;

            using (var connection = CreateConnection()) {
                try 
                {
                    events = await connection.GetTableRowAsync<ZoneEvent>(_pluginConfig.Database.TableZoneEvents, whereClause, null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in FindZoneEventAsync(): {ex}");    
                }
            }

            return events;
        }
        #endregion

        #region Zone Blocks
        public async Task AddBlockAsync(ulong zoneId, ushort unturnedId, EBlockType type)
        {
            using (var connection = CreateConnection()) {
                try 
                {
                    await connection.AddTableRowAsync(_pluginConfig.Database.TableZoneBlocklist, 
                        new Block(zoneId, unturnedId, type));
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in AddBlockAsync(): {ex}");    
                }
            }
        }

        public async Task RemoveBlockAsync(Block block)
        {
            using (var connection = CreateConnection()) {
                try 
                {
                    await connection.RemoveTableRowAsync<Block>(_pluginConfig.Database.TableZoneBlocklist, 
                    $"ZoneId=`{block.ZoneId}` AND UnturnedId=`{block.UnturnedId}` AND Type=`{block.Type}`", null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in RemoveBlockAsync(): {ex}");    
                }
            }
        }

        public async Task<List<Block>> GetBlocksAsync(string whereClause)
        {
            List<Block> blocks = null;

            using (var connection = CreateConnection()) {
                try 
                {
                    blocks = await connection.GetTableRowsAsync<Block>(_pluginConfig.Database.TableZoneBlocklist, whereClause, null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in GetBlocksAsync(): {ex}");    
                }
            }

            return blocks;
        }

        public async Task<Block> FindBlockAsync(string whereClause)
        {
            Block blocks = null;

            using (var connection = CreateConnection()) {
                try 
                {
                    blocks = await connection.GetTableRowAsync<Block>(_pluginConfig.Database.TableZoneBlocklist, whereClause, null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in FindBlockAsync(): {ex}");    
                }
            }

            return blocks;
        }
        #endregion
    }
}
