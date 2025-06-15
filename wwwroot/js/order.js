
function getUserIdFromToken() {
    const token = localStorage.getItem("jwt");
    if (!token) return null;

    try {
        const payload = token.split('.')[1];
        const decoded = JSON.parse(atob(payload));
        return decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
    } catch (e) {
        console.error("❌ Invalid token:", e);
        return null;
    }
}


document.addEventListener("DOMContentLoaded", () => {
    const address = JSON.parse(localStorage.getItem("order_address"));
    const cart = JSON.parse(localStorage.getItem("cart"));

    console.log("🛒 Cart items:", cart);

    if (address) {
        document.querySelector("input[name='County']").value = address.county || "";
        document.querySelector("input[name='City']").value = address.city || "";
        document.querySelector("input[name='Street']").value = address.street || "";
        document.querySelector("input[name='Number']").value = address.number || "";
        document.querySelector("input[name='BuildingEntrance']").value = address.buildingEntrance || "";
        document.querySelector("input[name='Floor']").value = address.floor || "";
        document.querySelector("input[name='ApartmentNumber']").value = address.apartmentNumber || "";
        document.querySelector("textarea[name='AdditionalDetails']").value = address.additionalDetails || "";
    }

    if (cart) {
        const container = document.getElementById("orderedProducts");
        container.innerHTML = "";

        cart.forEach(item => {
            const name = item.productVariant.product.name;
            const price = item.productVariant.product.price;
            const quantity = item.quantity;

            const el = document.createElement("div");
            el.classList.add("order-product");
            el.innerHTML = `
        <div class="ordered-product-info">
            <img src="${item.productVariant.product.imageURL || '/images/default-product.png'}" alt="${name}" class="ordered-product-img" />
            <div>
                <strong>${name}</strong><br/>
                ${quantity} x $ ${price} 
            </div>
        </div>
    `;

            container.appendChild(el);
        });
    }
    document.querySelector(".order-form")?.addEventListener("submit", async (e) => {
        e.preventDefault();

        const token = localStorage.getItem("jwt");
        const userId = getUserIdFromToken();
        const cart = JSON.parse(localStorage.getItem("cart"));

        if (!cart || cart.length === 0) {
            showToast("❌ Empty Cart");
            return;
        }

        console.log("🛒 Cart valid:", cart);

        for (const item of cart) {
            const variantId = item.productVariant.variantId;
            const quantity = item.quantity;


            const res = await fetch(`/api/ProductVariant/${variantId}/decreaseStock?amount=${quantity}`, {
                method: "PUT",
                headers: { "Authorization": `Bearer ${token}` }
            });

            if (!res.ok) {
                const msg = await res.text();
                console.error("eoare la decreasestock:", msg);
                showToast("❌ Stock error", "error");
                return;
            }
        }

        const orderDto = {
            userId: parseInt(userId),
            paymentMethod: document.querySelector("select[name='paymentMethod']").value,
            orderDetails: cart.map(item => {
                const variantId = item.productVariant?.variantId;
                const quantity = item.quantity;
                const price = item.productVariant?.product?.price || 0;
                const subtotal = quantity * price;

                return { variantId, quantity, price, subtotal };
            })

        };

        try {
            console.log("final order payload:", JSON.stringify(orderDto, null, 2));
            const res = await fetch("/api/Order", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify(orderDto)
            });

            if (!res.ok) throw new Error("❌ Order not saved");

            await fetch(`/api/Cart/clear/${userId}`, {
                method: "DELETE",
                headers: { "Authorization": `Bearer ${token}` }
            });

            localStorage.removeItem("cart");
            showToast("🎉 Order placed!");

            setTimeout(() => {
                 window.location.href = "/Index";
             }, 2500);
        } catch (err) {
            console.error("❌ Order failed:", err);
            showToast("❌ Could not place order", "error");
        }
    });


});

function showToast(message, type = "success") {
    const toast = document.createElement("div");
    toast.className = `toast ${type}`;
    toast.textContent = message;
    document.body.appendChild(toast);

    setTimeout(() => toast.classList.add("show"), 100);
    setTimeout(() => {
        toast.classList.remove("show");
        setTimeout(() => toast.remove(), 300);
    }, 2500);
}

