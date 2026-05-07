var dataTable;

$(document).ready(function () {
    loadDataTable("");

    $('#statusFilter').on('change', function () {
        var status = $(this).val();

        dataTable.destroy();
        loadDataTable(status);
    });
});
function cancelBooking(id) {

    Swal.fire({
        title: 'Cancel Booking?',
        text: "This booking will be cancelled.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#dc3545',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, Cancel it',
        cancelButtonText: 'Close'
    }).then((result) => {

        if (result.isConfirmed) {

            fetch('/Booking/CancelBooking', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                body: `bookingId=${id}`
            })
                .then(res => {

                    if (res.ok) {

                        Swal.fire({
                            icon: 'success',
                            title: 'Cancelled',
                            text: 'Booking cancelled successfully',
                            timer: 1800,
                            showConfirmButton: false
                        });

                        dataTable.ajax.reload();

                    } else {

                        Swal.fire({
                            icon: 'error',
                            title: 'Failed',
                            text: 'Cancel failed'
                        });

                    }

                });

        }

    });
}

function loadDataTable(status) {

    dataTable = $('#tblData').DataTable({
        ajax: {
            url: '/Booking/GetAll',
            data: {
                status: status
            }
        },

        columns: [
            {
                data: 'bookingId',
                render: function (data) {
                    return `#VW-${data.toString().padStart(5, '0')}`;
                }
            },
            { data: 'name' },
            { data: 'email' },
            { data: 'phoneNumber' },

            {
                data: 'checkInDate',
                render: function (data) {
                    return new Date(data).toLocaleDateString();
                }
            },

            {
                data: 'nights',
                render: d => `${d} Nights`
            },

            {
                data: 'totalCost',
                render: d => `$${parseFloat(d).toFixed(2)}`
            },

            {
                data: 'status',
                render: function (data) {

                    let color = "secondary";

                    if (data === "Approved") color = "success";
                    else if (data === "Pending") color = "warning";
                    else if (data === "Cancelled") color = "danger";
                    else if (data === "Completed") color = "primary";

                    return `<span class="badge bg-${color}">${data}</span>`;
                }
            },

            {
                data: 'bookingId',
                render: function (data, type, row) {

                    return `
                        <div class="text-end pe-4">
                            <div class="dropdown">
                                <button class="btn btn-light btn-sm rounded-circle shadow-none" type="button" data-bs-toggle="dropdown">
                                    <i class="bi bi-three-dots-vertical"></i>
                                </button>
                                <ul class="dropdown-menu dropdown-menu-end shadow-sm border-0 rounded-3">
                                    <li><a class="dropdown-item py-2" href="/Booking/BookingDetails?bookingId=${data}"><i class="bi bi-eye me-2"></i>View Details</a></li>
                                    ${row.status === "Pending" ? `
                                        <li><hr class="dropdown-divider"></li>
                                        <li><a class="dropdown-item py-2 text-danger" href="javascript:void(0)" onclick="cancelBooking(${data})"><i class="bi bi-x-square me-2"></i>Cancel Booking</a></li>
                                    ` : ''}
                                </ul>
                            </div>
                        </div>`;
                }
            }
        ],

        language: {
            emptyTable: "No bookings found"
        }
    });
}


