// Concession and Payment functionality

// Dynamic combos object, populated from DOM
const combos = {};

document.addEventListener('DOMContentLoaded', function () {
    // Khởi tạo combos từ DOM
    document.querySelectorAll('.combo-item').forEach(function (item) {
        const comboId = item.querySelector('.quantity').id.replace('-qty', '');
        const priceText = item.querySelector('.combo-price').textContent.replace(/[^\d]/g, '');
        const price = parseInt(priceText, 10) || 0;
        combos[comboId] = { price: price, quantity: 0 };
    });
    // Lấy seat base amount từ DOM nếu có
    const seatBaseAmountEl = document.getElementById('seat-base-amount');
    window.seatBaseAmount = seatBaseAmountEl ? parseInt(seatBaseAmountEl.dataset.amount, 10) || 0 : 0;
});

// Concession Functions
function updateComboQuantity(comboId, change) {
    if (!combos[comboId]) return;
    const currentQty = combos[comboId].quantity;
    const newQty = Math.max(0, currentQty + change);
    combos[comboId].quantity = newQty;
    document.getElementById(`${comboId}-qty`).textContent = newQty;
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
    const seatTotal = (typeof window.seatBaseAmount !== 'undefined') ? window.seatBaseAmount : 0;
    const grandTotal = seatTotal + comboTotal;
    
    const paymentTotalEl = document.getElementById('payment-total');
    const totalAmountEl = document.getElementById('total-amount');
    const mobileTotalEl = document.getElementById('mobile-total');
    
    if (paymentTotalEl) paymentTotalEl.textContent = grandTotal.toLocaleString('vi-VN') + ' ₫';
    if (totalAmountEl) totalAmountEl.textContent = grandTotal.toLocaleString('vi-VN');
    if (mobileTotalEl) mobileTotalEl.textContent = grandTotal.toLocaleString('vi-VN');
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
