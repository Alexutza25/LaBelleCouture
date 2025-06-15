console.log("navbar.js loaded");

document.addEventListener("DOMContentLoaded", function () {
    const token = localStorage.getItem("jwt");

    const navLinks = document.querySelector(".navbar-links");
    const loginLink = document.getElementById("loginLink");
    const signupLink = document.getElementById("signupLink");
    const logoutLink = document.getElementById("logoutLink");
    const cartLink = document.getElementById("cartLink");
    const favLink = document.getElementById("favouritesLink");
    const profileLink = document.getElementById("profileLink");
    const logoutBtn = document.getElementById("logoutBtn");

    if (token) {
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            const now = Math.floor(Date.now() / 1000);
          

            if (payload.exp < now) {
                console.warn("⌛ Token expirat");
                localStorage.clear();
            } else {
                const role = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
                if (profileLink) {
                    profileLink.style.display = "list-item";
                }
                if (role === "Administrator") {
                    if (navLinks) {
                        const adminLink = document.createElement("li");
                        adminLink.innerHTML = `<a href="/AdminDashboard">Admin Panel</a>`;
                        navLinks.appendChild(adminLink);
                    }

                    cartLink?.classList?.add("hidden");
                    favLink?.classList?.add("hidden");
                    logoutLink?.classList?.add("hidden");

                    const sidebar = document.createElement("div");
                    sidebar.className = "sidebar";
                    sidebar.innerHTML = `
                        <h1>Admin Dashboard</h1>
                        <div class="button-group">
                            <a class="sidebar-btn" href="#" id="openUsersModal">👤 Customers</a>
                            <a class="sidebar-btn" href="#" id="openCategoryModal">🗂️ Categories</a>
                            <a class="sidebar-btn" href="#" id="openProductModal">👗 Products</a>
                            <a class="sidebar-btn" href="#" id="openVariantModal">🎯 Product Variants</a>
                            <a class="sidebar-btn" href="#" id="openOrdersModal">📦 Orders</a>

                        </div>
                        <a class="sidebar-btn red" href="#" id="logoutBtnSidebar">Logout</a>
                    `;
                    document.body.appendChild(sidebar);

                    document.querySelector(".navbar")?.style?.setProperty("margin-left", "220px");
                    document.querySelector("main")?.style?.setProperty("margin-left", "220px");

                    document.getElementById("logoutBtnSidebar")?.addEventListener("click", () => {
                        localStorage.clear();
                        window.location.href = "/Login";
                    });
                }
            }

        } catch (e) {
            console.error("token corupt:", e);
            localStorage.clear();
        }

        profileLink?.style?.setProperty("display", "list-item");
        loginLink?.style?.setProperty("display", "none");
        signupLink?.style?.setProperty("display", "none");
        logoutLink?.style?.setProperty("display", "list-item");
    } else {
        logoutLink?.style?.setProperty("display", "none");
        loginLink?.style?.setProperty("display", "list-item");
        signupLink?.style?.setProperty("display", "list-item");
        profileLink?.style?.setProperty("display", "none");

    }

    logoutBtn?.addEventListener("click", function (e) {
        e.preventDefault();
        localStorage.clear();
        window.location.href = "/Login";
    });
});
