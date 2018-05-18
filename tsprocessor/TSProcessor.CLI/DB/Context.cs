using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace TSProcessor.CLI.DB
{
    public class Context
    {
        IMongoDatabase _db;
        string seriesCollectionName = "seriesCollection";

        public Context(IOptions<MongoOptions> options)
        {
            MongoOptions valueOptions = options.Value;
            MongoClientSettings settings = MongoClientSettings.FromUrl(
              new MongoUrl(valueOptions.ConnectionString)
            );
            settings.SslSettings = new SslSettings()
            {
                EnabledSslProtocols = SslProtocols.Tls12
            };
            var mongoClient = new MongoClient(settings);

            _db = mongoClient.GetDatabase(valueOptions.DatabaseName);
        }

        public IMongoCollection<Series> Series
        {
            get => _db.GetCollection<Series>(seriesCollectionName);
        }
    }
}
