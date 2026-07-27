# TZones

![Release (latest by date)](https://img.shields.io/github/v/release/TavstalDev/TZones?style=plastic-square)
![Workflow Status](https://img.shields.io/github/actions/workflow/status/TavstalDev/TZones/release.yml?branch=stable&label=build&style=plastic-square)
![License](https://img.shields.io/github/license/TavstalDev/TZones?style=plastic-square)
![Downloads](https://img.shields.io/github/downloads/TavstalDev/TZones/total?style=plastic-square)
![Issues](https://img.shields.io/github/issues/TavstalDev/TZones?style=plastic-square)

### What is this?
This is the source code of a .NETFramework library written in C#. This library is a plugin made for Unturned 3.24.x+ servers. 

### Description
A basic zones plugin with database support.

## Requirements

- Unturned 3.24.x or later
- [RocketMod](https://rocketmod.net/) installed on the server

## Installation

1. Download the latest release and its libraries from the [Releases](https://github.com/TavstalDev/TZones/releases) page.
2. Place `TZones.dll` into your server's `Rocket/Plugins/` directory.
3. Extract the libraries archive into `Rocket/Libraries` directory.
4. Start or restart the server. The plugin will generate a default YAML configuration file on first load.
5. Edit the configuration file to your liking, then reload the plugin or restart the server.

### Commands
| - means <b>or</b></br>
[] - means <b>required</b></br>
<> - means <b>optional</b>

<details>
<summary>/flags add [name] [description]</summary>
<b>Description:</b>
<br>
<b>Permission(s):</b> tzones.command.flags, tzones.command.flags.add
</details>

<details>
<summary>/flags list <page></summary>
<b>Description:</b>
<br>
<b>Permission(s):</b> tzones.command.flags, tzones.command.flags.list
</details>

<details>
<summary>/flags remove [name]</summary>
<b>Description:</b>
<br>
<b>Permission(s):</b> tzones.command.flags, tzones.command.flags.remove
</details>

<details>
<summary>/zones add [zone | node | flag | event | block]</summary>
<b>Description:</b>
<br>
<b>Permission(s):</b> tzones.command.zones, tzones.command.zones.add
</details>

<details>
<summary>/zones list [zone] <page></summary>
<b>Description:</b>
<br>
<b>Permission(s):</b> tzones.command.zones, tzones.command.zones.list
</details>

<details>
<summary>/zones list [node | flag | event | block] [zoneName] <page></summary>
<b>Description:</b>
<br>
<b>Permission(s):</b> tzones.command.zones, tzones.command.zones.list
</details>

<details>
<summary>/zones remove [zone | node | flag | event | block]</summary>
<b>Description:</b>
<br>
<b>Permission(s):</b> tzones.command.zones, tzones.command.zones.remove
</details>

## Default Available Flags

| Flag                 | Description                                    |
|----------------------|------------------------------------------------|
| `NoDamage`           | No damage is allowed                           |
| `NoVehicleDamage`    | No vehicle damage is allowed                   |
| `AllowPlayerDamage`  | Allows player damage. (Only works in PvE mode) |
| `NoPlayerDamage`     | No player damage is allowed                    |
| `NoAnimalDamage`     | No animal damage is allowed                    |
| `NoZombieDamage`     | No zombie damage is allowed                    |
| `NoTireDamage`       | No tire damage is allowed                      |
| `NoLockpick`         | No lockpicking is allowed                      |
| `NoBarricades`       | No barricades can be placed                    |
| `NoStructures`       | No structures can be placed                    |
| `NoBarricadeSalvage` | No barricades can be salvaged                  |
| `NoStructureSalvage` | No structures can be salvaged                  |
| `NoItemEquip`        | No items can be equipped                       |
| `NoItemUnequip`      | No items can be unequipped                     |
| `NoItemDrop`         | No items can be dropped                        |
| `NoEnter`            | No entry is allowed into the zone              |
| `NoLeave`            | No exit is allowed from the zone               |
| `NoZombie`           | Zombies are not allowed within the zone        |
| `InfiniteGenerator`  | The generator in the zone is infinite          |
| `NoVehicleCarjack`   | Vehicle carjacking is not allowed              |
| `NoVehicleSiphoning` | Vehicle siphoning is not allowed               |

## Event Types

| Event                 | Description                                                |
|-----------------------|------------------------------------------------------------|
| `MESSAGE_ENTER`       | Triggered when a player enters the zone                    |
| `MESSAGE_LEAVE`       | Triggered when a player leaves the zone                    |
| `ADD_GROUP_ENTER`     | Adds a group to the player when they enter the zone        |
| `ADD_GROUP_LEAVE`     | Adds a group to the player when they leave the zone        |
| `REMOVE_GROUP_ENTER`  | Removes a group from the player when they enter the zone   |
| `REMOVE_GROUP_LEAVE`  | Removes a group from the player when they leave the zone   |
| `ADD_EFFECT_ENTER`    | Adds an effect to the player when they enter the zone      |
| `ADD_EFFECT_LEAVE`    | Adds an effect to the player when they leave the zone      |
| `REMOVE_EFFECT_ENTER` | Removes an effect from the player when they enter the zone |
| `REMOVE_EFFECT_LEAVE` | Removes an effect from the player when they leave the zone |

## Node Types

| Node     | Description           |
|----------|-----------------------|
| `NONE`   | No specific node type |
| `UPPER`  | Upper bound node      |
| `LOWER`  | Lower bound node      |

## Restriction Types

| Restriction     | Description                 |
|-----------------|-----------------------------|
| `BUILD`         | Restricts building          |
| `EQUP`          | Restricts equipping items   |
| `UNEQUIP`       | Restricts unequipping items |
| `VEHICLE_ENTER` | Restricts entering vehicles |
| `VEHICLE_LEAVE` | Restricts leaving vehicles  |

## Building from Source

### Prerequisites

- .NET Framework 4.8 SDK / targeting pack

### Build Steps

1. Clone the repository:
   ```
   git clone https://github.com/TavstalDev/TZones.git
   ```
2. Open `TZones.sln` in your IDE.
3. Build the project:
   ```
   dotnet build -c Release
   ```
4. The compiled `TZones.dll` will be in `TZones/bin/Release/`.

## License

This project is licensed under the GNU General Public License v3.0. See the [LICENSE](LICENSE) file for more details.

## Contact

For issues or feature requests, please use the [GitHub issue tracker](https://github.com/TavstalDev/TZones/issues).