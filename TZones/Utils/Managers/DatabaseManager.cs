using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tavstal.TLibrary.Models.Database;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Managers;
using Tavstal.TZones.Models.Core;
using Tavstal.TZones.Utils.Constants;
using Tavstal.TZones.Models.Enums;

namespace Tavstal.TZones.Utils.Managers
{
    /// <summary>
    /// Manages database operations, extending the functionality of <see cref="DatabaseManagerBase"/>.
    /// </summary>
    /// <remarks>
    /// This class is responsible for interacting with the database, including CRUD operations and other database-related tasks.
    /// </remarks>
    public class DatabaseManager : DatabaseManagerBase
    {
        // ReSharper disable once InconsistentNaming
        private static ZonesConfig _pluginConfig => TZones.Instance.Config;

        public DatabaseManager(IPlugin plugin, IConfigurationBase config) : base(plugin, config)
        {

        }

        /// <summary>
        /// Checks the schema of the database, creates or modifies the tables if needed
        /// <br/>PS. If you change the Primary Key then you must delete the table.
        /// </summary>
        public override async Task CheckSchemaAsync()
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
                    if (!await connection.DoesTableExistAsync<Flag>(_pluginConfig.Database.TableFlags))
                    {
                        await connection.CreateTableAsync<Flag>(_pluginConfig.Database.TableFlags);
                        await connection.AddTableRowsAsync(_pluginConfig.Database.TableFlags, new List<Flag>()
                        {
                            new Flag(Flags.Damage, "Prevents barricade and structure damage.", "TZones"),
                            new Flag(Flags.VehicleDamage, "Prevents vehicle damage.", "TZones"),
                            new Flag(Flags.TireDamage, "Prevents tire damage.", "TZones"),
                            new Flag(Flags.PlayerDamage, "Prevents player damage.", "TZones"),
                            new Flag(Flags.AnimalDamage, "Prevents animal damage.", "TZones"),
                            new Flag(Flags.ZombieDamage, "Prevents zombie damage.", "TZones"),
                            new Flag(Flags.Lockpick, "Prevents lock picking.", "TZones"),
                            new Flag(Flags.Barricades, "Prevents placing barricades.", "TZones"),
                            new Flag(Flags.Structures, "Prevents placing structures.", "TZones"),
                            new Flag(Flags.ItemEquip, "Prevents equipping items.", "TZones"),
                            new Flag(Flags.ItemUnequip, "Prevents unequipping items.", "TZones"),
                            new Flag(Flags.ItemDrop, "Prevents dropping items.", "TZones"),
                            new Flag(Flags.Enter, "Prevents entering the zone.", "TZones"),
                            new Flag(Flags.Leave, "Prevents leaving the zone.", "TZones"),
                            new Flag(Flags.Zombie, "Prevents zombie spawning.", "TZones"),
                            new Flag(Flags.InfiniteGenerator, "Refuels generators.", "TZones"),
                            new Flag(Flags.VehicleCarjack, "Prevents carjacking vehicles", "TZones"),
                            new Flag(Flags.VehicleSiphoning, "Prevents siphoning vehicles", "TZones")
                        });
                    }
                    else
                        await connection.CheckTableAsync<Flag>(_pluginConfig.Database.TableFlags);


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
                        await connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.LogException("Error in checkSchema:");
                TZones.Logger.LogError(ex);
            }
        }

        #region Flags
        /// <summary>
        /// Asynchronously adds a new flag with the specified name, description, and registration details.
        /// </summary>
        /// <param name="name">The name of the flag to be added.</param>
        /// <param name="description">A description of the flag.</param>
        /// <param name="flagRegister">The registration details for the flag.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Asynchronously removes the flag with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the flag to be removed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RemoveFlagAsync(ulong id)
        {
            using (var connection = CreateConnection()) {
                try 
                {
                    await connection.RemoveTableRowAsync<Flag>(_pluginConfig.Database.TableFlags, $"Id='{id}'", null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in RemoveFlagAsync(): {ex}");    
                }
            }
        }

        /// <summary>
        /// Asynchronously retrieves a list of flags based on the specified query condition.
        /// </summary>
        /// <param name="whereClause">The condition used to filter the flags (e.g., SQL WHERE clause or custom filter).</param>
        /// <returns>A task representing the asynchronous operation, with a list of <see cref="Flag"/> objects matching the condition.</returns>
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

        /// <summary>
        /// Asynchronously finds a flag based on the specified name.
        /// </summary>
        /// <param name="name">The name of the flag to find.</param>
        /// <returns>A task representing the asynchronous operation, with the <see cref="Flag"/> object matching the name, or null if not found.</returns>
        public async Task<Flag> FindFlagAsync(string name)
        {
            Flag flag = null;

            using (var connection = CreateConnection()) {
                try 
                {
                    flag = await connection.GetTableRowAsync<Flag>(_pluginConfig.Database.TableFlags, $"Name='{name}'", null);
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
        /// <summary>
        /// Asynchronously adds a new zone with the specified name, description, and creator ID.
        /// </summary>
        /// <param name="name">The name of the zone to be added.</param>
        /// <param name="description">A description of the zone.</param>
        /// <param name="creatorId">The ID of the creator of the zone.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Asynchronously removes the zone with the specified ID.
        /// </summary>
        /// <param name="zoneId">The ID of the zone to be removed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RemoveZoneAsync(ulong zoneId)
        {
            using (var connection = CreateConnection()) {
                try 
                {
                    await connection.RemoveTableRowAsync<Zone>(_pluginConfig.Database.TableZones, $"Id='{zoneId}'", null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in RemoveZoneAsync(): {ex}");    
                }
            }
        }

        /// <summary>
        /// Asynchronously updates the description of the zone with the specified ID.
        /// </summary>
        /// <param name="zoneId">The ID of the zone to be updated.</param>
        /// <param name="description">The new description for the zone.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateZoneAsync(ulong zoneId, string description)
        {
            using (var connection = CreateConnection()) 
            {
                try 
                {
                    await connection.UpdateTableRowAsync<Zone>(_pluginConfig.Database.TableZones, $"Id='{zoneId}'", SqlParameter.Get<Zone>(x=> x.Description, description));
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in UpdateZoneAsync(): {ex}");    
                }
            }
        }

        /// <summary>
        /// Asynchronously retrieves a list of zones based on the specified query condition.
        /// </summary>
        /// <param name="whereClause">The condition used to filter the zones (e.g., SQL WHERE clause or custom filter).</param>
        /// <returns>A task representing the asynchronous operation, with a list of <see cref="Zone"/> objects matching the condition.</returns>
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

        /// <summary>
        /// Asynchronously finds a zone based on the specified query condition.
        /// </summary>
        /// <param name="whereClause">The condition used to filter the zone (e.g., SQL WHERE clause or custom filter).</param>
        /// <returns>A task representing the asynchronous operation, with the <see cref="Zone"/> object matching the condition, or null if not found.</returns>
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
        /// <summary>
        /// Asynchronously adds a new node to the specified zone with the given coordinates and node type.
        /// </summary>
        /// <param name="zoneId">The ID of the zone to which the node will be added.</param>
        /// <param name="x">The X-coordinate of the node.</param>
        /// <param name="y">The Y-coordinate of the node.</param>
        /// <param name="z">The Z-coordinate of the node.</param>
        /// <param name="type">The type of the node to be added (e.g., <see cref="ENodeType"/>).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddNodeAsync(ulong zoneId, float x, float y, float z, ENodeType type)
        {
            using (var connection = CreateConnection()) {
                try 
                {
                    await connection.AddTableRowAsync(_pluginConfig.Database.TableZoneNodes, 
                        new Node(zoneId, x, y, z, type));
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in AddNodeAsync(): {ex}");    
                }
            }
        }

        /// <summary>
        /// Asynchronously removes the specified node.
        /// </summary>
        /// <param name="node">The node to be removed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RemoveNodeAsync(Node node)
        {
            using (var connection = CreateConnection()) {
                try 
                {
                    await connection.RemoveTableRowAsync<Node>(_pluginConfig.Database.TableZoneNodes, $"ZoneId='{node.ZoneId}' AND X='{node.X}' AND Y='{node.Y}' AND Z='{node.Z}' AND Type='{node.Type}'", null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in RemoveNodeAsync(): {ex}");    
                }
            }
        }

        /// <summary>
        /// Asynchronously retrieves a list of nodes based on the specified query condition.
        /// </summary>
        /// <param name="whereClause">The condition used to filter the nodes (e.g., SQL WHERE clause or custom filter).</param>
        /// <returns>A task representing the asynchronous operation, with a list of <see cref="Node"/> objects matching the condition.</returns>
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

        /// <summary>
        /// Asynchronously finds a node based on the specified query condition.
        /// </summary>
        /// <param name="whereClause">The condition used to filter the node (e.g., SQL WHERE clause or custom filter).</param>
        /// <returns>A task representing the asynchronous operation, with the <see cref="Node"/> object matching the condition, or null if not found.</returns>
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
        /// <summary>
        /// Asynchronously adds a flag to the specified zone.
        /// </summary>
        /// <param name="zoneId">The ID of the zone to which the flag will be added.</param>
        /// <param name="flagId">The ID of the flag to be added to the zone.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Asynchronously removes a flag from the specified zone.
        /// </summary>
        /// <param name="zoneId">The ID of the zone from which the flag will be removed.</param>
        /// <param name="flagId">The ID of the flag to be removed from the zone.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RemoveZoneFlagAsync(ulong zoneId, ulong flagId)
        {
            using (var connection = CreateConnection()) {
                try 
                {
                    await connection.RemoveTableRowAsync<ZoneFlag>(_pluginConfig.Database.TableZoneFlags, $"ZoneId='{zoneId}' AND FlagId='{flagId}'", null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in RemoveZoneFlagAsync(): {ex}");    
                }
            }
        }

        /// <summary>
        /// Asynchronously retrieves a list of zone flags based on the specified query condition.
        /// </summary>
        /// <param name="whereClause">The condition used to filter the zone flags (e.g., SQL WHERE clause or custom filter).</param>
        /// <returns>A task representing the asynchronous operation, with a list of <see cref="ZoneFlag"/> objects matching the condition.</returns>
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

        /// <summary>
        /// Asynchronously finds a zone flag based on the specified query condition.
        /// </summary>
        /// <param name="whereClause">The condition used to filter the zone flag (e.g., SQL WHERE clause or custom filter).</param>
        /// <returns>A task representing the asynchronous operation, with the <see cref="ZoneFlag"/> object matching the condition, or null if not found.</returns>
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
        /// <summary>
        /// Asynchronously adds an event to the specified zone with the given event type and value.
        /// </summary>
        /// <param name="zoneId">The ID of the zone to which the event will be added.</param>
        /// <param name="type">The type of event to be added (e.g., <see cref="EEventType"/>).</param>
        /// <param name="value">The value associated with the event (e.g., event description or data).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Asynchronously removes an event of the specified type from the specified zone.
        /// </summary>
        /// <param name="zoneId">The ID of the zone from which the event will be removed.</param>
        /// <param name="eventType">The type of event to be removed (e.g., <see cref="EEventType"/>).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RemoveZoneEventAsync(ulong zoneId, EEventType eventType)
        {
            using (var connection = CreateConnection()) {
                try 
                {
                    await connection.RemoveTableRowAsync<ZoneEvent>(_pluginConfig.Database.TableZoneEvents, $"ZoneId='{zoneId}' AND Type='{eventType}'", null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in RemoveZoneEventAsync(): {ex}");    
                }
            }
        }

        /// <summary>
        /// Asynchronously updates the value of an existing event in the specified zone.
        /// </summary>
        /// <param name="zoneId">The ID of the zone containing the event to be updated.</param>
        /// <param name="eventType">The type of event to be updated (e.g., <see cref="EEventType"/>).</param>
        /// <param name="newValue">The new value to update the event with.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateZoneEventAsync(ulong zoneId, EEventType eventType, string newValue)
        {
            using (var connection = CreateConnection()) 
            {
                try 
                {
                    await connection.UpdateTableRowAsync<ZoneEvent>(_pluginConfig.Database.TableZoneEvents, $"ZoneId='{zoneId}' AND Type='{eventType}'", 
                    SqlParameter.Get<ZoneEvent>(x => x.Value, newValue));
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in UpdateZoneEventAsync(): {ex}");    
                }
            }
        }

        /// <summary>
        /// Asynchronously retrieves a list of zone events based on the specified query condition.
        /// </summary>
        /// <param name="whereClause">The condition used to filter the zone events (e.g., SQL WHERE clause or custom filter).</param>
        /// <returns>A task representing the asynchronous operation, with a list of <see cref="ZoneEvent"/> objects matching the condition.</returns>
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

        /// <summary>
        /// Asynchronously finds a zone event based on the specified query condition.
        /// </summary>
        /// <param name="whereClause">The condition used to filter the zone event (e.g., SQL WHERE clause or custom filter).</param>
        /// <returns>A task representing the asynchronous operation, with the <see cref="ZoneEvent"/> object matching the condition, or null if not found.</returns>
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
        /// <summary>
        /// Asynchronously adds a block to the specified zone with the given Unturned ID and block type.
        /// </summary>
        /// <param name="zoneId">The ID of the zone to which the block will be added.</param>
        /// <param name="unturnedId">The Unturned ID representing the specific block to be added.</param>
        /// <param name="type">The type of block to be added (e.g., <see cref="EBlockType"/>).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Asynchronously removes the specified block.
        /// </summary>
        /// <param name="block">The block to be removed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RemoveBlockAsync(Block block)
        {
            using (var connection = CreateConnection()) {
                try 
                {
                    await connection.RemoveTableRowAsync<Block>(_pluginConfig.Database.TableZoneBlocklist, 
                    $"ZoneId='{block.ZoneId}' AND UnturnedId='{block.UnturnedId}' AND Type='{block.Type}'", null);
                }
                catch (Exception ex) 
                {
                    TZones.Logger.LogException($"Error in RemoveBlockAsync(): {ex}");    
                }
            }
        }

        /// <summary>
        /// Asynchronously retrieves a list of blocks based on the specified query condition.
        /// </summary>
        /// <param name="whereClause">The condition used to filter the blocks (e.g., SQL WHERE clause or custom filter).</param>
        /// <returns>A task representing the asynchronous operation, with a list of <see cref="Block"/> objects matching the condition.</returns>
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

        /// <summary>
        /// Asynchronously finds a block based on the specified query condition.
        /// </summary>
        /// <param name="whereClause">The condition used to filter the block (e.g., SQL WHERE clause or custom filter).</param>
        /// <returns>A task representing the asynchronous operation, with the <see cref="Block"/> object matching the condition, or null if not found.</returns>
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
