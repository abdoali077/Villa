$(document).ready(function () {
    loadRevenueDashboardData();
});
function loadRevenueDashboardData() {
    $.ajax({
        url: '@Url.Action("GetTotalRevenueData", "Dashboard")',
        type: 'GET',
        dataType: 'json',
        success: function (data) {

            $('#totalRevenueCount').text(data.totalRevenue.toLocaleString());

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

            $('#revenueRatioSection').html(ratioHtml);

            renderRevenueRadialChart(data);
        }
    });
}

function renderRevenueRadialChart(data) {
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
    var chart = new ApexCharts(document.querySelector("#totalRevenueRadialChart"), options);
    chart.render();
}