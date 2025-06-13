(function() {
    // 检查全局对象是否已存在
    if (window.ActivityForm) {
        console.log("ActivityForm已经存在，跳过重复定义");
        return;
    }
    
    // 定义ActivityForm对象
    window.ActivityForm = {
        initialized: false,
        
        init: function() {
            console.log("ActivityForm.init() 被调用");
            
            if (this.initialized) {
                console.log("ActivityForm已经初始化，跳过");
                return;
            }
            
            this.initialized = true;
            console.log("ActivityForm初始化...");
            
            // 绑定创建活动按钮事件
            $("#createActivityBtn").click(function() {
                ActivityForm.resetForm();
                $("#createActivityModal").modal("show");
            });
            
            // 绑定提交按钮事件
            $("#submitActivityBtn").off("click").on("click", function() {
                const title = $("#activityTitle").val();
                const description = $("#activityDescription").val();
                const date = $("#activityDate").val();
                const location = $("#activityLocation").val();
                
                if (!title || !description || !date || !location) {
                    alert("请填写所有必填字段");
                    return;
                }
                
                // 调试信息
                console.log("提交的表单数据:", {
                    title,
                    description,
                    date,
                    location
                });
                
                // 确保日期格式正确
                const formattedDate = new Date(date).toISOString();
                console.log("格式化后的日期:", formattedDate);
                
                const requestData = {
                    title: title,
                    description: description,
                    date: formattedDate,
                    location: location
                };
                
                console.log("发送的JSON数据:", JSON.stringify(requestData));
                
                // 禁用提交按钮，防止重复提交
                $("#submitActivityBtn").prop("disabled", true).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> 提交中...');
                
                $.ajax({
                    url: "/api/activities",
                    type: "POST",
                    headers: {
                        "Authorization": "Bearer " + Auth.getToken(),
                        "Content-Type": "application/json"
                    },
                    data: JSON.stringify(requestData),
                    processData: false,
                    success: function(response) {
                        console.log("创建活动成功:", response);
                        alert("活动创建成功！");
                        
                        // 关闭模态框
                        $("#createActivityModal").modal("hide");
                        
                        // 重置表单
                        ActivityForm.resetForm();
                        
                        // 重新加载活动列表
                        if (typeof ActivityList !== 'undefined') {
                            ActivityList.loadActivities();
                        } else {
                            // 如果ActivityList不可用，则刷新页面
                            location.reload();
                        }
                    },
                    error: function(xhr, status, error) {
                        console.error("创建活动失败状态:", status);
                        console.error("创建活动失败错误:", error);
                        console.error("创建活动失败响应:", xhr.responseText);
                        
                        let errorMessage = "创建活动失败";
                        
                        try {
                            if (xhr.responseText) {
                                const errorResponse = JSON.parse(xhr.responseText);
                                console.log("解析的错误响应:", errorResponse);
                                
                                if (errorResponse.errors) {
                                    errorMessage += ":\n";
                                    for (const field in errorResponse.errors) {
                                        errorMessage += `- ${field}: ${errorResponse.errors[field].join(', ')}\n`;
                                    }
                                } else if (errorResponse.message) {
                                    errorMessage += ": " + errorResponse.message;
                                } else if (errorResponse.title) {
                                    errorMessage += ": " + errorResponse.title;
                                }
                            }
                        } catch (e) {
                            console.error("解析错误响应失败:", e);
                            errorMessage += ": " + xhr.responseText || error;
                        }
                        
                        alert(errorMessage);
                    },
                    complete: function() {
                        // 无论成功还是失败，都重新启用提交按钮
                        $("#submitActivityBtn").prop("disabled", false).html('创建');
                    }
                });
            });
            
            console.log("ActivityForm初始化完成");
        },
        
        resetForm: function() {
            $("#activityTitle").val('');
            $("#activityDescription").val('');
            $("#activityDate").val('');
            $("#activityLocation").val('');
        }
    };
    
    console.log("activity-form.js 已加载，ActivityForm 对象已定义");
})();