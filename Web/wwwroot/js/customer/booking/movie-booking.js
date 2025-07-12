// Movie Booking Page JavaScript - Multi-step booking system
let selectedSeats = [];
let currentStep = 1;

// Initialize seats functionality
function initializeSeats() {
    console.log("Initializing seats...");
    
    // Kiểm tra nếu có booking hiện có từ ViewBag
    const existingBookingId = document.getElementById('existing-booking-id')?.value;
    const existingBookedSeats = document.getElementById('existing-booked-seats')?.value;
    
    if (existingBookingId && existingBookedSeats) {
        console.log("Found existing booking:", existingBookingId);
        
        // Xóa mảng selectedSeats hiện tại để tránh trùng lặp
        selectedSeats = [];
        
        const seatIds = existingBookedSeats.split(',');
        
        // Chọn tất cả các ghế đã đặt trước đó
        seatIds.forEach(seatId => {
            const seatIdTrimmed = seatId.trim();
            const seat = document.querySelector(`.seat[data-seat-id="${seatIdTrimmed}"]`);
            if (seat) {
                // Make sure the seat is not marked as unavailable for current user's booking
                seat.classList.remove('unavailable');
                if (seat.hasAttribute('disabled')) {
                    seat.removeAttribute('disabled');
                }
                
                const price = parseInt(seat.dataset.price);
                const seatCode = seat.dataset.seatCode;
                
                seat.classList.add('selected');
                
                // Kiểm tra xem ghế đã có trong mảng selectedSeats chưa
                const existingSeat = selectedSeats.find(s => s.id === seatIdTrimmed);
                if (!existingSeat) {
                    selectedSeats.push({ id: seatIdTrimmed, price: price, code: seatCode });
                    console.log("Pre-selected seat:", seatIdTrimmed, seatCode);
                }
            }
        });
        
        // Hiển thị thông báo cho người dùng
        Swal.fire({
            title: 'Đặt vé chưa hoàn tất',
            text: 'Chúng tôi đã khôi phục lựa chọn ghế từ đơn đặt vé trước đó của bạn.',
            icon: 'info',
            confirmButtonText: 'Tiếp tục'
        });
    }
    
    // Remove existing event listeners first to prevent duplicates
    removeExistingSeatHandlers();
    
    // Then attach new ones
    attachSeatHandlers();
    
    // Setup continue button handlers
    setupContinueButtonHandlers();
    
    // Update UI to reflect current state
    updateUI();
    
    console.log("Seats initialization complete. Selected seats:", selectedSeats);
}

// Remove existing event listeners to prevent duplicates
function removeExistingSeatHandlers() {
    document.querySelectorAll('.seat:not(.taken)').forEach(seat => {
        // Clone the node to remove all event listeners
        const oldSeat = seat;
        const newSeat = oldSeat.cloneNode(true);
        if (oldSeat.parentNode) {
            oldSeat.parentNode.replaceChild(newSeat, oldSeat);
        }
    });
}

// Attach click handlers to seats
function attachSeatHandlers() {
    console.log("Attaching seat handlers...");
    const seats = document.querySelectorAll('.seat:not(.taken)');
    console.log(`Found ${seats.length} available seats`);
    
    seats.forEach(seat => {
        seat.addEventListener('click', function() {
            console.log("Seat clicked:", seat.dataset.seatCode);
            toggleSeat(seat);
        });
    });
}

