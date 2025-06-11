const MyActivities = {
    loadCreatedActivities: function() {
        const userId = Auth.getUserId();
        
        $.ajax({
            url: `/api/activities/user/${userId}/created`,
            type: "GET",
            headers: {
                "Authorization": "Bearer " + Auth.getToken()
            },
            success: function(activities) {
                if (activities.length === 0) {
                    $("#createdActivities").html("<p>您还没有创建任何活动</p>");
                    return;
                }
                
                var activitiesHtml = "<div class='list-group'>";
                activities.forEach(function(activity) {
                    activitiesHtml += `
                        <div class="list-group-item">
                            <div class="d-flex w-100 justify-content-between">
                                <h5 class="mb-1">${activity.title}</h5>
                                <small>${new Date(activity.date).toLocaleDateString()}</small>
                            </div>
                            <p class="mb-1">${activity.description.substring(0, 100)}${activity.description.length > 100 ? '...' : ''}</p>
                            <small>地点: ${activity.location} | 报名人数: ${activity.registrationsCount}</small>
                            <div class="mt-2">
                                <button class="btn btn-sm btn-info view-activity-btn" data-id="${activity.id}">查看详情</button>
                                <button class="btn btn-sm btn-danger delete-activity-btn" data-id="${activity.id}">删除活动</button>
                            </div>
                        </div>
                    `;
                });
                activitiesHtml += "</div>";
                
                $("#createdActivities").html(activitiesHtml);
                
                // 绑定删除按钮事件
                $(".delete-activity-btn").click(function() {
                    const activityId = $(this).data("id");
                    MyActivities.deleteActivity(activityId);
                });
                
                // 绑定查看详情按钮事件
                $(".view-activity-btn").click(function() {
                    const activityId = $(this).data("id");
                    ActivityDetail.loadActivityDetails(activityId);
                    $("#activityDetailsModal").modal("show");
                });
            },
            error: function(error) {
                $("#createdActivities").html(`<div class="alert alert-danger">加载活动失败</div>`);
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
                    // 重新加载创建的活动列表
                    MyActivities.loadCreatedActivities();
                },
                error: function(xhr) {
                    alert("删除活动失败: " + (xhr.responseJSON ? xhr.responseJSON.message : xhr.responseText));
                }
            });
        }
    },
    
    // ... 其他方法 ...
    
    init: function() {
        // 加载用户创建的活动
        this.loadCreatedActivities();
        
        // 加载用户报名的活动
        this.loadRegisteredActivities();
    }
};