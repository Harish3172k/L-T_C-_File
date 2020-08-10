using System;
using System.Web.Http;
using Landtapi.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace Landtapi.Controllers
{
    public class AciCalculate{

        public double calc(dynamic aci)
        {
			double cc=0.0;
			double R = Convert.ToDouble(aci.rate);
			bool flag = true;
			if (string.Equals(Convert.ToString(aci.retarders),"Yes"))
			{
				if (string.Equals(Convert.ToString(aci.cement),"Type 1")
						|| string.Equals(Convert.ToString(aci.cement), "Type 2")
						|| string.Equals(Convert.ToString(aci.cement), "Type 3")
								&& string.Equals(Convert.ToString(aci.slag), "None")
								&& string.Equals(Convert.ToString(aci.flyash), "None"))
				{
					cc = 1.2;
				}
				else if (string.Equals(Convert.ToString(aci.cement), "Any Type")
					  && string.Equals(Convert.ToString(aci.slag), "<70")
					  && string.Equals(Convert.ToString(aci.flyash), "<40"))
				{
					cc = 1.4;
				}
				else if (string.Equals(Convert.ToString(aci.cement), "Any Type")
					  && string.Equals(Convert.ToString(aci.slag), ">=70")
					  && string.Equals(Convert.ToString(aci.flyash), ">=40"))
				{
					cc = 1.5;
				}
			}
			else if (string.Equals(Convert.ToString(aci.retarders), "No"))
			{
				if (string.Equals(Convert.ToString(aci.cement), "Type 1")
						|| string.Equals(Convert.ToString(aci.cement), "Type 2")
						|| string.Equals(Convert.ToString(aci.cement), "Type 3")

								&& string.Equals(Convert.ToString(aci.slag), "None")
								&& string.Equals(Convert.ToString(aci.flyash), "None"))
				{
					cc = 1.0;
				}
				else if (string.Equals(Convert.ToString(aci.cement), "Any Type")
					  && string.Equals(Convert.ToString(aci.slag), "<70")
					  && string.Equals(Convert.ToString(aci.flyash), "<40"))
				{
					cc = 1.2;
				}
				else if (string.Equals(Convert.ToString(aci.cement), "Any Type")
					  && string.Equals(Convert.ToString(aci.slag), ">=70")
					  && string.Equals(Convert.ToString(aci.flyash), ">=40"))
				{
					cc = 1.4;
				}
			}

			double cw = 0;

			if (Convert.ToDouble(aci.den) < 2240)
			{
				cw = 0.5 * (1 + (Convert.ToDouble(aci.den) / 2320));
				if (cw < 0.8)
				{
					cw = 0.8;
				}

			}
			else if ((Convert.ToDouble(aci.den) <= 2400) && (Convert.ToDouble(aci.den) >= 2240))
			{
				cw = 1.0;

			}
			else if (Convert.ToDouble(aci.den) > 2400)
			{
				cw = (Convert.ToDouble(aci.den) / 2320);
			}
			//
			double pressure2 = 0;
			double ccpmax = 0;
			if (string.Equals(Convert.ToString(aci.element), "ANY"))
			{
				if (string.Equals(Convert.ToString(aci.slump), ">175")
						&& string.Equals(Convert.ToString(aci.ivd), "Any Type"))
				{
					pressure2 = (Convert.ToDouble(aci.den) * 9.8 * Convert.ToDouble(aci.vfh));
					pressure2 = pressure2 / 1000.00;
					ccpmax = pressure2;
					flag = false;
					return ccpmax;
					

				}
				else if (string.Equals(Convert.ToString(aci.slump), "<=175")
					  && string.Equals(Convert.ToString(aci.ivd), ">1.2"))
				{
					pressure2 = (Convert.ToDouble(aci.den) * 9.8 * Convert.ToDouble(aci.vfh));
					pressure2 = pressure2 / 1000.00;
					ccpmax = pressure2;
					flag = false;
					return ccpmax;
				}
			}
			else if (string.Equals(Convert.ToString(aci.element), "WALLS"))
			{
				if (string.Equals(Convert.ToString(aci.slump), "<=175")
						&& string.Equals(Convert.ToString(aci.ivd), "<=1.2") && (R >= 2.10) && (R <= 4.5))
				{
					ccpmax = cc * cw * (7.2 + (1156 / (Convert.ToDouble(aci.temp) + 17.8))
							+ ((244 * R) / (Convert.ToDouble(aci.temp) + 17.8)));
				}
				else if (string.Equals(Convert.ToString(aci.slump), "<=175")
					  && string.Equals(Convert.ToString(aci.ivd), "<=1.2") && (R > 4.50))
				{
					pressure2 = (Convert.ToDouble(aci.den) * 9.8 * Convert.ToDouble(aci.vfh));
					pressure2 = pressure2 / 1000;
					ccpmax = pressure2;
					flag = false;
					return ccpmax;
				}
				else if (string.Equals(Convert.ToString(aci.wall), "<=4.2")
					  && string.Equals(Convert.ToString(aci.slump), "<=175")
					  && string.Equals(Convert.ToString(aci.ivd), "<=1.2") && (R < 2.1))
				{
					ccpmax = cc * cw * (7.2 + ((785 * R) / (Convert.ToDouble(aci.temp) + 17.8)));
				}
				else if (string.Equals(Convert.ToString(aci.wall), ">4.2")
					  && string.Equals(Convert.ToString(aci.slump), "<=175")
					  && string.Equals(Convert.ToString(aci.ivd), "<=1.2") && (R < 2.1))
				{
					ccpmax = cc * cw * (7.2 + (1156 / (Convert.ToDouble(aci.temp) + 17.8))
							+ ((244 * R) / (Convert.ToDouble(aci.temp) + 17.8)));
				}
			}
			else if (string.Equals(Convert.ToString(aci.element), "COLUMNS"))
			{
				if (string.Equals(Convert.ToString(aci.slump), "<=175")
						&& string.Equals(Convert.ToString(aci.ivd), "<=1.2"))
				{
					ccpmax = cc * cw * (7.2 + ((785 * R) / (Convert.ToDouble(aci.temp) + 17.8)));
				}
			}

			pressure2 = (Convert.ToDouble(aci.den) * 9.8 * Convert.ToDouble(aci.vfh));
			pressure2 = pressure2 / 1000.00;

			if (flag)
			{
				if ((ccpmax > (30 * cw)) && (ccpmax < pressure2))
				{
					return ccpmax;
				}
				else if (ccpmax < (30 * cw))
				{
					ccpmax = 30 * cw;
					return ccpmax;
					
				}
				else
				{
					ccpmax = pressure2;
					return ccpmax;
					
				}
			}
			return 0;
		}

		public long Acistore(dynamic t)
		{
			MySqlCommand command = null;
			MySqlDataReader datareader = null;
			MySqlConnection conn = null;

			try
			{
				conn = DBUtil.OpenConnection();
				string query = "insert into aci(jobid," +
					"userid," +
					"elementtype," +
					"retarders, " +
					"cementtype," +
					"internalvibration," +
					"slag," +
					"slump," +
					"flyash," +
					"wallheight," +
					"den," +
					"volume," +
					"verticalformheight,"+
					"ratee,"+"latitude,"+"longtiude,"+"temperature,"+"pmax) " + "values(@id1,@id2,@id3,@id4,@id5,@id6,@id7,@id8,@id9,@id10,@id11,@id12,@id13,@id14,@id15,@id16,@id17,@id18)";

				command = new MySqlCommand(query, conn);
				command.Parameters.AddWithValue("@id1", Convert.ToInt32(t.jobid));
				command.Parameters.AddWithValue("@id2", Convert.ToInt32(t.userid));
				command.Parameters.AddWithValue("@id3", Convert.ToString(t.element));
				command.Parameters.AddWithValue("@id4", Convert.ToString(t.retarders));
				command.Parameters.AddWithValue("@id5", Convert.ToString(t.cement));
				command.Parameters.AddWithValue("@id6", Convert.ToString(t.ivd));
				command.Parameters.AddWithValue("@id7", Convert.ToString(t.slag));
				command.Parameters.AddWithValue("@id8", Convert.ToString(t.slump));
				command.Parameters.AddWithValue("@id9", Convert.ToString(t.flyash));
				command.Parameters.AddWithValue("@id10", Convert.ToString(t.wall));
				command.Parameters.AddWithValue("@id11", Convert.ToDouble(t.den));
				command.Parameters.AddWithValue("@id12", Convert.ToDouble(t.vol));
				command.Parameters.AddWithValue("@id13", Convert.ToDouble(t.vfh));
				command.Parameters.AddWithValue("@id14", Convert.ToDouble(t.rate));
				command.Parameters.AddWithValue("@id15", Convert.ToString(t.latitude));
				command.Parameters.AddWithValue("@id16", Convert.ToString(t.longitude));
				command.Parameters.AddWithValue("@id17", Convert.ToDouble(t.temp));
				command.Parameters.AddWithValue("@id18", Convert.ToDouble(t.pmax));
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

    
    public class AciController : ApiController
    {
        static dynamic aci = new JObject();
        static dynamic aciResult = new JObject();
		[HttpPost]
        public IHttpActionResult Post([FromBody]JObject value)
        {
            aci = JObject.Parse(value.ToString());
			AciCalculate obj = new AciCalculate();
			aciResult.pmax = obj.calc(aci);
			aci.pmax = aciResult.pmax;
			aciResult.aciid = obj.Acistore(aci);
            return Ok(aciResult);
        }
    }
}
