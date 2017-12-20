using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vinyl.Metadata;

namespace Vinyl.ParsingJob.Data
{
    public interface IShopInfoService
    {
        Task<List<ShopInfo>> GetShops(CancellationToken token);
    }
}