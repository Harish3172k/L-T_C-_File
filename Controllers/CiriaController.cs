using System;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using MySql.Data.MySqlClient;
using Landtapi.Models;

namespace Landtapi.Controllers
{
    public class Calculate
    {
        public double Calc( dynamic t)
        {
            double c1 = Convert.ToDouble(t.c1);
            double k1 = Convert.ToDouble(t.temp) + 16;
            double K2 = (36 / k1);
            double K = K2 * K2;
            double pmax1 = Convert.ToDouble(t.den) * Convert.ToDouble(t.vph);
            double pmax2;
            double R = Convert.ToDouble(t.rate);
            double pmax2_1 = Convert.ToDouble(t.vfh);
            double pmax2_2 = Math.Sqrt(R);
            double pmax2_8 = c1 * pmax2_2;
            double pmax2_3 = pmax2_1 - pmax2_8;
            double pmax2_7 = Math.Sqrt(pmax2_3);
            double pmax2_4 = pmax2_7 * K * Convert.ToDouble(t.c2);
            double pmax2_5 = c1 * Math.Sqrt(R);
            double pmax2_6 = pmax2_4 + pmax2_5;
            if (Convert.ToDouble(t.vfh) > c1 * Math.Sqrt(R))
            {
                pmax2 = Convert.ToDouble(t.den) * pmax2_6;
                /// pmax2=pmax2_8;
            }
            else
            {
                pmax2 = pmax1;
            }
            double pmax;
            if (pmax1 >= pmax2)
            {
                pmax = pmax2;
            }
            else
            {
                pmax = pmax1;
            }
            return pmax;

        }
        public long Ciriastore(dynamic t)
        {
            MySqlCommand command = null;
            MySqlDataReader datareader = null;
            MySqlConnection conn = null;

            try
            {
                conn = DBUtil.OpenConnection();
                string query = "insert into ciria(jobid," +
                    "userid," +
                    "typeofelement," +
                    "cementtype, " +
                    "density," +
                    "verticalformheight," +
                    "verticalpourheight," +
                    "rate," +
                    "latitude," +
                    "longtitude," +
                    "temperature," +
                    "pmax," +
                    "volume)" + "values(@id1,@id2,@id3,@id4,@id5,@id6,@id7,@id8,@id9,@id10,@id11,@id12,@id13)";

                command = new MySqlCommand(query, conn);
                command.Parameters.AddWithValue("@id1", Convert.ToInt32(t.jobid));
                command.Parameters.AddWithValue("@id2", Convert.ToInt32(t.userid));
                command.Parameters.AddWithValue("@id3", Convert.ToString(t.c1));
                command.Parameters.AddWithValue("@id4", Convert.ToString(t.c2));
                command.Parameters.AddWithValue("@id5", Convert.ToDouble(t.den));
                command.Parameters.AddWithValue("@id6", Convert.ToDouble(t.vfh));
                command.Parameters.AddWithValue("@id7", Convert.ToDouble(t.vph));
                command.Parameters.AddWithValue("@id8", Convert.ToDouble(t.rate));
                command.Parameters.AddWithValue("@id9", Convert.ToString(t.latitude));
                command.Parameters.AddWithValue("@id10", Convert.ToString(t.longitude));
                command.Parameters.AddWithValue("@id11", Convert.ToDouble(t.temp));
                command.Parameters.AddWithValue("@id12", Convert.ToDouble(t.pmax));
                command.Parameters.AddWithValue("@id13", Convert.ToDouble(t.vol));
                command.ExecuteNonQuery();
                long s = command.LastInsertedId;
                return s;

            }
            catch (Exception e)
            {
                throw new Exception("Database Connection Failed");
            }


        }
    }
    public class CiriaController : ApiController
    {
        public static dynamic t;
        //long  s = 0;
        static dynamic result = new JObject();
        Calculate c = new Calculate();

       



        // POST api/values
        public IHttpActionResult Post([FromBody]JObject value)
        {
            t = JObject.Parse(value.ToString());
            //double result = Calculate();
            //Console.WriteLine(result);
            result.pmax = c.Calc(t);
            t.pmax = result.pmax;
            result.ciriaid=c.Ciriastore(t);
            return Ok(result);
        }

    
    }
}
