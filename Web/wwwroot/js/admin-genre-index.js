$(document).ready(function () {
    var table = $('#genreTable').DataTable({
        "responsive": true,
        "pageLength": 5,
        "lengthMenu": [5, 10, 25, 50, 100],
        "order": [[0, "asc"]],
        // Thêm callback để style lại dropdown length
        "initComplete": function () {
            $('.dt-length select').addClass('bg-dark text-white');
        }
    });

    // // Filter by theater
    // $('select[name="theaterId"]').on('change', function () {
    //     var val = $(this).find('option:selected').text();
    //     if (val === "All Theaters" || $(this).val() === "") {
    //         table.column(4).search('').draw();
    //     } else {
    //         table.column(4).search(val).draw();
    //     }
    // });

    // Tìm kiếm bằng input custom thay vì input của DataTable
    $("input[name='searchTerm']").on('keyup change', function () {
        table.search($(this).val()).draw();
    });
    // Ẩn input search mặc định của DataTable
    $(".dt-search").hide();
});


// Handle delete screen
let deleteGenreId = null;
function deleteGenre(id, name) {
    deleteGenreId = id;
    document.getElementById('deleteGenreTitle').textContent = name;
    const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
    modal.show();
}
function confirmDeleteGenre(url) {
    if (deleteGenreId) {
        $.ajax({
            type: "DELETE",
            url: url.replace("__id__", deleteGenreId),
            success: function (data) {
                location.reload();
            }
        })
    }
}
