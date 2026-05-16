(function () {
    var departureSelect = document.getElementById('departureSelect');
    var arrivalSelect = document.getElementById('arrivalSelect');

    if (!departureSelect || !arrivalSelect) return;

    var selectLabel = arrivalSelect.dataset.selectLabel || '— Seçin —';
    var preSelectedArrival = arrivalSelect.dataset.selectedValue || '';

    function populateArrival(origin, restoreValue) {
        arrivalSelect.innerHTML = '<option value="">' + selectLabel + '</option>';
        if (!origin) return;

        fetch('/api/available-destinations?origin=' + encodeURIComponent(origin))
            .then(function (res) { return res.json(); })
            .then(function (airports) {
                airports.forEach(function (a) {
                    var opt = document.createElement('option');
                    opt.value = a.iataCode;
                    opt.textContent = a.iataCode + ' - ' + a.city;
                    if (restoreValue && a.iataCode === restoreValue) opt.selected = true;
                    arrivalSelect.appendChild(opt);
                });
            })
            .catch(function () {});
    }

    if (departureSelect.value) {
        populateArrival(departureSelect.value, preSelectedArrival);
    }

    departureSelect.addEventListener('change', function () {
        populateArrival(this.value, '');
    });
})();
