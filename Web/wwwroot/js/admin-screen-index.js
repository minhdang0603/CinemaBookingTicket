$(document).ready(function () {
    var table = $('#screenTable').DataTable({
        "responsive": true,
        "pageLength": 5,
        "lengthMenu": [5, 10, 25, 50, 100],
        "order": [[0, "asc"]],
        // Thêm callback để style lại dropdown length
        "initComplete": function () {
            $('.dt-length select').addClass('bg-dark text-white');
        }
    });

    // Filter by theater
    $('select[name="theaterId"]').on('change', function () {
        var val = $(this).find('option:selected').text();
        if (val === "All Theaters" || $(this).val() === "") {
            table.column(4).search('').draw();
        } else {
            table.column(4).search(val).draw();
        }
    });

    // Tìm kiếm bằng input custom thay vì input của DataTable
    $("input[name='searchTerm']").on('keyup change', function () {
        table.search($(this).val()).draw();
    });
    // Ẩn input search mặc định của DataTable
    $(".dt-search").hide();
});


// Handle delete screen
let deleteScreenId = null;
function deleteScreen(id, name) {
    deleteScreenId = id;
    document.getElementById('deleteScreenTitle').textContent = name;
    const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
    modal.show();
}
function confirmDeleteScreen(url) {
    if (deleteScreenId) {
        $.ajax({
            type: "DELETE",
            url: url.replace("__id__", deleteScreenId),
            success: function (data) {
                location.reload();
            }
        })
    }
}
