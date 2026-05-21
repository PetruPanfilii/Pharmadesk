using Microsoft.EntityFrameworkCore;
using PharmaDesk.Data;
using PharmaDesk.Models;

namespace PharmaDesk.Services;

public class AdminDashboardService(PharmaDeskDbContext db) : IAdminDashboardService
{
    public async Task<AdminMetrics> GetMetricsAsync() => new()
    {
        ClientsCount = await db.Users.CountAsync(x => x.Role!.Name == "User"),
        PharmacistsCount = await db.Users.CountAsync(x => x.Role!.Name == "Pharmacist"),
        OrdersCount = await db.Orders.CountAsync(),
        ProductsCount = await db.Medicines.CountAsync(x => x.IsActive),
        LowStockCount = await db.Medicines.CountAsync(x => x.StockQuantity <= x.ReorderLevel)
    };
}

public class UserService(PharmaDeskDbContext db) : IUserService
{
    public Task<List<User>> GetUsersAsync() =>
        db.Users.Include(x => x.Role).AsNoTracking().OrderBy(x => x.Username).ToListAsync();

    public async Task ToggleActiveAsync(int userId)
    {
        var entity = await db.Users.FirstAsync(x => x.Id == userId);
        entity.IsActive = !entity.IsActive;
        await db.SaveChangesAsync();
    }

    public async Task UpdateProfileAsync(int userId, string fullName, string email)
    {
        var user = await db.Users.FirstAsync(x => x.Id == userId);
        user.FullName = fullName;
        user.Email = email;
        await db.SaveChangesAsync();
    }
}
