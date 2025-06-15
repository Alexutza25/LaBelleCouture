let lastFilteredProducts = [];

document.addEventListener("DOMContentLoaded", async () => {
    const homepage = document.getElementById("homepageWrapper");
    const sidebar = document.getElementById("adminSidebar");

    if (isAdminUser()) {
        sidebar.style.display = "block";
        homepage.classList.add("admin-view");
    } else {
        sidebar.style.display = "none";
        homepage.classList.remove("admin-view");
        homepage.style.marginLeft = "0";
    }
    console.log("admin?", isAdminUser());

    await loadFilters(); 
    runSearch(); 
    await loadRecommendations();
    await loadAllProductsGroupedByCategory(); 

    const container = document.getElementById("topProductsContainer");

    try {
        const res = await fetch("/api/Order/top5-alltime");
        const products = await res.json();

        if (!Array.isArray(products) || products.length === 0) {
            container.innerHTML = "<p>No product bought yet.</p>";
            return;
        }

        products.forEach(p => {
            const div = document.createElement("div");
            div.className = "product-card";
            div.innerHTML = `
                <a href="/Product?Id=${p.productId}" style="text-decoration: none;">
                    <img src="${p.imageURL}" alt="${p.name}" />
                    <p class="title">${p.name}</p>
                    <p class="price">💸 ${p.price} $</p>
                </a>
            `;
            container.appendChild(div);
        });
    } catch (err) {
        console.error("❌ Top products fetch failed:", err);
        container.innerHTML = "<p>⚠️ Error loading top products</p>";
    }

    document.getElementById("searchInput").addEventListener("input", runSearch);

    const toggleBtn = document.getElementById('filterToggle');
    toggleBtn.addEventListener('click', () => {
        const sidebar = document.getElementById('filterSidebar');
        sidebar.classList.remove('hidden');
        sidebar.classList.add('show');
        updateFilterToggleForSidebar();
    });

    document.getElementById('closeFilter').addEventListener('click', async () => {
        const sidebar = document.getElementById('filterSidebar');
        sidebar.classList.remove('show');
        sidebar.classList.add('hidden');
        await loadAllProductsGroupedByCategory();
        updateFilterToggleForSidebar(); 
    });

    const applyBtn = document.getElementById('applyFilters');
    applyBtn.addEventListener('click', async () => {
        const selectedCategories = Array.from(document.querySelectorAll('.filter-category:checked')).map(cb => cb.value);
        const selectedColors = Array.from(document.querySelectorAll('.filter-color:checked')).map(cb => cb.value);
        const selectedMaterials = Array.from(document.querySelectorAll('.filter-material:checked')).map(cb => cb.value);
        const minPrice = document.getElementById('minPrice').value;
        const maxPrice = document.getElementById('maxPrice').value;

        const filters = {
            categories: selectedCategories,
            colors: selectedColors,
            materials: selectedMaterials,
            minPrice: minPrice ? parseFloat(minPrice) : null,
            maxPrice: maxPrice ? parseFloat(maxPrice) : null
        };

        const response = await fetch('/api/product/filter', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(filters)
        });

        const products = await response.json();
        lastFilteredProducts = [...products];
        renderProducts(products);
    });

    document.getElementById("sortSelect").addEventListener("change", async () => {
        const sortType = document.getElementById("sortSelect").value;

        if (!sortType) {
            await loadAllProductsGroupedByCategory();
            document.getElementById("searchResultsContainer").style.display = "none";
            document.querySelectorAll(".centered-wrapper, #productContainer").forEach(sec => sec.style.display = "block");
            return;
        }

        try {
            let products;
            if (lastFilteredProducts.length > 0) {
                products = [...lastFilteredProducts]; 
            } else {
                const res = await fetch("/api/Product");
                products = await res.json(); 
            }


            let sorted = [...products];
            if (sortType === "low") {
                sorted.sort((a, b) => a.price - b.price);
            } else if (sortType === "high") {
                sorted.sort((a, b) => b.price - a.price);
            }

            renderProducts(sorted);

            document.getElementById("productContainer").style.display = "block";
            const parentWrapper = document.getElementById("productContainer").closest(".centered-wrapper");
            if (parentWrapper) parentWrapper.style.display = "block";

            document.querySelectorAll(".centered-wrapper").forEach(sec => {
                if (!sec.contains(document.getElementById("productContainer"))) {
                    sec.style.display = "none";
                }
            });

            document.getElementById("searchResultsContainer").style.display = "none";
        } catch (err) {
            console.error("error sorting:", err);
        }
    });


});
async function loadRecommendations() {
    const userId = getUserIdFromToken();
    const wrapper = document.getElementById("youMightLikeSection");

    if (!userId || !wrapper) return;

    try {
        const res = await fetch(`/api/Product/recommendations/${userId}`);
        if (!res.ok) throw new Error("Server error");

        const products = await res.json();

        if (products.length === 0) {
            wrapper.style.display = "none";
            return;
        }

        wrapper.innerHTML = `
            <section class="category-section">
                <h2 class="section-title">You Might Like </h2>
                <div class="scroll-wrapper">
                    <button class="scroll-btn left" onclick="scrollLeftYml()">‹</button>
                    <div id="youMightLikeIndex" class="product-scroll-track"></div>
                    <button class="scroll-btn right" onclick="scrollRightYml()">›</button>
                </div>
            </section>
        `;

        const container = wrapper.querySelector("#youMightLikeIndex");

        products.forEach(p => {
            const card = document.createElement("div");
            card.classList.add("product-card");
            card.style.minWidth = "200px";
            card.style.flexShrink = "0";

            card.innerHTML = `
                <a href="/Product?id=${p.productId}" style="text-decoration: none;">
                    <img src="${p.imageURL || '/images/default-product.png'}" alt="${p.name}" />
                    <p class="title">${p.name}</p>
                    <p class="price">💸 ${p.price} $</p>
                </a>
            `;

            container.appendChild(card);
        });

    } catch (err) {
        console.error("💥 Failed to load recommendations:", err);
        wrapper.innerHTML = "";
    }
}


