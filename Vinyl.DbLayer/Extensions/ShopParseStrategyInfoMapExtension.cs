using System;

namespace Vinyl.DbLayer
{
    public static class ShopParseStrategyInfoMapExtension
    {
        public static Vinyl.Metadata.ShopParseStrategyInfo ToMetaData(this Vinyl.DbLayer.Models.ShopParseStrategyInfo obj)
            => new Vinyl.Metadata.ShopParseStrategyInfo
            {
                Id = obj.Id,
                ClassName = obj.ClassName,
                CreatedAt = obj.CreatedAt,
                DataLimit = obj.DataLimit,
                LastProcessedCount = obj.LastProcessedCount ?? 0,
                Parameters = obj.Parameters.FromParametersDbString(),
                ProcessedAt = obj.ProcessedAt ?? DateTime.MinValue,
                ShopId = obj.ShopId,
                UpdatedAt = obj.UpdatedAt,
                UpdatePeriodInHours = obj.UpdatePeriodInHours,
                Url = obj.StartUrl
            };

        public static Vinyl.DbLayer.Models.ShopParseStrategyInfo ToDbObject(this Vinyl.Metadata.ShopParseStrategyInfo obj)
            => new Vinyl.DbLayer.Models.ShopParseStrategyInfo
            {
                Id = obj.Id,
                ClassName = obj.ClassName,
                CreatedAt = obj.CreatedAt,
                DataLimit = obj.DataLimit,
                LastProcessedCount = obj.LastProcessedCount,
                Parameters = obj.Parameters.ToParametersDbString(),
                ProcessedAt = obj.ProcessedAt,
                ShopId = obj.ShopId,
                UpdatedAt = obj.UpdatedAt,
                UpdatePeriodInHours = obj.UpdatePeriodInHours,
                StartUrl = obj.Url
            };
    }
}
