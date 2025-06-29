// Concession and Payment functionality

// Combo data
const combos = {
    family: { price: 213000, quantity: 0 },
    sweet: { price: 88000, quantity: 0 },
    beta: { price: 68000, quantity: 0 }
};

// Concession Functions
function updateComboQuantity(comboType, change) {
    const currentQty = combos[comboType].quantity;
    const newQty = Math.max(0, currentQty + change);

    combos[comboType].quantity = newQty;
    document.getElementById(`${comboType}-qty`).textContent = newQty;

    updateUI();
    updateSummary();
}

// Payment Method Selection
function selectPaymentMethod(method) {
    document.querySelectorAll('.payment-option').forEach(option => {
        option.classList.remove('selected');
    });

    document.querySelector(`[data-method="${method}"]`).classList.add('selected');
}

// Update order summary in payment step
function updateSummary() {
    // This function should be called when updating the payment summary
    // It depends on selectedSeats which should be available globally
    if (typeof selectedSeats !== 'undefined') {
        // Update seat summary
        const seatCount = selectedSeats.length;
        const seatTotal = selectedSeats.reduce((sum, seat) => sum + seat.price, 0);

        const seatCountEl = document.getElementById('seat-count');
        const seatTotalEl = document.getElementById('seat-total');
        
        if (seatCountEl) seatCountEl.textContent = seatCount;
        if (seatTotalEl) seatTotalEl.textContent = seatTotal.toLocaleString('vi-VN') + ' ₫';
    }

    // Update combo summary
    const totalComboQty = Object.values(combos).reduce((sum, combo) => sum + combo.quantity, 0);
    const comboTotal = Object.values(combos).reduce((sum, combo) => sum + (combo.price * combo.quantity), 0);

    const comboSummary = document.getElementById('combo-summary');
    const comboCountEl = document.getElementById('combo-count');
    const comboTotalEl = document.getElementById('combo-total');
    
    if (comboSummary) {
        if (totalComboQty > 0) {
            comboSummary.style.display = 'grid';
            if (comboCountEl) comboCountEl.textContent = totalComboQty;
            if (comboTotalEl) comboTotalEl.textContent = comboTotal.toLocaleString('vi-VN') + ' ₫';
        } else {
            comboSummary.style.display = 'none';
        }
    }

    // Update total
    const seatTotal = (typeof selectedSeats !== 'undefined') 
        ? selectedSeats.reduce((sum, seat) => sum + seat.price, 0) 
        : 0;
    const grandTotal = seatTotal + comboTotal;
    
    const paymentTotalEl = document.getElementById('payment-total');
    const totalAmountEl = document.getElementById('total-amount');
    
    if (paymentTotalEl) paymentTotalEl.textContent = grandTotal.toLocaleString('vi-VN') + ' ₫';
    if (totalAmountEl) totalAmountEl.textContent = grandTotal.toLocaleString('vi-VN');
}

// Initialize event listeners for concession and payment
function initializeConcessionPayment() {
    // Payment method selection
    document.querySelectorAll('.payment-option').forEach(option => {
        option.addEventListener('click', function() {
            selectPaymentMethod(this.dataset.method);
        });
    });

    // Form validation
    document.querySelectorAll('.form-control').forEach(input => {
        input.addEventListener('input', function() {
            this.classList.remove('error');
        });
    });

    // Initialize summary
    updateSummary();
}

// Export functions for global access
if (typeof window !== 'undefined') {
    window.updateComboQuantity = updateComboQuantity;
    window.selectPaymentMethod = selectPaymentMethod;
    window.updateSummary = updateSummary;
    window.initializeConcessionPayment = initializeConcessionPayment;
    window.combos = combos;
}
