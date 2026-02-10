using Craft.Logging;
using Temple.Domain.Entities.PR;
using Temple.Persistence.Repositories.PR;
using System.Linq.Expressions;

namespace Temple.Persistence.Versioned.Repositories
{
    // Generelt når vi returnerer objekter herfra skal vi returnere kloner, da det ellers fucker, når man opdaterer
    public class PersonRepositoryFacade : IPersonRepository
    {
        private static DateTime _maxDate;

        static PersonRepositoryFacade()
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

        public PersonRepositoryFacade(
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
            Expression<Func<Person, bool>> predicate)
        {
            var predicates = new List<Expression<Func<Person, bool>>>
            {
                predicate
            };

            predicates.AddVersionPredicates(DatabaseTime);
            predicates.AddHistoryPredicates(HistoricalTime, IncludeHistoricalObjects, IncludeCurrentObjects);

            return UnitOfWork.People.Count(predicates);
        }

        public int Count(
            IList<Expression<Func<Person, bool>>> predicates)
        {
            predicates.AddVersionPredicates(DatabaseTime);
            predicates.AddHistoryPredicates(HistoricalTime, IncludeHistoricalObjects, IncludeCurrentObjects);

            return UnitOfWork.People.Count(predicates);
        }

        public async Task Add(
            Person person)
        {
            var now = DateTime.UtcNow;

            person.Created = now;
            person.Superseded = _maxDate;

            // Hvis ikke Start og End er sat a brugeren, så sæt det til default, dvs start nu og slut ikke
            if (person.Start.Year == 1)
            {
                person.Start = now;
            }

            if (person.End.Year == 1)
            {
                person.End = _maxDate;
            }

            if (person.ID == Guid.Empty)
            {
                person.ID = Guid.NewGuid();
            }

            await UnitOfWork.People.Add(person);
        }

        public async Task AddRange(
            IEnumerable<Person> people)
        {
            var now = DateTime.UtcNow;

            foreach (var person in people)
            {
                if (person.Start.Year == 1)
                {
                    person.Start = now;
                }

                if (person.End.Year == 1)
                {
                    person.End = _maxDate;
                }

                if (person.ID == Guid.Empty)
                {
                    person.ID = Guid.NewGuid();
                }
            }

            await UnitOfWork.People.AddRange(people);
        }

        public ILogger Logger { get; }

        public async Task<Person> Get(
            Guid id)
        {
            var predicates = new List<Expression<Func<Person, bool>>>
            {
                p => p.ID == id
            };

            predicates.AddVersionPredicates(DatabaseTime);
            predicates.AddHistoryPredicates(HistoricalTime, IncludeHistoricalObjects, IncludeCurrentObjects);

            var peopleRows = (await UnitOfWork.People.Find(predicates)).ToList();

            if (!peopleRows.Any())
            {
                throw new InvalidOperationException("Person doesn't exist");
            }

            var person = peopleRows
                .OrderBy(_ => _.Start)
                .LastOrDefault();

            if (person == null)
            {
                throw new InvalidOperationException("Person doesn't exist");
            }

            if (_returnClonesInsteadOfRepositoryObjects)
            {
                return (Person)person.Clone();
            }

            return person;
        }

        public async Task<IEnumerable<Person>> GetAllVariants(
            Guid id)
        {
            var predicates = new List<Expression<Func<Person, bool>>>
            {
                p => p.ID == id
            };

            predicates.AddVersionPredicates(DatabaseTime);

            var people = (await UnitOfWork.People.Find(predicates)).OrderBy(_ => _.Start);

            if (_returnClonesInsteadOfRepositoryObjects)
            {
                return people.Select(_ => (Person)_.Clone()).ToList();
            }

            return people;
        }

