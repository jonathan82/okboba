using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using okboba.MatchApi.Helpers;
using System.Data.SqlClient;

namespace ConsoleApp
{
    class Program
    {
        private static int NUM_OF_QUES_PER_USER = 100;
        private static int NUM_OF_USERS_ANSWERED = 1000;
        private static string connString = "Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Jonathan\\Documents\\Visual Studio 2015\\Projects\\okboba\\okboba.MatchApi\\App_Data\\okboba.mdf;Initial Catalog=okboba;Integrated Security=True";

        static void Main(string[] args)
        {
            ///////////////// Seed User Answers ///////////////////////////
            UserAnswerBulkDataReader bulkReader = new UserAnswerBulkDataReader(NUM_OF_USERS_ANSWERED, NUM_OF_QUES_PER_USER, "", "UserAnswers");
            SqlBulkCopy sbc = new SqlBulkCopy(connString);
            sbc.BatchSize = 1000;
            sbc.DestinationTableName = "UserAnswers";

            foreach (var col in bulkReader.ColumnMappings)
            {
                sbc.ColumnMappings.Add(col);
            }
            sbc.WriteToServer(bulkReader);
        }
    }
}
