using PharmaDesk.Models;

namespace PharmaDesk.Services;

public class MockDataStore
{
    private int cartItemId = 1;
    private int orderId = 1;

    public List<Role> Roles { get; } =
    [
        new() { Id = 1, Name = "Admin" },
        new() { Id = 2, Name = "Pharmacist" },
        new() { Id = 3, Name = "User" }
    ];

    public List<User> Users { get; } = [];
    public List<Category> Categories { get; } = [];
    public List<Medicine> Medicines { get; } = [];
    public List<CartItem> CartItems { get; } = [];
    public List<Order> Orders { get; } = [];
    public List<AuditLog> AuditLogs { get; } = [];

    public MockDataStore()
    {
        Users.AddRange(
        [
            new() { Id = 1, Username = "admin", FullName = "Admin PharmaDesk", Email = "admin@pharmadesk.local", RoleId = 1, Role = Roles[0], IsActive = true },
            new() { Id = 2, Username = "farmacist", FullName = "Farmacist Demo", Email = "farmacist@pharmadesk.local", RoleId = 2, Role = Roles[1], IsActive = true },
            new() { Id = 3, Username = "client", FullName = "Client Demo", Email = "client@pharmadesk.local", RoleId = 3, Role = Roles[2], IsActive = true }
        ]);

        Categories.AddRange(
        [
            new() { Id = 1, Name = "Raceala si gripa", Description = "Produse pentru simptome sezoniere" },
            new() { Id = 2, Name = "Vitamine", Description = "Suplimente pentru rutina zilnica" },
            new() { Id = 3, Name = "Dermato-cosmetice", Description = "Ingrijire piele" },
            new() { Id = 4, Name = "Dispozitive", Description = "Aparate si accesorii medicale" },
            new() { Id = 5, Name = "Retete RX", Description = "Produse care necesita prescriptie" }
        ]);

        Medicines.AddRange(
        [
            Product(1, "Paracetamol Forte", "Paracetamol", "PH-1001", 1, "Comprimate", "500 mg", 18.50m, 85, false, "Assets/Products/paracetamol-forte.png", true, true, 12, 4.8m),
            Product(2, "Nurofen Sinus", "Ibuprofen", "PH-1002", 1, "Comprimate", "200 mg", 42.90m, 24, false, "Assets/Products/nurofen-sinus.png", false, true, 8, 4.7m),
            Product(3, "Vitamina C + Zinc", null, "PH-1003", 2, "Comprimate efervescente", "1000 mg", 31.20m, 120, false, "Assets/Products/vitamina-c-zinc.png", true, true, 15, 4.9m),
            Product(4, "Magneziu B6", null, "PH-1004", 2, "Capsule", "60 buc", 27.40m, 64, false, "Assets/Products/magneziu-b6.png", false, false, 0, 4.6m),
            Product(5, "DermaCare Crema", null, "PH-1005", 3, "Crema", "50 ml", 56.00m, 18, false, "Assets/Products/dermacare-crema.png", true, false, 0, 4.5m),
            Product(6, "SPF 50", null, "PH-1006", 3, "Lotiune", "100 ml", 73.80m, 10, false, "Assets/Products/spf-50.png", false, true, 10, 4.8m),
            Product(7, "Termometru Digital", null, "PH-1007", 4, "Dispozitiv", "1 buc", 49.99m, 7, false, "Assets/Products/thermometer-digital.png", false, false, 0, 4.4m),
            Product(8, "Antibiotic RX", "Amoxicilina", "PH-1008", 5, "Capsule", "500 mg", 62.30m, 16, true, "Assets/Products/antibiotic-rx.png", false, false, 0, 4.3m),
            Product(9, "Insulin Care", null, "PH-1009", 5, "Solutie", "10 ml", 118.00m, 5, true, "Assets/Products/insulin-care.png", false, false, 0, 4.7m),
            Product(10, "Probiotic 10", null, "PH-1010", 2, "Capsule", "30 buc", 44.50m, 40, false, "Assets/Products/probiotic-10.png", true, false, 0, 4.6m)
        ]);

        CartItems.Add(new CartItem { Id = cartItemId++, UserId = 3, MedicineId = 1, Medicine = Medicines[0], Quantity = 2 });
        CartItems.Add(new CartItem { Id = cartItemId++, UserId = 3, MedicineId = 3, Medicine = Medicines[2], Quantity = 1 });
        SeedOrder("MOCK-20260521-0001", "Noua", DateTime.UtcNow.AddHours(-3), Users[2], [(Medicines[0], 2), (Medicines[2], 1)]);
        SeedOrder("MOCK-20260520-0002", "Platita", DateTime.UtcNow.AddDays(-1), Users[2], [(Medicines[5], 1)]);
        SeedOrder("MOCK-20260519-0003", "Expediata", DateTime.UtcNow.AddDays(-2), Users[2], [(Medicines[6], 1), (Medicines[3], 2)]);
        AuditLogs.Add(new AuditLog { Id = 1, UserId = 1, User = Users[0], Action = "Mock mode started", TableName = "Application", Timestamp = DateTime.UtcNow });
    }

