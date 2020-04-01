using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace RumbleMod.Configuration
{
    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        public virtual bool enabled { get; set; } = false;
        public virtual float strength { get; set; } = 1f;
        public virtual float duration { get; set; } = 0.13f;
        public virtual float strength_saber { get; set; } = 1f;
        public virtual float strength_wall { get; set; } = 1f;
    }
}