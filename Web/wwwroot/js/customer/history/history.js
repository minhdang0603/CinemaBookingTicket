document.addEventListener('DOMContentLoaded', function() {
    // Handle action buttons
    const actionButtons = document.querySelectorAll('.btn-action');
    actionButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            const action = this.textContent.trim();
            console.log('Action clicked:', action);
            
            if (action.includes('Hiện vé')) {
                alert('Đang hiển thị mã QR vé...');
            } else if (action.includes('Hủy vé')) {
                if (confirm('Bạn có chắc chắn muốn hủy vé này?')) {
                    console.log('Ticket cancelled');
                }
            } else if (action.includes('Tải vé')) {
                console.log('Downloading ticket...');
            }
        });
    });

    // Hover effects for tickets - removed as we're using CSS transitions
});
