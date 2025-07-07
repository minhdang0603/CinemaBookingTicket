// Movie Booking Page JavaScript - Multi-step booking system
let selectedSeats = [];
let currentStep = 1;

// Initialize seats functionality
function initializeSeats() {
    attachSeatHandlers();
    updateUI();
}

// Attach click handlers to seats
function attachSeatHandlers() {
    document.querySelectorAll('.seat:not(.taken)').forEach(seat => {
        seat.addEventListener('click', () => toggleSeat(seat));
    });
}

// Toggle seat selection
function toggleSeat(seat) {
    const seatId = seat.dataset.seatId;
    const price = parseInt(seat.dataset.price);
    const seatCode = seat.dataset.seatCode;

    if (seat.classList.contains('selected')) {
        seat.classList.remove('selected');
        selectedSeats = selectedSeats.filter(s => s.id !== seatId);
    } else {
        seat.classList.add('selected');
        selectedSeats.push({ id: seatId, price: price, code: seatCode });
    }

    updateUI();
    
    // Update payment summary if we're past step 1
    if (currentStep > 1 && typeof updateSummary === 'function') {
        updateSummary();
    }
}

// Update UI
function updateUI() {
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

// Export functions for global access
if (typeof window !== 'undefined') {
    window.selectedSeats = selectedSeats;
    window.initializeSeats = initializeSeats;
    window.updateUI = updateUI;
    window.updateButtonStates = updateButtonStates;
    window.resetBookingData = resetBookingData;
    window.calculateTotalAmount = calculateTotalAmount;
}
