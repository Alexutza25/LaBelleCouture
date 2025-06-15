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
let token = null;
let UserId = null;
let id = null;


document.addEventListener("DOMContentLoaded", async () => {
    token = localStorage.getItem("jwt");
    UserId = getUserIdFromToken();

    const urlParams = new URLSearchParams(window.location.search);
    id = urlParams.get("id") || urlParams.get("Id");
    const selectedSizeFromUrl = urlParams.get("size");
    if (!id) return;

    try { 
        const res = await fetch(`/api/Product/${id}`);
        const product = await res.json();

        console.log("📦 Product loaded:", product);
        console.log("📦 Category ID:", product.categoryId);

        const categoryId = product.categoryId;
        console.log("📦 Category ID:", product.categoryId);

        const categoryRes = await fetch(`/api/Product/category/${categoryId}`);
        const categoryProducts = await categoryRes.json();

        const boughtTogetherContainer = document.getElementById("boughtTogether");
        if (boughtTogetherContainer) {
            boughtTogetherContainer.innerHTML = "";

            const title = document.createElement("h2");
            title.textContent = "Similar Products";
            title.style.fontFamily = "Pacifico";
            title.style.marginBottom = "1rem";
            boughtTogetherContainer.appendChild(title);

            categoryProducts
                .filter(p => p.productId !== product.productId)
                .slice(0, 10)
                .forEach(p => {
                    const productCard = document.createElement("div");
                    productCard.classList.add("product-card");
                    productCard.style.minWidth = "200px";
                    productCard.style.flexShrink = "0";
                    productCard.innerHTML = `
                <a href="/Product?id=${p.productId}">
                    <img src="${p.imageURL || '/images/default-product.png'}" alt="${p.name}" class="related-product-image">
                    <h4 class="related-product-name">${p.name}</h4>
                    <p class="related-product-price">💸 ${p.price.toFixed(2)} $</p>
                </a>
            `;
                    boughtTogetherContainer.appendChild(productCard);
                });
        }

       
        document.getElementById("productImage").src = product.imageURL || "/images/default-product.png";

        const variantsRes = await fetch("/api/ProductVariant");
        const variants = await variantsRes.json();
        const productVariants = variants.filter(v => v.productId === product.productId);

        const availabilityElem = document.getElementById("availability");
        const options = document.getElementById("options");
        options.innerHTML = "";

        const sizeLabel = document.createElement("label");
        sizeLabel.setAttribute("for", "sizeSelect");
        sizeLabel.textContent = "Select size:";
        options.appendChild(sizeLabel);

        const sizeSelect = document.createElement("select");
        sizeSelect.id = "sizeSelect";
        options.appendChild(sizeSelect);

        const sizeGuideBtn = document.createElement("button");
        sizeGuideBtn.className = "size-guide";
        sizeGuideBtn.textContent = "📏 Size Guide";
        sizeGuideBtn.type = "button";
        sizeGuideBtn.addEventListener("click", () => {
            const modal = document.getElementById("sizeGuideModal");
            if (typeof modal.showModal === "function") {
                modal.showModal();
            } else {
                modal.style.display = "flex";
            }
            document.body.style.overflow = "hidden";
        });
        options.appendChild(sizeGuideBtn);

        const closeSizeBtn = document.getElementById("closeSizeGuide");
        if (closeSizeBtn) {
            closeSizeBtn.addEventListener("click", () => {
                const modal = document.getElementById("sizeGuideModal");
                if (typeof modal.close === "function") {
                    modal.close();
                } else {
                    modal.style.display = "none";
                }
                document.body.style.overflow = "auto";
            });
        }

        let firstValidSize = null;

        productVariants.forEach(v => {
            const size = v.size.trim();
            const opt = document.createElement("option");
            opt.value = size;
            opt.textContent = `Size ${size}`;
            sizeSelect.appendChild(opt);
            if (!firstValidSize) firstValidSize = size;
        });

        const details = document.getElementById("productDetails");
        details.innerHTML = `
            <h1>${product.name}</h1>
            <p><strong>Category:</strong> ${product.category}</p>
            <p><strong>Price:</strong> 💸 ${product.price} $</p>
            <p><strong>Description:</strong> ${product.description}</p>
        `;


        function updateAvailability(selectedSize) {
            const variant = productVariants.find(v =>
                v.size.trim().toLowerCase() === selectedSize.trim().toLowerCase()
            );

            if (!variant) {
                availabilityElem.innerHTML = `<span class="unknown">-</span>`;
                return;
            }

            if (variant.stock > 5) {
                availabilityElem.innerHTML = `<span class="in-stock">✅ In stock</span>`;
            } else if (variant.stock > 0) {
                availabilityElem.innerHTML = `<span class="low-stock">⚠️ Only ${variant.stock} left</span>`;
            } else {
                availabilityElem.innerHTML = `<span class="out-stock">❌ Out of stock</span>`;
            }
        }

        let selectedSize = selectedSizeFromUrl;
        if (selectedSize && Array.from(sizeSelect.options).some(opt => opt.value === selectedSize)) {
            sizeSelect.value = selectedSize;
        } else if (firstValidSize) {
            sizeSelect.value = firstValidSize;
            selectedSize = firstValidSize;
        }
        updateAvailability(sizeSelect.value);

        sizeSelect.addEventListener("change", (e) => {
            updateAvailability(e.target.value);
        });

        if (product.colour) {
            const colourP = document.createElement("p");
            colourP.innerHTML = `🎨 <strong>Colour:</strong> ${product.colour}`;
            options.appendChild(colourP);
        }
        if (product.material) {
            const materialP = document.createElement("p");
            materialP.innerHTML = `🧵 <strong>Material:</strong> ${product.material}`;
            options.appendChild(materialP);
        }


        document.getElementById("addToFavourites").addEventListener("click", () => {
            const selectedSize = sizeSelect.value;
            if (!selectedSize) return showToast("⚠️ Please select a size!", "error");

            const selectedVariant = productVariants.find(v =>
                v.size.trim().toLowerCase() === selectedSize.trim().toLowerCase()
            );
            if (!selectedVariant || !UserId) {
                console.log("Produs: :(( ", selectedVariant, UserId);
                return showToast("❌ Missing data.", "error");
            }
            console.log("Sending to favourites:", {
                userId: UserId,
                productVariantId: selectedVariant
            });

            addToFavourites(selectedVariant.variantId, UserId, token);
        });

        document.getElementById("addToCart").addEventListener("click", () => {
            const selectedSize = document.getElementById("sizeSelect").value;

            if (!selectedSize) {
                return showToast("⚠️ Please select a size!", "error");
            }

            const selectedVariant = productVariants.find(v =>
                v.size.trim().toLowerCase() === selectedSize.trim().toLowerCase()
            );

            if (!selectedVariant || !UserId) {
                return showToast("❌ Missing data.", "error");
            }

            addToCart(selectedVariant.variantId, UserId, token);
        });
        await loadAssociatedProducts(product.productId);

        if (typeof isAdminUser === "function" && isAdminUser()) {
            document.body.classList.add("with-sidebar");
        }

        await loadReviews();
        await checkEligibility();

        if (UserId) {
            const wrapper = document.getElementById("recommendationsWrapper");
            if (!wrapper) return;

            try {
                const res = await fetch(`/api/Product/recommendations/${UserId}`);
                if (!res.ok) throw new Error("Recomm error");

                const products = await res.json();

                if (products.length === 0) {
                    wrapper.style.display = "none"; 
                    return;
                }

                wrapper.style.display = "block"; 
                wrapper.innerHTML = "";

                const title = document.createElement("h2");
                title.textContent = "You Might Like This";
                title.style.fontFamily = "Pacifico";
                wrapper.appendChild(title);

                const scrollWrapper = document.createElement("div");
                scrollWrapper.className = "scroll-wrapper";

                const leftBtn = document.createElement("button");
                leftBtn.className = "scroll-btn left";
                leftBtn.textContent = "←";
                leftBtn.addEventListener("click", () => {
                    scrollYouMightLike(-1);
                });

                const rightBtn = document.createElement("button");
                rightBtn.className = "scroll-btn right";
                rightBtn.textContent = "→";
                rightBtn.addEventListener("click", () => {
                    scrollYouMightLike(1);
                });

                const scrollContainer = document.createElement("div");
                scrollContainer.className = "scroll-container";
                scrollContainer.id = "youMightLikeProduct";

                products.slice(0, 10).forEach(p => {
                    const card = document.createElement("div");
                    card.classList.add("related-product-card");

                    card.innerHTML = `
                <a href="/Product?id=${p.productId}">
                    <img src="${p.imageURL || '/images/default-product.png'}" alt="${p.name}">
                    <h4 class="related-product-name">${p.name}</h4>
                    <p class="related-product-price">💸 ${p.price.toFixed(2)} $</p>
                </a>
            `;
                    scrollContainer.appendChild(card);
                });

                scrollWrapper.appendChild(leftBtn);
                scrollWrapper.appendChild(scrollContainer);
                scrollWrapper.appendChild(rightBtn);

                wrapper.appendChild(scrollWrapper);

            } catch (err) {
                console.error("failed to load recommendations:", err);
                wrapper.style.display = "none";
            }
        }
    } catch (err) { 
            console.error("failed to load product or other main logic:", err);
            document.getElementById("productPage").innerHTML = "<p>❌ Failed to load product or an unexpected error occurred.</p>";
        }
    });




