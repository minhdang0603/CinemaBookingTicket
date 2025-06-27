$(document).ready(function () {
    var table = $('#ConcessionTable').DataTable({
        "responsive": true,
        "pageLength": 5,
        "lengthMenu": [5, 10, 25, 50, 100],
        "order": [[0, "asc"]],
        // Thêm callback để style lại dropdown length
        "initComplete": function () {
            $('.dt-length select').addClass('bg-dark text-white');
        }
    });

    $('select[name="categoryId"]').on('change', function () {
        var val = $(this).find('option:selected').text();
        if (val === "All Categories" || $(this).val() === "") {
            table.column(3).search('').draw(); // Column 3 is Category
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


// Handle delete Concession
let deleteConcessionId = null;
function deleteConcession(id, name) {
    deleteConcessionId = id;
    document.getElementById('deleteConcessionTitle').textContent = name;
    const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
    modal.show();
}
function confirmDeleteConcession(url) {
    if (deleteConcessionId) {
        $.ajax({
            type: "DELETE",
            url: url.replace("__id__", deleteConcessionId),
            success: function (data) {
                location.reload();
            }
        })
    }
}
