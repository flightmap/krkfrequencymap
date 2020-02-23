using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using System.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;


namespace WebApplication_geojson.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        KrkMapCommon comms = new KrkMapCommon();

       
        // GET api/values
        [HttpGet]
        public ActionResult<string> Get()
        {
            SqlConnection conn = new SqlConnection("Server=tcp:_________.database.windows.net;Database=_________;User ID=_________reader;Password=_________;Trusted_Connection=False;Encrypt=True;Connection Timeout=60;");
            try
            {
                conn.Open();
                SqlCommand sqlCommand = new SqlCommand("select max(ID) from [dbo].[tab_flightdataIDs]", conn);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                string count = "";
                while (sqlDataReader.Read())
                {
                    count = sqlDataReader[0].ToString();
                }
                conn.Close();
                return count.ToString(); //#new string[] { "value1", "value2" };
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

        // GET api/values/getChunk?from=a&to=b
        [HttpGet("getChunk")]
        public string Get(string from, string to)
        {
            //checks
            if (from == null || to == null)
                return "getChunk: 'from' or 'to' parameter is missing. Api usage: /api/valuesBelow/getChunk?from=<int>&to=<int>";
            int fromInt = 0;
            int toInt = 0;
            if (!int.TryParse(from, out fromInt) || !int.TryParse(to, out toInt))
                return "getChunk: cannot parse 'from' or 'to' values into integer type.";
            if (fromInt >= toInt)
                return "getChunk: value 'from' cannot be bigger or eqal to 'to'.";
            if (fromInt < 0 || toInt < 0)
                return "getChunk: 'from' or 'to' values have to be positive numbers.";
            //core code:
            SqlConnection conn = new SqlConnection("Server=tcp:_________.database.windows.net;Database=_________;User ID=_________reader;Password=_________;Trusted_Connection=False;Encrypt=True;Connection Timeout=60;");
            try
            {
                conn.Open();
                SqlCommand sqlCommand = new SqlCommand("select * from [dbo].[tab_flightdataIDs] where ID >= " + fromInt.ToString() + " and ID <= " + toInt.ToString(), conn);
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

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            if(id <= 0)
            {
                return "Sorry man! you can't pass values lower or equal to 0.";
            }

            SqlConnection conn = new SqlConnection("Server=tcp:_________.database.windows.net;Database=_________;User ID=_________reader;Password=_________;Trusted_Connection=False;Encrypt=True;Connection Timeout=60;");   
            try
            {
                conn.Open();
                SqlCommand sqlCommand = new SqlCommand("select * from [dbo].[tab_flightdataIDs] where ID = "+id.ToString(), conn);
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
