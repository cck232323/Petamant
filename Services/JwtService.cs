// // 确保在生成令牌时包含了用户ID
// using System.Security.Claims;

// List<Claim> claims = new List<Claim>
// {
//     new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//     // 或者使用 "sub" 声明
//     new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
//     // 其他声明...
// };