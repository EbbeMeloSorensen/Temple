using Craft.Domain;
using Craft.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Temple.Persistence.Versioned
{
    public static class Helpers2
    {
        public static void InsertNewVariant<T>(
            this IEnumerable<T> entities,
            T newEntity,
            out List<T> nonConflictingEntities,
            out List<T> coveredEntities,
            out List<T> trimmedEntities,
            out List<T> newEntities) where T : IObjectWithValidTime, IClonableObject
        {
            nonConflictingEntities = new List<T>();
            coveredEntities = new List<T>();
            trimmedEntities = new List<T>();
            newEntities = new List<T>();

            var newInterval = new Tuple<DateTime, DateTime>(newEntity.Start, newEntity.End);

            foreach (var entity in entities)
            {
                var otherInterval = new Tuple<DateTime, DateTime>(entity.Start, entity.End);

                if (!newInterval.Overlaps(otherInterval))
                {
                    nonConflictingEntities.Add(entity);
                    continue;
                }

                if (newInterval.Covers(otherInterval))
                {
                    coveredEntities.Add(entity);
                    continue;
                }

                var trimmedToTheRight = false;

                if (otherInterval.Item1 < newInterval.Item1)
                {
                    var clone = (T)entity.Clone();
                    clone.End = newInterval.Item1;
                    trimmedEntities.Add(clone);
                    trimmedToTheRight = true;
                }

                if (newInterval.Item2 < otherInterval.Item2)
                {
                    var clone = (T)entity.Clone();
                    clone.Start = newInterval.Item2;

                    if (trimmedToTheRight)
                    {
                        clone.ArchiveID = Guid.Empty;
                        newEntities.Add(clone);
                    }
                    else
                    {
                        trimmedEntities.Add(clone);
                    }
                }
            }
        }
    }

    internal static class Helpers
    {
        internal static void AddVersionPredicates<T>(
            this ICollection<Expression<Func<T, bool>>> predicates,
            DateTime? databaseTime) where T : IVersionedObject
        {
            if (databaseTime.HasValue)
            {
                predicates.Add(pa =>
                    pa.Created <= databaseTime &&
                    pa.Superseded > databaseTime);
            }
            else
            {
                predicates.Add(pa =>
                    pa.Superseded.Year == 9999);
            }
        }

        internal static void AddHistoryPredicates<T>(
            this ICollection<Expression<Func<T, bool>>> predicates,
            DateTime? historicalTime,
            bool includeHistoricalObjects,
            bool includeCurrentObjects) where T : IObjectWithValidTime
        {
            historicalTime ??= DateTime.UtcNow;

            if (includeHistoricalObjects)
            {
                predicates.Add(p => p.Start <= historicalTime);
            }
            else if (includeCurrentObjects)
            {
                // ONLY current objects
                predicates.Add(p => p.Start <= historicalTime && p.End > historicalTime);
            }
            else
            {
                throw new InvalidOperationException("Either Include current or include historical should be true");
            }
        }

        internal static IEnumerable<T> RemoveAllButLatestVariants<T>(
            this IEnumerable<T> entities) where T : IObjectWithGuidID, IObjectWithValidTime
        {
            return entities
                .GroupBy(p => p.ID)
                .Select(g => g
                    .OrderBy(p => p.Start)
                    .LastOrDefault())
                .Where(p => p != null);
        }

        internal static IEnumerable<T> RemoveCurrentVariants<T>(
            this IEnumerable<T> entities,
            DateTime? historicalTime) where T : IObjectWithGuidID, IObjectWithValidTime
        {
            var time = historicalTime.HasValue
                ? historicalTime.Value
                : new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            return entities.Where(_ => _.End < time);
        }

    }
}
