var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Vegetables/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseExceptionHandler("/Vegetables/Error");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Vegetables}/{action=SecondViewMethod}/{id?}");

app.Run();