        public async Task Erase(
            Person person)
        {
            _returnClonesInsteadOfRepositoryObjects = false;

            var objectFromRepository = (await UnitOfWork.People.Find(
                _ => _.ArchiveID == person.ArchiveID)).SingleOrDefault();

            _returnClonesInsteadOfRepositoryObjects = true;

            if (objectFromRepository == null)
            {
                throw new InvalidOperationException("Person doesn't exist");
            }

            objectFromRepository.Superseded = CurrentTime;
        }

        public async Task EraseRange(
            IEnumerable<Person> people)
        {
            var archiveIDs = people.Select(_ => _.ArchiveID).ToHashSet();

            _returnClonesInsteadOfRepositoryObjects = false;

            var objectsFromRepository = (await UnitOfWork.People.Find(
                _ => archiveIDs.Contains(_.ArchiveID))).ToList();

            _returnClonesInsteadOfRepositoryObjects = true;

            if (!objectsFromRepository.Any())
            {
                throw new InvalidOperationException("People don't exist");
            }

            objectsFromRepository.ForEach(_ => _.Superseded = CurrentTime);
        }

        public async Task<IEnumerable<DateTime>> GetAllValidTimeIntervalExtrema()
        {
            var predicates = new List<Expression<Func<Person, bool>>>();

            predicates.AddVersionPredicates(DatabaseTime);

            List<DateTime> timeStamps = null;

            if (true)
            {
                timeStamps = (await UnitOfWork.People.Find(predicates))
                    .GroupBy(_ => _.ID)
                    .SelectMany(g =>
                    {
                        var timeStampsForFeature = g
                            .SelectMany(_ => new[] { _.Start, _.End })
                            .OrderBy(_ => _);

                        if (timeStampsForFeature.Last().Year == 9999)
                        {
                            return new[] { timeStampsForFeature.First() };
                        }

                        return new[] { timeStampsForFeature.First(), timeStampsForFeature.Last() };
                    })
                    .Distinct()
                    .OrderBy(_ => _)
                    .ToList();
            }
            else
            {
                timeStamps = (await UnitOfWork.People.Find(predicates))
                    .SelectMany(_ => new[] { _.Start, _.End })
                    .Where(_ => _.Year < 9999)
                    .Distinct()
                    .OrderBy(_ => _)
                    .ToList();
            }

            return timeStamps;
        }

        public async Task<IEnumerable<DateTime>> GetAllDatabaseWriteTimes()
        {
            var timeStamps = (await UnitOfWork.People.GetAll())
                .SelectMany(_ => new[] { _.Created, _.Superseded })
                .Distinct()
                .ToList();

            if (timeStamps.Any() && timeStamps.Last().Year == 9999)
            {
                return timeStamps.SkipLast(1);
            }

            return timeStamps;
        }

        public async Task<Person> GetIncludingComments(
            Guid id)
        {
            var person = await Get(id);

            var personCommentPredicates = new List<Expression<Func<PersonComment, bool>>>
            {
                _ => _.PersonID == person.ID
            };

            personCommentPredicates.AddVersionPredicates(DatabaseTime);
            personCommentPredicates.AddHistoryPredicates(HistoricalTime, IncludeHistoricalObjects, IncludeCurrentObjects);

            var personCommentRows = (await UnitOfWork.PersonComments.Find(personCommentPredicates)).ToList();
            var personComments = personCommentRows.RemoveAllButLatestVariants();

            if (!IncludeCurrentObjects)
            {
                personComments = personComments.RemoveCurrentVariants(HistoricalTime);
            }

            person.Comments = personComments.ToList();

            return person;
        }

