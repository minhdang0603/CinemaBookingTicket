$(document).ready(function () {
    var table = $('#showtimeTable').DataTable({
        "responsive": true,
        "pageLength": 5,
        "lengthMenu": [5, 10, 25, 50, 100],
        "order": [[0, "asc"]],
        // Style l?i dropdown length n?u mu?n
        "initComplete": function () {
            $('.dt-length select').addClass('bg-dark text-white');
        }
    });

    // Search chung (tìm ki?m toàn b? b?ng)
    $("input[name='searchTerm']").on('keyup change', function () {
        table.search($(this).val()).draw();
    });

    // Filter theo Movie
    $("select[name='movieId']").on('change', function () {
        var val = $(this).find('option:selected').text();
        if (val === "All Movies" || $(this).val() === "") {
            table.column(0).search('').draw();
        } else {
            table.column(0).search('^' + $.fn.dataTable.util.escapeRegex(val) + '$', true, false).draw();
        }
    });

    // Filter theo Theater
    $("select[name='theaterId']").on('change', function () {
        var val = $(this).find('option:selected').text();
        if (val === "All Theaters" || $(this).val() === "") {
            table.column(2).search('').draw();
        } else {
            table.column(2).search('^' + $.fn.dataTable.util.escapeRegex(val) + '$', true, false).draw();
        }
    });

    // Filter theo ngày chi?u
    $("input[name='showDate']").on('change', function () {
        var val = $(this).val();
        if (!val) {
            table.column(3).search('').draw();
        } else {
            // Chuy?n yyyy-MM-dd thành dd-MM-yyyy ?? so sánh v?i b?ng
            var parts = val.split('-');
            var formatted = parts[2] + '-' + parts[1] + '-' + parts[0];
            table.column(3).search(formatted).draw();
        }
    });

    // Nút clear filter
    $('#clearFilterBtn').on('click', function () {
        $('#showtimeFilterForm')[0].reset();
        table.search('').columns().search('').draw();
    });

    // ?n input search m?c ??nh c?a DataTable
    $(".dt-search").hide();
});

// Handle delete showtime
let deleteShowtimeId = null;
function deleteShowtime(id, title) {
    deleteShowtimeId = id;
    document.getElementById('deleteShowtimeTitle').textContent = title;
    const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
    modal.show();
}

function confirmDeleteShowtime(url) {
    if (deleteShowtimeId) {
        $.ajax({
            type: "DELETE",
            url: url.replace("__id__", deleteShowtimeId),
            success: function (data) {
                location.reload();
            },
            error: function (xhr, status, error) {
                alert("Error deleting showtime: " + error);
            }
        });
    }
}