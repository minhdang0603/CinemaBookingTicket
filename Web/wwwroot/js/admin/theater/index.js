$(document).ready(function () {
    var table = $('#theaterTable').DataTable({
        "responsive": true,
        "pageLength": 5,
        "lengthMenu": [5, 10, 25, 50, 100],
        "order": [[0, "asc"]],
        // Thêm callback để style lại dropdown length
        "initComplete": function () {
            $('.dt-length select').addClass('bg-dark text-white');
        }
    });

    // Filter by province
    $('select[name="provinceId"]').on('change', function () {
        var val = $(this).find('option:selected').text();
        if (val === "All Provinces" || $(this).val() === "") {
            table.column(3).search('').draw();
        } else {
            table.column(3).search(val).draw();
        }
    });

    // Tìm kiếm bằng input custom thay vì input của DataTable
    $("input[name='searchTerm']").on('keyup change', function () {
        table.search($(this).val()).draw();
    });
    // Ẩn input search mặc định của DataTable
    $(".dt-search").hide();
});


// Handle delete theater
let deleteTheaterId = null;
function deleteTheater(id, name) {
    deleteTheaterId = id;
    document.getElementById('deleteTheaterTitle').textContent = name;
    const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
    modal.show();
}
function confirmDeleteTheater(url) {
    if (deleteTheaterId) {
        $.ajax({
            type: "DELETE",
            url: url.replace("__id__", deleteTheaterId),
            success: function (data) {
                location.reload();
            }
        })
    }
}
