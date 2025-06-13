(function() {
    // 检查全局对象是否已存在
    if (window.ActivityDetail) {
        console.log("ActivityDetail已经存在，跳过重复定义");
        return;
    }
    
    // 定义ActivityDetail对象
    window.ActivityDetail = {
        currentActivityId: null,
        initialized: false,
        
        init: function() {
            console.log("ActivityDetail.init() 被调用");
            
            if (this.initialized) {
                console.log("ActivityDetail已经初始化，跳过");
                return;
            }
            
            this.initialized = true;
            console.log("ActivityDetail初始化...");
            
            // 绑定模态框事件
            $("#activityDetailModal").on("hidden.bs.modal", function() {
                // 模态框关闭时重置状态
                ActivityDetail.currentActivityId = null;
            });
            
            // 只修改报名按钮事件处理程序
            $("#modalRegisterBtn").off("click").on("click", function() {
                if (ActivityDetail.currentActivityId) {
                    // 检查用户是否登录
                    if (!Auth.isAuthenticated()) {
                        alert("请先登录后再报名");
                        window.location.href = "/Home/Index";
                        return;
                    }
                    
                    // 显示报名模态框
                    $("#registrationModal").modal("show");
                    
                    // 确保Registration对象已初始化并设置当前活动ID
                    if (typeof Registration !== 'undefined') {
                        Registration.currentActivityId = ActivityDetail.currentActivityId;
                        if (typeof Registration.resetForm === 'function') {
                            Registration.resetForm();
                        }
                    }
                }
            });
            
            // 绑定删除按钮事件
            $("#deleteActivityBtn").off("click").on("click", function() {
                if (ActivityDetail.currentActivityId) {
                    ActivityDetail.deleteActivity(ActivityDetail.currentActivityId);
                }
            });
            
            console.log("ActivityDetail初始化完成");
        },
        
        loadActivityDetails: function(activityId) {
            console.log("加载活动详情，ID:", activityId);
            this.currentActivityId = activityId;
            
            // 显示加载中
            $("#activityDetailsContent").html(`
                <div class="text-center">
                    <div class="spinner-border" role="status">
                        <span class="sr-only">加载中...</span>
                    </div>
                </div>
            `);
            
            // 显示模态框
            $("#activityDetailModal").modal("show");
            
            // 发送AJAX请求
            $.ajax({
                url: `/api/activities/${activityId}`,
                type: "GET",
                headers: Auth.getToken() ? { "Authorization": "Bearer " + Auth.getToken() } : {},
                success: function(activity) {
                    console.log("成功获取活动详情:", activity);
                    
                    // 检查当前用户是否是活动创建者
                    const currentUserId = Auth.getUserId();
                    const isCreator = activity.creatorUserId === currentUserId;
                    
                    // 检查当前页面是否是"我的活动"页面
                    const isMyActivitiesPage = window.location.pathname.includes("/Home/MyActivities");
                    
                    // 只有在"我的活动"页面且用户是创建者时才显示删除按钮
                    $("#deleteActivityBtn").toggle(isMyActivitiesPage && isCreator);
                    
                    // 加载报名信息
                    ActivityDetail.loadRegistrations(activityId, function(registrations) {
                        let registrationsHtml = "";
                        if (registrations && registrations.length > 0) {
                            registrationsHtml = `
                                <h5 class="mt-4">已报名用户 (${registrations.length})</h5>
                                <ul class="list-group">
                            `;
                            registrations.forEach(function(reg) {
                                registrationsHtml += `
                                    <li class="list-group-item">
                                        <strong>${reg.userName}</strong>
                                        <p><small>宠物信息: ${reg.petInfo}</small></p>
                                    </li>
                                `;
                            });
                            registrationsHtml += "</ul>";
                        } else {
                            registrationsHtml = "<p>暂无人报名</p>";
                        }
                        
                        // 构建详情HTML
                        var detailsHtml = `
                            <h3>${activity.title}</h3>
                            <p class="text-muted">创建者: ${activity.creatorUserName}</p>
                            <p><strong>日期:</strong> ${new Date(activity.date).toLocaleString()}</p>
                            <p><strong>地点:</strong> ${activity.location}</p>
                            <p><strong>描述:</strong></p>
                            <p>${activity.description}</p>
                            <hr>
                            ${registrationsHtml}
                        `;
                        
                        $("#activityDetailsContent").html(detailsHtml);
                    });
                },
                error: function(xhr) {
                    console.error("获取活动详情失败:", xhr);
                    $("#activityDetailsContent").html(`
                        <div class="alert alert-danger">
                            加载活动详情失败: ${xhr.status} ${xhr.statusText}
                        </div>
                    `);
                }
            });
        },
        
        loadRegistrations: function(activityId, callback) {
            $.ajax({
                url: `/api/activities/${activityId}/registrations`,
                type: "GET",
                headers: Auth.getToken() ? { "Authorization": "Bearer " + Auth.getToken() } : {},
                success: function(registrations) {
                    console.log("成功获取报名信息:", registrations);
                    callback(registrations);
                },
                error: function(xhr) {
                    console.error("获取报名信息失败:", xhr);
                    callback([]);
                }
            });
        },
        
        registerForActivity: function(activityId, petInfo) {
            const userId = Auth.getUserId();
            if (!userId) {
                alert("请先登录");
                return;
            }
            
            // 显示加载状态
            $("#modalRegisterBtn").prop("disabled", true).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> 处理中...');
            
            $.ajax({
                url: "/api/registrations",
                type: "POST",
                headers: {
                    "Authorization": "Bearer " + Auth.getToken(),
                    "Content-Type": "application/json"
                },
                data: JSON.stringify({
                    activityId: activityId,
                    userId: userId,
                    petInfo: petInfo
                }),
                success: function(response) {
                    alert("报名成功！");
                    
                    // 关闭模态框
                    $("#activityDetailModal").modal("hide");
                    $("#registrationModal").modal("hide");
                    
                    // 重新加载活动详情
                    setTimeout(function() {
                        ActivityDetail.loadActivityDetails(activityId);
                    }, 500);
                },
                error: function(xhr) {
                    alert("报名失败: " + (xhr.responseJSON ? xhr.responseJSON.message : xhr.responseText));
                },
                complete: function() {
                    // 恢复按钮状态
                    $("#modalRegisterBtn").prop("disabled", false).html('报名参加');
                }
            });
        },
        
        deleteActivity: function(activityId) {
            if (confirm("确定要删除这个活动吗？此操作不可撤销。")) {
                $.ajax({
                    url: `/api/activities/${activityId}`,
                    type: "DELETE",
                    headers: {
                        "Authorization": "Bearer " + Auth.getToken()
                    },
                    success: function() {
                        alert("活动已成功删除");
                        $("#activityDetailModal").modal("hide");
                        // 重新加载活动列表
                        if (typeof ActivityList !== 'undefined') {
                            ActivityList.loadActivities();
                        }
                    },
                    error: function(xhr) {
                        alert("删除活动失败: " + (xhr.responseJSON ? xhr.responseJSON.message : xhr.responseText));
                    }
                });
            }
        }
    };
    
    console.log("activity-detail.js 已加载，ActivityDetail 对象已定义");
})();