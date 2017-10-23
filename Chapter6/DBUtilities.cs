using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data;
using Chapter6.GreetingRule;
using Chapter6.AutomatedUnderwriting;

namespace Chapter6
{
    public static class DBUtilities
    {
        public static IList<Database> GetAllDatabases(Server server)
        {
            var returnValue = new List<Database>();
            foreach (Database db in server.Databases)
                returnValue.Add(db);
            return returnValue;
        }
        public static IList<UnderwritingRule> GettingUnderwritingRules(Database database)
        {
            if (database == null) throw new Exception("Database was not found");
            var sql = @"SELECT RuleName ,
                               ShortDescription ,
                               EffectiveDate ,
                               ExpirationDate, UnderwritingRuleId
                        FROM dbo.UnderwritingRule";
            var data = database.ExecuteWithResults(sql);
            var table = data.Tables[0];
            var returnValue = new List<UnderwritingRule>();
            foreach (DataRow record in table.Rows)
            {
                var rule = new UnderwritingRule()
                {
                    RuleName = record.Field<string>(0),
                    ShortDescription = record.Field<string>(1),
                    EffectiveDate = record.Field<DateTime?>(2),
                    ExpirationDate = record.Field<DateTime?>(3)
                };
                sql = @"SELECT  UnderwritingRuleDetailId ,
                                UnderwritingRuleDetail.UnderwritingRuleId ,
                                UnderwritingRuleDetail.LoanCodeId ,
                                [Min] ,
                                [Max] ,
                                Sequence ,
                                LoanCode.LoanCodeTypeId ,
                                LoanCode.ShortDescription ,
                                LoanCode.LongDescription,
		                        IsRange
                        FROM    dbo.UnderwritingRuleDetail
                                INNER JOIN dbo.LoanCode ON LoanCode.LoanCodeId = UnderwritingRuleDetail.LoanCodeId
                                INNER JOIN dbo.LoanCodeType ON LoanCodeType.LoanCodeTypeId = LoanCode.LoanCodeTypeId
                        WHERE   UnderwritingRuleId = " + record.Field<int>(4) +
                        "ORDER BY Sequence";
                var details = data = database.ExecuteWithResults(sql);
                rule.Details = new List<UnderwritingRuleDetail>();
                returnValue.Add(rule);
                foreach (DataRow row in details.Tables[0].Rows)
                {
                    rule.Details.Add(new UnderwritingRuleDetail
                    {
                        UnderwritingRuleDetailId = row.Field<int>(0),
                        UnderwritingRuleId = row.Field<int>(1),
                        LoanCodeId = row.Field<int>(2),
                        Min = row.Field<decimal?>(3),
                        Max = row.Field<decimal?>(4),
                        Sequence = row.Field<int>(5),
                        LoanCodeTypeId = row.Field<int>(6),
                        ShortDescription = row.Field<string>(7),
                        LongDescription = row.Field<string>(8),
                        IsRange = row.Field<bool>(9)
                    });
                }
            }
            return returnValue;
        }

        public static IList<GreetingRuleDetail> GetGreetingRules (Database database)
        {
            if (database == null) throw new Exception("Data base not found");
            var sql = @"SELECT  GreetingRuleId ,
                                HourMin ,
                                HourMax ,
                                Gender ,
                                MaritalStatus ,
                                Greeting
                        FROM    GreetingRule";
            var data = database.ExecuteWithResults(sql);
            var table = data.Tables[0];
            var returnValue = new List<GreetingRuleDetail>();
            foreach (DataRow record in table.Rows)
            {
                returnValue.Add(new GreetingRuleDetail
                {
                    GreetingRuleId = (int)record[0],
                    HourMin = record.Field<int?>(1),
                    HourMax = record.Field<int?>(2),
                    Gender = record.Field<int?>(3),
                    MaritalStatus = record.Field<int?>(4),
                    Greeting = record.Field<string>(5)
                });
            }
            return returnValue;
        }
    }
}
