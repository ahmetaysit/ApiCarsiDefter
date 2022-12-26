using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DataLayer
{
    public static class DbExtentions
    {
        public static List<T> ExecuteQuery<T>(this DatabaseFacade db, string query, Dictionary<string, object> prmList = null)
        {
            DynamicParameters parameters = GenerateParameters(prmList);
            List<T> result;
            var conn = db.GetDbConnection();

            result = conn.Query<T>(query, parameters, commandTimeout: 120).ToList();


            return result;
        }

        public static void ExecuteQuery(this DatabaseFacade db, string query)
        {
            var conn = db.GetDbConnection();

            var result = conn.Execute(query);
        }
        private static DynamicParameters GenerateParameters(Dictionary<string, object> prmList)
        {
            var parameters = new DynamicParameters();
            if (prmList != null)
            {
                foreach (var item in prmList)
                {
                    parameters.Add(item.Key, item.Value);
                }
            }

            return parameters;
        }
    }
}
