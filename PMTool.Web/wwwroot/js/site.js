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

function getAntiForgeryToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? '';
}

function initNotifications() {
    const toggle = document.getElementById("notificationToggle");
    const list = document.getElementById("notificationList");
    const empty = document.getElementById("notificationEmpty");
    const badge = document.getElementById("notificationCount");

    if (!toggle || !list || !empty || !badge) {
        return;
    }

    const setBadge = (count) => {
        if (count > 0) {
            badge.textContent = count > 99 ? "99+" : count.toString();
            badge.style.display = "inline-flex";
        } else {
            badge.textContent = "";
            badge.style.display = "none";
        }
    };

    const renderItem = (notification, prepend) => {
        const item = document.createElement("div");
        item.className = "notification-item";
        item.dataset.id = notification.id;

        const body = document.createElement("div");
        body.className = "notification-body";

        const text = document.createElement("div");
        text.className = "notification-text";
        text.textContent = notification.message;

        const time = document.createElement("div");
        time.className = "notification-time";
        time.textContent = new Date(notification.createdAt).toLocaleString();

        body.appendChild(text);
        body.appendChild(time);

        const del = document.createElement("button");
        del.className = "notification-delete";
        del.type = "button";
        del.innerHTML = '<i class="bi bi-trash"></i>';
        del.addEventListener("click", async (e) => {
            e.stopPropagation();
            const success = await deleteNotification(notification.id);
            if (success) {
                item.remove();
                updateEmptyState();
            }
        });

        item.appendChild(body);
        item.appendChild(del);

        if (prepend) {
            list.prepend(item);
        } else {
            list.appendChild(item);
        }
    };

    const updateEmptyState = () => {
        const count = list.querySelectorAll(".notification-item").length;
        empty.style.display = count === 0 ? "block" : "none";
        setBadge(count);
    };

    const loadNotifications = async () => {
        const resp = await fetch("/Notifications?handler=List", {
            headers: {
                "Accept": "application/json"
            }
        });
        if (!resp.ok) return;
        const data = await resp.json();
        list.innerHTML = "";
        data.forEach((notification) => renderItem(notification, false));
        updateEmptyState();
    };

    const deleteNotification = async (id) => {
        const token = getAntiForgeryToken();
        const body = new URLSearchParams({ id });
        const resp = await fetch("/Notifications?handler=Delete", {
            method: "POST",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded",
                "RequestVerificationToken": token
            },
            body: body.toString()
        });
        if (!resp.ok) return false;
        const result = await resp.json();
        return result.success === true;
    };

    loadNotifications();

    if (window.signalR) {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/hubs/notifications")
            .withAutomaticReconnect()
            .build();

        connection.on("notificationReceived", (notification) => {
            renderItem(notification, true);
            updateEmptyState();
        });

        connection.on("notificationsSeed", (notifications) => {
            if (!Array.isArray(notifications)) {
                return;
            }
            list.innerHTML = "";
            notifications.forEach((notification) => renderItem(notification, false));
            updateEmptyState();
        });

        connection.start().catch(() => {});
    }
}

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", () => {
        initSidebar();
        initNotifications();
    });
} else {
    initSidebar();
    initNotifications();
}
