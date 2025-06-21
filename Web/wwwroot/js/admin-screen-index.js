$(document).ready(function () {
    var table = $('#screenTable').DataTable({
        "responsive": true,
        "pageLength": 5,
        "lengthMenu": [5, 10, 25, 50, 100],
        "order": [[0, "asc"]],
        // Thêm callback để style lại dropdown length
        "initComplete": function() {
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


// Handle create screen form submit
$(document).on('submit', '#createScreenForm', function (e) {
    e.preventDefault();
    var $form = $(this);
    $.ajax({
        url: $form.attr('action'),
        type: $form.attr('method'),
        data: $form.serialize(),
        success: function (res) {
            if (res.success) {
                location.reload();
            } else {
                $('#createModalContent').html(res);
            }
        },
        error: function (xhr) {
            alert('Error: ' + xhr.statusText);
        }
    });
});
// Handle delete screen
let deleteScreenId = null;
function deleteScreen(id, name) {
    deleteScreenId = id;
    document.getElementById('deleteScreenTitle').textContent = name;
    const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
    modal.show();
}
function confirmDeleteScreen() {
    if (deleteScreenId) {
        // TODO: Call API or submit form to delete screen
        alert('Demo: Will delete screen ID ' + deleteScreenId);
        bootstrap.Modal.getInstance(document.getElementById('deleteModal')).hide();
    }
}
// Demo: Edit screen (can open edit modal dynamically if needed)
function editScreen(id) {
    alert('Demo: Will open edit modal for screen ID ' + id);
}
