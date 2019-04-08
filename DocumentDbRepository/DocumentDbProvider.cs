using System;
using Microsoft.Azure.Documents.Client;

namespace DocumentDbRepository
{

    /// <summary>
    /// DocumentDB provider for wrapping common accessors
    /// </summary>
    public class DocumentDbProvider
    {
        private readonly DocumentDbSettings _settings;
        internal readonly Uri CollectionUri;
        internal readonly DocumentClient DbClient;

        public DocumentDbProvider(DocumentDbSettings settings)
        {
            _settings = settings;
            CollectionUri = GetCollectionLink();
            //See https://azure.microsoft.com/documentation/articles/documentdb-performance-tips/ for performance tips
            DbClient = new DocumentClient(_settings.DatabaseUri, _settings.DatabaseKey, new ConnectionPolicy
            {
                MaxConnectionLimit = 100,
                ConnectionMode = ConnectionMode.Gateway,
                ConnectionProtocol = Protocol.Tcp
            });
            DbClient.OpenAsync().Wait();
        }
        #region Private

        /// <summary>
        /// Obtains the link of a collection
        /// </summary>
        /// <returns></returns>
        private Uri GetCollectionLink()
        {
            return UriFactory.CreateDocumentCollectionUri(_settings.DatabaseName, _settings.CollectionName);
        }

        /// <summary>
        /// Obtains the link for a document
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal Uri GetDocumentLink(string id)
        {
            return UriFactory.CreateDocumentUri(_settings.DatabaseName, _settings.CollectionName, id);
        }

        /// <summary>
        /// Obtains the link of a stored procedure
        /// </summary>
        /// <param name="storeProdecureName"></param>
        /// <returns></returns>
        internal Uri GetProcedureLink(string storeProdecureName)
        {
            return UriFactory.CreateStoredProcedureUri(_settings.DatabaseName, _settings.CollectionName,
                storeProdecureName);
        }
        #endregion
    }
}
