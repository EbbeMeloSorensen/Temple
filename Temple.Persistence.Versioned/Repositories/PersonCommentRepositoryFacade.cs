using Craft.Logging;
using Temple.Domain;
using Temple.Domain.Entities.PR;
using Temple.Persistence.Repositories.PR;
using System.Linq.Expressions;

namespace Temple.Persistence.Versioned.Repositories
{
    public class PersonCommentRepositoryFacade : IPersonCommentRepository
    {
        private static DateTime _maxDate;

        static PersonCommentRepositoryFacade()
        {
            _maxDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        }

        private UnitOfWorkFacade _unitOfWorkFacade;
        private bool _returnClonesInsteadOfRepositoryObjects = true;

        private IUnitOfWork UnitOfWork => _unitOfWorkFacade.UnitOfWork;
        private DateTime? DatabaseTime => _unitOfWorkFacade.DatabaseTime;
        private DateTime? HistoricalTime => _unitOfWorkFacade.HistoricalTime;
        private bool IncludeCurrentObjects => _unitOfWorkFacade.IncludeCurrentObjects;
        private bool IncludeHistoricalObjects => _unitOfWorkFacade.IncludeHistoricalObjects;
        private DateTime CurrentTime => _unitOfWorkFacade.TransactionTime;
        private DateTime TimeOfChange => _unitOfWorkFacade.TimeOfChange ?? CurrentTime;

        public ILogger Logger { get; }

        public PersonCommentRepositoryFacade(
            ILogger logger,
            UnitOfWorkFacade unitOfWorkFacade)
        {
            Logger = logger;
            _unitOfWorkFacade = unitOfWorkFacade;
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public int Count(
            Expression<Func<PersonComment, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(
            IList<Expression<Func<PersonComment, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public async Task<PersonComment> Get(
            Guid id)
        {
            var predicates = new List<Expression<Func<PersonComment, bool>>>
            {
                p => p.ID == id
            };

            predicates.AddVersionPredicates(DatabaseTime);
            predicates.AddHistoryPredicates(HistoricalTime, IncludeHistoricalObjects, IncludeCurrentObjects);

            var personComment = (await UnitOfWork.PersonComments.Find(predicates)).SingleOrDefault();

            if (personComment == null)
            {
                throw new InvalidOperationException("Person comment doesn't exist");
            }

            if (_returnClonesInsteadOfRepositoryObjects)
            {
                return personComment.Clone();
            }

            return personComment;
        }

        public Task Erase(PersonComment personComment)
        {
            throw new NotImplementedException();
        }

        public Task EraseRange(IEnumerable<PersonComment> personComments)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PersonComment>> GetAll()
        {
            var predicates = new List<Expression<Func<PersonComment, bool>>>();

            predicates.AddVersionPredicates(DatabaseTime);
            predicates.AddHistoryPredicates(HistoricalTime, IncludeHistoricalObjects, IncludeCurrentObjects);

            var personCommentRows = (await UnitOfWork.PersonComments.Find(predicates)).ToList();

            Logger?.WriteLine(LogMessageCategory.Information, $"PersonCommentRepositoryFacade: Retrieved {personCommentRows.Count} rows");

            return personCommentRows;
        }

        public async Task<IEnumerable<PersonComment>> Find(
            Expression<Func<PersonComment, bool>> predicate)
        {
            var predicates = new List<Expression<Func<PersonComment, bool>>>
            {
                predicate
            };

            return await Find(predicates);
        }

        public async Task<IEnumerable<PersonComment>> Find(
            IList<Expression<Func<PersonComment, bool>>> predicates)
        {
            predicates.AddVersionPredicates(DatabaseTime);
            predicates.AddHistoryPredicates(HistoricalTime, IncludeHistoricalObjects, IncludeCurrentObjects);

            var personComments = await UnitOfWork.PersonComments.Find(predicates);

            if (_returnClonesInsteadOfRepositoryObjects)
            {
                return personComments.Select(_ => _.Clone()).ToList();
            }

            return personComments;
        }

        public PersonComment SingleOrDefault(
            Expression<Func<PersonComment, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task Add(
            PersonComment personComment)
        {
            var person = (await UnitOfWork.People.Find(_ =>
                    _.ID == personComment.PersonID &&
                    _.Superseded.Year == 9999))
                .OrderBy(_ => _.Start)
                .Last();

            var now = DateTime.UtcNow;
            personComment.ID = Guid.NewGuid();
            personComment.PersonArchiveID = person.ArchiveID;
            personComment.Created = now;
            personComment.Superseded = _maxDate;

            if (personComment.Start.Year == 1)
            {
                personComment.Start = now;
            }

            if (personComment.End.Year == 1)
            {
                personComment.End = _maxDate;
            }

            await UnitOfWork.PersonComments.Add(personComment);
        }

        public Task AddRange(
            IEnumerable<PersonComment> entities)
        {
            throw new NotImplementedException();
        }

        public async Task Update(
            PersonComment personComment)
        {
            _returnClonesInsteadOfRepositoryObjects = false;
            var objectFromRepository = await Get(personComment.ID);
            _returnClonesInsteadOfRepositoryObjects = true;
            objectFromRepository.Superseded = CurrentTime;
            await UnitOfWork.PersonComments.Update(objectFromRepository);

            personComment.ArchiveID = Guid.NewGuid();
            personComment.Created = CurrentTime;
            personComment.Superseded = _maxDate;
            await UnitOfWork.PersonComments.Add(personComment);
        }

        public Task UpdateRange(
            IEnumerable<PersonComment> entities)
        {
            throw new NotImplementedException();
        }

        public async Task Remove(
            PersonComment personComment)
        {
            _returnClonesInsteadOfRepositoryObjects = false;
            var objectFromRepository = await Get(personComment.ID);
            _returnClonesInsteadOfRepositoryObjects = true;
            objectFromRepository.Superseded = CurrentTime;
        }

        public async Task RemoveRange(
            IEnumerable<PersonComment> personComments)
        {
            // Make sure we don't use a time of change that is in the future
            if (TimeOfChange > DateTime.UtcNow)
            {
                throw new InvalidOperationException("Time of change cannot be in the future");
            }

            var ids = personComments.Select(p => p.ID).ToList();

            _returnClonesInsteadOfRepositoryObjects = false;
            var objectsFromRepository = (await Find(p => ids.Contains(p.ID))).ToList();
            _returnClonesInsteadOfRepositoryObjects = true;

            // Make sure we don't use a time of change that is too early
            if (TimeOfChange < objectsFromRepository.Max(_ => _.Start))
            {
                throw new InvalidOperationException("Time of change cannot be earlier than the most recent time of change for a person comment in the collection");
            }

            objectsFromRepository.ForEach(p => p.Superseded = CurrentTime);

            var newPersonCommentRows = objectsFromRepository.Select(_ => _.Clone()).ToList();

            newPersonCommentRows.ForEach(_ =>
            {
                _.ArchiveID = new Guid();
                _.Created = CurrentTime;
                _.Superseded = _maxDate;
                _.End = TimeOfChange;
            });

            await UnitOfWork.PersonComments.AddRange(newPersonCommentRows);
        }

        public async Task Clear()
        {
            await UnitOfWork.PersonComments.Clear();
        }

        public void Load(
            IEnumerable<PersonComment> entities)
        {
            throw new NotImplementedException();
        }
    }
}
