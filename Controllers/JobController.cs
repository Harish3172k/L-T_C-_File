using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using MySql.Data.MySqlClient;
using Landtapi.Models;

namespace Landtapi.Controllers
{

    public class JobController : ApiController
    {
        dynamic jobdetails = new JObject();
        JArray array = new JArray();
        int count = 0;
        MySqlCommand command = null;
        MySqlDataReader datareader = null;
        MySqlConnection conn = null;

        public int getjob(int userid)
        {
            try
            {
                conn = DBUtil.OpenConnection();
                string query = "select * from jobdetails where userid = @id1";
                command = new MySqlCommand(query, conn);
                command.Parameters.AddWithValue("@id1", userid);
                datareader = command.ExecuteReader();

                while(datareader.Read())
                {
                    jobdetails.jobid = datareader.GetInt32(0);
                    jobdetails.cluster = datareader.GetString(2);
                    jobdetails.jobcode = datareader.GetString(3);
                    jobdetails.jobname = datareader.GetString(4);
                    jobdetails.joblocation = datareader.GetString(5);
                    jobdetails.structure = datareader.GetString(6);
                    jobdetails.element = datareader.GetString(7);
                    jobdetails.duration = datareader.GetInt32(8);
                    array.Add(jobdetails);
                    count += 1;
                }

                return count;
            }

            catch(Exception e)
            {
                throw new Exception("Error In Jobs");
            }



        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] JObject value)
        {
            dynamic finaljob = value;
            int userid = Convert.ToInt32(finaljob.userid);

            finaljob.count = getjob(userid);
            finaljob.data = array;
            return Ok(finaljob);

        }




    }

    
}
