const ActivityList = {
    initialized: false, // 添加初始化标志
    
    init: function() {
        // 防止重复初始化
        if (this.initialized) {
            console.log("ActivityList已经初始化，跳过");
            return;
        }
        
        console.log("ActivityList初始化...");
        
        // 加载活动列表
        this.loadActivities();
        
        // 绑定搜索按钮事件 - 使用off().on()防止重复绑定
        $("#searchBtn").off("click").on("click", function() {
            ActivityList.loadActivities($("#searchTerm").val());
        });
        
        // 绑定活动项点击事件
        $(document).off("click", ".activity-item").on("click", ".activity-item", function() {
            const activityId = $(this).data("id");
            console.log("点击活动项，ID:", activityId);
            ActivityDetail.loadActivityDetails(activityId);
        });

        // 绑定创建活动按钮事件
        $("#createActivityBtn").off("click").on("click", function() {
            ActivityForm.resetForm();
        });
        
        // 绑定活动详情按钮事件
        $(".activityDetailsBtn").off("click").on("click", function(e) {
            e.preventDefault();
            const activityId = $(this).data("id");
            ActivityDetail.loadActivityDetails(activityId);
        });
        
        // 绑定快速报名按钮事件
        $(".registerBtn").off("click").on("click", function(e) {
            e.preventDefault();
            const activityId = $(this).data("id");
            
            // 弹出模态框收集宠物信息
            const petInfo = prompt("请输入您的宠物信息:", "");
            if (petInfo) {
                Registration.registerForActivity(activityId, petInfo);
            }
        });
        
        this.initialized = true; // 标记为已初始化
    },
    
    loadActivities: function(searchTerm) {
        console.log("加载活动列表，搜索词:", searchTerm);
        
        // 显示加载中
        $("#activityList").html(`
            <div class="text-center">
                <div class="spinner-border" role="status">
                    <span class="sr-only">加载中...</span>
                </div>
            </div>
        `);
        
        // 构建API URL
        let url = "/api/activities";
        if (searchTerm) {
            url += `?searchTerm=${encodeURIComponent(searchTerm)}`;
        }
        
        // 发送AJAX请求
        $.ajax({
            url: url,
            type: "GET",
            headers: Auth.getToken() ? { "Authorization": "Bearer " + Auth.getToken() } : {},
            success: function(activities) {
                console.log("成功获取活动列表:", activities);
                
                if (activities.length === 0) {
                    $("#activityList").html("<p>暂无活动</p>");
                    return;
                }
                
                let activitiesHtml = "";
                activities.forEach(function(activity) {
                    activitiesHtml += `
                        <div class="col-md-6 mb-4">
                            <div class="card h-100">
                                <div class="card-body">
                                    <h5 class="card-title">${activity.title}</h5>
                                    <h6 class="card-subtitle mb-2 text-muted">
                                        ${new Date(activity.date).toLocaleDateString()} | ${activity.location}
                                    </h6>
                                    <p class="card-text">
                                        ${activity.description.substring(0, 100)}${activity.description.length > 100 ? '...' : ''}
                                    </p>
                                </div>
                                <div class="card-footer">
                                    <small class="text-muted">创建者: ${activity.creatorUserName || '未知'} | 报名人数: ${activity.registrationsCount || 0}</small>
                                    <a href="#" class="btn btn-primary float-right activity-item" data-id="${activity.id}">查看详情</a>
                                </div>
                            </div>
                        </div>
                    `;
                });
                
                $("#activityList").html(`<div class="row">${activitiesHtml}</div>`);
            },
            error: function(xhr) {
                console.error("获取活动列表失败:", xhr);
                $("#activityList").html(`
                    <div class="alert alert-danger">
                        加载活动失败: ${xhr.status} ${xhr.statusText}
                    </div>
                `);
            }
        });
    }
};