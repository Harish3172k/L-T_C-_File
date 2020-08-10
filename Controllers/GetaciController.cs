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
    public class GetaciController : ApiController
    {

        dynamic aci = new JObject();
        JArray array = new JArray();
        int count = 0;
        MySqlCommand command = null;
        MySqlDataReader datareader = null;
        MySqlConnection conn = null;

        public int getaci(int userid, int jobid)
        {
            try
            {
                conn = DBUtil.OpenConnection();
                string query = "select * from aci where userid = @id1 and jobid = @id2";
                command = new MySqlCommand(query, conn);
                command.Parameters.AddWithValue("@id1", userid);
                command.Parameters.AddWithValue("@id2", jobid);
                datareader = command.ExecuteReader();

                while (datareader.Read())
                {
                    aci.aci_id = datareader.GetInt32(0);
                    aci.element = datareader.GetString(3);
                    aci.retarders = datareader.GetString(4);
                    aci.cement = datareader.GetString(5);
                    aci.ivd = datareader.GetString(6);
                    aci.slag = datareader.GetString(7);
                    aci.slump = datareader.GetString(8);
                    aci.flyash = datareader.GetString(9);
                    aci.wallheight = datareader.GetString(10);


                    aci.density = datareader.GetDouble(11);
                    aci.volume = datareader.GetDouble(12);
                    aci.vfh = datareader.GetDouble(13);
                    aci.rate = datareader.GetDouble(14);
                    aci.temp = datareader.GetDouble(17);
                    aci.pmax = datareader.GetDouble(18);

                    aci.date = datareader.GetString(19);




                    array.Add(aci);
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

            finaljob.count = getaci(userid, jobid);
            finaljob.data = array;
            return Ok(finaljob);

        }
    }
}
