(function () {
    'use strict';

    function populateSelect(selectEl, items, selectedId) {
        items.forEach(function (item) {
            var opt = document.createElement('option');
            opt.value = item.id;
            opt.textContent = item.name;
            if (selectedId && item.id.toLowerCase() === selectedId.toLowerCase()) {
                opt.selected = true;
            }
            selectEl.appendChild(opt);
        });
    }

    function loadSelect(selectId, url) {
        var el = document.getElementById(selectId);
        if (!el) return;
        var selectedId = el.dataset.selected || '';
        fetch(url)
            .then(function (r) { return r.json(); })
            .then(function (items) { populateSelect(el, items, selectedId); })
            .catch(function () { /* silently ignore — select stays empty */ });
    }

    document.addEventListener('DOMContentLoaded', function () {
        loadSelect('airlineSelect', '/api/aircraft-airlines');
        loadSelect('modelSelect',   '/api/aircraft-models');
    });
})();