async function loadAssociatedProducts(productId) {
    try {
        const assocRes = await fetch(`/api/Product/${productId}/associated`);
        if (!assocRes.ok) return;

        const associated = await assocRes.json();
        if (!associated.length) return;

        const container = document.getElementById("associatedProductsContainer");
        container.innerHTML = `<h2 style="font-family: Pacifico;">Frequently Bought Together</h2>`;

        const grid = document.createElement("div");
        grid.className = "associated-products-grid";

        associated.forEach(p => {
            const card = document.createElement("div");
            card.className = "associated-product-card";
            card.innerHTML = `
                <a href="/Product?id=${p.productId}">
                    <img src="${p.imageUrl}" alt="${p.name}" />
                    <h4 class="related-product-name">${p.name}</h4>
                    <p class="related-product-price">💸 ${p.price.toFixed(2)} $</p>
                </a>
            `;
            grid.appendChild(card);
        });

        container.appendChild(grid);
    } catch (err) {
        console.error("Error", err);
    }
}


function addToFavourites(ProductVariantId, UserId, token) {
        fetch("/api/Favourite", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "Authorization": `Bearer ${token}` })
            },
            body: JSON.stringify({
                userId: parseInt(UserId),
                variantId: ProductVariantId
            })
        })
            .then(async res => {
                const msg = await res.text();
                console.log("raspuns server:", msg);

                if (res.ok) {
                    showToast("❤️ Added to favourites!");
                } else {
                    showToast("❌ Failed to add", "error");
                    console.warn("eroare", msg);
                }
            })

            .catch(err => {
                console.error("eroare retea:", err);
                showToast("❌ Error sending request", "error");
            });
    

}

