using Discord;

namespace Yukiko
{
    public sealed class RoleInfo
    {
        public RoleInfo(ulong chanId, ulong messageId, IEmote emote, ulong role)
        {
            ChanId = chanId;
            MessageId = messageId;
            Emote = emote;
            Role = role;
        }

        public ulong ChanId;
        public ulong MessageId;
        public IEmote Emote;
        public ulong Role;
    }
}
