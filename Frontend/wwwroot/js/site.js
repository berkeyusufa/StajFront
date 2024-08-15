document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.querySelector(".left-menu");
    const toggleBtn = document.querySelector(".toggle-btn");
    const mainContent = document.querySelector(".main");

    if (toggleBtn) {
        toggleBtn.addEventListener("click", function () {
            sidebar.classList.toggle("collapsed");
            mainContent.classList.toggle("collapsed");
            toggleBtn.classList.toggle("collapsed");
        });
    }
});
