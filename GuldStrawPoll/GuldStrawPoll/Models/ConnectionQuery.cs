using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace GuldStrawPoll.Models
{
    public class ConnectionQuery
    {
        String connectionString = "";
        SqlConnection connection;

        public ConnectionQuery(String strConnection)
        {
            connectionString = strConnection;
        }

        //Open the connection
        public void OpenConnection()
        {
            connection = new SqlConnection(connectionString);
            connection.Open();
        }

        //Close the connection
        public void CloseConnection()
        {
            connection.Close();
        }

        //Performs insert, delete and update
        public void ExecuteQueries(String query)
        {
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.ExecuteNonQuery();
        }

        //Show data in a TextBox/Label
        public SqlDataReader DataReader(String query)
        {
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader dr = cmd.ExecuteReader();
            return dr;
        }

        //Show data in a DataGridView
        public object ShowDataInGridView(String query)
        {
            SqlDataAdapter da = new SqlDataAdapter(query, connectionString);
            DataSet ds = new DataSet();
            da.Fill(ds);
            object dataum = ds.Tables[0];
            return dataum;
        }

    }
}