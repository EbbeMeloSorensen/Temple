using Temple.Domain.Entities.PR;
using Temple.Domain.Entities.Smurfs;

namespace Temple.Persistence.EFCore.AppData
{
    public class Seeding
    {
        private static DateTime _timeOfPopulation = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static DateTime _maxDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        // We're seeding the database with characters from the Star Wars Universe
        // https://cdn.shopify.com/s/files/1/1835/6621/files/star-wars-family-tree-episde-9.png?v=1578255836

        public static async Task SeedDatabase(
            PRDbContextBase context)
        {
            if (context.People.Any()) return;

            CreateDataForSeeding(
                PRDbContextBase.Versioned,
                out var people,
                out var personComments,
                out var smurfs);

            await context.Smurfs.AddRangeAsync(smurfs);
            await context.People.AddRangeAsync(people);
            await context.PersonComments.AddRangeAsync(personComments);

            context.SaveChanges();
        }

        public static void CreateDataForSeeding(
            bool versioned,
            out List<Person> people,
            out List<PersonComment> personComments,
            out List<Smurf> smurfs)
        {
            if (versioned)
            {
                CreateBitemporalDataForSeeding(out people, out personComments);
            }
            else
            {
                CreateCurrentDataForSeeding(out people, out personComments);
            }

            smurfs = new List<Smurf>
            {
                new Smurf{ Name = "Gammelsmolf"},
                new Smurf{ Name = "Smolfine"},
            };
        }

