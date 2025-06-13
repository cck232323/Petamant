document.addEventListener('DOMContentLoaded', function() {
    console.log("main.js 开始执行");
    
    // 检查Auth模块是否存在
    if (typeof Auth === 'undefined') {
        console.error("Auth模块未定义！页面可能无法正常工作");
        // 显示错误信息
        if (document.getElementById('auth-loading')) {
            document.getElementById('auth-loading').innerHTML = `
                <div class="alert alert-danger">
                    <h4>加载错误</h4>
                    <p>认证模块未加载，请刷新页面或联系管理员。</p>
                    <button class="btn btn-primary" onclick="location.reload()">刷新页面</button>
                </div>
            `;
        }
        return;
    }
    
    // 初始化Auth模块
    if (typeof Auth.init === 'function') {
        Auth.init();
    }
    
    // // 检查登录状态
    // try {
    //     const isLoggedIn = Auth.checkLoginWithExpiry();
    //     console.log("登录状态检查结果:", isLoggedIn);
        
    //     if (!isLoggedIn) {
    //         console.log("用户未登录，重定向到登录页面");
    //         window.location.href = "/Home/Index";
    //         return;
    //     }
        
    //     // 初始化页面组件
    //     ActivityManager.init();
    // } catch (e) {
    //     console.error("检查登录状态时出错:", e);
    //     if (document.getElementById('auth-loading')) {
    //         document.getElementById('auth-loading').innerHTML = `
    //             <div class="alert alert-danger">
    //                 <h4>发生错误</h4>
    //                 <p>检查登录状态时出错: ${e.message}</p>
    //                 <button class="btn btn-primary" onclick="location.reload()">刷新页面</button>
    //             </div>
    //         `;
    //     }
    // }
});

// 使用IIFE避免全局命名空间污染
(function() {
    // 检查全局对象是否已存在
    if (window.ActivityManager) {
        console.log("ActivityManager已经存在，跳过重复定义");
        return;
    }
    
    // 定义ActivityManager对象
    window.ActivityManager = {
        init: function() {
            console.log("ActivityManager初始化...");
            
            try {
                // 初始化各个组件
                if (typeof ActivityList !== 'undefined' && typeof ActivityList.init === 'function') {
                    console.log("初始化活动列表...");
                    ActivityList.init();
                } else {
                    console.error("ActivityList 未定义或缺少init方法");
                }
                
                if (typeof ActivityForm !== 'undefined' && typeof ActivityForm.init === 'function') {
                    console.log("初始化活动表单...");
                    ActivityForm.init();
                } else {
                    console.log("ActivityForm模块未定义或缺少init方法");
                }
                
                if (typeof ActivityDetail !== 'undefined' && typeof ActivityDetail.init === 'function') {
                    console.log("初始化活动详情...");
                    ActivityDetail.init();
                } else {
                    console.log("ActivityDetail模块未定义或缺少init方法");
                }
                
                if (typeof Registration !== 'undefined' && typeof Registration.init === 'function') {
                    console.log("初始化报名模块...");
                    Registration.init();
                } else {
                    console.log("Registration模块未定义或缺少init方法");
                }
            } catch (e) {
                console.error("初始化组件时出错:", e);
                alert("加载页面组件时出错，请刷新页面重试");
            }
        }
    };
    
    console.log("main.js 已加载，ActivityManager 对象已定义");
})();

$(document).ready(function() {
    console.log("DOM加载完成，开始初始化所有模块...");
    
    // 首先初始化Auth模块
    if (typeof Auth !== 'undefined') {
        console.log("Auth对象存在，开始初始化");
        if (typeof Auth.init === 'function') {
            Auth.init();
        } else {
            console.error("Auth.init不是一个函数");
        }
    } else {
        console.error("Auth对象未定义！请检查auth.js是否正确加载");
    }
    
    // 然后初始化其他模块
    if (typeof Registration !== 'undefined' && typeof Registration.init === 'function') {
        Registration.init();
    }
    
    if (typeof ActivityDetail !== 'undefined' && typeof ActivityDetail.init === 'function') {
        ActivityDetail.init();
    }
    
    if (typeof ActivityForm !== 'undefined' && typeof ActivityForm.init === 'function') {
        ActivityForm.init();
    }
    
    if (typeof ActivityList !== 'undefined' && typeof ActivityList.init === 'function') {
        ActivityList.init();
    }
    
    console.log("所有模块初始化完成");
    
    // 隐藏加载提示，显示内容
    $("#auth-loading").hide();
    
    // 检查用户是否已登录
    if (typeof Auth !== 'undefined' && typeof Auth.isAuthenticated === 'function') {
        if (Auth.isAuthenticated()) {
            $("#activities-content").show();
        } else {
            $("#login-required").show();
        }
    } else {
        console.error("无法检查用户登录状态，Auth.isAuthenticated不可用");
        $("#activities-content").show(); // 默认显示内容
    }
});
