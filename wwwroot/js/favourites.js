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


document.addEventListener("DOMContentLoaded", async () => {
    const container = document.getElementById("favouritesContainer");
    const token = localStorage.getItem("jwt");

    const res = await fetch("/api/favourite/my", {
        headers: {
            "Authorization": `Bearer ${token}`
        }
    });

    if (!res.ok) {
        container.innerHTML = "<p>❌ Couldn't load favourites</p>";
        return;
    }

    const favourites = await res.json();
    console.log("🧪 Favourites:", favourites);


    if (favourites.length === 0) {
        container.innerHTML = "<p> No favourites yet!</p>";
        return;
    }

    favourites.forEach(fav => {
        const variant = fav.productVariant;

        const card = document.createElement("div");
        card.className = "favourite-card";
        const product = variant?.product;

        const inStock = variant?.stock > 0;
        const stockText = inStock ? "In stock" : "Out of stock";
        const stockClass = inStock ? "in-stock" : "out-of-stock";

        card.innerHTML = `
          <img src="${product?.imageURL || 'https://via.placeholder.com/150'}" alt="${product?.name || 'No name'}" class="product-image" />
          <div class="favourite-info">
              <h3>${product?.name || 'Unknown Product'}</h3>
              <p class="${stockClass}"><strong>${stockText}</strong></p>
              <p><strong>Size:</strong> ${variant?.size}</p>
              <p><strong>Price:</strong> ${product?.price ? product.price + '$' : 'Unavailable'}</p>
              <button class="btn-remove" data-id="${fav.favouriteId}">🗑️ Remove</button>
              <button class="add-to-cart-btn">🛒 Add to Cart</button>

          </div>
`;


        card.setAttribute("data-productid", product?.productId);
        card.setAttribute("data-size", variant?.size);
        card.setAttribute("data-variantid", variant?.variantId);


        card.addEventListener("click", (e) => {
            if (
                e.target.classList.contains("btn-remove") ||
                e.target.classList.contains("add-to-cart-btn")
            ) return;

            const productId = card.getAttribute("data-productid");
            const size = card.getAttribute("data-size");

            window.location.href = `/Product?id=${productId}&size=${size}`;
        });


        container.appendChild(card);
    });

    container.addEventListener("click", (e) => {
        if (
            !e.target.classList.contains("btn-remove")
        ) return;

        const btn = e.target;
        const id = btn.dataset.id;
        const confirmToast = document.getElementById("confirmToast");
        const confirmYes = document.getElementById("confirmYes");
        const confirmNo = document.getElementById("confirmNo");

        confirmToast.classList.remove("hidden");

        function cleanup() {
            confirmYes.removeEventListener("click", onYes);
            confirmNo.removeEventListener("click", onNo);
            confirmToast.classList.add("hidden");
        }

        function onYes() {
            cleanup();
            fetch(`/api/Favourite/${id}`, {
                method: "DELETE",
                headers: {"Authorization": `Bearer ${token}`}
            })
                .then(res => {
                    if (res.ok) {
                        btn.closest(".favourite-card").remove();
                        showSideToast("🗑️ Deleted successfully!");
                    } else {
                        alert("❌ Failed to remove");
                    }
                });
        }

        function onNo() {
            cleanup();
        }

        confirmYes.addEventListener("click", onYes);
        confirmNo.addEventListener("click", onNo);
    });

    //add to cart handler
    container.addEventListener("click", async (e) => {
        if (!e.target.classList.contains("add-to-cart-btn")) return;

        const card = e.target.closest(".favourite-card");
        const productId = card?.dataset.variantid;
        const token = localStorage.getItem("jwt");

        if (!card) {
            console.error(" card not found");
            showSideToast("❌ Card not found", "error");
            return;
        }
        if (!productId) {
            console.error("variantId not found", card?.dataset);
            //showSideToast("❌ Variant ID missing", "error");
            return;
        }
        if (!token) {
            console.error("token not found");
            showSideToast("❌ Not authenticated", "error");
            return;
        }


        const payload = JSON.parse(atob(token.split(".")[1]));
        const userId = payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];

        try {
            const res = await fetch("/api/Cart/add", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({
                    userId: parseInt(userId),
                    variantId: parseInt(productId),
                    quantity: 1
                })
            });

            if (!res.ok) throw new Error("❌ Error adding to cart");
            showSideToast("✅ Added to cart!");
            const favId = card.querySelector(".btn-remove")?.dataset.id;

            if (favId) {
                await fetch(`/api/Favourite/${favId}`, {
                    method: "DELETE",
                    headers: {
                        "Authorization": `Bearer ${token}`
                    }
                });
                card.remove();
            }

        } catch (err) {
            showSideToast(err.message, "error");
        }
    });


});