using Discord;
using Discord.WebSocket;
using DiscordUtils;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Yukiko
{
    public sealed class Program
    {
        private RoleInfo[] _roles;

        private readonly DiscordSocketClient _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Verbose,
        });

        public static async Task Main()
        {
            try
            {
                await new Program().StartAsync();
            }
            catch (FileNotFoundException) // This probably means a dll is missing
            {
                throw;
            }
            catch (Exception) // If an exception occur, the program exit and is relaunched
            {
                if (Debugger.IsAttached)
                    throw;
            }
        }

        public async Task StartAsync()
        {
            _roles = new RoleInfo[]
            {
                new RoleInfo(739044431176663136, 739048073317318698, new Emoji("🤘"), 704773216916471865),
                new RoleInfo(704620076028133436, 739051234253865040, new Emoji("🚹"), 738011437900890154),
                new RoleInfo(704620076028133436, 739051234253865040, new Emoji("🚺"), 711879036611198977),
                new RoleInfo(704620076028133436, 739051234253865040, new Emoji("🚻"), 739119770976387072),
                new RoleInfo(704620076028133436, 739051977429876736, new Emoji("👍"), 04955574386688001),
                new RoleInfo(704620076028133436, 739051977429876736, new Emoji("👎"), 704955274435493918),
                new RoleInfo(704620076028133436, 739052564724580412, new Emoji("👍"), 725744412339994734),
                new RoleInfo(704620076028133436, 739052564724580412, new Emoji("👎"), 725735045612175360),
                new RoleInfo(704620076028133436, 739053232743252061, new Emoji("🌎"), 739114646132490240),
                new RoleInfo(704620076028133436, 739053232743252061, new Emoji("🌍"), 739115889718198342),
                new RoleInfo(704620076028133436, 739053232743252061, new Emoji("🌏"), 739116093641195540)
            };

            // Setting Logs callback
            _client.Log += Utils.Log;

            // Discord callbacks
            _client.ReactionAdded += ReactionAdded;
            _client.ReactionRemoved += ReactionRemoved;

            await _client.LoginAsync(TokenType.Bot, File.ReadAllText("Keys/BotToken.txt"));
            await _client.StartAsync();

            // We keep the bot online
            await Task.Delay(-1);
        }

        private async Task ReactionAdded(Cacheable<IUserMessage, ulong> msg, ISocketMessageChannel chan, SocketReaction react)
        {
            var role = GetRole(msg, chan, react);
            if (role == null)
                return;
            if (!react.User.IsSpecified)
                return;
            var user = react.User.Value as IGuildUser;
            if (user == null)
                return;
            await user.AddRoleAsync(user.Guild.GetRole(role.Value));
        }

        private async Task ReactionRemoved(Cacheable<IUserMessage, ulong> msg, ISocketMessageChannel chan, SocketReaction react)
        {
            var role = GetRole(msg, chan, react);
            if (role == null)
                return;
            if (!react.User.IsSpecified)
                return;
            var user = react.User.Value as IGuildUser;
            if (user == null)
                return;
            await user.RemoveRoleAsync(user.Guild.GetRole(role.Value));
        }

        private ulong? GetRole(Cacheable<IUserMessage, ulong> msg, ISocketMessageChannel chan, SocketReaction react)
        {
            var roles = _roles.Where(x => x.ChanId == chan.Id);
            if (roles.Count() == 0)
                return null;
            roles = _roles.Where(x => x.MessageId == msg.Id);
            if (roles.Count() == 0)
                return null;
            roles = _roles.Where(x => x.Emote == react.Emote);
            if (roles.Count() == 0)
                return null;
            return roles.FirstOrDefault()?.Role;
        }
    }
}
