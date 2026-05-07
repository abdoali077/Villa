// =========================
// AJAX SEARCH (NO RELOAD)
// =========================
document.getElementById("searchForm").addEventListener("submit", function (e) {
    e.preventDefault();

    const formData = new FormData(this);

    //save scroll before request
    formData.set("scroll", window.scrollY);

    fetch('/Home/GetVillaByDate', {
        method: 'POST',
        body: formData
    })
        .then(res => res.text())
        .then(html => {

            const doc = new DOMParser().parseFromString(html, "text/html");

            const newVilla = doc.querySelector("#villaContainer");

            document.querySelector("#villaContainer").innerHTML =
                newVilla.innerHTML;

            //  scroll smoothly to villas
            document.querySelector("#villaContainer")
                .scrollIntoView({ behavior: "smooth", block: "start" });
        });
});


// =========================
// DETAILS MODAL
// =========================
function openVillaDetails(id) {

    const modal = new bootstrap.Modal(
        document.getElementById('villaDetailsModal')
    );

    modal.show();

    fetch('/Villas/GetDetails?id=' + id)
        .then(r => r.text())
        .then(data => {
            document.getElementById("villaModalBody").innerHTML = data;
        });
}


// =========================
// LOGIN + STATE SAVE
// =========================
function redirectToLogin(villaId) {

    const scroll = window.scrollY;

    const url = new URL(window.location.href);

    url.searchParams.set("villaId", villaId);
    url.searchParams.set("scroll", scroll);

    window.location.href =
        `/Account/Login?returnUrl=${encodeURIComponent(url.pathname + url.search)}`;
}


// =========================
// RESTORE AFTER LOGIN
// =========================
document.addEventListener("DOMContentLoaded", function () {

    const params = new URLSearchParams(window.location.search);

    const scroll = parseInt(params.get("scroll") || "0");
    const villaId = params.get("villaId");

    setTimeout(() => {

        if (scroll > 0) {
            window.scrollTo({
                top: scroll,
                behavior: "smooth"
            });
        }

        if (villaId) {
            openVillaDetails(villaId);
        }

    }, 200);
});