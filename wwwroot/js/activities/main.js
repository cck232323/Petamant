const ActivityManager = {
    init: function() {
        console.log("ActivityManager初始化...");
        
        // 检查登录状态
        if (!Auth.checkLogin()) {
            console.log("用户未登录，重定向到登录页面");
            return;
        }
        
        // 初始化各个组件
        console.log("初始化活动列表...");
        ActivityList.init();
        
        console.log("初始化活动表单...");
        ActivityForm.init();
        
        console.log("初始化活动详情...");
        ActivityDetail.init();
    }
};

// 页面加载完成后初始化
$(document).ready(function() {
    ActivityManager.init();
});
