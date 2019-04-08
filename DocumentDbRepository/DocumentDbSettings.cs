using System;
using Microsoft.Extensions.Configuration;

namespace DocumentDbRepository
{
    public class DocumentDbSettings
    {
        public DocumentDbSettings(IConfiguration configuration)
        {
            try
            {
                DatabaseName = configuration.GetSection("DatabaseName").Value;
                CollectionName = configuration.GetSection("CollectionName").Value;
                DatabaseUri = new Uri(configuration.GetSection("EndpointUri").Value);
                DatabaseKey = configuration.GetSection("Key").Value;
            }
            catch
            {
                throw new MissingFieldException("IConfiguration missing a valid Azure DocumentDB fields on DocumentDB > [DatabaseName,CollectionName,EndpointUri,Key]");
            }
        }

        public string DatabaseName { get; }
        public string CollectionName { get; }
        public Uri DatabaseUri { get; }
        public string DatabaseKey { get; }
    }
}
