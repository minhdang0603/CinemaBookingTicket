document.addEventListener('DOMContentLoaded', function() {
    // Handle cancel booking
    const cancelButtons = document.querySelectorAll('.btn-cancel');
    cancelButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            const bookingId = this.getAttribute('data-booking-id');


            if (confirm('Bạn có chắc chắn muốn hủy vé này?')) {
                // Simple form submission to controller
                const form = document.createElement('form');
                form.method = 'POST';
                form.action = '/Customer/History/CancelBooking';
                
                const input = document.createElement('input');
                input.type = 'hidden';
                input.name = 'bookingId';
                input.value = bookingId;
                
                const token = document.querySelector('input[name="__RequestVerificationToken"]');
                if (token) {
                    form.appendChild(token.cloneNode());
                }
                
                form.appendChild(input);
                document.body.appendChild(form);
                form.submit();
            }
        });
    });


});