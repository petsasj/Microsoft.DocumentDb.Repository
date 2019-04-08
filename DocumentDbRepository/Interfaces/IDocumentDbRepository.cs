using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;

namespace DocumentDbRepository.Interfaces
{
    public interface IDocumentDbRepository<T> where T : class
    {
        /// <summary>
        /// Creates a Query with FeedOptions
        /// </summary>
        /// <typeparam name="T">Type of Class to serialize</typeparam>
        /// <param name="feedOptions"></param>
        /// <returns></returns>
        IQueryable<T> CreateQuery(FeedOptions feedOptions = null);

        /// <summary>
        /// Creates a Query with FeedOptions and a SQL expression
        /// </summary>
        /// <typeparam name="T">Type of Class to serialize</typeparam>
        /// <param name="sqlExpression">SQL query</param>
        /// <param name="feedOptions"></param>
        /// <returns></returns>
        IQueryable<T> CreateQuery(string sqlExpression, FeedOptions feedOptions = null);

        /// <summary>
        /// Adds an item to a collection
        /// </summary>
        /// <typeparam name="T">Type of Class to serialize</typeparam>
        /// <param name="document">Document to add</param>
        /// <returns></returns>
        Task<string> AddItemΑsync(T document);

        /// <summary>
        /// Adds multiple items to a collection
        /// </summary>
        /// <typeparam name="T">Type of Class to serialize</typeparam>
        /// <param name="documents">Documents to add</param>
        /// <param name="availableRus">Available number of Rus per second</param>
        /// <param name="safeAvailableRus">How many RUs to keep available for reading operations</param>
        /// <returns></returns>
        Task AddItemsAsync(IEnumerable<T> documents, int availableRus, int? safeAvailableRus = null);

        /// <summary>
        /// Updates a document on a collection
        /// </summary>
        /// <typeparam name="T">Type of Class to serialize</typeparam>
        /// <param name="document">Document to add</param>
        /// <param name="id">Id of the document to update</param>
        /// <returns></returns>
        Task<string> UpdateItemAsync(T document, string id);

        /// <summary>
        /// Deletes a document from a collection
        /// </summary>
        /// <param name="id">Id of the document to delete</param>
        /// <returns></returns>
        Task DeleteItemAsync(string id);
    }
}