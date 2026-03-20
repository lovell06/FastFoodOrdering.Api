namespace FastFoodOrdering.Api.Data.Seeders;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        UserSeeder.Seed(context);
        ProductSeeder.Seed(context);
    }

    public static async Task SeedAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        await UserSeeder.SeedAsync(context, cancellationToken);
        await ProductSeeder.SeedAsync(context, cancellationToken);
    }
}