// Toggle seat selection
function toggleSeat(seat) {
    if (!seat) {
        console.error("Toggle called with invalid seat");
        return;
    }
    
    const seatId = seat.dataset.seatId;
    const price = parseInt(seat.dataset.price) || 0;
    const seatCode = seat.dataset.seatCode;
    
    console.log(`Toggle seat: ID=${seatId}, Code=${seatCode}, Price=${price}`);

    // Kiểm tra xem có booking hiện có không
    const existingBookingId = document.getElementById('existing-booking-id')?.value;
    const existingBookedSeats = document.getElementById('existing-booked-seats')?.value || '';
    
    if (existingBookingId && existingBookedSeats) {
        const seatIds = existingBookedSeats.split(',').map(id => id.trim());
        
        // Nếu đây là ghế trong booking hiện có và người dùng đang bỏ chọn nó
        if (seatIds.includes(seatId) && seat.classList.contains('selected')) {
            Swal.fire({
                title: 'Xác nhận bỏ chọn ghế',
                text: 'Bạn đang bỏ chọn ghế từ đơn đặt vé trước đó. Điều này sẽ thay đổi đơn đặt vé của bạn khi tiếp tục.',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Bỏ chọn',
                cancelButtonText: 'Giữ ghế này'
            }).then((result) => {
                if (result.isConfirmed) {
                    // Bỏ chọn ghế
                    seat.classList.remove('selected');
                    selectedSeats = selectedSeats.filter(s => s.id !== seatId);
                    updateUI();
                    console.log("Removed seat from selection:", seatId);
                }
            });
            return;
        }
    }

    // Xử lý bình thường cho các trường hợp khác
    if (seat.classList.contains('selected')) {
        seat.classList.remove('selected');
        selectedSeats = selectedSeats.filter(s => s.id !== seatId);
        console.log("Removed seat from selection:", seatId);
    } else {
        seat.classList.add('selected');
        
        // Kiểm tra xem ghế đã có trong mảng chưa
        const existingSeat = selectedSeats.find(s => s.id === seatId);
        if (!existingSeat) {
            selectedSeats.push({ id: seatId, price: price, code: seatCode });
            console.log("Added seat to selection:", seatId);
        } else {
            console.log("Seat already in selection, not adding again:", seatId);
        }
    }

    updateUI();
    
    // Update payment summary if we're past step 1
    if (currentStep > 1 && typeof updateSummary === 'function') {
        updateSummary();
    }
}

// Setup continue button handlers
function setupContinueButtonHandlers() {
    const continueButtons = document.querySelectorAll('#continue-btn, #mobile-continue-btn');
    
    if (!continueButtons || continueButtons.length === 0) {
        console.warn("Continue buttons not found");
        return;
    }
    
    console.log(`Found ${continueButtons.length} continue buttons`);
    
    continueButtons.forEach(btn => {
        // Remove existing listeners
        const oldBtn = btn;
        const newBtn = oldBtn.cloneNode(true);
        if (oldBtn.parentNode) {
            oldBtn.parentNode.replaceChild(newBtn, oldBtn);
        }
        
        // Add new listener
        newBtn.addEventListener('click', function(e) {
            e.preventDefault();
            console.log("Continue button clicked");
            
            if (selectedSeats.length === 0) {
                console.warn("No seats selected");
                return;
            }
            
            const existingBookingId = document.getElementById('existing-booking-id')?.value;
            
            // If this is an existing booking, update it instead of creating a new one
            if (existingBookingId) {
                console.log("Continuing with existing booking:", existingBookingId);
                continueWithExistingBooking();
            } else {
                console.log("Creating new booking");
                createNewBooking();
            }
        });
    });
}

// Hàm loại bỏ các phần tử trùng lặp trong mảng selectedSeats
function removeDuplicateSeats() {
    const uniqueSeats = [];
    const seenIds = new Set();
    
    selectedSeats.forEach(seat => {
        if (!seenIds.has(seat.id)) {
            seenIds.add(seat.id);
            uniqueSeats.push(seat);
        }
    });
    
    // Nếu phát hiện có ghế trùng lặp
    if (uniqueSeats.length < selectedSeats.length) {
        console.log(`Removed ${selectedSeats.length - uniqueSeats.length} duplicate seats`);
        selectedSeats = uniqueSeats;
    }
}

