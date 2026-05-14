$(document).ready(function () {
    loadBookingDashboardData();
});
function loadBookingDashboardData() {
    $.ajax({
        url: '@Url.Action("GetTotalBookingsData", "Dashboard")',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            $('#totalBookingsCount').text(data.totalCount.toLocaleString());

            let ratioHtml = "";
            if (data.hasRatioIncreases) {
                ratioHtml = `<span class="ratio-badge bg-success-soft">
                                        <i class="bi bi-arrow-up-short fs-5"></i> +${data.difference}
                                     </span>`;
            } else {
                ratioHtml = `<span class="ratio-badge bg-danger-soft">
                                        <i class="bi bi-arrow-down-short fs-5"></i> ${data.difference}
                                     </span>`;
            }

            $('#bookingRatioSection').html(ratioHtml);
            renderRadialChart(data);
        }
    });
}
function loadUserDashboardData() {
    $.ajax({
        url: '@Url.Action("GetTotalRegisterUserData", "Dashboard")',
        type: 'GET',
        dataType: 'json',
        success: function (data) {

            $('#totalUsersCount').text(data.totalCount.toLocaleString());

            let ratioHtml = "";
            if (data.hasRatioIncreases) {
                ratioHtml = `<span class="ratio-badge bg-success-soft">
                                        <i class="bi bi-arrow-up-short fs-5"></i> +${data.difference}
                                     </span>`;
            } else {
                ratioHtml = `<span class="ratio-badge bg-danger-soft">
                                        <i class="bi bi-arrow-down-short fs-5"></i> ${data.difference}
                                     </span>`;
            }

            $('#userRatioSection').html(ratioHtml);

            renderUserRadialChart(data);
        }
    });
}

function renderRadialChart(data) {
    var options = {
        series: data.series,
        chart: {
            type: 'radialBar',
            height: 200,
            sparkline: { enabled: true }
        },
        plotOptions: {
            radialBar: {
                hollow: { size: '60%' },
                track: { background: '#f8fafc' },
                dataLabels: {
                    name: { show: false },
                    value: {
                        offsetY: 8,
                        fontSize: '20px',
                        fontWeight: '900',
                        color: '#1e293b',
                        formatter: function (val) { return Math.round(data.series[0]) + "%"; }
                    }
                }
            }
        },
        fill: {
            colors: [data.hasRatioIncreases ? '#10b981' : '#ef4444']
        },
        stroke: { lineCap: 'round' }
    };

    var chart = new ApexCharts(document.querySelector("#totalBookingsRadialChart"), options);
    chart.render();
}