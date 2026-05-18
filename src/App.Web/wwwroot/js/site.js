// ── Page Loader ──
(function () {
    var loader = document.getElementById('pageLoader');
    if (!loader) return;

    var _needsAirports = !!document.getElementById('departurePicker');
    var _pageLoaded    = false;
    var _airportsReady = !_needsAirports;

    function _tryHide() {
        if (_pageLoaded && _airportsReady) loader.classList.add('loader-hide');
    }

    window.addEventListener('load', function () {
        _pageLoaded = true;
        _tryHide();
    });

    document.addEventListener('airportsReady', function () {
        _airportsReady = true;
        _tryHide();
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