// Update UI
function updateUI() {
    // Loại bỏ các ghế trùng lặp trước khi cập nhật UI
    removeDuplicateSeats();
    
    console.log("Updating UI with", selectedSeats.length, "selected seats");
    
    const selectedSeatsText = selectedSeats.length > 0
        ? selectedSeats.map(s => s.code).join(', ')
        : '...';

    const totalAmount = calculateTotalAmount();
    const formattedTotal = totalAmount.toLocaleString('vi-VN');

    // Update all instances of selected seats text
    const selectedSeatsElements = document.querySelectorAll('#selected-seats');
    selectedSeatsElements.forEach(el => {
        if (el) el.textContent = selectedSeatsText;
    });
    
    // Update all total amount displays
    const totalAmountElements = document.querySelectorAll('#total-amount');
    totalAmountElements.forEach(el => {
        if (el) el.textContent = formattedTotal;
    });
    
    // Update mobile totals
    const mobileTotalElements = document.querySelectorAll('[id^="mobile-total"]');
    mobileTotalElements.forEach(el => {
        if (el) el.textContent = formattedTotal;
    });

    updateButtonStates();
}

// Calculate total amount including seats and combos
function calculateTotalAmount() {
    const seatTotal = selectedSeats.reduce((sum, seat) => sum + seat.price, 0);
    let comboTotal = 0;
    
    if (typeof combos !== 'undefined') {
        comboTotal = Object.values(combos).reduce((sum, combo) => sum + (combo.price * combo.quantity), 0);
    }
    
    return seatTotal + comboTotal;
}

// Update button states based on current step
function updateButtonStates() {
    // Update both desktop and mobile continue buttons
    const continueButtons = document.querySelectorAll('#continue-btn, #mobile-continue-btn');
    if (continueButtons.length === 0) return;

    continueButtons.forEach(btn => {
        switch (currentStep) {
            case 1: // Seat selection
                if (selectedSeats.length > 0) {
                    btn.removeAttribute('disabled');
                    btn.classList.remove('disabled');
                    btn.style.pointerEvents = 'auto';
                    btn.style.cursor = 'pointer';
                } else {
                    btn.setAttribute('disabled', 'true');
                    btn.classList.add('disabled');
                    btn.style.pointerEvents = 'none';
                    btn.style.cursor = 'not-allowed';
                }
                break;

            case 2: // Concession (optional step)
            case 3: // Payment
                btn.removeAttribute('disabled');
                btn.classList.remove('disabled');
                btn.style.pointerEvents = 'auto';
                btn.style.cursor = 'pointer';
                break;
        }
    });
}

// Reset booking data
function resetBookingData() {
    selectedSeats = [];
    
    if (typeof combos !== 'undefined') {
        Object.keys(combos).forEach(key => combos[key].quantity = 0);
    }

    // Reset UI
    document.querySelectorAll('.seat.selected').forEach(seat => {
        seat.classList.remove('selected');
    });

    document.querySelectorAll('.quantity').forEach(qty => {
        qty.textContent = '0';
    });

    document.querySelectorAll('.form-control').forEach(input => {
        input.value = '';
        input.classList.remove('error');
    });

    updateUI();
}

// Add fade-in animation to elements
function initializeAnimations() {
    setTimeout(() => {
        document.querySelectorAll('.fade-in').forEach(el => {
            el.style.opacity = '1';
        });
    }, 100);
}

// Function for continuing with an existing booking
function continueWithExistingBooking() {
    const existingBookingId = document.getElementById('existing-booking-id')?.value;
    const showtimeIdElement = document.getElementById('showtime-id');
    
    if (!existingBookingId || !showtimeIdElement) {
        console.error('Missing booking information, ID:', existingBookingId);
        return;
    }
    
    if (selectedSeats.length === 0) {
        console.error('No seats selected');
        Swal.fire({
            icon: 'error',
            title: 'Lỗi',
            text: 'Vui lòng chọn ít nhất một ghế để tiếp tục'
        });
        return;
    }
    
    // Prepare data for updating booking
    const bookingData = {
        bookingId: parseInt(existingBookingId),
        showTimeId: parseInt(showtimeIdElement.value),
        bookingDetails: selectedSeats.map(seat => ({
            seatId: parseInt(seat.id),
            seatName: seat.code,
            seatPrice: seat.price
        }))
    };
    
    console.log("Updating booking with data:", bookingData);
    
    // Show loading spinner
    Swal.fire({
        title: 'Đang cập nhật đơn đặt vé...',
        text: 'Vui lòng đợi trong giây lát',
        allowOutsideClick: false,
        showConfirmButton: false,
        willOpen: () => {
            Swal.showLoading();
        }
    });
    
    const csrfToken = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
    
    // Call API to update booking
    fetch('/Customer/Booking/UpdateBooking', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': csrfToken
        },
        body: JSON.stringify(bookingData)
    })
    .then(response => {
        console.log("Update booking response status:", response.status);
        return response.json();
    })
    .then(data => {
        console.log("Update booking response data:", data);
        
        if (data.success) {
            // Close loading dialog
            Swal.close();
            
            // Save expiry timer
            if (data.expiryMinutes) {
                const expiryDate = new Date();
                expiryDate.setMinutes(expiryDate.getMinutes() + data.expiryMinutes);
                localStorage.setItem('bookingExpiry', expiryDate.toISOString());
            }
            
            // Redirect to next step
            window.location.href = data.redirect;
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Lỗi',
                text: data.message || 'Đã xảy ra lỗi khi cập nhật đơn đặt vé'
            });
        }
    })
    .catch(error => {
        console.error('Error updating booking:', error);
        Swal.fire({
            icon: 'error',
            title: 'Lỗi',
            text: 'Đã xảy ra lỗi khi kết nối đến máy chủ'
        });
    });
}

