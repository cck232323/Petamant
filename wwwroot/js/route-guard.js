document.addEventListener('DOMContentLoaded', function() {
    // 获取当前页面路径
    const path = window.location.pathname;
    
    // 需要登录才能访问的页面列表
    const protectedRoutes = [
        '/Home/Activities',
        '/Home/Profile',
        '/Home/MyActivities'
    ];
    
    // 检查当前页面是否需要登录
    const requiresAuth = protectedRoutes.some(route => path.startsWith(route));
    
    if (requiresAuth) {
        // 获取token
        const token = localStorage.getItem('token');
        
        // 检查token是否存在且未过期
        if (!token) {
            // 直接重定向，不显示警告
            window.location.replace('/Home/Index');
            return; // 阻止页面继续加载
        }
        
        // 检查token是否过期
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            const expiry = payload.exp * 1000; // 转换为毫秒
            
            if (Date.now() >= expiry) {
                // token已过期，清除并重定向
                localStorage.removeItem('token');
                window.location.replace('/Home/Index');
                return; // 阻止页面继续加载
            }
        } catch (e) {
            console.error("解析JWT失败:", e);
            localStorage.removeItem('token');
            window.location.replace('/Home/Index');
            return; // 阻止页面继续加载
        }
    }
});