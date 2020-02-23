using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;

namespace WebApplication_geojson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesAboveController : ControllerBase
    {
        KrkMapCommon comms = new KrkMapCommon();

        // GET api/valuesAbove/getChunk?from=a&to=b&minAtt=c
        [HttpGet("getChunk")]
        public string Get(string from, string to, string minAtt)
        {
            //checks
            if (from == null || to == null || minAtt == null)
                return "getChunk: 'from', 'to' or 'minAtt' parameter is missing. Api usage: /api/valuesAbove/getChunk?from=<int>&to=<int>&minAtt=<int>";
            int fromInt = 0;
            int toInt = 0;
            int minAttInt = 0;
            if (!int.TryParse(from, out fromInt) || !int.TryParse(to, out toInt) || !int.TryParse(minAtt, out minAttInt))
                return "getChunk: cannot parse 'from', 'to' or 'minAtt' values into integer type.";
            if (fromInt >= toInt)
                return "getChunk: value 'from' cannot be bigger or eqal to 'to'.";
            if (fromInt < 0 || toInt < 0 || minAttInt < 0)
                return "getChunk: 'from', 'to' or 'minAtt' values have to be positive numbers.";
            //core code:
            SqlConnection conn = new SqlConnection("Server=tcp:_________.database.windows.net;Database=_________;User ID=_________reader;Password=_________;Trusted_Connection=False;Encrypt=True;Connection Timeout=60;");
            try
            {
                conn.Open();
                SqlCommand sqlCommand = new SqlCommand("select * from [dbo].[tab_flightdataIDs] where ID >= " + fromInt.ToString() + " and ID <= " + toInt.ToString() + " and z > " + minAtt.ToString(), conn);
                var toReturn = comms.GetDataAndConvertToJson(sqlCommand);
                conn.Close();
                return toReturn;
            }
            catch (Exception ex)
            {
                return ("Exception: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

    }
}