function scrollYouMightLike(direction) {
    const container = document.getElementById("youMightLikeProduct"); 
    const scrollAmount = 300;

    if (container) {
        container.scrollLeft += direction * scrollAmount;
    } else {
        console.warn("❌ #youMightLikeProduct not found");
    }
}

function addToCart(ProductVariantId, UserId, token) {
    fetch("/api/Cart/add", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            ...(token && { "Authorization": `Bearer ${token}` })
        },
        body: JSON.stringify({
            userId: parseInt(UserId),
            variantId: ProductVariantId,
            Quantity: 1 
        })
        
    })
        .then(res => {
            if (res.ok) {
                showToast("🛒 Added to cart!");
            } else {
                showToast("❌ Failed to add to cart", "error");
                res.text().then(msg => console.warn("❌ Cart Error Body:", msg));
            }
        })
        .catch(err => {
            console.error("eroare Cart:", err);
            showToast("❌ Error sending cart request", "error");
        });
    
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

const reviewList = document.getElementById("reviewList");
const reviewForm = document.getElementById("reviewFormContainer");
let selectedRating = 0;

const starContainer = document.getElementById("starRating");
for (let i = 1; i <= 5; i++) {
    const star = document.createElement("span");
    star.innerHTML = "★";
    star.dataset.value = i;

    star.addEventListener("click", () => {
        selectedRating = i;
        document.querySelectorAll("#starRating span").forEach(s => {
            s.classList.toggle("selected", parseInt(s.dataset.value) <= i);
        });
    });

    starContainer.appendChild(star);
}

