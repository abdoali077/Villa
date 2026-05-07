$(document).ready(function () {
    // فتح القائمة
    $('#sidebarToggle').on('click', function () {
        $('#sidebar').addClass('active');
        $('#overlay').addClass('show');
    });

    // إغلاق القائمة (عن طريق الزر أو الضغط على الخلفية المعتمة)
    $('#closeSidebar, #overlay').on('click', function () {
        $('#sidebar').removeClass('active');
        $('#overlay').removeClass('show');
    });

    // جعل الرابط النشط يأخذ كلاس active برمجياً بناءً على الـ URL الحالي
    const currentUrl = window.location.pathname;
    $('.sidebar .nav-link').each(function () {
        if ($(this).attr('href') === currentUrl) {
            $(this).addClass('active');
        }
    });
});