$(document).ready(function () {
    var table = $('#movieTable').DataTable({
        "responsive": true,
        "scrollX": true,
        "pageLength": 5,
        "lengthMenu": [5, 10, 25, 50, 100],
        "order": [[0, "asc"]],
        "columnDefs": [
            { "targets": [6], "visible": false, "searchable": true } // Ẩn cột GenreIds nhưng vẫn có thể search
        ],
        "initComplete": function () {
            $('.dt-length select').addClass('bg-dark text-white');
        }
    });

    // Custom filter by genre using the hidden column
    $.fn.dataTable.ext.search.push(
        function (settings, data, dataIndex) {
            const selectedGenre = $('select[name="genreId"]').val();
            if (!selectedGenre || selectedGenre === "" || selectedGenre === "All Genres") return true;

            // Lấy dữ liệu từ cột GenreIds (cột index 6)
            const genreData = data[6]; // Cột GenreIds
            return genreData && genreData.includes(selectedGenre);
        }
    );

    $('select[name="genreId"]').on('change', function () {
        table.draw();
    });

    // Tìm kiếm bằng input custom thay vì input của DataTable
    $("input[name='searchTerm']").on('keyup change', function () {
        table.search($(this).val()).draw();
    });
    
    // Ẩn input search mặc định của DataTable
    $(".dt-search").hide();
});


// Handle delete Movie
let deleteMovieId = null;
function deleteMovie(id, name) {
    deleteMovieId = id;
    document.getElementById('deleteMovieTitle').textContent = name;
    const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
    modal.show();
}

function confirmDeleteMovie(url) {
    if (deleteMovieId) {
        $.ajax({
            type: "DELETE",
            url: url.replace("__id__", deleteMovieId),
            success: function (data) {
                location.reload();
            },
            error: function(xhr, status, error) {
                console.error('Delete failed:', error);
                alert('Failed to delete movie. Please try again.');
            }
        });
    }
}
