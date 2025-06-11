const Auth = {
    // 获取存储的token
    getToken: function() {
        return localStorage.getItem("token");
    },
    
    // 检查是否已登录
    checkLogin: function() {
        const token = this.getToken();
        if (!token) {
            alert("请先登录！");
            window.location.href = "/Home/Index";
            return false;
        }
        return true;
    },
    
    // 获取当前用户ID
    getUserId: function() {
        const token = this.getToken();
        if (!token) return 0;
        
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            console.log("JWT payload in getUserId:", payload);
            
            // 尝试所有可能的用户ID字段，确保转换为整数
            // let userId = 0;
            
            // 检查标准声明
            if (payload.nameid) {
                userId = parseInt(payload.nameid);
            } 
            // 检查XML命名空间声明
            else if (payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]) {
                userId = parseInt(payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]);
            }
            
            console.log("解析出的用户ID:", userId);
            
            if (isNaN(userId) || userId === 0) {
                console.warn("无法从JWT中获取有效的用户ID");
                return 0;
            }
            
            return userId;
        } catch (e) {
            console.error("解析JWT失败:", e);
            return 0;
        }
    },
    
    getUserName: function() {
        const token = this.getToken();
        if (!token) return "";
        
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            return payload.unique_name || payload.name || "";
        } catch (e) {
            console.error("解析JWT失败:", e);
            return "";
        }
    }
};