// app.js

const API_CONFIG = {
    baseUrl: 'https://localhost:5001/api', // Thay đổi port này theo backend của bạn
    storageKey: 'ds_token',
    userKey: 'ds_user'
};

// --- MOCK DATA FOR DEMO ---
const mockProducts = [
    {
        id: 1,
        name: "AutoCAD Advanced Layer Manager",
        description: "Optimized DLL for AutoCAD, managing thousands of layers with one click. Supports AutoCAD 2020-2025. This tool helps you automatically rename, group and delete layers matching your company standard without manual headache.",
        type: "TOOL",
        price: 499000,
        img: "https://images.unsplash.com/photo-1581091226825-a6a2a5aee158?w=800&q=80",
        categoryName: "AutoCAD Add-ins"
    },
    {
        id: 2,
        name: "Mastering C# for CAD Automation",
        description: "A comprehensive course from A-Z guiding you on how to write AutoCAD and Revit Add-ins using C# and ASP.NET Core. Includes sample code and lifetime access.",
        type: "COURSE",
        price: 1290000,
        img: "https://images.unsplash.com/photo-1498050108023-c5249f4df085?w=800&q=80",
        categoryName: "Video Courses"
    },
    {
        id: 3,
        name: "Revit Structural Scripter (Python)",
        description: "Python scripts for Dynamo and Revit to automate structural framing creation based on Excel data. Perfect for structural engineers.",
        type: "SCRIPT",
        price: 750000,
        img: "https://images.unsplash.com/photo-1503387762-592dea58ef23?w=800&q=80",
        categoryName: "Revit Scripts"
    }
];

// --- APP STATE ---
let currentUser = JSON.parse(localStorage.getItem(API_CONFIG.userKey)) || null;

// --- INITIALIZATION ---
document.addEventListener('DOMContentLoaded', () => {
    renderProducts(mockProducts);
    updateNavUI();
    
    // Navbar scroll effect
    window.addEventListener('scroll', () => {
        const nav = document.getElementById('navbar');
        if (window.scrollY > 50) {
            nav.classList.add('py-2', 'bg-dark/80');
            nav.classList.remove('py-4', 'bg-dark/0');
        } else {
            nav.classList.add('py-4');
            nav.classList.remove('py-2', 'bg-dark/80');
        }
    });

    // Login Form
    const loginForm = document.getElementById('login-form');
    if (loginForm) {
        loginForm.addEventListener('submit', (e) => {
            e.preventDefault();
            const email = document.getElementById('login-email').value;
            handleMockLogin(email);
        });
    }
});

// --- UI RENDERING ---
function renderProducts(products) {
    const container = document.getElementById('product-container');
    if (!container) return;
    
    container.innerHTML = products.map(p => `
        <div class="group bg-dark-soft/50 border border-white/5 rounded-3xl p-6 hover:border-${p.type === 'COURSE' ? 'secondary' : 'primary'}/50 transition-all duration-500 hover:shadow-2xl hover:shadow-${p.type === 'COURSE' ? 'secondary' : 'primary'}/10 flex flex-col h-full cursor-pointer" onclick="showProductDetail(${p.id})">
            <div class="relative mb-6 rounded-2xl overflow-hidden aspect-video bg-slate-800">
                <img src="${p.img}" class="product-img-zoom object-cover w-full h-full opacity-80">
                <div class="absolute top-4 left-4">
                    <span class="px-3 py-1 bg-${p.type === 'COURSE' ? 'secondary' : 'primary'}/20 backdrop-blur-md border border-${p.type === 'COURSE' ? 'secondary' : 'primary'}/30 rounded-full text-xs font-bold text-${p.type === 'COURSE' ? 'secondary' : 'primary'}">${p.type}</span>
                </div>
            </div>
            <h3 class="text-xl font-bold mb-3 group-hover:text-primary transition-colors line-clamp-2">${p.name}</h3>
            <p class="text-slate-400 text-sm mb-6 flex-grow line-clamp-3">${p.description}</p>
            <div class="flex items-center justify-between mt-auto">
                <span class="text-2xl font-bold text-white">${p.price.toLocaleString('vi-VN')}đ</span>
                <span class="text-sm font-semibold text-primary group-hover:underline flex items-center gap-1">
                    Xem chi tiết 
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path></svg>
                </span>
            </div>
        </div>
    `).join('');
}

