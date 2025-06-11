const Registration = {
    registerForActivity: function(activityId, petInfo) {
        if (!Auth.checkLogin()) {
            return;
        }
        
        const token = Auth.getToken();
        const userId = Auth.getUserId();
        
        if (userId === 0) {
            alert("无法获取用户ID，请重新登录");
            return;
        }
        
        $.ajax({
            url: "/api/registrations",
            type: "POST",
            headers: {
                "Authorization": "Bearer " + token
            },
            contentType: "application/json",
            data: JSON.stringify({
                activityId: activityId,
                petInfo: petInfo
            }),
            success: function(response) {
                alert("报名成功！");
                // 关闭模态框（如果有的话）
                $("#activityDetailsModal").modal("hide");
                // 可选：刷新活动列表
                if (typeof ActivityList !== 'undefined' && ActivityList.loadActivities) {
                    ActivityList.loadActivities();
                }
            },
            error: function(xhr) {
                let errorMessage = "报名失败";
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMessage += ": " + xhr.responseJSON.message;
                } else if (xhr.responseText) {
                    try {
                        const errorObj = JSON.parse(xhr.responseText);
                        if (errorObj.message) {
                            errorMessage += ": " + errorObj.message;
                        }
                    } catch (e) {
                        errorMessage += ": " + xhr.responseText;
                    }
                }
                alert(errorMessage);
            }
        });
    }
};