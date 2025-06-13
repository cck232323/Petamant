(function() {
    // 检查全局对象是否已存在
    if (window.Auth) {
        console.log("Auth已经存在，跳过重复定义");
        return;
    }
    
    // 定义Auth对象
    window.Auth = {
        initialized: false,
        
        init: function() {
            if (this.initialized) {
                console.log("Auth已经初始化，跳过");
                return;
            }
            
            console.log("Auth模块初始化...");
            this.initialized = true;
            console.log("Auth初始化完成");
        },
        
        isAuthenticated: function() {
            console.log("检查用户是否已登录");
            return this.getToken() !== null;
        },
        
        getToken: function() {
            return localStorage.getItem("token");
        },
        
        getUserId: function() {
            const token = this.getToken();
            if (!token) return 0;
            
            try {
                const payload = JSON.parse(atob(token.split('.')[1]));
                console.log("JWT payload:", payload);
                
                return parseInt(
                    payload.nameid || 
                    payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || 
                    0
                );
            } catch (e) {
                console.error("解析JWT失败:", e);
                return 0;
            }
        }
    };
    
    console.log("auth.js 已加载，Auth 对象已定义");
})();

// 登录函数
function login(email, password) {
    return $.ajax({
        url: "/api/users/login",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            email: email,
            password: password
        })
    });
}

// 注册函数
function register(userName, email, password) {
    return $.ajax({
        url: "/api/users/register",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            userName: userName,
            email: email,
            password: password
        })
    });
}