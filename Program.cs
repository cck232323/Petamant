using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyDotnetApp.Data;
using MyDotnetApp.Services;
using System.Text;
using dotenv.net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.DataProtection;
var builder = WebApplication.CreateBuilder(args);
DotEnv.Load();
builder.Configuration
    .AddEnvironmentVariables();
// 添加配置文件
// 添加在 DotEnv.Load() 和 Configuration 初始化之后：
// builder.Configuration["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");
// 如果使用 Docker Compose，取消注释以下行并设置环境变量
builder.Configuration["ConnectionStrings:DefaultConnection"] =
    Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
    ?? "Host=db;Database=petactivities;Username=postgres;Password=postgres";
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
        var allowedOrigins = (Environment.GetEnvironmentVariable("ALLOWED_ORIGINS") ?? "http://localhost:5173")
            .Split(',')
            .Select(o => o.Trim())
            .ToArray();
            
        builder.WithOrigins(allowedOrigins)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// 添加服务注册
// builder.Services.AddScoped<IUserService, UserService>();
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

// 如果需要使用PostgreSQL，请取消注释以下行并确保已安装Npgsql.EntityFrameworkCore.PostgreSQL包
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// for docker-compose postgresql
var connectionString = builder.Configuration["ConnectionStrings__DefaultConnection"] 
                       ?? builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("No database connection string provided.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
// 添加AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);


// 添加服务
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
// 添加其他服务...
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"))  // 改为容器内可写的目录
    .SetApplicationName("PetamantApp");

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
builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenAnyIP(80);   // HTTP
    o.ListenAnyIP(443);  // HTTPS，证书由环境变量提供
});
// builder.WebHost.ConfigureKestrel(options =>
// {
//     // 如果容器里使用 443 端口，也暴露 443；本地开发可以仍然跑 5001
//     options.ListenAnyIP(443, listenOptions =>
//     {
//         listenOptions.UseHttps(
//             // new X509Certificate2("/https/aspnetapp.pfx", "Aq@112211")
//             new X509Certificate2("/https/aspnetapp.pfx", "Aq@112211", X509KeyStorageFlags.MachineKeySet)
//         );  // 路径 & 密码

//     });

//     // 保留 HTTP（如果你还需要 80 或 5000）
//     options.ListenAnyIP(80);
// });

var app = builder.Build();

// 初始化数据库
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var retryCount = 0;
    const int maxRetries = 10;
    
    while (retryCount < maxRetries)
    {
        try
        {
            logger.LogInformation($"尝试连接数据库 (尝试 {retryCount+1}/{maxRetries})");
            var context = services.GetRequiredService<ApplicationDbContext>();
            
            // 检查数据库连接
            bool canConnect = false;
            try 
            {
                canConnect = context.Database.CanConnect();
                logger.LogInformation($"数据库连接测试: {(canConnect ? "成功" : "失败")}");
            }
            catch (Exception ex)
            {
                logger.LogWarning($"数据库连接测试异常: {ex.Message}");
            }
            
            if (canConnect)
            {
                // 确保数据库已创建
                context.Database.EnsureCreated();
                
                // 初始化数据库
                DbInitializer.Initialize(context);
                logger.LogInformation("数据库初始化成功");
                break;
            }
            else
            {
                throw new Exception("无法连接到数据库");
            }
        }
        catch (Exception ex)
        {
            retryCount++;
            logger.LogWarning(ex, $"初始化数据库时出错 (尝试 {retryCount}/{maxRetries})");
            
            if (retryCount >= maxRetries)
            {
                logger.LogError(ex, "达到最大重试次数，无法初始化数据库");
                // 可以选择继续运行应用或抛出异常终止
                // 在开发环境中，我们可以继续运行应用
                if (app.Environment.IsDevelopment())
                {
                    logger.LogWarning("在开发环境中继续运行应用，但数据库功能可能不可用");
                    break;
                }
                else
                {
                    // 在生产环境中，如果数据库不可用，可能需要终止应用
                    throw;
                }
            }
            
            // 等待一段时间后重试
            logger.LogInformation($"等待 {2 * retryCount} 秒后重试...");
            Thread.Sleep(2000 * retryCount); // 递增等待时间
        }
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
Console.WriteLine("连接字符串: " + builder.Configuration.GetConnectionString("DefaultConnection"));
app.Run();