using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;

namespace re_platform_fapp_msd_sap_di
{
    public static class Msdsapsp
    {
        [FunctionName("msdsapsp")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var str = Environment.GetEnvironmentVariable("sql_db_connection");
            string spname = req.Headers["Sp-name"];
            string date = req.Headers["Sp-date"];
            DataSet ds = new DataSet();
            var result = string.Empty;
            using (SqlConnection objConn = new SqlConnection(str))
            {

                try
                {
                    // Connection object 
                    objConn.Open();

                    // Command Object 
                    using (SqlCommand objCmd = new SqlCommand(spname, objConn))
                    {
                        objCmd.CommandType = CommandType.StoredProcedure;
                        objCmd.Parameters.Add("@LastPullDate", SqlDbType.VarChar).Value = date;
                        objCmd.ExecuteNonQuery();

                        //objCmd.Parameters.AddWithValue("@Start", StartTime);

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = objCmd;

                        da.Fill(ds);
                        result = ds.GetXml();
                        objConn.Close();

                    }

                }
                catch (Exception ex)
                {
                    result = null;
                    log.LogInformation("RE Middleware Service, Function App, Error Exec:  " + ex.Message);
                }
                
                
            }

            return new OkObjectResult(result);

        }
    }
}

