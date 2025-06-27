// Movie Detail Page JavaScript
document.addEventListener('DOMContentLoaded', function() {
    // Smooth scroll for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });

    // Parallax effect for floating elements
    window.addEventListener('scroll', () => {
        const scrolled = window.pageYOffset;
        const parallax = document.querySelectorAll('.floating-element');

        parallax.forEach(element => {
            const speed = 0.5;
            element.style.transform = `translateY(${scrolled * speed}px)`;
        });
    });

    // Hover effects for meta items
    document.querySelectorAll('.meta-item').forEach(item => {
        item.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-5px) scale(1.05)';
        });

        item.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0) scale(1)';
        });
    });

    // Cinema group toggling
    window.toggleCinemaGroup = function(header) {
        const cinemaGroup = header.parentElement;
        const isActive = cinemaGroup.classList.contains('active');

        document.querySelectorAll('.cinema-group').forEach(group => {
            group.classList.remove('active');
        });

        if (!isActive) {
            cinemaGroup.classList.add('active');
        }
    };

    // Showtimes toggling
    window.toggleShowtimes = function(cinemaItem) {
        const isActive = cinemaItem.classList.contains('active');

        const cinemaGroup = cinemaItem.closest('.cinema-group');
        cinemaGroup.querySelectorAll('.cinema-item').forEach(item => {
            item.classList.remove('active');
        });

        if (!isActive) {
            cinemaItem.classList.add('active');
        }
    };

    // Showtime slot click animation
    document.addEventListener('click', function(e) {
        if (e.target.classList.contains('showtime-slot')) {
            e.target.style.transform = 'scale(0.95)';
            setTimeout(() => {
                e.target.style.transform = 'scale(1)';
            }, 100);

            console.log('Booking showtime:', e.target.textContent);
        }
    });
});
