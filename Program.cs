using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyDotnetApp.Data;
using MyDotnetApp.Services;
using System.Text;
using dotenv.net;
var builder = WebApplication.CreateBuilder(args);
DotEnv.Load();
builder.Configuration
    .AddEnvironmentVariables();
// 添加配置文件
// 添加在 DotEnv.Load() 和 Configuration 初始化之后：
builder.Configuration["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");
builder.Configuration["Jwt:Key"] = Environment.GetEnvironmentVariable("JWT_KEY");
builder.Configuration["Jwt:Issuer"] = Environment.GetEnvironmentVariable("JWT_ISSUER");
builder.Configuration["Jwt:Audience"] = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
builder.Configuration["Jwt:ExpiryMinutes"] = Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES");
// 添加服务到容器
builder.Services.AddControllers();
builder.Services.AddControllersWithViews(); // 在 builder.Services.AddControllers(); 之后添加
builder.Services.AddEndpointsApiExplorer(); 

// 添加 CORS 服务
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:5173") // 前端应用的地址
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
               
    });
});

// 添加服务注册
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "宠物活动聚合社交平台 API", Version = "v1" });
    
    // 添加JWT认证配置到Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 使用内存数据库替代PostgreSQL
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseInMemoryDatabase("PetActivitiesDb"));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 添加AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// 添加服务
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
// 添加其他服务...

// 配置JWT认证
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"] ?? "defaultSecretKey12345678901234567890"))
        };
        // options.MapInboundClaims = false;
    });

var app = builder.Build();

// 初始化数据库
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "初始化数据库时出错");
    }
}

// 配置HTTP请求管道
app.UseSwagger();
app.UseSwaggerUI();

// 启用 CORS
app.UseCors("AllowFrontend");

// 配置静态文件中间件 - 确保在认证中间件之前
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // 对于静态文件请求不要求身份验证
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=600");
    }
});

// 添加路由中间件
app.UseRouting();

// 认证和授权中间件应该在路由之后，终结点之前
app.UseAuthentication();
app.UseAuthorization();

// 映射控制器和路由
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();