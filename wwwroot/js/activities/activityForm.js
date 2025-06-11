const ActivityForm = {
    initialized: false, // 添加初始化标志
    
    resetForm: function() {
        $("#activityTitle").val('');
        $("#activityDescription").val('');
        $("#activityDate").val('');
        $("#activityLocation").val('');
    },
    
    init: function() {
        // 防止重复初始化
        if (this.initialized) {
            console.log("ActivityForm已经初始化，跳过");
            return;
        }
        
        console.log("ActivityForm初始化...");
        
        // 创建新活动 - 使用off().on()防止重复绑定
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
                    $("#createActivityModal").modal("hide");
                    // 刷新页面显示新活动
                    location.reload();
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
                }
            });
        });
        
        this.initialized = true; // 标记为已初始化
    }
};