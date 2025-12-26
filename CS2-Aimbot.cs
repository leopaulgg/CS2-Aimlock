using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Timers;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Modules.Entities;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Channels;
using Microsoft.VisualBasic;
using System.Data;
using System.Runtime.Versioning;

namespace Aimbot
{
    public class Aimbot : BasePlugin
    {
        public override string ModuleAuthor => "leopaulgg";
        public override string ModuleName => "CS2-Aimbot";
        public override string ModuleVersion => "1.0";

        private Config config = new();
        // private readonly ConcurrentDictionary<ulong, PlayerInfo> players = new();

        public override void Load(bool hotReload)
        {
            LoadConfig();
            // RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnect, HookMode.Post);
            // RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect, HookMode.Post);
            AddTimer(config.Interval, OnTick, TimerFlags.REPEAT);
            //         BANNER displayed on the console on Load()
            Console.WriteLine("\n $$$$$$\\  $$\\               $$$$$$$\\             $$\\     \n$$  __$$\\ \\__|              $$  __$$\\            $$ |    \n$$ /  $$ |$$\\ $$$$$$\\$$$$\\  $$ |  $$ | $$$$$$\\ $$$$$$\\   \n$$$$$$$$ |$$ |$$  _$$  _$$\\ $$$$$$$\\ |$$  __$$\\\\_$$  _|  \n$$  __$$ |$$ |$$ / $$ / $$ |$$  __$$\\ $$ /  $$ | $$ |    \n$$ |  $$ |$$ |$$ | $$ | $$ |$$ |  $$ |$$ |  $$ | $$ |$$\\ \n$$ |  $$ |$$ |$$ | $$ | $$ |$$$$$$$  |\\$$$$$$  | \\$$$$  |\n\\__|  \\__|\\__|\\__| \\__| \\__|\\_______/  \\______/   \\____/ \n                                                         \n                                                         \n                                                         ");
        }
        // private void Unload()
        // {

        // }

