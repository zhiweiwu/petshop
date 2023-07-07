using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using petshop.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
// if would like to effect all page use this method, otherwise use[filter] on top of action to work only for specific action
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<CartCountAsyncActionFilter>();
}); 
builder.Services.AddDbContext<MyDbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();   
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

//create cartCount filter that will activeated before and after all the action is done
public class CartCountAsyncActionFilter : IAsyncActionFilter
{   //IHttpContextAccessor can fetch the interface of httpcontext,httpcontext including all the seesion and reqire and respose,route.
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MyDbContext _context;

    public CartCountAsyncActionFilter(IHttpContextAccessor httpContextAccessor, MyDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var carts = await _context.Carts.Include(c => c.CartItems).ToListAsync();
        var cart = carts.FirstOrDefault(c => c.CartItems != null && c.CartItems.Count > 0);
        var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        // if not sign out or cart is not empty  return quantity
        if (cart != null && userId !=null)
        {
            int cartItemCount = cart.CartItems.Sum(ci => ci.Quantity);
            _httpContextAccessor.HttpContext.Session.SetInt32("CartItem", cartItemCount);
        }
        else
        {
            _httpContextAccessor.HttpContext.Session.SetInt32("CartItem", 0);
        }
        //next method is to make sure the program will run in the right way, wont block, if there is mutiple filter method
        await next();
    }
}
