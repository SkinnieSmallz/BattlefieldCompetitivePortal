//using Microsoft.AspNetCore.Authentication.Cookies;
//using BattlefieldCompetitivePortal.Framework.Services;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container
//builder.Services.AddControllersWithViews();

//// Register your Framework services
//builder.Services.AddScoped<UserService>();

//// Cookie Authentication for MVC
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/User/Login";
//        options.LogoutPath = "/User/Logout";
//        options.AccessDeniedPath = "/User/AccessDenied";
//        options.ExpireTimeSpan = TimeSpan.FromDays(1);
//        options.SlidingExpiration = true;
//    });

//builder.Services.AddAuthorization();

//var app = builder.Build();

//// Configure the HTTP request pipeline
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
