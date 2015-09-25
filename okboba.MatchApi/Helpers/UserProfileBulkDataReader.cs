using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataUtilities;
using System.Data;
using okboba.Entities;

namespace okboba.MatchApi.Helpers
{
    public class UserProfileBulkDataReader : BulkDataReader
    {
        int rowCount = 0;
        int numOfUsers;
        string schemaName;
        string tableName;
        UserProfile[] userProfiles;

        public UserProfileBulkDataReader(int numOfUsers, string schemaName, string tableName)
        {
            this.numOfUsers = numOfUsers;
            this.schemaName = schemaName;
            this.tableName = tableName;

            userProfiles = new UserProfile[] {
                new UserProfile { Name = "Jonathan", Birthdate = new DateTime(1982, 3, 24), Gender = "M", Height = 155, Location = "SF" },
                new UserProfile { Name = "Maggie", Birthdate = new DateTime(1980, 12, 15), Gender = "F", Height = 145, Location = "SF" } };
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
                    return userProfiles[rowCount % 2].Location;
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
            AddSchemaTableRow("Location", 20, null, null, false, false, false, SqlDbType.NVarChar, null, null, null, null, null);
        }
    }
}