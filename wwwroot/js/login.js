console.log("login.js loaded");

document.addEventListener("DOMContentLoaded", function () {
    const token = localStorage.getItem("jwt");

    if (token) {
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            const now = Math.floor(Date.now() / 1000);
            if (payload.exp > now) {
                window.location.href = "/Index";
                return;
            } else {
                localStorage.clear();
            }
        } catch {
            localStorage.clear();
        }
    }

    const loginForm = document.getElementById("loginForm");
    const emailInput = document.getElementById("emailInput");
    const passwordInput = document.getElementById("passwordInput");
    const errorText = document.getElementById("errorText");

    loginForm.addEventListener("submit", async function (e) {
        e.preventDefault();

        const email = emailInput.value.trim();
        const password = passwordInput.value.trim();
        errorText.textContent = "";

        if (!email || !password) {
            errorText.textContent = "Please complete all fields!";
            return;
        }

        const dto = { email, password };

        try {
            const response = await fetch("/api/User/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(dto)
            });

            if (response.ok) {
                const result = await response.json();
                localStorage.setItem("jwt", result.token);
                localStorage.setItem("userId", result.userId);

                const payload = JSON.parse(atob(result.token.split('.')[1]));
                const role = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

                if (role === "Administrator") {
                    window.location.href = "/AdminDashboard";
                } else {
                    window.location.href = "/Index";
                }

            } else {
                errorText.textContent = "Email or Password is incorrect ";
            }
        } catch (error) {
            errorText.textContent = "Connection Error";
            console.error(error);
        }
    });
});
