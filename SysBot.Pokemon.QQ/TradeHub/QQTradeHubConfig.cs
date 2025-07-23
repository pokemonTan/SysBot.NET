using System.ComponentModel;
using System.Runtime;

namespace SysBot.Pokemon.QQ;

public sealed class QQTradeHubConfig : BaseConfig
{

    [Browsable(false)]
    public override bool Shuffled => Distribution.Shuffled;
    public DistributionSettings Distribution { get; set; } = new();

}
