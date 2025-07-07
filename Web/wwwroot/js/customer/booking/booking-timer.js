// booking-timer.js - Handles the booking countdown timer using only data-expiry
$(document).ready(function () {
    // Check if we have a timer element on the page
    const timerElement = document.getElementById('booking-timer');
    if (!timerElement) return;

    // Get the initial expiry time from the data attribute
    let expiryTime = timerElement.getAttribute('data-expiry');
    if (!expiryTime) return;

    // Parse the expiry time
    const expiryDate = new Date(expiryTime);

    // Function to update the countdown timer
    function updateTimer() {
        const now = new Date();
        let remainingSeconds = Math.max(0, Math.floor((expiryDate - now) / 1000));
        if (remainingSeconds <= 0) {
            timerElement.textContent = 'Hết thời gian';
            timerElement.classList.add('text-danger');
            cancelBookingAndRedirect();
            return;
        }
        // Calculate minutes and seconds
        const minutes = Math.floor(remainingSeconds / 60);
        const seconds = remainingSeconds % 60;


        timerElement.textContent = `${minutes}:${seconds.toString().padStart(2, '0')}`;

        console.log(timerElement.textContent);

        // Add visual indication when time is running low
        if (remainingSeconds < 60) {
            timerElement.classList.add('text-danger', 'blink');
        } else if (remainingSeconds < 120) {
            timerElement.classList.add('text-warning');
        }
        setTimeout(updateTimer, 1000);
    }

    // Function to cancel booking and redirect
    function cancelBookingAndRedirect() {
        $.ajax({
            url: '/Customer/Booking/CancelBooking',
            type: 'POST',
            success: function(response) {
                Swal.fire({
                    icon: 'error',
                    title: 'Hết thời gian đặt vé',
                    text: 'Phiên đặt vé của bạn đã hết hạn. Vui lòng đặt lại.',
                    confirmButtonText: 'Quay về trang chủ'
                }).then(() => {
                    window.location.href = '/';
                });
            },
            error: function() {
                // Ngay cả khi API gặp lỗi, chúng ta vẫn chuyển hướng người dùng
                window.location.href = '/';
            }
        });
    }

    // Start the countdown
    updateTimer();
});
