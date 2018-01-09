using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Vinyl.DbLayer
{
    public static class ScriptHelper
    {
        public static async Task RunScriptFromResources(this VinylShopContext context, string scriptName)
        {
            try
            {
                var assembly = typeof(DatabaseServiceRegistrator).Assembly;
                var resourceStream = assembly.GetManifestResourceStream(scriptName);
                if (resourceStream == null)
                    throw new Exception($"Script does not find {scriptName}.");

                using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
                {
                    var sql = await reader.ReadToEndAsync();

                    if (!string.IsNullOrEmpty(sql))
                    {
                        await context.Database.ExecuteSqlCommandAsync(new RawSqlString(sql));
                    }
                }
            }
            catch (Exception exc)
            {
                System.Diagnostics.Trace.WriteLine($"Error during execute script {scriptName}. Exception:" + exc.ToString());
                throw;
            }
        }
    }
}
