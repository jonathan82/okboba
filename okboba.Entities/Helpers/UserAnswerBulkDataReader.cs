using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataUtilities;

namespace okboba.Entities.Helpers
{
    public class UserAnswerBulkDataReader : BulkDataReader
    {
        int rowCount = 0;
        int numOfUsers;
        int numOfQuesPerUser;
        string schemaName;
        string tableName;

        public UserAnswerBulkDataReader(int numOfUsers, int numOfQuesPerUser, string schemaName, string tableName)
        {
            this.numOfUsers = numOfUsers;
            this.numOfQuesPerUser = numOfQuesPerUser;
            this.schemaName = schemaName;
            this.tableName = tableName;
        }

        protected override string SchemaName
        {
            get
            {
                return schemaName;
            }
        }

        protected override string TableName
        {
            get
            {
                return tableName;
            }
        }

        public override object GetValue(int i)
        {
            switch (i)
            {
                case 0:
                    return (rowCount / numOfQuesPerUser) + 1;
                case 1:
                    return (rowCount % numOfQuesPerUser) + 1;
                case 2:
                    return 1;
                case 3:
                    return 1;
                case 4:
                    return 1;
                case 5:
                    return DateTime.Now;
                default:
                    break;
            }

            throw new IndexOutOfRangeException();
        }

        public override bool Read()
        {
            return rowCount++ < (numOfUsers * numOfQuesPerUser) - 1;
        }

        protected override void AddSchemaTableRows()
        {
            AddSchemaTableRow("UserProfileId", null, null, null, false, true, false, System.Data.SqlDbType.Int, null, null, null, null, null);
            AddSchemaTableRow("QuestionId", null, null, null, false, true, false, System.Data.SqlDbType.SmallInt, null, null, null, null, null);
            AddSchemaTableRow("ChoiceIndex", null, null, null, false, false, false, System.Data.SqlDbType.TinyInt, null, null, null, null, null);
            AddSchemaTableRow("ChoiceWeight", null, null, null, false, false, false, System.Data.SqlDbType.TinyInt, null, null, null, null, null);
            AddSchemaTableRow("ChoiceAcceptable", null, null, null, false, false, false, System.Data.SqlDbType.TinyInt, null, null, null, null, null);
            AddSchemaTableRow("LastAnswered", null, null, null, false, false, false, System.Data.SqlDbType.SmallDateTime, null, null, null, null, null);
        }
    }
}