using Dapper;
using HtmlComparer.Model;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace HtmlComparer.Infrastructure.Data
{
    public class DnnPageProvider : ICustomPageProvider
    {
        public string ConnectionString { get; }

        public DnnPageProvider()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["dnnDb"].ConnectionString;
        }

        public IEnumerable<Page> GetPages()
        {
            string sql = @"SELECT CONCAT('/Blog/', LOWER(N.TitleLink), '-', U.ID) 
                           FROM [dnn7].[dbo].[EasyDNNNews] N
                           INNER JOIN [dnn7].[dbo].[EasyDNNnewsUrlProviderData] U 
                           ON (U.ArticleID = N.ArticleID)";

            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.Query<string>(sql)
                    .Select(titleLink => new Page(titleLink));
            }
        }
    }
}
