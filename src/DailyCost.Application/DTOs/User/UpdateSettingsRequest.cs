using DailyCost.Domain.Enums;

namespace DailyCost.Application.DTOs.User;

public sealed class UpdateSettingsRequest
{
    public CalcMode DefaultCalcMode { get; set; }
    public string Currency { get; set; } = "CNY";
    public string Timezone { get; set; } = "Asia/Shanghai";
}