async function loadReviews() {
    try {
        const res = await fetch(`/api/Review/product/${id}`);

        if (!res.ok) {
            reviewList.innerHTML = "<p>❌ Couldn't load reviews</p>";
            return;
        }

        const reviews = await res.json();
        
        reviewList.innerHTML = "";

        displayAverageRating(reviews);

        if (reviews.length === 0) {
            reviewList.innerHTML = "<p>No reviews yet </p>";
            return;
        }

        reviews.forEach(r => {
            const div = document.createElement("div");
            const stars = "★".repeat(r.rating) + "☆".repeat(5 - r.rating);

            div.innerHTML = `
        <div class="review-stars">${stars}</div>
        <strong>${r.userName}</strong> – ${new Date(r.datePosted).toLocaleDateString("ro-RO")}
        <p>${r.comment}</p>
      `;
            reviewList.appendChild(div);
        });
    } catch (err) {
        console.error("💥 Failed to load reviews:", err);
        reviewList.innerHTML = "<p>❌ Error loading reviews</p>";
    }
}

function displayAverageRating(reviews) {
    if (!reviews || reviews.length === 0) return;

    const total = reviews.reduce((sum, r) => sum + r.rating, 0);
    const average = total / reviews.length;
    const averageRounded = Math.round(average * 2) / 2;

    const fullStars = Math.floor(averageRounded);
    const halfStar = averageRounded % 1 === 0.5;
    const emptyStars = 5 - fullStars - (halfStar ? 1 : 0);

    const ratingDiv = document.createElement("div");
    ratingDiv.classList.add("average-rating");

    const starHtml = [];

    // stele pline
    for (let i = 0; i < fullStars; i++) {
        starHtml.push(`<svg class="star" viewBox="0 0 20 20" fill="#FFD700"><polygon points="10,1 12.59,7.36 19.51,7.36 13.96,11.91 16.55,18.27 10,13.72 3.45,18.27 6.04,11.91 0.49,7.36 7.41,7.36"/></svg>`);
    }

    // stea jumate
    if (halfStar) {
        starHtml.push(`
            <svg class="star" viewBox="0 0 20 20">
                <defs>
                    <linearGradient id="half">
                        <stop offset="50%" stop-color="#FFD700" />
                        <stop offset="50%" stop-color="#e0e0e0" />
                    </linearGradient>
                </defs>
                <polygon points="10,1 12.59,7.36 19.51,7.36 13.96,11.91 16.55,18.27 10,13.72 3.45,18.27 6.04,11.91 0.49,7.36 7.41,7.36" fill="url(#half)" />
            </svg>
        `);
    }

    // stele goale
    for (let i = 0; i < emptyStars; i++) {
        starHtml.push(`<svg class="star" viewBox="0 0 20 20" fill="#e0e0e0"><polygon points="10,1 12.59,7.36 19.51,7.36 13.96,11.91 16.55,18.27 10,13.72 3.45,18.27 6.04,11.91 0.49,7.36 7.41,7.36"/></svg>`);
    }

    ratingDiv.innerHTML = `
        <div class="stars">${starHtml.join('')}</div>
        <span class="value">${average.toFixed(1)} / 5</span>
        <span class="count">(${reviews.length} reviews)</span>
    `;

    const details = document.getElementById("productDetails");
    details.appendChild(ratingDiv);
}


function scrollRecommendations(direction) {
    const container = document.getElementById("youMightLikeProduct");
    const scrollAmount = 220;

    if (container) {
        container.scrollLeft += direction * scrollAmount;
    }
}


async function checkEligibility() {
    if (!UserId) return;

    const res = await fetch(`/api/Review/can-review?userId=${UserId}&productId=${id}`, {
        headers: { Authorization: `Bearer ${token}` }
    });

    if (res.ok) {
        const canReview = await res.json();
        if (canReview) {
            reviewForm.classList.remove("hidden");
        }
    }
}

document.getElementById("submitReviewBtn").addEventListener("click", async () => {
    const comment = document.getElementById("reviewText").value.trim();
    if (!selectedRating || !comment) {
        return showToast("⚠️ Give a rating and write a comment!", "error");
    }

    const review = {
        userId: parseInt(UserId),
        productId: parseInt(id),
        rating: selectedRating,
        comment
    };

    const res = await fetch("/api/Review", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify(review)
    });

    if (res.ok) {
        showToast("✅ Review submitted! Thank you!");
        document.getElementById("reviewText").value = "";
        selectedRating = 0;
        document.querySelectorAll("#starRating span").forEach(s => s.classList.remove("selected"));
        await loadReviews();
    } else {
        showToast("❌ Failed to send review", "error");
    }
});

