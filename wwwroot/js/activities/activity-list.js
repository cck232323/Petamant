(function() {
    // 检查全局对象是否已存在
    if (window.ActivityList) {
        console.log("ActivityList已经存在，跳过重复定义");
        return;
    }
    
    // 定义ActivityList对象
    window.ActivityList = {
        initialized: false,
        isLoading: false,
        loadTimeout: null,
        
        init: function() {
            console.log("ActivityList.init() 被调用");
            
            if (this.initialized) {
                console.log("ActivityList已经初始化，跳过");
                return;
            }
            
            this.initialized = true;
            console.log("ActivityList初始化...");
            
            // 显示活动内容区域
            $("#auth-loading").hide();
            $("#activities-content").show();
            
            // 加载活动列表
            this.loadActivities();
            
            // 绑定搜索按钮事件
            $("#searchBtn").off("click").on("click", function() {
                ActivityList.loadActivities($("#searchTerm").val());
            });
            
            // 绑定活动项点击事件 - 使用事件委托
            $(document).off("click", ".view-activity-btn").on("click", ".view-activity-btn", function(e) {
                e.preventDefault();
                const activityId = $(this).data("id");
                console.log("查看活动详情，ID:", activityId);
                
                if (typeof ActivityDetail !== 'undefined') {
                    ActivityDetail.loadActivityDetails(activityId);
                } else {
                    console.error("ActivityDetail 未定义，无法加载活动详情");
                    alert("无法加载活动详情，请刷新页面重试");
                }
            });
            
            // 绑定创建活动按钮事件
            $("#createActivityBtn").off("click").on("click", function() {
                if (typeof ActivityForm !== 'undefined') {
                    ActivityForm.resetForm();
                    $("#createActivityModal").modal("show");
                } else {
                    console.error("ActivityForm 未定义，无法创建活动");
                    alert("无法创建活动，请刷新页面重试");
                }
            });
            
            console.log("ActivityList初始化完成");
        },
        
        loadActivities: function(searchTerm) {
            // 如果已经在加载中，则取消
            if (this.isLoading) return;
            
            this.isLoading = true;
            
            // 显示加载中
            $("#activitiesList").html(`
                <div class="text-center">
                    <div class="spinner-border" role="status">
                        <span class="sr-only">加载中...</span>
                    </div>
                </div>
            `);
            
            // 使用setTimeout避免频繁请求
            clearTimeout(this.loadTimeout);
            this.loadTimeout = setTimeout(() => {
                let url = "/api/activities";
                if (searchTerm) {
                    url += `?searchTerm=${encodeURIComponent(searchTerm)}`;
                }
                
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
                                            <button class="btn btn-primary float-right view-activity-btn" data-id="${activity.id}">查看详情</button>
                                        </div>
                                    </div>
                                </div>
                            `;
                        });
                        
                        $("#activityList").html(activitiesHtml);
                    },
                    error: function(xhr) {
                        console.error("获取活动列表失败:", xhr);
                        $("#activityList").html(`
                            <div class="alert alert-danger">
                                加载活动失败: ${xhr.status} ${xhr.statusText}
                            </div>
                        `);
                    },
                    complete: () => {
                        this.isLoading = false;
                    }
                });
            }, 300); // 300毫秒防抖
        }
    };
    
    console.log("activity-list.js 已加载，ActivityList 对象已定义");
})();