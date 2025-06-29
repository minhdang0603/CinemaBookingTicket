document.addEventListener('DOMContentLoaded', function() {
            const movieCards = document.querySelectorAll('.movie-card');
            
            movieCards.forEach(card => {
                card.addEventListener('mouseenter', function() {
                    this.style.transform = 'translateY(-5px)';
                });
                
                card.addEventListener('mouseleave', function() {
                    this.style.transform = 'translateY(0)';
                });
            });

            const dropdowns = document.querySelectorAll('.custom-dropdown');
            dropdowns.forEach(dropdown => {
                dropdown.addEventListener('click', function() {
                    console.log('Dropdown clicked:', this.textContent.trim());
                });
            });

            const pageLinks = document.querySelectorAll('.pagination .page-link');
            pageLinks.forEach(link => {
                link.addEventListener('click', function(e) {
                    e.preventDefault();
                    
                    document.querySelectorAll('.pagination .page-item').forEach(item => {
                        item.classList.remove('active');
                    });
                    
                    if (!this.parentElement.classList.contains('disabled')) {
                        this.parentElement.classList.add('active');
                        
                        const pageNum = this.textContent.trim();
                        if (!isNaN(pageNum)) {
                            const startItem = (parseInt(pageNum) - 1) * 6 + 1;
                            const endItem = Math.min(parseInt(pageNum) * 6, 24);
                            document.querySelector('.text-muted').innerHTML = 
                                `<i class="fas fa-film me-2"></i>Hiển thị ${startItem}-${endItem} trong tổng số 24 phim`;
                            
                            document.querySelectorAll('.text-muted')[1].textContent = `Trang ${pageNum} / 4`;
                        }
                    }
                });
            });
        });