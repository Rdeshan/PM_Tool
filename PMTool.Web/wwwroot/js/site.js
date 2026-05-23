// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function initSidebar() {
    const sidebar = document.getElementById("pm-global-sidebar");
    const toggleBtn = document.getElementById("sidebar-toggle");

    if (!sidebar || !toggleBtn) {
        return;
    }

    if (localStorage.getItem("sidebarCollapsed") === "true") {
        document.body.classList.add("sidebar-collapsed");
        sidebar.classList.add("collapsed");
    }

    toggleBtn.addEventListener("click", function () {
        if (window.innerWidth < 992) {
            sidebar.classList.toggle("mobile-open");
            return;
        }

        sidebar.classList.toggle("collapsed");
        document.body.classList.toggle("sidebar-collapsed");

        if (sidebar.classList.contains("collapsed")) {
            localStorage.setItem("sidebarCollapsed", "true");
        } else {
            localStorage.setItem("sidebarCollapsed", "false");
        }
    });
}

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initSidebar);
} else {
    initSidebar();
}
