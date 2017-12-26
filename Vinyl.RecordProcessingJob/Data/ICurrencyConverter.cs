using System.Threading.Tasks;

namespace Vinyl.RecordProcessingJob.Data
{
    public interface ICurrencyConverter
    {
        Task<decimal> ConvertCurrencyToBYN(string currency, decimal? value);
    }
}