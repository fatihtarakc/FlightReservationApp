(function () {
    'use strict';

    function populateSelect(selectEl, items, selectedVal) {
        items.forEach(function (item) {
            var opt = document.createElement('option');
            opt.value = item;
            opt.textContent = item;
            if (selectedVal && item === selectedVal)
                opt.selected = true;
            selectEl.appendChild(opt);
        });
    }

    function loadSelect(selectId, url) {
        var el = document.getElementById(selectId);
        if (!el) return;
        var selectedVal = el.dataset.selected || '';
        fetch(url)
            .then(function (r) { return r.json(); })
            .then(function (items) { populateSelect(el, items, selectedVal); })
            .catch(function () {});
    }

    document.addEventListener('DOMContentLoaded', function () {
        loadSelect('countrySelect',  '/api/airport-countries');
        loadSelect('timezoneSelect', '/api/airport-timezones');
    });
})();
