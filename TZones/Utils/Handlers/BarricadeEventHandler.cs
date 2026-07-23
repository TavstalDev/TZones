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
    public static class BarricadeEventHandler
    {
        private static bool _isAttached;

        public static void AttachEvents()
        {
            if (_isAttached)
                return;

            BarricadeManager.onDamageBarricadeRequested += OnDamageBarricadeRequested;
            BarricadeManager.onDeployBarricadeRequested += OnDeployBarricadeRequested;
            BarricadeDrop.OnSalvageRequested_Global += OnSalvageBarricadeRequested;
            
            _isAttached = true;
        }

        public static void DetachEvents()
        {
            if (!_isAttached)
                return;
            
            BarricadeManager.onDamageBarricadeRequested -= OnDamageBarricadeRequested;
            BarricadeManager.onDeployBarricadeRequested -= OnDeployBarricadeRequested;
            BarricadeDrop.OnSalvageRequested_Global -= OnSalvageBarricadeRequested;

            _isAttached = true;
        }
        
        private static void OnDeployBarricadeRequested(Barricade barricade, ItemBarricadeAsset asset, Transform hit, ref Vector3 point, ref float angleX, ref float angleY, ref float angleZ, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID((CSteamID)owner);
                if (uPlayer == null)
                    return;

                ZonePlayerComponent comp = uPlayer.GetComponent<ZonePlayerComponent>();

                foreach (var zone in comp.Zones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.Barricades) ||
                        ZoneManager.Queries.IsBlocked(zone, asset.id, ERestrictionType.BUILD))
                    {
                        shouldAllow = false;
                        break;
                    }
                }

                var objectZones = ZoneManager.GetZonesFromPosition(point);
                foreach (Zone zone in objectZones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.Barricades) || ZoneManager.Queries.IsBlocked(zone, asset.id, ERestrictionType.BUILD))
                    {
                        shouldAllow = false;
                        break;
                    }
                }

                if (shouldAllow && asset.build == EBuild.GENERATOR)
                    ZoneManager.Cache.RefreshGeneratorCache();
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDeployBarricadeRequested)}.", ex);
                shouldAllow = true;
            }
        }
        
        private static void OnSalvageBarricadeRequested(BarricadeDrop barricade, SteamPlayer instigatorClient, ref bool shouldAllow)
        {
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(instigatorClient);
                if (uPlayer == null)
                    return;

                ZonePlayerComponent comp = uPlayer.GetComponent<ZonePlayerComponent>();

                foreach (var zone in comp.Zones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.BarricadeSalvage))
                    {
                        shouldAllow = false;
                        break;
                    }
                }

                var objectZones = ZoneManager.GetZonesFromPosition(barricade.interactable.transform.position);
                foreach (Zone zone in objectZones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.BarricadeSalvage))
                    {
                        shouldAllow = false;
                        break;
                    }
                }

                if (shouldAllow && barricade.asset.build == EBuild.GENERATOR)
                    ZoneManager.Cache.RefreshGeneratorCache();
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnSalvageBarricadeRequested)}.", ex);
                shouldAllow = true;
            }
        }
        
        private static void OnDamageBarricadeRequested(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
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

                var objectZones = ZoneManager.GetZonesFromPosition(barricadeTransform.position);
                foreach (Zone zone in objectZones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.Damage))
                    {
                        shouldAllow = false;
                        break;
                    }
                }

                var barricade = BarricadeManager.FindBarricadeByRootTransform(barricadeTransform);
                if (shouldAllow && barricade.GetServersideData().barricade.health - pendingTotalDamage <= 0 &&
                    barricade.asset.build == EBuild.GENERATOR)
                    ZoneManager.Cache.RefreshGeneratorCache();
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDamageBarricadeRequested)}.", ex);
                shouldAllow = true;
            }
        }
    }
}