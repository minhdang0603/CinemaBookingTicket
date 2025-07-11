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
    
    // Update all total displays - for both sidebar and mobile view
    const formattedTotal = grandTotal.toLocaleString('vi-VN');
    
    // Update all total amount displays
    document.querySelectorAll('#total-amount').forEach(el => {
        if (el) el.textContent = formattedTotal;
    });
    
    // Update all mobile total displays
    document.querySelectorAll('[id^="mobile-total"]').forEach(el => {
        if (el) el.textContent = formattedTotal;
    });
}

// Calculate total amount - useful for other scripts that need this value
function calculateTotalAmount() {
    const seatTotal = (typeof window.seatBaseAmount !== 'undefined') ? window.seatBaseAmount : 0;
    const comboTotal = Object.values(combos).reduce((sum, combo) => sum + (combo.price * combo.quantity), 0);
    return seatTotal + comboTotal;
}

// Export functions for global access
if (typeof window !== 'undefined') {
    window.updateComboQuantity = updateComboQuantity;
    window.updateSummary = updateSummary;
    window.calculateTotalAmount = calculateTotalAmount;
    window.combos = combos;
}
