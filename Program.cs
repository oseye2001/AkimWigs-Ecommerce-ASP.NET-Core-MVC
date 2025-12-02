using AkimWigs.Infrastructure.Data;
using AkimWigs.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// 1) DB Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2) Session (pour le panier)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";        // où aller si non connecté
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
    });

builder.Services.AddAuthorization();

// 3) MVC
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();


var app = builder.Build();
Stripe.StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// 4) SEED : catégories + 6 wigs
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    db.Database.EnsureCreated();

    // On supprime tout pour forcer un seed propre
    db.Products.RemoveRange(db.Products);
    db.Categories.RemoveRange(db.Categories);
    db.SaveChanges();

    var laceFront = new Category { Name = "Lace Front Wigs" };
    var closure = new Category { Name = "Closure Wigs" };
    var frontal = new Category { Name = "Frontal Wigs" };

    db.Categories.AddRange(laceFront, closure, frontal);

    var wigs = new List<Product>
    {
        new Product
        {
            Name = "Akim Lace Front 14\" Straight",
            Description = "...",
            Price = 180m,
            Color = "Natural Black",
            Length = "14\"",
            Texture = "Straight",
            ImageUrl = "/images/wigs/akim-lace-front-14-straight.png",
            Category = laceFront
        },
        new Product
        {
            Name = "Akim Lace Front 18\" Body Wave",
            Description = "...",
            Price = 220m,
            Color = "Natural Black",
            Length = "18\"",
            Texture = "Body Wave",
            ImageUrl = "/images/wigs/akim-lace-front-18-bodywave.png",
            Category = laceFront
        },
        new Product
        {
            Name = "Akim Closure 16\" Curly",
            Description = "...",
            Price = 210m,
            Color = "Dark Brown",
            Length = "16\"",
            Texture = "Curly",
            ImageUrl = "/images/wigs/akim-closure-16-curly.png",
            Category = closure
        },
        new Product
        {
            Name = "Akim Closure 20\" Deep Wave",
            Description = "...",
            Price = 240m,
            Color = "Natural Black",
            Length = "20\"",
            Texture = "Deep Wave",
            ImageUrl = "/images/wigs/akim-closure-20-deepwave.png",
            Category = closure
        },
        new Product
        {
            Name = "Akim Frontal 18\" Straight",
            Description = "...",
            Price = 260m,
            Color = "Jet Black",
            Length = "18\"",
            Texture = "Straight",
            ImageUrl = "/images/wigs/akim-frontal-18-straight.png",
            Category = frontal
        },
        new Product
        {
            Name = "Akim Frontal 22\" Body Wave",
            Description = "...",
            Price = 290m,
            Color = "Natural Black",
            Length = "22\"",
            Texture = "Body Wave",
            ImageUrl = "/images/wigs/akim-frontal-22-bodywave.png",
            Category = frontal
        }
    };

    db.Products.AddRange(wigs);
    db.SaveChanges();
}



// 5) Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Page d'accueil = catalogue
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.Run();