function updateNavUI() {
    const btn = document.getElementById('nav-user-name');
    if (!btn) return;
    btn.innerText = currentUser ? `Hi, ${currentUser.fullName}` : 'Đăng nhập';
}

// --- MODALS ---
function toggleAuthModal() {
    const modal = document.getElementById('auth-modal');
    modal.classList.toggle('hidden');
}

function toggleProductModal() {
    const modal = document.getElementById('product-modal');
    modal.classList.toggle('hidden');
}

function showProductDetail(id) {
    const product = mockProducts.find(p => p.id === id);
    if (!product) return;

    document.getElementById('modal-img').src = product.img;
    document.getElementById('modal-title').innerText = product.name;
    document.getElementById('modal-desc').innerText = product.description;
    document.getElementById('modal-price').innerText = product.price.toLocaleString('vi-VN') + 'đ';
    
    const badge = document.getElementById('modal-badge');
    const typeColor = product.type === 'COURSE' ? 'secondary' : 'primary';
    badge.innerHTML = `<span class="px-3 py-1 bg-${typeColor}/20 border border-${typeColor}/30 rounded-full text-xs font-bold text-${typeColor}">${product.type}</span>`;

    // Access Logic section
    const accessSection = document.getElementById('access-section');
    
    // Giả lập logic: Nếu user đã đăng nhập, cho phép "tải xuống" (mock)
    if (currentUser) {
        accessSection.innerHTML = `
            <div class="p-4 bg-green-500/10 border border-green-500/20 rounded-2xl mb-4">
                <p class="text-green-500 text-sm flex items-center gap-2 font-medium">
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
                    Bạn đã có quyền truy cập sản phẩm này.
                </p>
            </div>
            <button onclick="mockDownload(${product.id})" class="w-full py-4 bg-white text-dark font-bold rounded-2xl flex items-center justify-center gap-2 hover:bg-primary hover:text-white transition-all">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4"></path></svg>
                Tải về ngay
            </button>
        `;
    } else {
        accessSection.innerHTML = `
            <button onclick="mockBuy(${product.id})" class="w-full py-4 bg-primary text-white font-bold rounded-2xl flex items-center justify-center gap-2 shadow-xl shadow-primary/20 hover:brightness-110 transition-all">
                Thêm vào giỏ hàng
            </button>
        `;
    }

    toggleProductModal();
}

// --- AUTH HANDLERS ---
function handleMockLogin(email) {
    // Demo: Bất kỳ email nào cũng đăng nhập được
    currentUser = {
        fullName: email.split('@')[0],
        email: email,
        role: "User"
    };
    localStorage.setItem(API_CONFIG.userKey, JSON.stringify(currentUser));
    updateNavUI();
    toggleAuthModal();
}

// --- PRODUCT ACTIONS ---
async function mockDownload(id) {
    alert("🚀 Đang khởi tạo luồng stream file bảo mật qua API backend...\nEndpoint: " + API_CONFIG.baseUrl + "/downloads/" + id + "/tool");
    // Phải chạy Backend và có DB thật mới thực thi được code download này:
    /*
    const response = await fetch(`${API_CONFIG.baseUrl}/downloads/${id}/tool`, {
        headers: { 'Authorization': `Bearer ${localStorage.getItem(API_CONFIG.storageKey)}` }
    });
    const blob = await response.blob();
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = "MyTool_v1.2.zip";
    a.click();
    */
}

function mockBuy(id) {
    alert("Đang chuyển hướng tới cổng thanh toán VNPay cho sản phẩm ID: " + id);
}