// Create new booking function
function createNewBooking() {
    // Ensure we have selected seats
    if (selectedSeats.length === 0) {
        console.error('No seats selected');
        Swal.fire({
            icon: 'error',
            title: 'Lỗi',
            text: 'Vui lòng chọn ít nhất một ghế để tiếp tục'
        });
        return;
    }
    
    const showtimeIdElement = document.getElementById('showtime-id');
    if (!showtimeIdElement) {
        console.error('Missing showtime ID element');
        return;
    }
    
    // Prepare booking data
    const bookingData = {
        showTimeId: parseInt(showtimeIdElement.value),
        bookingDetails: selectedSeats.map(seat => ({
            seatId: parseInt(seat.id),
            seatName: seat.code,
            seatPrice: seat.price
        }))
    };
    
    console.log("Creating booking with data:", bookingData);
    
    // Show loading spinner
    Swal.fire({
        title: 'Đang tạo đơn đặt vé...',
        text: 'Vui lòng đợi trong giây lát',
        allowOutsideClick: false,
        showConfirmButton: false,
        willOpen: () => {
            Swal.showLoading();
        }
    });
    
    const csrfToken = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
    
    // Call API to create booking
    fetch('/Customer/Booking/CreateBooking', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': csrfToken
        },
        body: JSON.stringify(bookingData)
    })
    .then(response => {
        console.log("Create booking response status:", response.status);
        return response.json();
    })
    .then(data => {
        console.log("Create booking response data:", data);
        
        if (data.success) {
            // Close loading dialog
            Swal.close();
            
            // Save expiry timer
            if (data.expiryMinutes) {
                const expiryDate = new Date();
                expiryDate.setMinutes(expiryDate.getMinutes() + data.expiryMinutes);
                localStorage.setItem('bookingExpiry', expiryDate.toISOString());
            }
            
            // Redirect to next step
            window.location.href = data.redirect;
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Lỗi',
                text: data.message || 'Đã xảy ra lỗi khi tạo đơn đặt vé'
            });
        }
    })
    .catch(error => {
        console.error('Error creating booking:', error);
        Swal.fire({
            icon: 'error',
            title: 'Lỗi',
            text: 'Đã xảy ra lỗi khi kết nối đến máy chủ'
        });
    });
}

// Initialize everything when the DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    console.log("DOM fully loaded and parsed");
    initializeSeats();
    initializeAnimations();
});

// Export functions for global access
if (typeof window !== 'undefined') {
    window.selectedSeats = selectedSeats;
    window.initializeSeats = initializeSeats;
    window.updateUI = updateUI;
    window.updateButtonStates = updateButtonStates;
    window.resetBookingData = resetBookingData;
    window.calculateTotalAmount = calculateTotalAmount;
    window.toggleSeat = toggleSeat;
    window.continueWithExistingBooking = continueWithExistingBooking;
    window.createNewBooking = createNewBooking;
}
