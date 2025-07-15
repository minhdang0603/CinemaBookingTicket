
        
        $(document).ready(function() {
    // Movie card hover effects
    $('.movie-card').hover(
        function() {
            $(this).css('transform', 'translateY(-5px)');
        },
        function() {
            $(this).css('transform', 'translateY(0)');
        }
    );

    // Remove active class from all genre badges when page loads
    $('.genre-badge').removeClass('active');
    
    // Add active class to current selected genre if any
    const currentGenreId = $('select[name="genreId"]').val();
    if (currentGenreId) {
        $(`.genre-badge[onclick*="${currentGenreId}"]`).addClass('active');
    }
});

// Function to submit filter form
function submitFilter() {
    document.getElementById('filterForm').submit();
}

// Function to select genre from badge
function selectGenre(genreId) {
    // Update the select dropdown
    $('select[name="genreId"]').val(genreId);
    
    // Submit the form
    submitFilter();
}