const slider = document.getElementById('sliderTrack');

function scrollLeft1() {
    slider.scrollBy({ left: -300, behavior: 'smooth' });
}

function scrollRight() {
    slider.scrollBy({ left: 300, behavior: 'smooth' });
}

function scrollLeftYml() {
    const el = document.getElementById("youMightLikeIndex");
    el.scrollBy({ left: -300, behavior: "smooth" });
}

function scrollRightYml() {
    const el = document.getElementById("youMightLikeIndex");
    el.scrollBy({ left: 300, behavior: "smooth" });
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


function updateFilterToggleForSidebar() {
    const sidebar = document.getElementById('filterSidebar');
    const toggle = document.getElementById('filterToggle');

    if (sidebar.classList.contains('show')) {
        toggle.style.right = '320px';
        document.body.classList.add('with-filter-sidebar'); 
    } else {
        toggle.style.right = '20px';
        document.body.classList.remove('with-filter-sidebar'); 
    }
}
function renderProducts(products) {
    const container = document.getElementById("productContainer");
    container.innerHTML = "";

    if (!products || products.length === 0) {
        container.innerHTML = "<p>No products found </p>";
        return;
    }

    const section = document.createElement("section");
    section.classList.add("category-section");

    const grid = document.createElement("div");
    grid.classList.add("product-grid"); 

    products.forEach(p => {
        const card = document.createElement("div");
        card.classList.add("product-card");
        card.innerHTML = `
        <img src="${p.imageURL}" alt="${p.name}" />
        <p class="title">${p.name}</p>
        <p class="desc">${p.description}</p>
        <p class="price">💸 ${p.price} $</p>
    `;
        card.onclick = () => {
            window.location.href = `/Product?id=${p.productId}`;
        };
        grid.appendChild(card);
    });

    section.appendChild(grid);
    container.appendChild(section);

}

async function runSearch() {
    const name = document.getElementById("searchInput").value.trim();

    const searchResults = document.getElementById("searchResultsContainer");
    const otherSections = document.querySelectorAll(".centered-wrapper, #productContainer");

    if (!name) {
        searchResults.style.display = "none";
        otherSections.forEach(section => section.style.display = "block");
        return;
    }

    try {
        const res = await fetch(`/api/Product/search-by-name?name=${encodeURIComponent(name)}`);
        if (!res.ok) {
            const errorText = await res.text();
            throw new Error(`Server error: ${errorText}`);
        }

        const products = await res.json();

        searchResults.innerHTML = "";
        searchResults.style.display = "block";
        otherSections.forEach(section => section.style.display = "none");

        if (products.length === 0) {
            searchResults.innerHTML = "<p>No products found </p>";
            return;
        }

        const grid = document.createElement("div");
        grid.classList.add("product-grid");

        products.forEach(p => {
            const card = document.createElement("div");
            card.classList.add("product-card");
            card.innerHTML = `
                <img src="${p.imageURL}" alt="${p.name}" />
                <p class="title">${p.name}</p>
                <p class="desc">${p.description}</p>
                <p class="price">💸 ${p.price} $</p>
            `;
            card.onclick = () => {
                window.location.href = `/Product?id=${p.productId}`;
            };
            grid.appendChild(card);
        });

        searchResults.appendChild(grid);

    } catch (err) {
        console.error("Error", err);
    }
}


async function loadFilters() {
    try {
        const [categories, colors, materials, priceRange] = await Promise.all([
            fetch('/api/product/categories').then(r => r.json()),
            fetch('/api/product/colors').then(r => r.json()),
            fetch('/api/product/materials').then(r => r.json()),
            fetch('/api/product/price-range').then(r => r.json())
        ]);

        fillFilterOptions('#filterCategories', categories, 'filter-category');
        fillFilterOptions('#filterColors', colors, 'filter-color');
        fillFilterOptions('#filterMaterials', materials, 'filter-material');

        const priceMinInput = document.getElementById('minPrice');
        const priceMaxInput = document.getElementById('maxPrice');

        priceMinInput.min = priceRange.min;
        priceMinInput.max = priceRange.max;
        priceMinInput.value = priceRange.min;

        priceMaxInput.min = priceRange.min;
        priceMaxInput.max = priceRange.max;
        priceMaxInput.value = priceRange.max;

    } catch (err) {
        console.error('eroare la filtre:', err);
    }
}

function fillFilterOptions(containerSelector, values, className) {
    const container = document.querySelector(containerSelector);
    container.innerHTML = '';
    values.forEach(value => {
        const label = document.createElement('label');
        label.innerHTML = `<input type="checkbox" value="${value}" class="${className}"> ${value}`;
        container.appendChild(label);
    });
}

async function loadAllProductsGroupedByCategory() {
    const container = document.getElementById("productContainer");
    container.innerHTML = "";

    try {
        const res = await fetch("/api/Category");
        const categories = await res.json();

        for (const category of categories) {
            const section = document.createElement("section");
            section.classList.add("category-section");

            const title = document.createElement("h2");
            title.textContent = category.name;
            section.appendChild(title);

            const scrollWrapper = document.createElement("div");
            scrollWrapper.classList.add("scroll-wrapper");

            const leftBtn = document.createElement("button");
            leftBtn.classList.add("scroll-btn", "left");
            leftBtn.innerHTML = "‹";
            leftBtn.onclick = () => scrollCategory(leftBtn, -1);

            const rightBtn = document.createElement("button");
            rightBtn.classList.add("scroll-btn", "right");
            rightBtn.innerHTML = "›";
            rightBtn.onclick = () => scrollCategory(rightBtn, 1);

            const productGrid = document.createElement("div");
            productGrid.classList.add("product-scroll-track");

            // încarcă produsele
            const prodRes = await fetch(`/api/Product/category/${category.categoryId}`);
            const products = await prodRes.json();

            products.forEach(p => {
                const card = document.createElement("div");
                card.classList.add("product-card");
                card.innerHTML = `
                    <img src="${p.imageURL}" alt="${p.name}" />
                    <p class="title">${p.name}</p>
                    <p class="desc">${p.description}</p>
                    <p class="price">💸 ${p.price} $</p>
                `;
                card.onclick = () => {
                    window.location.href = `/Product?id=${p.productId}`;
                };
                productGrid.appendChild(card);
            });

            scrollWrapper.appendChild(leftBtn);
            scrollWrapper.appendChild(productGrid);
            scrollWrapper.appendChild(rightBtn);

            section.appendChild(scrollWrapper);
            container.appendChild(section);
        }

    } catch (err) {
        console.error("⚠️", err);
        container.innerHTML = "<p>We couldn't load the products</p>";
    }
}

function scrollCategory(button, direction) {
    const wrapper = button.parentElement;
    const track = wrapper.querySelector(".product-scroll-track");
    const scrollAmount = 300;

    track.scrollBy({ left: direction * scrollAmount, behavior: "smooth" });
}


function isAdminUser() {
    const token = localStorage.getItem("jwt");
    if (!token) return false;

    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const role = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
        return role === "Admin" || role === "Administrator";
    } catch (e) {
        return false;
    }
}
