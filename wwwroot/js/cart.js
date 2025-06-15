document.addEventListener("DOMContentLoaded", async () => {
    const token = localStorage.getItem("jwt");
    const userId = getUserIdFromToken();
    const cartItemsContainer = document.getElementById("cart-items");
    const productTotalElem = document.getElementById("product-total");
    const cartTotalElem = document.getElementById("cart-total");

    if (!token || !userId) {
        cartItemsContainer.innerHTML = "<p>⚠️ You need to be logged in to see your cart.</p>";
        return;
    }

    try {
        const res = await fetch(`/api/Cart/me`, {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (!res.ok) throw new Error("❌ Cart fetch failed");

        const cartItems = await res.json();
        let productTotal = 0;
        const deliveryFee = 19.99;

        if (cartItems.length === 0) {
            cartItemsContainer.innerHTML = "<p>🛒 Your Cart is empty</p>";
            productTotalElem.textContent = "$0.00";
            cartTotalElem.textContent = `${deliveryFee.toFixed(2)} $`;
            return;
        }

        cartItemsContainer.innerHTML = "";

        cartItems.forEach(item => {
            const name = item.productVariant.product.name;
            const size = item.productVariant.size;
            const quantity = item.quantity;
            const price = item.productVariant.product.price;
            const totalPrice = quantity * price;

            productTotal += totalPrice;

            cartItemsContainer.innerHTML += `
            <div class="cart-item" data-cart-id="${item.cartId}" data-stock="${item.productVariant.stock}">
                <div class="cart-image">
                  <img src="${item.productVariant.product.imageURL}" alt="${name}" />
                </div>
                <div class="cart-details">
                  <p><strong>${name}</strong></p>
                  <p>Size: ${size}</p>
                  <p>Price: $${price.toFixed(2)}</p>
                  <div class="quantity-control">
                    <button class="qty-btn minus">−</button>
                    <span class="qty-value">${quantity}</span>
                    <button class="qty-btn plus">+</button>
                  </div>
                  <p><strong>Subtotal: $${totalPrice.toFixed(2)}</strong></p>
                  <button class="delete-btn">🗑️ Remove</button>
                </div>
              </div>
            `;


        });
        // Activam butoanele plus minus dupa ce DOM ul a fost actualizat
        setTimeout(() => {
            document.querySelectorAll(".cart-item").forEach(cartItem => {
                const minusBtn = cartItem.querySelector(".minus");
                const plusBtn = cartItem.querySelector(".plus");
                const qtySpan = cartItem.querySelector(".qty-value");

                let quantity = parseInt(qtySpan.textContent);

                minusBtn.addEventListener("click", () => {
                    if (quantity > 1) {
                        quantity--;
                        qtySpan.textContent = quantity;
                        updateQuantity(cartItem.dataset.cartId, quantity);
                        recalculateCart(cartItem, quantity);
                        plusBtn.disabled = false;
                    }
                });


                plusBtn.addEventListener("click", () => {
                    const maxQty = parseInt(cartItem.dataset.stock);
                    if (quantity < maxQty) {
                        quantity++;
                        qtySpan.textContent = quantity;
                        plusBtn.disabled = quantity >= maxQty;
                        updateQuantity(cartItem.dataset.cartId, quantity);
                        recalculateCart(cartItem, quantity);
                    } else {
                        showToast("There's not so many products in stock!", "error");
                    }
                });

                const deleteBtn = cartItem.querySelector(".delete-btn");

                deleteBtn.addEventListener("click", () => {
                    const cartItemElem = deleteBtn.closest(".cart-item");
                    const cartId = cartItemElem.dataset.cartId;

                    const modal = document.getElementById("confirmModal");
                    const confirmYes = document.getElementById("confirmYes");
                    const confirmNo = document.getElementById("confirmNo");

                    modal.classList.remove("hidden");

                    // Yes
                    confirmYes.onclick = async () => {
                        try {
                            const res = await fetch(`/api/Cart/${cartId}`, {
                                method: "DELETE",
                                headers: {
                                    ...(token && { "Authorization": `Bearer ${token}` })
                                }
                            });

                            if (!res.ok) throw new Error("❌ Failed to delete");

                            showToast("🗑️ Product removed from cart!");
                            cartItemElem.remove();
                            recalculateCartTotals();
                        } catch (err) {
                            console.error("💥 Delete failed:", err);
                            showToast("❌ Eroare la ștergere", "error");
                        } finally {
                            modal.classList.add("hidden");
                        }
                    };

                    // No
                    confirmNo.onclick = () => {
                        modal.classList.add("hidden");
                    };
                });

            });
            

        }, 0);

        productTotalElem.textContent = `$ ${productTotal.toFixed(2)} `;
        cartTotalElem.textContent = `$ ${(productTotal + deliveryFee).toFixed(2)} `;

    } catch (err) {
        console.error("Error loading Cart", err);
        cartItemsContainer.innerHTML = "<p>❌ Eroare la încărcarea coșului.</p>";
    }

    document.getElementById("placeOrderBtn")?.addEventListener("click", async () => {
        const token = localStorage.getItem("jwt");

        // Get user address
        const userId = getUserIdFromToken();
        const addressRes = await fetch(`/api/User/address/${userId}`, {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        const address = await addressRes.json();

        // Get cart
        const cartRes = await fetch(`/api/Cart/me`, {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        const cartItems = await cartRes.json();

        // Save to localStorage
        localStorage.setItem("order_address", JSON.stringify(address));
        localStorage.setItem("cart", JSON.stringify(cartItems));

        // Redirect
        window.location.href = "/Order";
    });

});

async function updateQuantity(cartId, quantity) {
    const token = localStorage.getItem("jwt");
    try {
        const res = await fetch(`/api/Cart/${cartId}/quantity`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` })
            },
            body: JSON.stringify(quantity)
        });

        if (!res.ok) throw new Error("❌ Update failed");
        showToast("🔁 Quantity updated!");
    } catch (err) {
        console.error("💥 Eroare actualizare:", err);
        showToast("❌ Failed to update quantity", "error");
    }
}

function recalculateCart(cartItemElement, newQuantity) {
    const priceText = cartItemElement.querySelector("p:nth-of-type(3)").textContent;
    const price = parseFloat(priceText.replace("Price: $", ""));

    const newSubtotal = (newQuantity * price).toFixed(2);

    // actualizeaza subtotalul vizibil
    const subtotalElem = cartItemElement.querySelector("p:last-of-type");
    subtotalElem.innerHTML = `<strong>Subtotal: $${newSubtotal}</strong>`;

    // recalculam totalul cosului
    let newProductTotal = 0;
    document.querySelectorAll(".cart-item").forEach(item => {
        const qty = parseInt(item.querySelector(".qty-value").textContent);
        const priceText = item.querySelector("p:nth-of-type(3)").textContent;
        const price = parseFloat(priceText.replace("Price: $", ""));
        newProductTotal += qty * price;
    });

    const deliveryFee = 19.99;
    document.getElementById("product-total").textContent = `$${newProductTotal.toFixed(2)}`;
    document.getElementById("cart-total").textContent = `$${(newProductTotal + deliveryFee).toFixed(2)}`;
}

function recalculateCartTotals() {
    let newProductTotal = 0;
    const deliveryFee = 19.99;

    document.querySelectorAll(".cart-item").forEach(item => {
        const qty = parseInt(item.querySelector(".qty-value").textContent);
        const priceText = item.querySelector("p:nth-of-type(3)").textContent;
        const price = parseFloat(priceText.replace("Price: $", ""));
        newProductTotal += qty * price;
    });

    document.getElementById("product-total").textContent = `$${newProductTotal.toFixed(2)}`;
    document.getElementById("cart-total").textContent = `$${(newProductTotal + deliveryFee).toFixed(2)}`;
}

function showSideToast(message, type = "success") {
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
