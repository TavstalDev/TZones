using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Extensions.Database;
using Tavstal.TLibrary.Managers;
using Tavstal.TLibrary.Models.Database;
using Tavstal.TZones.Models.Core;

namespace Tavstal.TZones.Utils.Managers
{
    /// <summary>
    /// Manages all database operations and provides typed repositories for each data model.
    /// </summary>
    public class DatabaseManager : DatabaseManagerBase
    {
        /// <summary>
        /// Repository for flag data.
        /// </summary>
        public MySqlRepository<ulong, Flag> Flags { get; }
        
        /// <summary>
        /// Repository for zone data.
        /// </summary>
        public MySqlRepository<ulong, Zone> Zones { get; }
        
        /// <summary>
        /// Repository for node data.
        /// </summary>
        public MySqlRepository<ulong, Node> Nodes { get; }
        
        /// <summary>
        /// Repository for zone-flag association data.
        /// </summary>
        public MySqlRepository<ulong, ZoneFlag> ZoneFlags { get; }
        
        /// <summary>
        /// Repository for zone event data.
        /// </summary>
        public MySqlRepository<ulong, ZoneEvent> ZoneEvents { get; }
        
        /// <summary>
        /// Repository for restriction data.
        /// </summary>
        public MySqlRepository<ulong, Restriction> Restrictions { get; }

        /// <summary>
        /// Initializes the database manager and its repositories.
        /// </summary>
        /// <param name="plugin">The plugin instance.</param>
        /// <param name="config">The plugin configuration containing database settings.</param>
        public DatabaseManager(IPlugin plugin, ZonesConfig config) : base(plugin, config.Database)
        {
            Flags = new MySqlRepository<ulong, Flag>(this, config.Database.TablePrefix);
            Zones = new MySqlRepository<ulong, Zone>(this, config.Database.TablePrefix);
            Nodes = new MySqlRepository<ulong, Node>(this, config.Database.TablePrefix);
            ZoneFlags = new MySqlRepository<ulong, ZoneFlag>(this, config.Database.TablePrefix);
            ZoneEvents = new MySqlRepository<ulong, ZoneEvent>(this, config.Database.TablePrefix);
            Restrictions = new MySqlRepository<ulong, Restriction>(this, config.Database.TablePrefix);
        }
        
        /// <summary>
        /// Verifies and creates the database schema, and seeds default flags if none exist.
        /// </summary>
        public override async Task CheckSchemaAsync()
        {
            try
            {
                await using var connection = CreateConnection();
                if (connection == null)
                {
                    TZones.Logger.Error("Could not connect to database.");
                    return;
                }

                var state = await connection.OpenSafeAsync();
                if (state != EDatabaseState.SUCCESS)
                {
                    IsAuthenticationFailed = state == EDatabaseState.AUTHENTICATION_FAILED;
                    return;
                }
                
                await using var mySqlTransaction = connection.BeginTransaction();
                try
                {
                    await Flags.CheckSchemaAsync(connection, mySqlTransaction);
                    await Zones.CheckSchemaAsync(connection, mySqlTransaction);
                    await Nodes.CheckSchemaAsync(connection, mySqlTransaction);
                    await ZoneFlags.CheckSchemaAsync(connection, mySqlTransaction);
                    await ZoneEvents.CheckSchemaAsync(connection, mySqlTransaction);
                    await Restrictions.CheckSchemaAsync(connection, mySqlTransaction);
                    await mySqlTransaction.CommitAsync();

                    var flags = await Flags.GetAsync(queryParameters: QueryParameter.not("Id", "0"));
                    if (flags == null || flags.Count == 0)
                    {
                        await Flags.AddRangeAsync(new List<Flag>
                        {
                            new Flag(Constants.Flags.NoDamage, "Prevents barricade and structure damage.", "TZones"),
                            new Flag(Constants.Flags.NoVehicleDamage, "Prevents vehicle damage.", "TZones"),
                            new Flag(Constants.Flags.NoTireDamage, "Prevents tire damage.", "TZones"),
                            new Flag(Constants.Flags.AllowPlayerDamage, "Allows player damage.", "TZones"),
                            new Flag(Constants.Flags.NoPlayerDamage, "Prevents player damage.", "TZones"),
                            new Flag(Constants.Flags.NoAnimalDamage, "Prevents animal damage.", "TZones"),
                            new Flag(Constants.Flags.NoZombieDamage, "Prevents zombie damage.", "TZones"),
                            new Flag(Constants.Flags.NoLockpick, "Prevents lock picking.", "TZones"),
                            new Flag(Constants.Flags.NoBarricades, "Prevents placing barricades.", "TZones"),
                            new Flag(Constants.Flags.NoStructures, "Prevents placing structures.", "TZones"),
                            new Flag(Constants.Flags.NoItemEquip, "Prevents equipping items.", "TZones"),
                            new Flag(Constants.Flags.NoItemUnequip, "Prevents unequipping items.", "TZones"),
                            new Flag(Constants.Flags.NoItemDrop, "Prevents dropping items.", "TZones"),
                            new Flag(Constants.Flags.NoEnter, "Prevents entering the zone.", "TZones"),
                            new Flag(Constants.Flags.NoLeave, "Prevents leaving the zone.", "TZones"),
                            new Flag(Constants.Flags.NoZombie, "Prevents zombie spawning.", "TZones"),
                            new Flag(Constants.Flags.InfiniteGenerator, "Refuels generators.", "TZones"),
                            new Flag(Constants.Flags.NoVehicleCarjack, "Prevents carjacking vehicles", "TZones"),
                            new Flag(Constants.Flags.NoVehicleSiphoning, "Prevents siphoning vehicles", "TZones")
                        });
                    }
                }
                catch (Exception)
                {
                    await mySqlTransaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error("Error in checkSchema:", ex);
            }
        }
    }
}
