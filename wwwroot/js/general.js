function isAdminUser() {
    const token = localStorage.getItem("jwt");
    if (!token) return false;
    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const now = Math.floor(Date.now() / 1000);
        const role = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
        return payload.exp > now && role === "Administrator";
    } catch {
        return false;
    }
}

document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById("adminSidebar");
    if (sidebar && isAdminUser()) {
        sidebar.style.display = "block";
    }

    const logoutBtn = document.getElementById("logoutBtn");
    if (logoutBtn) {
        logoutBtn.addEventListener("click", () => {
            localStorage.clear();
        });
    }
});