    public int NextCartItemId() => cartItemId++;
    public int NextOrderId() => orderId++;

    private Medicine Product(int id, string name, string? generic, string barcode, int categoryId, string form, string strength, decimal price, int stock, bool rx, string imagePath, bool isNew, bool promo, int discount, decimal rating)
    {
        var category = Categories.First(x => x.Id == categoryId);
        return new Medicine
        {
            Id = id,
            Name = name,
            GenericName = generic,
            Barcode = barcode,
            CategoryId = categoryId,
            Category = category,
            DosageForm = form,
            Strength = strength,
            UnitPrice = price,
            StockQuantity = stock,
            ReorderLevel = 10,
            IsPrescriptionRequired = rx,
            ImageUrl = $"pack://application:,,,/{imagePath}",
            Description = rx ? "Necesita prescriptie inainte de validarea comenzii." : "Produs disponibil pentru comanda rapida.",
            IsNew = isNew,
            IsPromotion = promo,
            DiscountPercent = discount,
            Rating = rating,
            CreatedAt = DateTime.UtcNow.AddDays(-id)
        };
    }

    private void SeedOrder(string number, string status, DateTime date, User user, IEnumerable<(Medicine Product, int Quantity)> lines)
    {
        var order = new Order
        {
            Id = orderId++,
            OrderNumber = number,
            Status = status,
            OrderDate = date,
            UserId = user.Id,
            User = user,
            PaymentMethod = status == "Platita" ? "Card" : "Cash la livrare",
            ShippingAddress = "Str. Exemplu 10, Chisinau"
        };

        foreach (var (product, quantity) in lines)
        {
            order.Items.Add(new OrderItem
            {
                Id = order.Items.Count + 1,
                OrderId = order.Id,
                MedicineId = product.Id,
                Medicine = product,
                Quantity = quantity,
                UnitPrice = product.UnitPrice,
                TotalPrice = product.UnitPrice * quantity
            });
        }

        order.TotalAmount = order.Items.Sum(x => x.TotalPrice);
        order.Discount = Math.Round(order.Items.Sum(x => x.TotalPrice * ((x.Medicine?.DiscountPercent ?? 0) / 100m)), 2);
        order.Tax = Math.Round((order.TotalAmount - order.Discount) * 0.09m, 2);
        order.GrandTotal = order.TotalAmount - order.Discount + order.Tax;
        Orders.Add(order);
    }
}

public class MockAuthService(MockDataStore store, AppSession session, IAuditService audit) : IAuthService
{
    public async Task<User?> LoginAsync(string username, string password)
    {
        var user = store.Users.FirstOrDefault(x =>
            x.IsActive &&
            (x.Username.Equals(username, StringComparison.OrdinalIgnoreCase) ||
             x.Email.Equals(username, StringComparison.OrdinalIgnoreCase)));

        if (user is null)
        {
            return null;
        }

        session.SignIn(user);
        await audit.LogAsync(user.Id, "Login mock", "Users", user.Id);
        return user;
    }

    public async Task<(bool Success, string Message)> RegisterAsync(string username, string password, string email, string fullName)
    {
        if (store.Users.Any(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase) || x.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
        {
            return (false, "Username sau email deja folosit.");
        }

        var role = store.Roles.First(x => x.Name == "User");
        var user = new User
        {
            Id = store.Users.Max(x => x.Id) + 1,
            Username = username.Trim(),
            Email = email.Trim(),
            FullName = fullName.Trim(),
            RoleId = role.Id,
            Role = role,
            IsActive = true
        };
        store.Users.Add(user);
        await audit.LogAsync(user.Id, "Register mock", "Users", user.Id);
        return (true, "Cont creat cu succes in modul UI mock.");
    }

    public Task ChangePasswordAsync(int userId, string oldPassword, string newPassword) => Task.CompletedTask;
}

public class MockCatalogService(MockDataStore store) : ICatalogService
{
    public Task<List<Category>> GetCategoriesAsync() => Task.FromResult(store.Categories.OrderBy(x => x.Name).ToList());

