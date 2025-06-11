const ActivityDetail = {
    loadActivityDetails: function(activityId) {
        // 设置模态框中的报名按钮数据
        $("#modalRegisterBtn").data("id", activityId);
        
        // 加载活动详情
        $.ajax({
            url: `/api/activities/${activityId}`,
            type: "GET",
            headers: {
                "Authorization": "Bearer " + Auth.getToken()
            },
            success: function(activity) {
                // 检查当前用户是否为活动创建者
                const userId = Auth.getUserId();
                if (activity.creatorUserId === userId) {
                    // 如果是创建者，隐藏报名按钮
                    $("#modalRegisterBtn").hide();
                } else {
                    // 如果不是创建者，显示报名按钮
                    $("#modalRegisterBtn").show();
                    
                    // 检查用户是否已报名
                    const isRegistered = activity.registrations && 
                        activity.registrations.some(r => r.userId === userId);
                    
                    if (isRegistered) {
                        // 如果已报名，禁用报名按钮
                        $("#modalRegisterBtn").prop('disabled', true).text('已报名');
                    } else {
                        // 如果未报名，启用报名按钮
                        $("#modalRegisterBtn").prop('disabled', false).text('报名参加');
                    }
                }
                
                const detailsHtml = `
                    <h3>${activity.title}</h3>
                    <p class="text-muted">创建者: ${activity.creatorUserName}</p>
                    <p><strong>日期:</strong> ${new Date(activity.date).toLocaleString()}</p>
                    <p><strong>地点:</strong> ${activity.location}</p>
                    <p><strong>描述:</strong></p>
                    <p>${activity.description}</p>
                    <p><strong>已报名人数:</strong> ${activity.registrationsCount}</p>
                    <hr>
                    <h5>报名参加</h5>
                    <div class="form-group">
                        <label for="petInfo">宠物信息</label>
                        <textarea class="form-control" id="petInfo" rows="2" placeholder="请描述您的宠物信息（品种、年龄等）"></textarea>
                    </div>
                `;
                
                $("#activityDetailsContent").html(detailsHtml);
            },
            error: function(error) {
                $("#activityDetailsContent").html(`<div class="alert alert-danger">加载活动详情失败: ${error.responseText}</div>`);
            }
        });
    },
    
    init: function() {
        // 确保只绑定一次事件
        $("#modalRegisterBtn").off('click').on('click', function() {
            const activityId = $(this).data("id");
            const petInfo = $("#petInfo").val();
            
            if (!petInfo) {
                alert("请输入宠物信息");
                return;
            }
            
            Registration.registerForActivity(activityId, petInfo);
        });
    }
};