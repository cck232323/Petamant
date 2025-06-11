const Notification = {
    // 显示成功消息
    showSuccess: function(message) {
        this._showNotification(message, 'success');
    },
    
    // 显示错误消息
    showError: function(message) {
        this._showNotification(message, 'danger');
    },
    
    // 显示警告消息
    showWarning: function(message) {
        this._showNotification(message, 'warning');
    },
    
    // 显示信息消息
    showInfo: function(message) {
        this._showNotification(message, 'info');
    },
    
    // 内部方法：显示通知
    _showNotification: function(message, type) {
        // 创建通知元素
        const notificationId = 'notification-' + Date.now();
        const notification = `
            <div id="${notificationId}" class="alert alert-${type} alert-dismissible fade show" role="alert">
                ${message}
                <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
        `;
        
        // 检查通知容器是否存在，如果不存在则创建
        let notificationContainer = $('#notification-container');
        if (notificationContainer.length === 0) {
            $('body').append('<div id="notification-container" style="position: fixed; top: 20px; right: 20px; z-index: 9999;"></div>');
            notificationContainer = $('#notification-container');
        }
        
        // 添加通知到容器
        notificationContainer.append(notification);
        
        // 5秒后自动关闭
        setTimeout(function() {
            $(`#${notificationId}`).alert('close');
        }, 5000);
    },
    
    // 确认对话框
    confirm: function(message) {
        return confirm(message);
    }
};