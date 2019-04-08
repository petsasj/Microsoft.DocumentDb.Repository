using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocumentDbRepository.Extensions;
using DocumentDbRepository.Interfaces;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DocumentDbRepository
{
    public class DocumentDbRepository<T> : IDocumentDbRepository<T> where T : class
    {
        private readonly DocumentDbProvider _provider;

        public DocumentDbRepository(IConfiguration configuration)
        {
            _provider = new DocumentDbProvider(new DocumentDbSettings(configuration));
        }

        /// <inheritdoc />
        public IQueryable<T> CreateQuery(FeedOptions feedOptions = null)
        {
            return _provider.DbClient.CreateDocumentQuery<T>(_provider.CollectionUri, feedOptions);
        }

        /// <inheritdoc />
        public IQueryable<T> CreateQuery(string sqlExpression, FeedOptions feedOptions = null)
        {
            return _provider.DbClient.CreateDocumentQuery<T>(_provider.CollectionUri, sqlExpression, feedOptions);
        }

        /// <inheritdoc />
        public async Task<string> AddItemΑsync(T document)
        {
            var result = await _provider.DbClient.CreateDocumentAsync(_provider.CollectionUri, document);
            return result.Resource.Id;
        }

        public async Task AddItemsAsync(IEnumerable<T> documents, int availableRus, int? remainingRus = null)
        {
            if (remainingRus > availableRus)
                throw new ArgumentException("Remaining RUs must be lower than the Available RUs");

            // Avoid multiple enumeration of IEnumerable
            documents = documents.ToList();

            // Find the average length of each json, then convert to KB
            var averageSize = documents.Average(d => JsonConvert.SerializeObject(d).Length) / 1024;

            // Available RUs, minus remainingRus saved for Reading Operations
            // Divided by average size which is multiplied by 1KB RU cost (17.1RU/Kb)
            var safeAvailableRus = availableRus - remainingRus.GetValueOrDefault(100);
            var chunks = (int)Math.Floor(safeAvailableRus / (averageSize * 17.1));

            var results = new List<dynamic>();

            foreach (var documentChunk in documents.ChunkBy(chunks))
            {
                var executionMinute = DateTime.UtcNow.TimeOfDay;
                var result = await _provider.DbClient.ExecuteStoredProcedureAsync<dynamic>(
                    _provider.GetProcedureLink("bulkImport"), new {items = documentChunk});
                if (result.IsRUPerMinuteUsed)
                {
                    var nextFullMinute = TimeSpan.FromMinutes(Math.Ceiling(executionMinute.TotalMinutes));
                    var delta = (nextFullMinute - executionMinute).TotalMilliseconds;
                    Thread.Sleep((int)delta);
                }

                results.Add(result);
            }
        }

        /// <inheritdoc />
        public async Task<string> UpdateItemAsync(T document, string id)
        {
            var result = await _provider.DbClient.ReplaceDocumentAsync(_provider.GetDocumentLink(id), document);
            return result.Resource.Id;
        }

        /// <inheritdoc />
        public async Task DeleteItemAsync(string id)
        {
            await _provider.DbClient.DeleteDocumentAsync(_provider.GetDocumentLink(id));
        }
    }
}