        private static void CreateBitemporalDataForSeeding(
            out List<Person> people,
            out List<PersonComment> personComments)
        {
            // Registration time
            var chewbaccaIsEnteredIncorrectly = _timeOfPopulation;
            var chewbaccaIsCorrected = _timeOfPopulation.AddDays(1);
            var commentOnReyIsEnteredIncorrectly = _timeOfPopulation.AddDays(2);
            var commentOnReyIsCorrected = _timeOfPopulation.AddDays(3);
            var reyIsUpdatedProspectively = _timeOfPopulation.AddDays(4);

            // Valid time
            var anakinIsIntroduced = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var anakinMovesToCoruscant = new DateTime(2001, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var lukeSkywalkerIsIntroduced = new DateTime(2003, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var anakinBecomesDarthVader = new DateTime(2003, 10, 4, 0, 0, 0, DateTimeKind.Utc);
            var chewbaccaIsIntroduced = new DateTime(2004, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var darthVaderMovesToMustafar = new DateTime(2004, 2, 1, 0, 0, 0, DateTimeKind.Utc);
            var maxReboIsIntroduced = new DateTime(2006, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var darthVaderDies = new DateTime(2006, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var reyIsIntroduced = new DateTime(2007, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var lukeSkywalkerDies = new DateTime(2008, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var reyBecomesAJedi = new DateTime(2008, 6, 1, 0, 0, 0, DateTimeKind.Utc);
            var reyBecomesReySkywalker = new DateTime(2009, 10, 1, 0, 0, 0, DateTimeKind.Utc);

            var maxRebo_1_1 = new Person
            {
                ID = new Guid("00000001-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000001-0001-0001-0000-000000000000"),
                Created = _timeOfPopulation,
                Superseded = _maxDate,
                Start = maxReboIsIntroduced,
                End = _maxDate,
                FirstName = "Max Rebo",
                Latitude = 10,
                Longitude = 56
            };

            var lukeSkywalker_1_1 = new Person
            {
                ID = new Guid("00000002-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000002-0001-0001-0000-000000000000"),
                Created = _timeOfPopulation,
                Superseded = _maxDate,
                Start = lukeSkywalkerIsIntroduced,
                End = lukeSkywalkerDies,
                FirstName = "Luke Skywalker",
                Latitude = 10,
                Longitude = 56
            };

            var anakin_1_1 = new Person
            {
                ID = new Guid("00000004-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000004-0001-0001-0000-000000000000"),
                Created = _timeOfPopulation,
                Superseded = _maxDate,
                Start = anakinIsIntroduced,
                End = anakinBecomesDarthVader,
                FirstName = "Anakin Skywalker",
                Latitude = 10,
                Longitude = 56
            };

            var anakin_2_1 = new Person
            {
                ID = new Guid("00000004-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000004-0002-0001-0000-000000000000"),
                Created = _timeOfPopulation,
                Superseded = _maxDate,
                Start = anakinBecomesDarthVader,
                End = darthVaderDies,
                FirstName = "Darth Vader",
                Latitude = 10.2,
                Longitude = 56
            };

            var chewbacca_1_1 = new Person
            {
                ID = new Guid("00000005-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000005-0001-0001-0000-000000000000"),
                Created = chewbaccaIsEnteredIncorrectly,
                Superseded = chewbaccaIsCorrected,
                Start = chewbaccaIsIntroduced,
                End = _maxDate,
                FirstName = "Chewing Gum",
                Latitude = 9,
                Longitude = 56
            };

            var chewbacca_1_2 = new Person
            {
                ID = new Guid("00000005-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000005-0001-0002-0000-000000000000"),
                Created = chewbaccaIsCorrected,
                Superseded = _maxDate,
                Start = chewbaccaIsIntroduced,
                End = _maxDate,
                FirstName = "Chewbacca",
                Latitude = 9,
                Longitude = 56
            };

            var rey_1_1 = new Person
            {
                ID = new Guid("00000006-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000006-0001-0001-0000-000000000000"),
                Created = _timeOfPopulation,
                Superseded = reyIsUpdatedProspectively,
                Start = reyIsIntroduced,
                End = _maxDate,
                FirstName = "Rey",
                Latitude = 10,
                Longitude = 56
            };

            var rey_1_2 = new Person
            {
                ID = new Guid("00000006-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000006-0001-0002-0000-000000000000"),
                Created = reyIsUpdatedProspectively,
                Superseded = _maxDate,
                Start = reyIsIntroduced,
                End = reyBecomesReySkywalker,
                FirstName = "Rey",
                Latitude = 10,
                Longitude = 56
            };

            var rey_2_1 = new Person
            {
                ID = new Guid("00000006-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000006-0002-0001-0000-000000000000"),
                Created = reyIsUpdatedProspectively,
                Superseded = _maxDate,
                Start = reyBecomesReySkywalker,
                End = _maxDate,
                FirstName = "Rey Skywalker",
                Latitude = 10.2,
                Longitude = 56
            };

            people = new List<Person>
            {
                maxRebo_1_1,
                lukeSkywalker_1_1,
                anakin_1_1,
                anakin_2_1,
                chewbacca_1_1,
                chewbacca_1_2,
                rey_1_1,
                rey_1_2,
                rey_2_1
            };

            AddBitemporalSeedingDataForWicket(people);

            var rey_comment_1_1_1 = new PersonComment
            {
                ID = new Guid("00000001-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000001-0001-0001-0000-000000000000"),
                PersonID = new Guid("00000006-0000-0000-0000-000000000000"),
                PersonArchiveID = new Guid("00000006-0001-0001-0000-000000000000"),
                Created = commentOnReyIsEnteredIncorrectly,
                Superseded = commentOnReyIsCorrected,
                Start = reyIsIntroduced,
                End = _maxDate,
                Text = "She is a ravager"
            };

            var rey_comment_1_1_2 = new PersonComment
            {
                ID = new Guid("00000001-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000001-0001-0002-0000-000000000000"),
                PersonID = new Guid("00000006-0000-0000-0000-000000000000"),
                PersonArchiveID = new Guid("00000006-0001-0001-0000-000000000000"),
                Created = commentOnReyIsCorrected,
                Superseded = _maxDate,
                Start = reyIsIntroduced,
                End = reyBecomesAJedi,
                Text = "She is a scavenger"
            };

            var rey_comment_1_2_1 = new PersonComment
            {
                ID = new Guid("00000001-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000001-0002-0001-0000-000000000000"),
                PersonID = new Guid("00000006-0000-0000-0000-000000000000"),
                PersonArchiveID = new Guid("00000006-0001-0001-0000-000000000000"),
                Created = commentOnReyIsCorrected,
                Superseded = _maxDate,
                Start = reyBecomesAJedi,
                End = _maxDate,
                Text = "She is a jedi"
            };

            var chewbacca_comment_1_1 = new PersonComment
            {
                ID = new Guid("00000002-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000002-0001-0001-0000-000000000000"),
                PersonID = new Guid("00000005-0000-0000-0000-000000000000"),
                PersonArchiveID = new Guid("00000005-0001-0001-0000-000000000000"),
                Created = _timeOfPopulation,
                Superseded = _maxDate,
                Start = chewbaccaIsIntroduced,
                End = _maxDate,
                Text = "He likes his crossbow"
            };

            var chewbacca_comment_2_1 = new PersonComment
            {
                ID = new Guid("00000003-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000003-0002-0001-0000-000000000000"),
                PersonID = new Guid("00000005-0000-0000-0000-000000000000"),
                PersonArchiveID = new Guid("00000005-0001-0001-0000-000000000000"),
                Created = _timeOfPopulation,
                Superseded = _maxDate,
                Start = chewbaccaIsIntroduced,
                End = _maxDate,
                Text = "He is a furry fellow"
            };

            var anakin_comment_1_1_1 = new PersonComment
            {
                ID = new Guid("00000004-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000004-0001-0001-0000-000000000000"),
                PersonID = new Guid("00000004-0000-0000-0000-000000000000"),
                PersonArchiveID = new Guid("00000004-0001-0001-0000-000000000000"),
                Created = _timeOfPopulation,
                Superseded = _maxDate,
                Start = anakinIsIntroduced,
                End = darthVaderDies,
                Text = "He is strong with the force"
            };

            var anakin_comment_2_1_1 = new PersonComment
            {
                ID = new Guid("00000005-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000005-0001-0001-0000-000000000000"),
                PersonID = new Guid("00000004-0000-0000-0000-000000000000"),
                PersonArchiveID = new Guid("00000004-0001-0001-0000-000000000000"),
                Created = _timeOfPopulation,
                Superseded = _maxDate,
                Start = anakinIsIntroduced,
                End = anakinMovesToCoruscant,
                Text = "Lives on Tatooine"
            };

            var anakin_comment_2_2_1 = new PersonComment
            {
                ID = new Guid("00000005-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000005-0002-0001-0000-000000000000"),
                PersonID = new Guid("00000004-0000-0000-0000-000000000000"),
                PersonArchiveID = new Guid("00000004-0001-0001-0000-000000000000"),
                Created = _timeOfPopulation,
                Superseded = _maxDate,
                Start = anakinMovesToCoruscant,
                End = darthVaderMovesToMustafar,
                Text = "Lives on Coruscant"
            };

            var anakin_comment_2_3_1 = new PersonComment
            {
                ID = new Guid("00000005-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000005-0003-0001-0000-000000000000"),
                PersonID = new Guid("00000004-0000-0000-0000-000000000000"),
                PersonArchiveID = new Guid("00000004-0002-0001-0000-000000000000"),
                Created = _timeOfPopulation,
                Superseded = _maxDate,
                Start = anakinMovesToCoruscant,
                End = darthVaderMovesToMustafar,
                Text = "Lives on Mustafar"
            };

            var anakin_comment_3_1_1 = new PersonComment
            {
                ID = new Guid("00000008-0000-0000-0000-000000000000"),
                ArchiveID = new Guid("00000008-0001-0001-0000-000000000000"),
                PersonID = new Guid("00000004-0000-0000-0000-000000000000"),
                PersonArchiveID = new Guid("00000004-0002-0001-0000-000000000000"),
                Created = _timeOfPopulation,
                Superseded = _maxDate,
                Start = anakinBecomesDarthVader,
                End = darthVaderDies,
                Text = "He is a cruel fellow"
            };

            personComments = new List<PersonComment>
            {
                rey_comment_1_1_1,
                rey_comment_1_1_2,
                rey_comment_1_2_1,
                chewbacca_comment_1_1,
                chewbacca_comment_2_1,
                anakin_comment_1_1_1,
                anakin_comment_2_1_1,
                anakin_comment_2_2_1,
                anakin_comment_2_3_1,
                anakin_comment_3_1_1
            };
        }

        private static void CreateCurrentDataForSeeding(
            out List<Person> people,
            out List<PersonComment> personComments)
        {
            var maxRebo = new Person
            {
                ID = new Guid("00000001-0000-0000-0000-000000000000"),
                FirstName = "Max Rebo",
                Latitude = 10,
                Longitude = 56
            };

            var chewbacca = new Person
            {
                ID = new Guid("00000005-0000-0000-0000-000000000000"),
                FirstName = "Chewbacca",
                Latitude = 9,
                Longitude = 56
            };

            var rey = new Person
            {
                ID = new Guid("00000006-0000-0000-0000-000000000000"),
                FirstName = "Rey Skywalker",
                Latitude = 10.2,
                Longitude = 56
            };

            var wicket = new Person
            {
                ID = new Guid("00000009-0000-0000-0000-000000000000"),
                FirstName = "Wicket",
                Latitude = 10.2,
                Longitude = 56
            };

            people = new List<Person>
            {
                maxRebo,
                rey,
                chewbacca,
                wicket
            };

            var rey_comment = new PersonComment
            {
                ID = new Guid("00000001-0000-0000-0000-000000000000"),
                PersonID = new Guid("00000006-0000-0000-0000-000000000000"),
                Text = "She is a jedi"
            };

            var chewbacca_comment_1 = new PersonComment
            {
                ID = new Guid("00000002-0000-0000-0000-000000000000"),
                PersonID = new Guid("00000005-0000-0000-0000-000000000000"),
                Text = "He likes his crossbow"
            };

            var chewbacca_comment_2 = new PersonComment
            {
                ID = new Guid("00000003-0000-0000-0000-000000000000"),
                PersonID = new Guid("00000005-0000-0000-0000-000000000000"),
                Text = "He is a furry fellow"
            };

            personComments = new List<PersonComment>
            {
                rey_comment,
                chewbacca_comment_1,
                chewbacca_comment_2,
            };
        }

        private static void AddBitemporalSeedingDataForWicket(
            List<Person> people)
        {
            // (Wicket flytter sig hvert nytår)
            //var firstYear = 2000;
            var firstYear = 2021;
            var count = DateTime.UtcNow.Year - firstYear;

            var wicketVariants = Enumerable.Range(1, count).Select(_ => new Person
            {
                ID = new Guid("00000003-0000-0000-0000-000000000000"),
                ArchiveID = new Guid($"00000003-{_.ToString().PadLeft(4, '0')}-0001-0000-000000000000"),
                Created = _timeOfPopulation,
                Superseded = _maxDate,
                Start = new DateTime(firstYear + _, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                End = new DateTime(firstYear + 1 + _, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                FirstName = "Wicket",
                Latitude = 9 + _,
                Longitude = 50
            }).ToList();

            wicketVariants.Last().End = _maxDate;

            foreach (var wicketVariant in wicketVariants)
            {
                people.Add(wicketVariant);
            }
        }
    }
}
