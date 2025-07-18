document.addEventListener('DOMContentLoaded', function() {
    // Lấy token CSRF từ form
    const antiForgeryToken = document.querySelector('input[name="__RequestVerificationToken"]').value;
    
    // Khởi tạo các nút hủy vé
    document.querySelectorAll('.btn-cancel').forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            const bookingId = this.getAttribute('data-booking-id');
            showCancelConfirmation(bookingId);
        });
    });
    
    function showCancelConfirmation(bookingId) {
        Swal.fire({
            title: 'Xác nhận hủy vé?',
            text: "Lưu ý: Vé chỉ có thể hủy trước 1 ngày so với ngày chiếu. Bạn có chắc chắn muốn hủy vé này không?",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Xác nhận hủy',
            cancelButtonText: 'Không'
        }).then((result) => {
            if (result.isConfirmed) {
                processCancelBooking(bookingId);
            }
        });
    }
    
    function processCancelBooking(bookingId) {
        // Hiển thị loading
        Swal.fire({
            title: 'Đang xử lý...',
            text: 'Vui lòng chờ trong giây lát.',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
        
        // Gửi yêu cầu hủy vé
        fetch('/Customer/History/CancelBooking', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': antiForgeryToken
            },
            body: `bookingId=${bookingId}`
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Hủy vé thành công!',
                    text: data.message || 'Vé đã được hủy thành công và yêu cầu hoàn tiền đã được xử lý.',
                    confirmButtonColor: '#28a745'
                }).then(() => {
                    // Reload trang để cập nhật trạng thái
                    window.location.reload();
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi!',
                    text: data.message,
                    confirmButtonColor: '#dc3545'
                });
            }
        })
        .catch(error => {
            console.error('Error:', error);
            Swal.fire({
                icon: 'error',
                title: 'Lỗi!',
                text: 'Đã xảy ra lỗi trong quá trình hủy vé. Vui lòng thử lại sau.',
                confirmButtonColor: '#dc3545'
            });
        });
    }
});