        public async Task<IEnumerable<Person>> GetAll()
        {
            var predicates = new List<Expression<Func<Person, bool>>>();

            predicates.AddVersionPredicates(DatabaseTime);
            predicates.AddHistoryPredicates(HistoricalTime, IncludeHistoricalObjects, IncludeCurrentObjects);

            // Her hiver du ALLE gældende rækker op, uanset hvad deres virkningstidsinterval er.
            // Mon ikke det var smartere kun at hive dem op, der faktisk skærer time Of interest?
            // Ellers filtrerer du jo i memory, hvor det helst skal foregå i databasen.

            // I første omgang får vi ALLE historiske objekter med, og eneste måde at fjerne de ældste er jo netop
            // at gruppere dem efter id og så tage den seneste

            var peopleRows = (await UnitOfWork.People.Find(predicates)).ToList();

            Logger?.WriteLine(LogMessageCategory.Information, $"PersonRepositoryFacade: Retrieved {peopleRows.Count} rows");

            if (!peopleRows.Any())
            {
                return new List<Person>();
            }

            // Med denne forsvinder tidligere varianter.
            // Det er fint, men hvis nu vi har sat filteret til at vi kun skal have levende eller kun døde,
            // så skal vi altså filtrere det endnu en gang

            var people = peopleRows.RemoveAllButLatestVariants();

            if (!IncludeCurrentObjects)
            {
                people = people.RemoveCurrentVariants(HistoricalTime);
            }

            //Logger?.WriteLine(LogMessageCategory.Information, $"PersonRepositoryFacade: Retrieved {people.Count()} objects");

            return people;
        }

        public async Task<IEnumerable<Person>> Find(
            Expression<Func<Person, bool>> predicate)
        {
            var predicates = new List<Expression<Func<Person, bool>>>
            {
                predicate
            };

            return await Find(predicates);
        }

        public async Task<IEnumerable<Person>> Find(
            IList<Expression<Func<Person, bool>>> predicates)
        {
            predicates.AddVersionPredicates(DatabaseTime);
            predicates.AddHistoryPredicates(HistoricalTime, IncludeHistoricalObjects, IncludeCurrentObjects);

            var personRows = await UnitOfWork.People.Find(predicates);
            var people = personRows.RemoveAllButLatestVariants();

            if (!IncludeCurrentObjects)
            {
                people = people.RemoveCurrentVariants(HistoricalTime);
            }

            if (_returnClonesInsteadOfRepositoryObjects)
            {
                return people.Select(_ => (Person)_.Clone()).ToList();
            }

            return people;
        }

        public async Task<IEnumerable<Person>> FindIncludingComments(
            Expression<Func<Person, bool>> predicate)
        {
            var people = (await Find(predicate)).ToList();
            var personIDs = people.Select(_ => _.ID);

            var personCommentPredicates = new List<Expression<Func<PersonComment, bool>>>
            {
                _ => personIDs.Contains(_.PersonID)
            };

            personCommentPredicates.AddVersionPredicates(DatabaseTime);
            personCommentPredicates.AddHistoryPredicates(HistoricalTime, IncludeHistoricalObjects, IncludeCurrentObjects);

            var personCommentRows = (await UnitOfWork.PersonComments.Find(personCommentPredicates)).ToList();
            var personCommentGroups = personCommentRows.GroupBy(_ => _.PersonID);

            people.ToList().ForEach(p =>
            {
                var personCommentGroup = personCommentGroups.SingleOrDefault(_ => _.Key == p.ID);

                p.Comments = personCommentGroup != null
                    ? personCommentGroup.ToList()
                    : new List<PersonComment>();
            });

            return people;
        }

