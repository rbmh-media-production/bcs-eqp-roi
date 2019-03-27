using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Data.SqlClient;


namespace EasyJobRoi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnOpenDatavase_Click(object sender, EventArgs e)
        {

            string connectionString = "Server = localhost\\SQLEXPRESS; Database = easyjob; Trusted_Connection = True;";
            string queryString = @"SELECT Item.IdStockItem AS ITEM_ID, 
                                    Item.SerialNumber AS ITEM_SERIAL,
                                    Type.Caption AS ITEM_NAME,
                                    Count(Item2Job.IdStockItem) AS TOTAL_RENTALS,
                                    Sum(Type2Job.RentalPrice) AS TOTAL_EARNINGS
                                    
                                    FROM[easyjob].[dbo].[StockItem] AS Item
                                    LEFT OUTER JOIN[easyjob].[dbo].[StockType] AS Type
                                    ON Type.IdStockType = Item.IdStockType
                                    LEFT OUTER JOIN[easyjob].[dbo].[StockItem2Job] AS Item2Job
                                    ON Item2Job.IdStockItem = Item.IdStockItem
                                    LEFT OUTER JOIN[easyjob].[dbo].[StockType2Job] AS Type2Job
                                    ON Type2Job.IdStockType2Job = Item2Job.IdStockType2Job


                                    GROUP BY Item.IdStockItem,
                                    Item.SerialNumber,
                                    Type.Caption
                                    ORDER BY ITEM_ID";

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, cn);
                cn.Open();

                SqlDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dtSchema = dr.GetSchemaTable();
                DataTable dt = new DataTable();
                // You can also use an ArrayList instead of List<>
                List<DataColumn> listCols = new List<DataColumn>();

                if (dtSchema != null)
                {
                    foreach (DataRow drow in dtSchema.Rows)
                    {
                        string columnName = System.Convert.ToString(drow["ColumnName"]);
                        DataColumn column = new DataColumn(columnName, (Type)(drow["DataType"]));
                        column.Unique = (bool)drow["IsUnique"];
                        column.AllowDBNull = (bool)drow["AllowDBNull"];
                        column.AutoIncrement = (bool)drow["IsAutoIncrement"];
                        listCols.Add(column);
                        dt.Columns.Add(column);
                    }
                }

                // Read rows from DataReader and populate the DataTable
                while (dr.Read())
                {
                    DataRow dataRow = dt.NewRow();
                    for (int i = 0; i < listCols.Count; i++)
                    {
                        dataRow[((DataColumn)listCols[i])] = dr[i];
                    }
                    dt.Rows.Add(dataRow);
                }

                dataGridView1.DataSource = dt;
                
            }

        }
    }
}
