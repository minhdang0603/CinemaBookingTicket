$(document).ready(function () {
    var table = $('#provinceTable').DataTable({
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
    //$('select[name="theaterId"]').on('change', function () {
    //    var val = $(this).find('option:selected').text();
    //    if (val === "All Theaters" || $(this).val() === "") {
    //        table.column(4).search('').draw();
    //    } else {
    //        table.column(4).search(val).draw();
    //    }
    //});

    // Tìm kiếm bằng input custom thay vì input của DataTable
    $("input[name='searchTerm']").on('keyup change', function () {
        table.search($(this).val()).draw();
    });
    // Ẩn input search mặc định của DataTable
    $(".dt-search").hide();
});


// Handle delete screen
let deleteProvinceId = null;
function deleteProvince(id, name) {
    deleteProvinceId = id;
    document.getElementById('deleteProvinceTitle').textContent = name;
    const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
    modal.show();
}
function confirmDeleteProvince(url) {
    if (deleteProvinceId) {
        $.ajax({
            type: "DELETE",
            url: url.replace("__id__", deleteProvinceId),
            success: function (data) {
                location.reload();
            }
        })
    }
}
