using System;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder; // Assurez-vous que votre namespace est correct
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services au conteneur.
builder.Services.AddControllersWithViews();

// Configuration pour la session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Durée de vie de la session
    options.Cookie.HttpOnly = true;  // Le cookie est uniquement accessible via HTTP (pas JavaScript)
    options.Cookie.IsEssential = true;  // Assurez-vous que les cookies sont utilisés même en l'absence de consentement
});

// Configurer le DbContext pour utiliser SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Créer l'application
var app = builder.Build();

// Configurer le pipeline de requêtes HTTP.
app.UseSession();  // Ajouter ici pour que la session fonctionne indépendamment de l'environnement

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}




app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();