    public Task<List<Medicine>> SearchMedicinesAsync(string? query, int? categoryId = null, int skip = 0, int take = 40)
    {
        IEnumerable<Medicine> products = store.Medicines.Where(x => x.IsActive);
        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            products = products.Where(x =>
                x.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                (x.GenericName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (x.Category?.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (categoryId.HasValue)
        {
            products = products.Where(x => x.CategoryId == categoryId.Value);
        }

        return Task.FromResult(products.OrderByDescending(x => x.IsPromotion).ThenByDescending(x => x.IsNew).Skip(skip).Take(take).ToList());
    }

    public Task<List<Medicine>> GetFeaturedAsync(string section) =>
        Task.FromResult(store.Medicines.OrderByDescending(x => x.Rating).Take(8).ToList());

    public Task<Medicine?> GetMedicineAsync(int id) => Task.FromResult(store.Medicines.FirstOrDefault(x => x.Id == id));

    public Task SaveMedicineAsync(Medicine medicine)
    {
        if (medicine.Id == 0)
        {
            medicine.Id = store.Medicines.Max(x => x.Id) + 1;
            medicine.Category = store.Categories.FirstOrDefault(x => x.Id == medicine.CategoryId);
            store.Medicines.Add(medicine);
        }
        else
        {
            var index = store.Medicines.FindIndex(x => x.Id == medicine.Id);
            if (index >= 0)
            {
                medicine.Category = store.Categories.FirstOrDefault(x => x.Id == medicine.CategoryId);
                store.Medicines[index] = medicine;
            }
        }

        return Task.CompletedTask;
    }

    public Task DeleteMedicineAsync(int id)
    {
        var product = store.Medicines.FirstOrDefault(x => x.Id == id);
        if (product is not null)
        {
            product.IsActive = false;
        }

        return Task.CompletedTask;
    }

    public Task SaveCategoryAsync(Category category)
    {
        if (category.Id == 0)
        {
            category.Id = store.Categories.Max(x => x.Id) + 1;
            store.Categories.Add(category);
        }
        else
        {
            var existing = store.Categories.FirstOrDefault(x => x.Id == category.Id);
            if (existing is not null)
            {
                existing.Name = category.Name;
                existing.Description = category.Description;
                existing.ParentCategoryId = category.ParentCategoryId;
            }
        }

        return Task.CompletedTask;
    }

    public Task<List<Medicine>> GetLowStockAsync() =>
        Task.FromResult(store.Medicines.Where(x => x.StockQuantity <= x.ReorderLevel).ToList());
}

public class MockCartService(MockDataStore store) : ICartService
{
    public Task<List<CartItem>> GetCartAsync(int userId)
    {
        var items = store.CartItems
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.AddedAt)
            .ToList();
        foreach (var item in items)
        {
            item.Medicine = store.Medicines.FirstOrDefault(x => x.Id == item.MedicineId);
        }

        return Task.FromResult(items);
    }

    public Task AddToCartAsync(int userId, int medicineId, int quantity = 1)
    {
        var existing = store.CartItems.FirstOrDefault(x => x.UserId == userId && x.MedicineId == medicineId);
        if (existing is null)
        {
            store.CartItems.Add(new CartItem
            {
                Id = store.NextCartItemId(),
                UserId = userId,
                MedicineId = medicineId,
                Medicine = store.Medicines.FirstOrDefault(x => x.Id == medicineId),
                Quantity = quantity,
                AddedAt = DateTime.UtcNow
            });
        }
        else
        {
            existing.Quantity += quantity;
        }

        return Task.CompletedTask;
    }

    public Task UpdateQuantityAsync(int cartItemId, int quantity)
    {
        var item = store.CartItems.FirstOrDefault(x => x.Id == cartItemId);
        if (item is not null)
        {
            item.Quantity = Math.Max(1, quantity);
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(int cartItemId)
    {
        store.CartItems.RemoveAll(x => x.Id == cartItemId);
        return Task.CompletedTask;
    }

    public Task ClearAsync(int userId)
    {
        store.CartItems.RemoveAll(x => x.UserId == userId);
        return Task.CompletedTask;
    }
}

public class MockOrderService(MockDataStore store, ICartService cart) : IOrderService
{
    public async Task<Order> CheckoutAsync(int userId, string shippingAddress, string paymentMethod, string? prescriptionSourcePath)
    {
        var cartItems = await cart.GetCartAsync(userId);
        if (cartItems.Count == 0)
        {
            throw new InvalidOperationException("Cosul este gol.");
        }

        if (cartItems.Any(x => x.Medicine?.IsPrescriptionRequired == true) && string.IsNullOrWhiteSpace(prescriptionSourcePath))
        {
            throw new InvalidOperationException("Comanda contine produse cu prescriptie. Incarca un PDF.");
        }

        var order = new Order
        {
            Id = store.NextOrderId(),
            UserId = userId,
            User = store.Users.FirstOrDefault(x => x.Id == userId),
            OrderDate = DateTime.UtcNow,
            OrderNumber = $"MOCK-{DateTime.Now:yyyyMMdd-HHmmss}",
            PaymentMethod = paymentMethod,
            ShippingAddress = shippingAddress,
            Status = paymentMethod == "Card" ? "Platita" : "Noua",
            PrescriptionUploadUrl = prescriptionSourcePath
        };

        foreach (var item in cartItems)
        {
            order.Items.Add(new OrderItem
            {
                Id = order.Items.Count + 1,
                OrderId = order.Id,
                MedicineId = item.MedicineId,
                Medicine = item.Medicine,
                Quantity = item.Quantity,
                UnitPrice = item.Medicine?.UnitPrice ?? 0,
                TotalPrice = item.LineTotal
            });
        }

        order.TotalAmount = order.Items.Sum(x => x.TotalPrice);
        order.Discount = Math.Round(order.Items.Sum(x => x.TotalPrice * ((x.Medicine?.DiscountPercent ?? 0) / 100m)), 2);
        order.Tax = Math.Round((order.TotalAmount - order.Discount) * 0.09m, 2);
        order.GrandTotal = order.TotalAmount - order.Discount + order.Tax;
        order.InvoicePath = "Mock invoice - baza de date va fi conectata ulterior";

        store.Orders.Add(order);
        await cart.ClearAsync(userId);
        return order;
    }

    public Task<List<Order>> GetOrdersForUserAsync(int userId) =>
        Task.FromResult(store.Orders.Where(x => x.UserId == userId).OrderByDescending(x => x.OrderDate).ToList());

    public Task<List<Order>> GetAllOrdersAsync() =>
        Task.FromResult(store.Orders.OrderByDescending(x => x.OrderDate).ToList());

    public Task MarkShippedAsync(int orderId)
    {
        var order = store.Orders.FirstOrDefault(x => x.Id == orderId);
        if (order is not null)
        {
            order.Status = "Expediata";
        }

        return Task.CompletedTask;
    }
}

public class MockReportService(MockDataStore store) : IReportService
{
    public Task<string> GenerateInvoiceAsync(Order order) => Task.FromResult(order.InvoicePath ?? "Mock invoice");
    public Task<string> ExportSalesExcelAsync(DateTime from, DateTime to) => Task.FromResult($"Mock raport Excel: {store.Orders.Count} comenzi");
    public Task<string> ExportSalesPdfAsync(DateTime from, DateTime to) => Task.FromResult($"Mock raport PDF: {store.Orders.Count} comenzi");
}

public class MockAuditService(MockDataStore store) : IAuditService
{
    public Task LogAsync(int? userId, string action, string tableName, int? recordId = null)
    {
        store.AuditLogs.Insert(0, new AuditLog
        {
            Id = store.AuditLogs.Count + 1,
            UserId = userId,
            User = userId.HasValue ? store.Users.FirstOrDefault(x => x.Id == userId.Value) : null,
            Action = action,
            TableName = tableName,
            RecordId = recordId,
            Timestamp = DateTime.UtcNow
        });
        return Task.CompletedTask;
    }

    public Task<List<AuditLog>> GetLogsAsync() => Task.FromResult(store.AuditLogs.Take(300).ToList());
}

public class MockAdminDashboardService(MockDataStore store) : IAdminDashboardService
{
    public Task<AdminMetrics> GetMetricsAsync() => Task.FromResult(new AdminMetrics
    {
        ClientsCount = store.Users.Count(x => x.Role?.Name == "User"),
        PharmacistsCount = store.Users.Count(x => x.Role?.Name == "Pharmacist"),
        OrdersCount = store.Orders.Count,
        ProductsCount = store.Medicines.Count(x => x.IsActive),
        LowStockCount = store.Medicines.Count(x => x.StockQuantity <= x.ReorderLevel)
    });
}

public class MockUserService(MockDataStore store, AppSession session) : IUserService
{
    public Task<List<User>> GetUsersAsync() => Task.FromResult(store.Users.OrderBy(x => x.Username).ToList());

    public Task ToggleActiveAsync(int userId)
    {
        var user = store.Users.FirstOrDefault(x => x.Id == userId);
        if (user is not null)
        {
            user.IsActive = !user.IsActive;
        }

        return Task.CompletedTask;
    }

    public Task UpdateProfileAsync(int userId, string fullName, string email)
    {
        var user = store.Users.FirstOrDefault(x => x.Id == userId);
        if (user is not null)
        {
            user.FullName = fullName;
            user.Email = email;
            if (session.CurrentUser?.Id == userId)
            {
                session.SignIn(user);
            }
        }

        return Task.CompletedTask;
    }
}
