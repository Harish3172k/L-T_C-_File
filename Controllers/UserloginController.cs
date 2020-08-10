using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MySql.Data.MySqlClient;
using Landtapi.Models;
using Newtonsoft.Json.Linq;

namespace Landtapi.Controllers
{
    public class UserloginController : ApiController
    {
       
        int userid;
        
        MySqlCommand  command = null;
        MySqlDataReader datareader = null;
        public  int CheckUserLogin(string username,string password)
        {
            try
            {
                MySqlConnection connection = DBUtil.OpenConnection();
                string query = "select * from users where username = @id1 and password = @id2";
                command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id1", username);
                command.Parameters.AddWithValue("@id2", password);
                datareader = command.ExecuteReader(); 

                if(datareader.HasRows)
                {
                    datareader.Read();
                    userid = datareader.GetInt32(0);
                }
                else
                {
                    throw new Exception("Check username And Password");
                }


            }
            catch(Exception e)
            {
                throw (e);
            }
            return userid;
        }



        [HttpPost]
        public IHttpActionResult Post([FromBody] JObject value  )
        {
            string username, password;
            dynamic t = value;
            dynamic result= new JObject();
            username = t.username;
            password = t.password;
            result.userid = CheckUserLogin(username,password);
            return Ok(result);

        }
        
        

        

        

        
        

    }
        
        
       

    
}
