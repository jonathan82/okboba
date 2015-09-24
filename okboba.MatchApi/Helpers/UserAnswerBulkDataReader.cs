using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataUtilities;

namespace okboba.MatchApi.Helpers
{
    public class UserAnswerBulkDataReader : BulkDataReader
    {
        protected override string SchemaName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override string TableName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override object GetValue(int i)
        {
            throw new NotImplementedException();
        }

        public override bool Read()
        {
            throw new NotImplementedException();
        }

        protected override void AddSchemaTableRows()
        {
            throw new NotImplementedException();
        }
    }
}