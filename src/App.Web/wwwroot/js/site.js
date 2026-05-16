// ── Page Loader ──
(function () {
    var loader = document.getElementById('pageLoader');
    if (!loader) return;

    window.addEventListener('load', function () {
        loader.classList.add('loader-hide');
    });

    document.addEventListener('click', function (e) {
        var a = e.target.closest('a[href]');
        if (!a) return;
        var href = a.getAttribute('href');
        if (!href || href === '#' || href.startsWith('#') || href.startsWith('javascript') || a.target === '_blank') return;
        if (a.hasAttribute('data-bs-toggle') || a.hasAttribute('data-bs-target')) return;
        loader.classList.remove('loader-hide');
    });

    document.addEventListener('submit', function () {
        loader.classList.remove('loader-hide');
    });
})();

// Toggle password field visibility
function togglePassword(id, btn) {
    const input = document.getElementById(id);
    const isText = input.type === 'text';
    input.type = isText ? 'password' : 'text';
    btn.innerHTML = isText ? '<i class="fas fa-eye"></i>' : '<i class="fas fa-eye-slash"></i>';
}
