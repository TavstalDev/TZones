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
    public class DatabaseManager : DatabaseManagerBase
    {
        public MySqlRepository<ulong, Flag> Flags { get; }
        
        public MySqlRepository<ulong, Zone> Zones { get; }
        
        public MySqlRepository<ulong, Node> Nodes { get; }
        
        public MySqlRepository<ulong, ZoneFlag> ZoneFlags { get; }
        
        public MySqlRepository<ulong, ZoneEvent> ZoneEvents { get; }
        
        public MySqlRepository<ulong, Restriction> Restrictions { get; }

        public DatabaseManager(IPlugin plugin, ZonesConfig config) : base(plugin, config.Database)
        {
            Flags = new MySqlRepository<ulong, Flag>(this, config.Database.TablePrefix);
            Zones = new MySqlRepository<ulong, Zone>(this, config.Database.TablePrefix);
            Nodes = new MySqlRepository<ulong, Node>(this, config.Database.TablePrefix);
            ZoneFlags = new MySqlRepository<ulong, ZoneFlag>(this, config.Database.TablePrefix);
            ZoneEvents = new MySqlRepository<ulong, ZoneEvent>(this, config.Database.TablePrefix);
            Restrictions = new MySqlRepository<ulong, Restriction>(this, config.Database.TablePrefix);
        }
        
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
                            new Flag(Constants.Flags.Damage, "Prevents barricade and structure damage.", "TZones"),
                            new Flag(Constants.Flags.VehicleDamage, "Prevents vehicle damage.", "TZones"),
                            new Flag(Constants.Flags.TireDamage, "Prevents tire damage.", "TZones"),
                            new Flag(Constants.Flags.PlayerDamage, "Prevents player damage.", "TZones"),
                            new Flag(Constants.Flags.AnimalDamage, "Prevents animal damage.", "TZones"),
                            new Flag(Constants.Flags.ZombieDamage, "Prevents zombie damage.", "TZones"),
                            new Flag(Constants.Flags.Lockpick, "Prevents lock picking.", "TZones"),
                            new Flag(Constants.Flags.Barricades, "Prevents placing barricades.", "TZones"),
                            new Flag(Constants.Flags.Structures, "Prevents placing structures.", "TZones"),
                            new Flag(Constants.Flags.ItemEquip, "Prevents equipping items.", "TZones"),
                            new Flag(Constants.Flags.ItemUnequip, "Prevents unequipping items.", "TZones"),
                            new Flag(Constants.Flags.ItemDrop, "Prevents dropping items.", "TZones"),
                            new Flag(Constants.Flags.Enter, "Prevents entering the zone.", "TZones"),
                            new Flag(Constants.Flags.Leave, "Prevents leaving the zone.", "TZones"),
                            new Flag(Constants.Flags.Zombie, "Prevents zombie spawning.", "TZones"),
                            new Flag(Constants.Flags.InfiniteGenerator, "Refuels generators.", "TZones"),
                            new Flag(Constants.Flags.VehicleCarjack, "Prevents carjacking vehicles", "TZones"),
                            new Flag(Constants.Flags.VehicleSiphoning, "Prevents siphoning vehicles", "TZones")
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
