// Movie Booking Page JavaScript - Multi-step booking system
let selectedSeats = [];
let currentStep = 1;

// Seat configuration
const seatConfig = {
    'A': { count: 11, type: 'standard', price: 70000 },
    'B': { count: 11, type: 'standard', price: 70000 },
    'C': { count: 9, type: 'standard', price: 70000 },
    'D': { count: 9, type: 'vip', price: 100000 },
    'E': { count: 9, type: 'vip', price: 100000 },
    'F': { count: 9, type: 'vip', price: 100000 },
    'G': { count: 9, type: 'vip', price: 100000 },
    'H': { count: 9, type: 'vip', price: 100000 },
    'I': { count: 10, type: 'vip', price: 100000 },
    'J': { count: 8, type: 'couple', price: 150000 }
};

const takenSeats = ['A5', 'B3', 'D7', 'F2', 'H8', 'J4']; // Example taken seats

// Initialize seats functionality
function initializeSeats() {
    generateSeatMap();
    updateUI();
}

// Generate seat map
function generateSeatMap() {
    const seatMap = document.getElementById('seat-map');
    
    if (!seatMap) return; // Safety check

    Object.keys(seatConfig).forEach(row => {
        const seatRow = document.createElement('div');
        seatRow.className = 'seat-row';

        // Row label
        const rowLabel = document.createElement('div');
        rowLabel.className = 'row-label';
        rowLabel.textContent = row;
        seatRow.appendChild(rowLabel);

        // Generate seats
        for (let i = seatConfig[row].count; i >= 1; i--) {
            const seatId = row + i;
            const seat = document.createElement('button');
            seat.className = `seat ${seatConfig[row].type}`;
            seat.textContent = seatId;
            seat.dataset.seatId = seatId;
            seat.dataset.price = seatConfig[row].price;

            if (takenSeats.includes(seatId)) {
                seat.classList.add('taken');
                seat.disabled = true;
            } else {
                seat.addEventListener('click', () => toggleSeat(seat));
            }

            seatRow.appendChild(seat);
        }

        seatMap.appendChild(seatRow);
    });
}

// Toggle seat selection
function toggleSeat(seat) {
    const seatId = seat.dataset.seatId;
    const price = parseInt(seat.dataset.price);

    if (seat.classList.contains('selected')) {
        seat.classList.remove('selected');
        selectedSeats = selectedSeats.filter(s => s.id !== seatId);
    } else {
        seat.classList.add('selected');
        selectedSeats.push({ id: seatId, price: price });
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
        ? selectedSeats.map(s => s.id).join(', ')
        : '...';

    const totalAmount = calculateTotalAmount();
    const formattedTotal = totalAmount.toLocaleString('vi-VN');

    const selectedSeatsEl = document.getElementById('selected-seats');
    const totalAmountEl = document.getElementById('total-amount');
    const mobileTotal = document.getElementById('mobile-total');
    
    if (selectedSeatsEl) selectedSeatsEl.textContent = selectedSeatsText;
    if (totalAmountEl) totalAmountEl.textContent = formattedTotal;
    if (mobileTotal) mobileTotal.textContent = formattedTotal;

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
    const continueBtn = document.getElementById('continue-btn');
    const backBtn = document.getElementById('back-btn');
    
    if (!continueBtn) return;

    switch (currentStep) {
        case 1: // Seat selection
            if (selectedSeats.length > 0) {
                continueBtn.removeAttribute('disabled');
                continueBtn.classList.remove('disabled');
                continueBtn.style.pointerEvents = 'auto';
                continueBtn.style.cursor = 'pointer';
            } else {
                continueBtn.setAttribute('disabled', 'true');
                continueBtn.classList.add('disabled');
                continueBtn.style.pointerEvents = 'none';
                continueBtn.style.cursor = 'not-allowed';
            }
            break;

        case 2: // Concession (optional step)
        case 3: // Payment
            continueBtn.removeAttribute('disabled');
            continueBtn.classList.remove('disabled');
            continueBtn.style.pointerEvents = 'auto';
            continueBtn.style.cursor = 'pointer';
            break;

        case 4: // Ticket info
            continueBtn.style.display = 'none';
            break;
    }
    
    // Back button visibility
    if (backBtn) {
        if (currentStep > 1) {
            backBtn.style.display = 'block';
        } else {
            backBtn.style.display = 'none';
        }
    }
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