        private void LoadConfig()
        {
            var configPath = Path.Combine(ModuleDirectory, "config.json");
            if (File.Exists(configPath))
            {
                config = JsonSerializer.Deserialize<Config>(File.ReadAllText(configPath)) ?? new Config();
            }
            else
            {
                File.WriteAllText(configPath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
            }
        }

        // private HookResult OnPlayerConnect(EventPlayerConnectFull @event, GameEventInfo info)
        // {
        //     var player = @event.Userid;
        //     if (player == null || !player.IsValid || player.IsBot)
        //         return HookResult.Continue;

        //     players[player.SteamID] = new PlayerInfo
        //     {
        //         SteamID = player.SteamID
        //     };

        //     return HookResult.Continue;
        // }

        // private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
        // {
        //     var player = @event.Userid;
        //     if (player != null) players.TryRemove(player.SteamID, out _);
        //     return HookResult.Continue;
        // }

        private System.Enum ButtonFinder()
        {
            if (config.KeyLock == "Attack") return PlayerButtons.Attack;
            else if (config.KeyLock == "Jump") return PlayerButtons.Jump;
            else if (config.KeyLock == "Duck") return PlayerButtons.Duck;
            else if (config.KeyLock == "Forward") return PlayerButtons.Forward;
            else if (config.KeyLock == "Back") return PlayerButtons.Back;
            else if (config.KeyLock == "Use") return PlayerButtons.Use;
            else if (config.KeyLock == "Cancel") return PlayerButtons.Cancel;
            else if (config.KeyLock == "Left") return PlayerButtons.Left;
            else if (config.KeyLock == "Right") return PlayerButtons.Right;
            else if (config.KeyLock == "Moveleft") return PlayerButtons.Moveleft;
            else if (config.KeyLock == "Moveright") return PlayerButtons.Moveright;
            else if (config.KeyLock == "Attack2") return PlayerButtons.Attack2;
            else if (config.KeyLock == "Run") return PlayerButtons.Run;
            else if (config.KeyLock == "Reload") return PlayerButtons.Reload;
            else if (config.KeyLock == "Alt1") return PlayerButtons.Alt1;
            else if (config.KeyLock == "Alt2") return PlayerButtons.Alt2;
            else if (config.KeyLock == "Speed") return PlayerButtons.Speed;
            else if (config.KeyLock == "Walk") return PlayerButtons.Walk;
            else if (config.KeyLock == "Zoom") return PlayerButtons.Zoom;
            else if (config.KeyLock == "Weapon1") return PlayerButtons.Weapon1;
            else if (config.KeyLock == "Weapon2") return PlayerButtons.Weapon2;
            else if (config.KeyLock == "Bullrush") return PlayerButtons.Bullrush;
            else if (config.KeyLock == "Grenade1") return PlayerButtons.Grenade1;
            else if (config.KeyLock == "Grenade2") return PlayerButtons.Grenade2;
            else if (config.KeyLock == "Attack3") return PlayerButtons.Attack3;
            else if (config.KeyLock == "Scoreboard") return PlayerButtons.Scoreboard;
            else if (config.KeyLock == "Inspect") return PlayerButtons.Inspect;
            else { throw new ArgumentException($"Setting \"Key For Aimbot\" (= {config.KeyLock}) not recognized"); }
        }

        private static void print(CCSPlayerController player, string Message)
        {
            player.PrintToChat(Message);
        }
        private static void log(CCSPlayerController player, string Message)
        {
            player.PrintToConsole(Message);
        }
        private static IEnumerable<CCSPlayerController> Players()
        {
            return Utilities.GetPlayers().Where(p => p.IsValid && !p.IsBot && p.PlayerPawn.Value != null);
        }
        private static IEnumerable<CCSPlayerController> PlayersAndBots()
        {
            return Utilities.GetPlayers().Where(p => p.IsValid && p.PlayerPawn.Value != null);
        }

        private static double Distance(CCSPlayerController player1, CCSPlayerController player2)
        {
            return Math.Sqrt(
                (player1.Pawn.Value!.AbsOrigin!.X - player2.Pawn.Value!.AbsOrigin!.X) * (player1.Pawn.Value.AbsOrigin.X - player2.Pawn.Value.AbsOrigin.X) +
                (player1.Pawn.Value.AbsOrigin.Y - player2.Pawn.Value.AbsOrigin.Y) * (player1.Pawn.Value.AbsOrigin.Y - player2.Pawn.Value.AbsOrigin.Y) +
                (player1.Pawn.Value.AbsOrigin.X - player2.Pawn.Value.AbsOrigin.Z) * (player1.Pawn.Value.AbsOrigin.X - player2.Pawn.Value.AbsOrigin.Z)
            );
        }
        private void OnTick()
        {
            foreach (var player in Players())
            {
                ulong[] Allowed = [
                    config.AllowedUsers,
                    config.AllowedUsers2
                ];
                foreach (ulong SteamID in Allowed)
                {
                    if (player.SteamID != SteamID) continue;
                }

                System.Enum button = ButtonFinder();
                bool @lock = player.Buttons.HasFlag(button);
                if (@lock)
                {
                    FindClosestPlayer(player, config.CanAimAtTeammates, config.CanAimAtDead);
                }
            }
        }
        private static void FindClosestPlayer(CCSPlayerController Player, bool EnemyOnly, bool AliveOnly)
        {
            CCSPlayerController? TragetedPlayer = null;
            float closestDistance = float.MaxValue;

            if (Player.Pawn?.Value?.CBodyComponent == null) return;
            foreach (var other in PlayersAndBots())
            {
                if (other == null || other == Player) continue;
                if (other.Team == Player.Team && !EnemyOnly) continue;
                if (!other.PawnIsAlive && !AliveOnly) continue;
                var dist = Distance(Player, other);
                if (dist < closestDistance)
                {
                    closestDistance = (float)dist;
                    TragetedPlayer = other;
                }
            }
            if (Player == null || TragetedPlayer == null) return;
            ApplyAimbot(Player, TragetedPlayer);
        }
        private static void ApplyAimbot(CCSPlayerController player, CCSPlayerController TargetedPlayer)
        {
            if (TargetedPlayer?.Pawn?.Value?.AbsOrigin?.X == null) return;
            if (player?.Pawn?.Value?.AbsOrigin?.X == null) return;

            // Direction from me -> enemy
            var dx = TargetedPlayer.Pawn.Value.AbsOrigin.X - player.Pawn.Value.AbsOrigin.X;
            var dy = TargetedPlayer.Pawn.Value.AbsOrigin.Y - player.Pawn.Value.AbsOrigin.Y;
            var dz = TargetedPlayer.Pawn.Value.AbsOrigin.Z - player.Pawn.Value.AbsOrigin.Z;

            var move = TargetedPlayer.Pawn.Value.MovementServices as CCSPlayer_MovementServices;

            // Convert to angles (radians → degrees)
            float yawAngle = MathF.Atan2(dy, dx) * (180f / MathF.PI);
            float pitchAngle = -MathF.Atan2(dz, MathF.Sqrt(dx * dx + dy * dy)) * (180f / MathF.PI);
            // Normalize if needed
            yawAngle %= 360;

            player.Pawn.Value.Teleport(
                null,
                new QAngle(pitchAngle, yawAngle, 0),
                null
            );
        }

        // private class PlayerInfo
        // {
        //     public ulong SteamID { get; set; }
        //     public bool Authorized { get; set; }
        // }

        private class Config
        {
            // JsonCommentHandling = 1;
            [JsonPropertyName("List Keys")]
            public string Comment { get; set; } = "Attack, Jump, Duck, Forward, Back, Use, Cancel, Left, Right, Moveleft, Moveright, Attack2, Run, Reload, Alt1, Alt2, Speed, Walk, Zoom, Weapon1, Weapon2, Bullrush, Grenade1, Grenade2, Attack3, Scoreboard, Inspect";
            [JsonPropertyName("Key For Aimlock")]
            public string KeyLock { get; set; } = "Inspect";

            [JsonPropertyName("Authorized Player1 (Steam ID)")]
            public ulong AllowedUsers { get; set; } = 76561199461992993;
            
            [JsonPropertyName("Authorized Player2 (Steam ID)")]
            public ulong AllowedUsers2 { get; set; } = 76561199053046240;

            [JsonPropertyName("Can Aim At Teammates Too (If false Only Aims At Enemies)")]
            public bool CanAimAtTeammates {get; set;} = false;

            [JsonPropertyName("Can Aim At Dead Players Too (If false Only Aims At Alive Players)")]
            public bool CanAimAtDead {get; set;} = false;

            [JsonPropertyName("Interval In Seconds")]
            public float Interval { get; set; } = 0.015f; // 64 tps: 0.015625 ; 128 tps: 0.0078125
        }
        private class Authorized(ulong SteamID)
        {
            private ulong AllowedUsers { get; set; } = SteamID;
        }
    }
}