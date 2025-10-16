namespace Temple.Persistence.EFCore.Dummies
{
    public class Seed
    {
        public static async Task SeedData(
            DataContext2 context)
        {
            if (!context.Dummies.Any())
            {
                var dummies = new List<Dummy>();

                Enumerable.Range(1, 10).ToList().ForEach(i =>
                {
                    dummies.Add(new Dummy { Name = $"Dummy {i}" });
                });

                await context.Dummies.AddRangeAsync(dummies);
                await context.SaveChangesAsync();
            }
        }
    }
}