        public Task<IEnumerable<Person>> FindIncludingComments(
            IList<Expression<Func<Person, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public Person SingleOrDefault(
            Expression<Func<Person, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        // Dette er en PROSPEKTIV ændring
        public async Task Update(
            Person person)
        {
            // Make sure we don't use a time of change that is in the future
            if (TimeOfChange > DateTime.UtcNow)
            {
                throw new InvalidOperationException("Time of change cannot be in the future");
            }

            _returnClonesInsteadOfRepositoryObjects = false;
            var objectFromRepository = await Get(person.ID);
            _returnClonesInsteadOfRepositoryObjects = true;

            // Make sure we don't use a time of change that is too early
            if (TimeOfChange < objectFromRepository.Start)
            {
                throw new InvalidOperationException("Time of change cannot be earlier than the most recent time of change for the person");
            }

            objectFromRepository.Superseded = CurrentTime;

            var personCopy = (Person)objectFromRepository.Clone();
            personCopy.ArchiveID = new Guid();
            personCopy.Created = CurrentTime;
            personCopy.Superseded = _maxDate;
            personCopy.End = CurrentTime;

            person.ArchiveID = Guid.NewGuid();
            person.Created = CurrentTime;
            person.Superseded = _maxDate;
            person.Start = CurrentTime;
            person.End = _maxDate;

            var newPersonRows = new List<Person>
            {
                personCopy,
                person
            };

            await UnitOfWork.People.AddRange(newPersonRows);
        }

        // Dette er en PROSPEKTIV ændring
        public async Task UpdateRange(
            IEnumerable<Person> people)
        {
            // Make sure we don't use a time of change that is in the future
            if (TimeOfChange > DateTime.UtcNow)
            {
                throw new InvalidOperationException("Time of change cannot be in the future");
            }

            var ids = people.Select(p => p.ID).ToList();

            _returnClonesInsteadOfRepositoryObjects = false;
            var objectsFromRepository = (await Find(p => ids.Contains(p.ID))).ToList();
            _returnClonesInsteadOfRepositoryObjects = true;

            // Make sure we don't use a time of change that is too early
            if (TimeOfChange < objectsFromRepository.Max(_ => _.Start))
            {
                throw new InvalidOperationException("Time of change cannot be earlier than the most recent time of change for a person in the collection");
            }

            objectsFromRepository.ForEach(pRepo =>
            {
                pRepo.Superseded = CurrentTime;
            });

            var newPersonRows = objectsFromRepository
                .Select(_ => (Person)_.Clone())
                .ToList();

            newPersonRows.ForEach(_ =>
            {
                _.ArchiveID = new Guid();
                _.Created = CurrentTime;
                _.Superseded = _maxDate;
                _.End = TimeOfChange;
            });

            people.ToList().ForEach(_ =>
            {
                _.ArchiveID = Guid.NewGuid();
                _.Created = CurrentTime;
                _.Superseded = _maxDate;
                _.Start = TimeOfChange;
                _.End = _maxDate;
            });

            newPersonRows.AddRange(people);

            // Notice that we have 2 rows for each person here
            await UnitOfWork.People.AddRange(newPersonRows);
        }

        // Dette er en RETROAKTIV ændring
        public async Task Correct(
            Person person)
        {
            // Det her shit vil ikke virke - du skal finde objektet på basis af ARCHIVE ID´et

            _returnClonesInsteadOfRepositoryObjects = false;
            var objectFromRepository = await Get(person.ID);
            _returnClonesInsteadOfRepositoryObjects = true;
            objectFromRepository.Superseded = CurrentTime;

            person.ArchiveID = Guid.NewGuid();
            person.Created = CurrentTime;
            person.Superseded = _maxDate;
            person.Start = CurrentTime;
            person.End = _maxDate;

            await UnitOfWork.People.Add(person);

            // Old
            // Todo: I stedet for at finde én, som man så bare opdaterer, så skal vi finde alle varianter, så vi kan se,
            // om en ændring vil munde ud i en konflikt.
            // Det kan også komme på tale, at man sletter eller korrigerer andre varianter, 

            //_returnClonesInsteadOfRepositoryObjects = false;
            //var objectsFromRepository = (await GetAllVariants(person.ID)).ToList();
            //_returnClonesInsteadOfRepositoryObjects = true;

            //var targetedObjectFromRepository = objectsFromRepository.Single(_ => _.ArchiveID == person.ArchiveID);

            //if (targetedObjectFromRepository == null)
            //{
            //    throw new InvalidOperationException("Person doesn't exist");
            //}

            //targetedObjectFromRepository.Superseded = CurrentTime;

            //person.ArchiveID = Guid.NewGuid();
            //person.Created = CurrentTime;
            //person.Superseded = _maxDate;

            //await UnitOfWork.People.Add(person);
        }

        public async Task CorrectRange(
            IEnumerable<Person> people)
        {
            try
            {
                var archiveIDs = people.Select(p => p.ArchiveID).ToList();

                var predicates = new List<Expression<Func<Person, bool>>>
                {
                    p => archiveIDs.Contains(p.ArchiveID)
                };

                // Bemærk, at du bruger den WRAPPEDE unitofwork her, for ikke at få smidt de der bitemporale predicates ind
                _returnClonesInsteadOfRepositoryObjects = false;
                var objectsFromRepository = (await UnitOfWork.People.Find(predicates)).ToList();
                _returnClonesInsteadOfRepositoryObjects = true;

                objectsFromRepository.ForEach(pRepo =>
                {
                    pRepo.Superseded = CurrentTime;
                });

                people.ToList().ForEach(_ =>
                {
                    _.ArchiveID = Guid.NewGuid();
                    _.Created = CurrentTime;
                    _.Superseded = _maxDate;
                });

                await UnitOfWork.People.AddRange(people);
            }
            catch (Exception e)
            {
                var message = e.Message;
            }
        }

        // This is a socalled SOFT DELETE, where the last valid time interval is closed. A soft delete is actually a retroactive update
        public async Task Remove(
            Person person)
        {
            var personFromRepo = await GetIncludingComments(person.ID);

            if (personFromRepo.Comments.Any())
            {
                throw new InvalidOperationException("Cannot delete person with child rows (comments)");
            }

            _returnClonesInsteadOfRepositoryObjects = false;
            var objectFromRepository = await Get(person.ID);
            _returnClonesInsteadOfRepositoryObjects = true;
            objectFromRepository.Superseded = CurrentTime;

            var personCopy = (Person)objectFromRepository.Clone();
            personCopy.ArchiveID = new Guid();
            personCopy.Created = CurrentTime;
            personCopy.Superseded = _maxDate;
            personCopy.End = CurrentTime;

            await UnitOfWork.People.Add(personCopy);
        }

        // This is a socalled SOFT DELETE, where the last valid time interval is closed. A soft delete is actually a retroactive update
        public async Task RemoveRange(
            IEnumerable<Person> people)
        {
            // Make sure we don't use a time of change that is in the future
            if (TimeOfChange > DateTime.UtcNow)
            {
                throw new InvalidOperationException("Time of change cannot be in the future");
            }

            var ids = people.Select(p => p.ID).ToList();
            var peopleFromRepo = await FindIncludingComments(_ => ids.Contains(_.ID));

            // Make sure we don't use a time of change that is too early
            if (TimeOfChange < peopleFromRepo.Max(_ => _.Start))
            {
                throw new InvalidOperationException("Time of change cannot be earlier than the most recent time of change for a person in the collection");
            }

            if (peopleFromRepo.Any(_ => _.Comments != null && _.Comments.Any(_ => _.End > CurrentTime)))
            {
                throw new InvalidOperationException("Cant delete people with current child rows (comments)");
            }

            _returnClonesInsteadOfRepositoryObjects = false;
            var objectsFromRepository = (await Find(p => ids.Contains(p.ID))).ToList();
            _returnClonesInsteadOfRepositoryObjects = true;

            objectsFromRepository.ForEach(p => p.Superseded = CurrentTime);

            var newPersonRows = objectsFromRepository.Select(_ => (Person)_.Clone()).ToList();

            newPersonRows.ForEach(_ =>
            {
                _.ArchiveID = new Guid();
                _.Created = CurrentTime;
                _.Superseded = _maxDate;
                _.End = TimeOfChange;
            });

            await UnitOfWork.People.AddRange(newPersonRows);
        }

        public async Task Clear()
        {
            await UnitOfWork.People.Clear();
        }

        public void Load(
            IEnumerable<Person> people)
        {
            throw new NotImplementedException();
        }
    }
}
