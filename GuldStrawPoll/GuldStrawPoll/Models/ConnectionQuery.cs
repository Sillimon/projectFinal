﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace GuldStrawPoll.Models
{
    public class ConnectionQuery
    {
        SqlConnection connection = new SqlConnection("Data Source=POLE414-T;Initial Catalog=StrawPoll;Integrated Security=True");
        SqlCommand cmd;

        public ConnectionQuery(){}

        //Open the connection
        public void OpenConnection()
        {
            connection.Open();
        }

        //Close the connection
        public void CloseConnection()
        {
            connection.Close();
        }

        //Performs insert, delete and update
        public void ExecuteNonQuery()
        {
            this.cmd.ExecuteNonQuery();
        }

        //Show data in a TextBox/Label
        public SqlDataReader DataReader()
        {
            SqlDataReader dr = cmd.ExecuteReader();
            return dr;
        }
        
        //GET & SET
        public void setMySqlCommand(SqlCommand newCommand)
        {
            this.cmd = newCommand;
        }


        public SqlConnection getSqlConnection()
        {
            return this.connection;
        }

    }
}