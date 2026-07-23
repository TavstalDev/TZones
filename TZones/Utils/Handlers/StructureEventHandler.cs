using System;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using Tavstal.TLibrary.Extensions;
using Tavstal.TZones.Components;
using Tavstal.TZones.Models.Core;
using Tavstal.TZones.Models.Enums;
using Tavstal.TZones.Utils.Constants;
using Tavstal.TZones.Utils.Managers;
using UnityEngine;

namespace Tavstal.TZones.Utils.Handlers
{
    public static class StructureEventHandler
    {
        private static bool _isAttached;

        public static void AttachEvents()
        {
            if (_isAttached)
                return;
            
            StructureManager.onDamageStructureRequested += OnDamageStructureRequested;
            StructureManager.onDeployStructureRequested += OnDeployStructureRequested;
            StructureDrop.OnSalvageRequested_Global += OnSalvageStructureRequested;

            _isAttached = true;
        }

        public static void DetachEvents()
        {
            if (!_isAttached)
                return;
            
            StructureManager.onDamageStructureRequested -= OnDamageStructureRequested;
            StructureManager.onDeployStructureRequested -= OnDeployStructureRequested;
            StructureDrop.OnSalvageRequested_Global -= OnSalvageStructureRequested;

            _isAttached = true;
        }
        
        private static void OnDeployStructureRequested(Structure structure, ItemStructureAsset asset, ref Vector3 point, ref float angleX, ref float angleY, ref float angleZ, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID((CSteamID)owner);
                if (uPlayer == null)
                    return;

                ZonePlayerComponent comp = uPlayer.GetComponent<ZonePlayerComponent>();
                foreach (var zone in comp.Zones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.Structures) ||
                        ZoneManager.Queries.IsBlocked(zone, asset.id, ERestrictionType.BUILD))
                    {
                        shouldAllow = false;
                        break;
                    }
                }

                var objectZones = ZoneManager.GetZonesFromPosition(point);
                foreach (Zone zone in objectZones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.Structures) || 
                        ZoneManager.Queries.IsBlocked(zone, asset.id, ERestrictionType.BUILD))
                    {
                        shouldAllow = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDeployStructureRequested)}.", ex);
                shouldAllow = true;
            }
        }
        
        private static void OnSalvageStructureRequested(StructureDrop structure, SteamPlayer instigatorClient, ref bool shouldAllow)
        {
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(instigatorClient);
                if (uPlayer == null)
                    return;

                ZonePlayerComponent comp = uPlayer.GetComponent<ZonePlayerComponent>();

                foreach (var zone in comp.Zones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.StructureSalvage))
                    {
                        shouldAllow = false;
                        break;
                    }
                }

                var objectZones = ZoneManager.GetZonesFromPosition(structure.GetServersideData().point);
                foreach (Zone zone in objectZones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.StructureSalvage))
                    {
                        shouldAllow = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnSalvageStructureRequested)}.", ex);
                shouldAllow = true;
            }
        }
        
        private static void OnDamageStructureRequested(CSteamID instigatorSteamID, Transform structureTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(instigatorSteamID);
                if (uPlayer == null)
                    return;

                ZonePlayerComponent comp = uPlayer.GetComponent<ZonePlayerComponent>();
                foreach (var zone in comp.Zones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.Damage))
                    {
                        shouldAllow = false;
                        break;
                    }
                }

                var objectZones = ZoneManager.GetZonesFromPosition(structureTransform.position);
                foreach (Zone zone in objectZones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.Damage))
                    {
                        shouldAllow = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDamageStructureRequested)}.", ex);
                shouldAllow = true;
            }
        }
    }
}