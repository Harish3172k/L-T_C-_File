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
    public class GetciriaController : ApiController
    {

        dynamic ciria = new JObject();
        JArray array = new JArray();
        int count = 0;
        MySqlCommand command = null;
        MySqlDataReader datareader = null;
        MySqlConnection conn = null;

        public int getciria(int userid, int jobid)
        {
            try
            {
                conn = DBUtil.OpenConnection();
                string query = "select * from ciria where userid = @id1 and jobid = @id2";
                command = new MySqlCommand(query, conn);
                command.Parameters.AddWithValue("@id1", userid);
                command.Parameters.AddWithValue("@id2", jobid);
                datareader = command.ExecuteReader();

                while (datareader.Read())
                {
                    ciria.ciriaid = datareader.GetInt32(0);
                    ciria.element = datareader.GetDouble(3);
                    ciria.cement = datareader.GetDouble(4);
                    ciria.density = datareader.GetDouble(5);
                    ciria.vfh = datareader.GetDouble(6);
                    ciria.vph = datareader.GetDouble(7);
                    ciria.rate = datareader.GetDouble(8);
                    ciria.temp = datareader.GetDouble(11);
                    ciria.pmax = datareader.GetDouble(12);
                    ciria.volume = datareader.GetDouble(13);
                    ciria.date = datareader.GetString(14);



                    array.Add(ciria);
                    count += 1;
                }

                return count;
            }

            catch (Exception e)
            {
                throw new Exception("Error In Ciria");
            }



        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] JObject value)
        {
            dynamic finaljob = value;
            int userid = Convert.ToInt32(finaljob.userid);
            int jobid = Convert.ToInt32(finaljob.jobid);

            finaljob.count = getciria(userid,jobid);
            finaljob.data = array;
            return Ok(finaljob);

        }
    }
}
