using System.ComponentModel;
using System.Runtime;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace SysBot.Pokemon;

public sealed class PokeTradeHubConfig : BaseConfig
{
    private const string BotTrade = nameof(BotTrade);
    private const string BotEncounter = nameof(BotEncounter);
    private const string Integration = nameof(Integration);

    [Browsable(false)]
    public override bool Shuffled => Distribution.Shuffled;

    [Category(FeatureToggle), Description("Method for detecting the overworld. Original method may not work consistently for some users, while ConsoleLanguageSpecific method requires your Switch console language.")]
    public ScreenDetectionMode ScreenDetection { get; set; } = ScreenDetectionMode.ConsoleLanguageSpecific;

    [Category(Operation)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public QueueSettings Queues { get; set; } = new();

    [Category(Operation), Description("Add extra time for slower Switches.")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public TimingSettings Timings { get; set; } = new();

    // Trade Bots

    [Category(BotTrade)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public TradeSettings Trade { get; set; } = new();

    [Category(BotTrade), Description("Settings for idle distribution trades.")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public DistributionSettings Distribution { get; set; } = new();

    [Category(BotTrade)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public SeedCheckSettings SeedCheckSWSH { get; set; } = new();

    [Category(BotTrade)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public TradeAbuseSettings TradeAbuse { get; set; } = new();

    // Encounter Bots - For finding or hosting Pokémon in-game.

    [Category(BotEncounter)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public EncounterSettings EncounterSWSH { get; set; } = new();

    [Category(BotEncounter)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public RaidSettings RaidSWSH { get; set; } = new();

    [Category(BotEncounter), Description("Stop conditions for EncounterBot.")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public StopConditionSettings StopConditions { get; set; } = new();

    // Integration

    [Category(Integration)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public DiscordSettings Discord { get; set; } = new();

    [Category(Integration)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public TwitchSettings Twitch { get; set; } = new();

    [Category(Integration)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public YouTubeSettings YouTube { get; set; } = new();

    [Category(Integration), Description("QQ机器人的集成.")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public QQSettings QQ { get; set; } = new();

    [Category(Integration), Description("Configure generation of assets for streaming.")]
    [TypeConverter(typeof(ExpandableObjectConverter))]

    // Integration
    public StreamSettings Stream { get; set; } = new();

    [Category(Integration), Description("Allows favored users to join the queue with a more favorable position than unfavored users.")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public FavoredPrioritySettings Favoritism { get; set; } = new();
}
