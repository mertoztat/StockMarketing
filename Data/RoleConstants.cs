namespace StockMarkets.Data;

public static class RoleConstants
{
    public const string AdminName = "Admin";
    public const string UserName = "User";

    // Fixed IDs: seed data must be deterministic, otherwise every migration regenerates them.
    // Never change these values once they are applied to a database.
    public const string AdminRoleId = "83914810-9a2d-429f-8e7e-0297b0989493";
    public const string UserRoleId = "938d001f-ef22-4687-8bd9-ec3f9027b00c";
}
