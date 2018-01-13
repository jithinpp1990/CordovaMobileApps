using System.ComponentModel;

namespace CCBankWebAPI.Infrastructure
{
    public enum TransferType
    {
        [Description("O")]
        OtherBank,
        [Description("T")]
        WithInBank,
        [Description("R")]
        RTGS,
        [Description("N")]
        NEFT,
        [Description("M")]
        IMPS
    }

    public enum RequestMode
    {
        [Description("N")]
        NetBanking,
        [Description("M")]
        MobileApp,
        [Description("S")]
        SmsBanking
    }
}