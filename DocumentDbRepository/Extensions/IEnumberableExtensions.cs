using System.Collections.Generic;
using System.Linq;

namespace DocumentDbRepository.Extensions
{
    /// <summary>
    /// Helper methods for the lists.
    /// </summary>
    public static class IEnumberableExtensions
    {
        public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> @this, int chunkSize)
        {
            return @this
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}