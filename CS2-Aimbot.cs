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
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Commands.Targeting;

namespace Aimbot
{
    public class Aimbot : BasePlugin
    {
        public override string ModuleAuthor => "leopaulgg";
        public override string ModuleName => "CS2-Aimbot";
        public override string ModuleVersion => "1.0";
        
        private static readonly MemoryFunctionVoid<CBasePlayerPawn, QAngle> SnapViewAngles = new(GameData.GetSignature("CCSBot_SnapViewAngles"));
        private Config config = new();
        // private readonly ConcurrentDictionary<ulong, PlayerInfo> players = new();
        // public MemoryFunctionVoid<CBasePlayerPawn, QAngle> SnapViewAngles = new(GameData.GetSignature("SnapViewAnglesSignature"));
        public override void Load(bool hotReload)
        {
            LoadConfig();
            // RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnect, HookMode.Post);
            // RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect, HookMode.Post);
            AddTimer(1 / config.Interval, OnTick, TimerFlags.REPEAT);
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
        private static void hud(CCSPlayerController player, string Message)
        {
            player.PrintToCenter(Message);
        }
        private static void html(CCSPlayerController player, string Message)
        {
            player.PrintToCenterHtml(Message);
        }
        private static bool NullCheck(IEnumerable<CCSPlayerController> Players)
        {
            foreach (CCSPlayerController player in Players)
            {
                if (player?.PlayerPawn?.Value?.CBodyComponent == null
                || player?.Pawn?.Value?.CBodyComponent == null
                || player?.CBodyComponent == null)
                    return true;
            }
            return false;
        }
        private static IEnumerable<CCSPlayerController> Players()
        {
            return Utilities.GetPlayers().Where(p => p.IsValid && !p.IsBot && p.PlayerPawn.Value != null);
        }
        private static IEnumerable<CCSPlayerController> PlayersAndBots()
        {
            return Utilities.GetPlayers().Where(p => p.IsValid && p.PlayerPawn.Value != null);
        }

        private static float Distance(CCSPlayerController player1, CCSPlayerController player2)
        {
            return (float)Math.Sqrt(
                (player1.Pawn.Value!.AbsOrigin!.X - player2.Pawn.Value!.AbsOrigin!.X) * (player1.Pawn.Value.AbsOrigin.X - player2.Pawn.Value.AbsOrigin.X) +
                (player1.Pawn.Value.AbsOrigin.Y - player2.Pawn.Value.AbsOrigin.Y) * (player1.Pawn.Value.AbsOrigin.Y - player2.Pawn.Value.AbsOrigin.Y) +
                (player1.Pawn.Value.AbsOrigin.X - player2.Pawn.Value.AbsOrigin.Z) * (player1.Pawn.Value.AbsOrigin.X - player2.Pawn.Value.AbsOrigin.Z)
            );
        }
        private float vAngleScalarTarget(CCSPlayerController player1, CCSPlayerController player2, bool Degrees = true, byte Type = 1)
        {
            Vector P1ToP2 = new(
                player2.PlayerPawn.Value!.AbsOrigin!.X - player1.PlayerPawn.Value!.AbsOrigin!.X,
                player2.PlayerPawn.Value!.AbsOrigin!.Y - player1.PlayerPawn.Value!.AbsOrigin!.Y,
                player2.PlayerPawn.Value!.AbsOrigin!.Z - player1.PlayerPawn.Value!.AbsOrigin!.Z
            );
            Vector eyes = new(
                (float)(Math.Cos(0.017453292519 * player1.PlayerPawn.Value.EyeAngles.Y) * Math.Cos(0.017453292519 * player1.PlayerPawn.Value.EyeAngles.X)),
                (float)(Math.Sin(0.017453292519 * player1.PlayerPawn.Value.EyeAngles.Y) * Math.Cos(0.017453292519 * player1.PlayerPawn.Value.EyeAngles.X)),
                (float)-Math.Sin(0.017453292519 * player1.PlayerPawn.Value.EyeAngles.X)
            );
            if (Type == 0)
            {
                return ((float)(
                    (player2.PlayerPawn.Value!.AbsOrigin!.X - player1.PlayerPawn.Value!.AbsOrigin!.X)
                    *
                    (Math.Cos(0.017453292519 * player1.PlayerPawn.Value.EyeAngles.Y) * Math.Cos(0.017453292519 * player1.PlayerPawn.Value.EyeAngles.X))
                    +
                    (player2.PlayerPawn.Value!.AbsOrigin!.Y - player1.PlayerPawn.Value!.AbsOrigin!.Y)
                    *
                    (Math.Sin(0.017453292519 * player1.PlayerPawn.Value.EyeAngles.Y) * Math.Cos(0.017453292519 * player1.PlayerPawn.Value.EyeAngles.X))
                    +
                    (player2.PlayerPawn.Value!.AbsOrigin!.Z - player1.PlayerPawn.Value!.AbsOrigin!.Z)
                    *
                    -Math.Sin(0.017453292519 * player1.PlayerPawn.Value.EyeAngles.X))
                );
            }
            if (Type == 1)
            {
                if (Degrees)
                {
                    return (float)((
                        Math.Acos((eyes.X * P1ToP2.X + eyes.Y * P1ToP2.Y + eyes.Z * P1ToP2.Z) / Math.Sqrt((eyes.X * eyes.X + eyes.Y * eyes.Y + eyes.Z * eyes.Z) * (P1ToP2.X * P1ToP2.X + P1ToP2.Y * P1ToP2.Y + P1ToP2.Z * P1ToP2.Z)))
                    ) * 57.295779513082);
                }
                else if (!Degrees)
                {
                    return (float)(
                        Math.Acos((eyes.X * P1ToP2.X + eyes.Y * P1ToP2.Y + eyes.Z * P1ToP2.Z) / Math.Sqrt((eyes.X * eyes.X + eyes.Y * eyes.Y + eyes.Z * eyes.Z) * (P1ToP2.X * P1ToP2.X + P1ToP2.Y * P1ToP2.Y + P1ToP2.Z * P1ToP2.Z)))
                    );
                }
                else throw new ArgumentException();
            }
            else throw new ArgumentException("Parameter \"mode\" from vAngleScalarTarget out of range (valid values: 0, 1)");
        }

        private bool AuthorizedCheck(CCSPlayerController player)
        {
            if (player.SteamID == config.AllowedUsers
            || player.SteamID == config.AllowedUsers2)
                return true;
            else return false;
        }
        static Dictionary<ulong, bool> toggled = new()
        {
            {76561199461992993UL, true}
        };

        private static bool Toggled(CCSPlayerController player)
        {
            log(player, $"toggled[steamID]{toggled[player.SteamID]}");
            log(player, $"toggled.TryGetValue(steamID, out bool value)({toggled.TryGetValue(player.SteamID, out bool value2)})\ntoggled({toggled})");
            ulong id = player.SteamID;
            if (toggled.ContainsKey(id))
            {
                toggled[id] = !toggled[id];
                log(player, $"if containskey : toggled[steamID]({toggled[player.SteamID]})");
                return toggled[id];
            }
            else
            {
                toggled.Add(id, true);
                log(player, $"toggle.Add({toggled[id]})");
                log(player, $"if not contains key : toggle[steamID]({toggled[player.SteamID]})");
                return true;
            }
        }
        private void OnTick()
        {
            foreach (var player in Players())
            {
                if (NullCheck([player])) continue;
                if (!AuthorizedCheck(player) && !config.AllowedForAll) continue;

                System.Enum button = ButtonFinder();
                bool @lock = player.Buttons.HasFlag(button);
                ulong id = player.SteamID;

                // Aimlock Logic
                // if (config.AimlockToggle)
                // {
                //     if (@lock && Toggled(player))
                //     {
                //         Redir(player);
                //     }
                //     if (Toggled(player))
                //     {
                //         Redir(player);
                //     }
                // }
                // else
                {
                    if (@lock)
                    {
                        Redir(player);
                    }
                }
            }
        }
        private void Redir(CCSPlayerController player)
        {
            if (!AuthorizedCheck(player) && !config.AllowedForAll) return;
            if (config.ProduitScalaire == true) VectorClosest(player);
            if (config.ProduitScalaire == false) FindClosestPlayer(player);
        }

        private int i = 0;
        private void VectorClosest(CCSPlayerController player)
        {
            if (NullCheck([player])) return;
            if (!AuthorizedCheck(player) && !config.AllowedForAll) return;
            CCSPlayerController? TargetedPlayer = null;
            float closest = float.MaxValue;

            foreach (CCSPlayerController other in PlayersAndBots())
            {
                if (NullCheck([other])) continue;
                if (other == player) continue;
                if (other.Team == player.Team && !config.CanAimAtTeammates) continue;
                if (!other.PawnIsAlive && !config.CanAimAtDead) continue;
                float angle = vAngleScalarTarget(player, other);
                if (vAngleScalarTarget(player, other) < closest)
                {
                    TargetedPlayer = other;
                    closest = (float)vAngleScalarTarget(player, other);
                    i += 1;
                }
            }
            if (TargetedPlayer?.Pawn?.Value?.CBodyComponent == null) { return; }
            ApplyAimbot(player, TargetedPlayer!);
        }
        private void FindClosestPlayer(CCSPlayerController player)
        {
            if (!AuthorizedCheck(player) && !config.AllowedForAll) return;
            CCSPlayerController? TargetedPlayer = null;
            float closestDistance = float.MaxValue;

            foreach (var other in PlayersAndBots())
            {
                if (NullCheck([other])) continue;
                if (other == player) continue;
                if (other.Team == player.Team && !config.CanAimAtTeammates) continue;
                if (!other.PawnIsAlive && !config.CanAimAtDead) continue;
                var dist = Distance(player, other);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    TargetedPlayer = other;
                }
            }
            if (NullCheck([TargetedPlayer!])) return;
            ApplyAimbot(player, TargetedPlayer!);
        }
        private void ApplyAimbot(CCSPlayerController player, CCSPlayerController TargetedPlayer, byte type = 1)
        {
            if (!AuthorizedCheck(player) && !config.AllowedForAll) return;
            if (NullCheck([player])) return;
            // Direction from me -> enemy
            var dx = TargetedPlayer.Pawn.Value!.AbsOrigin!.X - player.Pawn.Value!.AbsOrigin!.X;
            var dy = TargetedPlayer.Pawn.Value.AbsOrigin.Y - player.Pawn.Value.AbsOrigin.Y;
            var dz = TargetedPlayer.Pawn.Value.AbsOrigin.Z - 19 * GetDuckAmount(TargetedPlayer)
                   - player.Pawn.Value.AbsOrigin.Z + 19 * GetDuckAmount(player);

            // Convert to angles (radians → degrees)
            float yawAngle = MathF.Atan2(dy, dx) * (180f / MathF.PI);
            float pitchAngle = -MathF.Atan2(dz, MathF.Sqrt(dx * dx + dy * dy)) * (180f / MathF.PI);
            // Normalize if needed
            yawAngle %= 360;

            var pawn = player.Pawn.Value;
            QAngle angle = new(pitchAngle, yawAngle, 0);
            // log(player, $"pawn({pawn}) angle({angle})");
            // SnapViewAngles.Invoke(pawn, angle);
            if (type == 0)
                player.Pawn.Value.Teleport(
                    new Vector(pawn!.AbsOrigin!.X, pawn.AbsOrigin.Y, pawn.AbsOrigin.Z),
                    new QAngle(angle.X, angle.Y, 0),
                    new Vector(pawn.AbsVelocity.X, pawn.AbsVelocity.Y, pawn.AbsVelocity.Z)
                );
            if (type == 1)
                SnapViewAngles.Invoke(player.PlayerPawn.Value!, angle);
        }
        static float GetDuckAmount(CCSPlayerController player)
        {
            if (NullCheck([player])) return -1;

            var move = player.Pawn!.Value!.MovementServices?.As<CCSPlayer_MovementServices>();
            if (move == null)
                return -1;

            return move.DuckAmount;
        }

