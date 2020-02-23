using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace WebApplication_geojson
{
    public class KrkMapCommon
    {
        public string GetDataAndConvertToJson(SqlCommand sqlCommand)
        {
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            JArray jarray = new JArray();
            while (sqlDataReader.Read())
            {
                string x = sqlDataReader["x"].ToString();
                string y = sqlDataReader["y"].ToString();
                string z = sqlDataReader["z"].ToString();
                jarray.Add(new JObject(
                                new JProperty("type", "Feature"),
                                new JProperty("geometry",
                                    new JObject(
                                        new JProperty("type", "Point"),
                                        new JProperty("coordinates",
                                        new JArray(new JValue(y), new JValue(x), new JValue(z)))
                                    )
                                ),
                                new JProperty("properties",
                                    new JObject(new JProperty("z", new JValue(z)))
                                )
                            ));

            }
            JObject rss =
                new JObject(
                    new JProperty("type", "FeatureCollection"),
                    new JProperty("features",
                        jarray
                    )
                );

            return JsonConvert.SerializeObject(rss, Formatting.None);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
