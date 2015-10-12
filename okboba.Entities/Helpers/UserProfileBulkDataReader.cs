using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataUtilities;
using System.Data;
using okboba.Entities;

namespace okboba.Entities.Helpers
{
    public class UserProfileBulkDataReader : BulkDataReader
    {
        int rowCount = 0;
        int numOfUsers;
        string schemaName;
        string tableName;
        Profile[] userProfiles;

        public UserProfileBulkDataReader(int numOfUsers, string schemaName, string tableName)
        {
            this.numOfUsers = numOfUsers;
            this.schemaName = schemaName;
            this.tableName = tableName;

            userProfiles = new Profile[] {
                new Profile { Name = "Jonathan", Birthdate = new DateTime(1982, 3, 24), Gender = "M", Height = 155, LocationId1 = 1, LocationId2 = 1 },
                new Profile { Name = "Maggie", Birthdate = new DateTime(1980, 12, 15), Gender = "F", Height = 145, LocationId1 = 1, LocationId2 = 1 } };
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
                    return userProfiles[rowCount % 2].Name;
                case 1:
                    return userProfiles[rowCount % 2].Birthdate;
                case 2:
                    return userProfiles[rowCount % 2].Gender;
                case 3:
                    return userProfiles[rowCount % 2].Height;
                case 4:
                    return userProfiles[rowCount % 2].LocationId1;
                case 5:
                    return userProfiles[rowCount % 2].LocationId2;
                default:
                    break;
            }
            throw new NotImplementedException();
        }

        public override bool Read()
        {
            return rowCount++ < numOfUsers;
        }

        protected override void AddSchemaTableRows()
        {
            AddSchemaTableRow("Name", 10, null, null, false, false, false, SqlDbType.NVarChar, null, null, null, null, null);
            AddSchemaTableRow("Birthdate", null, null, null, false, false, false, SqlDbType.Date, null, null, null, null, null);
            AddSchemaTableRow("Gender", 1, null, null, false, false, false, SqlDbType.NChar, null, null, null, null, null);
            AddSchemaTableRow("Height", null, null, null, false, false, true, SqlDbType.SmallInt, null, null, null, null, null);
            AddSchemaTableRow("LocationId1", null, null, null, false, false, false, SqlDbType.SmallInt, null, null, null, null, null);
            AddSchemaTableRow("LocationId2", null, null, null, false, false, false, SqlDbType.SmallInt, null, null, null, null, null);
        }
    }
}