        private class Config
        {
            // JsonCommentHandling = 1;
            [JsonPropertyName("List Keys")]
            public string Comment { get; set; } = "Attack, Jump, Duck, Forward, Back, Use, Cancel, Left, Right, Moveleft, Moveright, Attack2, Run, Reload, Alt1, Alt2, Speed, Walk, Zoom, Weapon1, Weapon2, Bullrush, Grenade1, Grenade2, Attack3, Scoreboard, Inspect";
            [JsonPropertyName("Key For Aimlock")]
            public string KeyLock { get; set; } = "Inspect";

            [JsonPropertyName("Authorized Player1 (SteamID64)")]
            public ulong AllowedUsers { get; set; } = 76561199461992993;

            [JsonPropertyName("Authorized Player2 (SteamID64)")]
            public ulong AllowedUsers2 { get; set; } = 0;

            [JsonPropertyName("Allowed For All Players")]
            public bool AllowedForAll { get; set; } = false;

            [JsonPropertyName("Finding Closest Player By Eye Angle")]
            public bool ProduitScalaire { get; set; } = true;

            // [JsonPropertyName("Aimlock Toggle")]
            // public bool AimlockToggle { get; set; } = false;

            [JsonPropertyName("Can Aim At Teammates Too (If false Only Aims At Enemies)")]
            public bool CanAimAtTeammates { get; set; } = false;

            [JsonPropertyName("Can Aim At Dead Players Too (If false Only Aims At Alive Players)")]
            public bool CanAimAtDead { get; set; } = false;

            [JsonPropertyName("Plugin Tickrate")]
            public float Interval { get; set; } = 25000f; // Recommended: 0.00002f 64 tps: 0.015625f ; 128 tps: 0.0078125f

            // [JsonPropertyName("SnapViewAnglesSignature (Do not touch)")]
            // public string CameraAngleSignature { get; set; } = "55 48 89 E5 41 57 41 56 41 55 41 54 53 48 89 FB 48 89 F7 48 81 EC ? ? ? ? E8 ? ? ? ? 48 8B 93 ? ? ? ? 48 89 DF F3 0F";
        }
    }
}