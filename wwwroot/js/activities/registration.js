(function() {
    // 检查全局对象是否已存在
    if (window.Registration) {
        console.log("Registration已经存在，跳过重复定义");
        return;
    }
    
    // 定义Registration对象
    window.Registration = {
        initialized: false,
        currentActivityId: null,
        
        init: function() {
            if (this.initialized) {
                console.log("Registration已经初始化，跳过");
                return;
            }
            
            console.log("Registration初始化...");
            
            // 检查Auth对象是否存在
            if (typeof Auth === 'undefined') {
                console.error("Auth模块未定义！请确保auth.js已正确加载");
                alert("认证模块加载失败，请刷新页面重试");
                return;
            }
            
            // 绑定报名按钮事件 - 修改这里，不要使用document.on
            $("#modalRegisterBtn").off("click").on("click", function(e) {
                console.log("报名按钮被点击");
                e.preventDefault(); // 阻止默认行为
                e.stopPropagation(); // 阻止事件冒泡
                
                // 检查用户是否登录
                if (typeof Auth.isAuthenticated === 'function') {
                    if (!Auth.isAuthenticated()) {
                        alert("请先登录后再报名");
                        window.location.href = "/Home/Index";
                        return;
                    }
                } else {
                    console.error("Auth.isAuthenticated 不是一个函数");
                    alert("认证模块加载失败，请刷新页面重试");
                    return;
                }
                
                const activityId = ActivityDetail.currentActivityId;
                if (!activityId) {
                    console.error("未找到活动ID");
                    return;
                }
                
                // 保存当前活动ID
                Registration.currentActivityId = activityId;
                
                // 重置表单
                Registration.resetForm();
                
                // 显示报名模态框
                $("#registrationModal").modal("show");
            });
            
            // 绑定报名表单提交事件
            $("#submitRegistrationBtn").off("click").on("click", function() {
                Registration.submitRegistrationForm();
            });
            
            // 绑定模态框关闭事件
            $("#registrationModal").on("hidden.bs.modal", function() {
                Registration.resetForm();
            });
            
            // 绑定表单字段的实时验证
            $("#petName, #petType, #petDescription").on("input change", function() {
                if ($(this).val().trim() !== "") {
                    $(this).removeClass("is-invalid").addClass("is-valid");
                } else {
                    $(this).removeClass("is-valid").addClass("is-invalid");
                }
            });
            
            $("#agreeTerms").change(function() {
                if ($(this).prop("checked")) {
                    $(this).removeClass("is-invalid");
                } else {
                    $(this).addClass("is-invalid");
                }
            });
            
            this.initialized = true;
            console.log("Registration初始化完成");
        },
        
        resetForm: function() {
            // 重置表单字段
            $("#petName").val("").removeClass("is-valid is-invalid");
            $("#petType").val("").removeClass("is-valid is-invalid");
            $("#petAge").val("").removeClass("is-valid is-invalid");
            $("#petDescription").val("").removeClass("is-valid is-invalid");
            $("#agreeTerms").prop("checked", false).removeClass("is-invalid");
            
            // 隐藏错误信息
            $("#registrationFormError").hide();
        },
        
        validateForm: function() {
            let isValid = true;
            let errorMessage = "";
            
            // 验证宠物名称
            if (!$("#petName").val().trim()) {
                isValid = false;
                errorMessage += "请输入宠物名称<br>";
                $("#petName").removeClass("is-valid").addClass("is-invalid");
            } else {
                $("#petName").removeClass("is-invalid").addClass("is-valid");
            }
            
            // 验证宠物类型
            if (!$("#petType").val()) {
                isValid = false;
                errorMessage += "请选择宠物类型<br>";
                $("#petType").removeClass("is-valid").addClass("is-invalid");
            } else {
                $("#petType").removeClass("is-invalid").addClass("is-valid");
            }
            
            // 验证宠物描述
            if (!$("#petDescription").val().trim()) {
                isValid = false;
                errorMessage += "请描述您的宠物<br>";
                $("#petDescription").removeClass("is-valid").addClass("is-invalid");
            } else {
                $("#petDescription").removeClass("is-invalid").addClass("is-valid");
            }
            
            // 验证是否同意条款
            if (!$("#agreeTerms").prop("checked")) {
                isValid = false;
                errorMessage += "请同意活动规则和条款<br>";
                $("#agreeTerms").addClass("is-invalid");
            } else {
                $("#agreeTerms").removeClass("is-invalid");
            }
            
            // 显示错误信息
            if (!isValid) {
                $("#registrationFormError").html(errorMessage).show();
            } else {
                $("#registrationFormError").hide();
            }
            
            return isValid;
        },
        
        submitRegistrationForm: function() {
            // 表单验证
            if (!this.validateForm()) {
                return;
            }
            
            // 检查用户是否登录和Auth对象
            if (typeof Auth === 'undefined' || typeof Auth.isAuthenticated !== 'function') {
                alert("认证模块加载失败，请刷新页面重试");
                return;
            }
            
            if (!Auth.isAuthenticated()) {
                alert("请先登录后再报名");
                window.location.href = "/Home/Index";
                return;
            }
            
            const token = Auth.getToken();
            const userId = Auth.getUserId();
            
            if (userId === 0) {
                alert("无法获取用户ID，请重新登录");
                return;
            }
            
            // 构建宠物信息对象
            const petInfoObj = {
                name: $("#petName").val().trim(),
                type: $("#petType").val(),
                age: $("#petAge").val() || "未提供",
                description: $("#petDescription").val().trim()
            };
            
            // 将宠物信息对象转换为JSON字符串
            const petInfo = JSON.stringify(petInfoObj);
            
            // 显示加载状态
            $("#submitRegistrationBtn").prop("disabled", true).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> 提交中...');
            
            $.ajax({
                url: "/api/registrations",
                type: "POST",
                headers: {
                    "Authorization": "Bearer " + token,
                    "Content-Type": "application/json"
                },
                data: JSON.stringify({
                    activityId: this.currentActivityId,
                    userId: userId,
                    petInfo: petInfo
                }),
                success: function(response) {
                    console.log("报名成功:", response);
                    
                    // 关闭报名模态框
                    $("#registrationModal").modal("hide");
                    
                    // 显示成功消息
                    alert("报名成功！");
                    
                    // 重新加载活动详情
                    setTimeout(function() {
                        if (typeof ActivityDetail !== 'undefined' && ActivityDetail.loadActivityDetails) {
                            ActivityDetail.loadActivityDetails(Registration.currentActivityId);
                        } else if (typeof ActivityList !== 'undefined' && ActivityList.loadActivities) {
                            ActivityList.loadActivities();
                        }
                    }, 500);
                },
                error: function(xhr, status, error) {
                    console.error("报名失败:", xhr.responseText);
                    
                    let errorMessage = "报名失败";
                    try {
                        if (xhr.responseText) {
                            const errorResponse = JSON.parse(xhr.responseText);
                            if (errorResponse.message) {
                                errorMessage += ": " + errorResponse.message;
                            } else if (errorResponse.title) {
                                errorMessage += ": " + errorResponse.title;
                            }
                        }
                    } catch (e) {
                        errorMessage += ": " + xhr.responseText || error;
                    }
                    
                    // 显示错误信息
                    $("#registrationFormError").html(errorMessage).show();
                },
                complete: function() {
                    // 恢复按钮状态
                    $("#submitRegistrationBtn").prop("disabled", false).html('提交报名');
                }
            });
        }
    };
    
    console.log("registration.js 已加载，Registration 对象已定义");
})();
