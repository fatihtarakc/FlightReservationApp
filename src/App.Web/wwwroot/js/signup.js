var countries = [
    { code: '+90', flag: '🇹🇷', name: 'Türkiye' },
    { code: '+1', flag: '🇺🇸', name: 'ABD / Kanada' },
    { code: '+44', flag: '🇬🇧', name: 'İngiltere' },
    { code: '+49', flag: '🇩🇪', name: 'Almanya' },
    { code: '+33', flag: '🇫🇷', name: 'Fransa' },
    { code: '+39', flag: '🇮🇹', name: 'İtalya' },
    { code: '+34', flag: '🇪🇸', name: 'İspanya' },
    { code: '+31', flag: '🇳🇱', name: 'Hollanda' },
    { code: '+32', flag: '🇧🇪', name: 'Belçika' },
    { code: '+41', flag: '🇨🇭', name: 'İsviçre' },
    { code: '+43', flag: '🇦🇹', name: 'Avusturya' },
    { code: '+46', flag: '🇸🇪', name: 'İsveç' },
    { code: '+47', flag: '🇳🇴', name: 'Norveç' },
    { code: '+45', flag: '🇩🇰', name: 'Danimarka' },
    { code: '+358', flag: '🇫🇮', name: 'Finlandiya' },
    { code: '+30', flag: '🇬🇷', name: 'Yunanistan' },
    { code: '+7', flag: '🇷🇺', name: 'Rusya' },
    { code: '+380', flag: '🇺🇦', name: 'Ukrayna' },
    { code: '+48', flag: '🇵🇱', name: 'Polonya' },
    { code: '+36', flag: '🇭🇺', name: 'Macaristan' },
    { code: '+40', flag: '🇷🇴', name: 'Romanya' },
    { code: '+420', flag: '🇨🇿', name: 'Çekya' },
    { code: '+971', flag: '🇦🇪', name: 'Birleşik Arap Emirlikleri' },
    { code: '+966', flag: '🇸🇦', name: 'Suudi Arabistan' },
    { code: '+20', flag: '🇪🇬', name: 'Mısır' },
    { code: '+212', flag: '🇲🇦', name: 'Fas' },
    { code: '+213', flag: '🇩🇿', name: 'Cezayir' },
    { code: '+216', flag: '🇹🇳', name: 'Tunus' },
    { code: '+98', flag: '🇮🇷', name: 'İran' },
    { code: '+964', flag: '🇮🇶', name: 'Irak' },
    { code: '+962', flag: '🇯🇴', name: 'Ürdün' },
    { code: '+961', flag: '🇱🇧', name: 'Lübnan' },
    { code: '+90392', flag: '🇨🇾', name: 'Kıbrıs' },
    { code: '+994', flag: '🇦🇿', name: 'Azerbaycan' },
    { code: '+995', flag: '🇬🇪', name: 'Gürcistan' },
    { code: '+374', flag: '🇦🇲', name: 'Ermenistan' },
    { code: '+86', flag: '🇨🇳', name: 'Çin' },
    { code: '+81', flag: '🇯🇵', name: 'Japonya' },
    { code: '+82', flag: '🇰🇷', name: 'Güney Kore' },
    { code: '+91', flag: '🇮🇳', name: 'Hindistan' },
    { code: '+92', flag: '🇵🇰', name: 'Pakistan' },
    { code: '+880', flag: '🇧🇩', name: 'Bangladeş' },
    { code: '+27', flag: '🇿🇦', name: 'Güney Afrika' },
    { code: '+234', flag: '🇳🇬', name: 'Nijerya' },
    { code: '+254', flag: '🇰🇪', name: 'Kenya' },
    { code: '+55', flag: '🇧🇷', name: 'Brezilya' },
    { code: '+54', flag: '🇦🇷', name: 'Arjantin' },
    { code: '+52', flag: '🇲🇽', name: 'Meksika' },
    { code: '+57', flag: '🇨🇴', name: 'Kolombiya' },
    { code: '+61', flag: '🇦🇺', name: 'Avustralya' },
    { code: '+64', flag: '🇳🇿', name: 'Yeni Zelanda' }
];

var selectedCountryCode = '+90';

// ── Nationality picker ────────────────────────────────────────────────────────
var _natAll = [];

function isoFlag(code) {
    if (!code || code.length !== 2) return '🌍';
    return [...code.toUpperCase()].map(function (c) {
        return String.fromCodePoint(0x1F1E6 + c.charCodeAt(0) - 65);
    }).join('');
}

function renderNat(list) {
    var $ul = $('#natDropdown');
    $ul.empty();
    if (!list.length) {
        $ul.append('<li style="padding:10px 14px;color:#6c757d;font-size:.85rem">Sonuç bulunamadı</li>');
    } else {
        list.forEach(function (c) {
            var flag = isoFlag(c.code);
            $('<li></li>')
                .html('<span style="margin-right:8px;font-size:1.15rem">' + flag + '</span>' +
                      '<span>' + c.name + '</span>' +
                      '<span style="float:right;color:#6c757d;font-size:.8rem">' + c.code + '</span>')
                .css({ padding: '9px 14px', cursor: 'pointer', fontSize: '.875rem' })
                .on('mouseenter', function () { $(this).css('background', '#f0f4ff'); })
                .on('mouseleave', function () { $(this).css('background', ''); })
                .on('mousedown', function (e) {
                    e.preventDefault();
                    $('#natCode').val(c.code);
                    $('#natSearch').val(c.name).removeClass('is-invalid');
                    $('#natFlag').text(flag);
                    $('#natDropdown').hide();
                    $('#natValidation').text('');
                })
                .appendTo($ul);
        });
    }
    $ul.show();
}

function initNatPicker() {
    fetch('/api/countries')
        .then(function (r) { return r.json(); })
        .then(function (data) {
            _natAll = data;
            var preCode = $('#natCode').val();
            if (preCode) {
                var found = data.find(function (c) { return c.code === preCode; });
                if (found) { $('#natSearch').val(found.name); $('#natFlag').text(isoFlag(found.code)); }
            }
        });

    $('#natSearch')
        .on('focus', function () {
            var q = $(this).val().toLowerCase();
            renderNat(q ? _natAll.filter(function (c) { return c.name.toLowerCase().includes(q) || c.code.toLowerCase().includes(q); }) : _natAll);
        })
        .on('input', function () {
            var q = $(this).val().toLowerCase();
            if (!q) { renderNat(_natAll); return; }
            renderNat(_natAll.filter(function (c) { return c.name.toLowerCase().includes(q) || c.code.toLowerCase().includes(q); }));
        })
        .on('blur', function () { setTimeout(function () { $('#natDropdown').hide(); }, 150); });
}
// ─────────────────────────────────────────────────────────────────────────────

function renderCountries(list) {
    var $ul = $('#countryList');
    $ul.find('li:not(:first-child)').remove();
    list.forEach(function (c) {
        var li = $('<li></li>');
        var a = $('<a class="dropdown-item" href="#"></a>')
            .html('<span class="me-2">' + c.flag + '</span><span>' + c.name + '</span><span class="float-end text-muted">' + c.code + '</span>')
            .on('click', function (e) {
                e.preventDefault();
                selectedCountryCode = c.code;
                $('#selectedFlag').text(c.flag);
                $('#selectedCode').text(c.code);
                updateFullPhone();
                $('#countryDropBtn').dropdown('hide');
            });
        li.append(a);
        $ul.append(li);
    });
}

function updateFullPhone() {
    var num = $('#phoneNumber').val().replace(/\D/g, '');
    $('#fullPhoneNumber').val(selectedCountryCode + num);
}

$(document).ready(function () {
    if (!document.getElementById('signUpForm')) return;

    var form = document.getElementById('signUpForm');
    var msgs = {
        name: form.dataset.valName || '',
        surname: form.dataset.valSurname || '',
        username: form.dataset.valUsername || '',
        email: form.dataset.valEmail || '',
        phone: form.dataset.valPhone || '',
        birthdate: form.dataset.valBirthdate || '',
        birthdateAge: form.dataset.valBirthdateAge || '',
        password: form.dataset.valPassword || '',
        confirm: form.dataset.valConfirm || '',
        nationality: form.dataset.valNationality || ''
    };

    initNatPicker();
    renderCountries(countries);

    $('#countrySearch').on('input', function () {
        var q = $(this).val().toLowerCase();
        renderCountries(countries.filter(function (c) {
            return c.name.toLowerCase().includes(q) || c.code.includes(q);
        }));
    });

    $('#phoneNumber').on('input', function () {
        $(this).val($(this).val().replace(/[^0-9\s\-]/g, ''));
        updateFullPhone();
    });

    $('#regPassword').on('input', function () {
        var val = $(this).val();
        var strength = 0;
        if (val.length >= 8) strength++;
        if (/[A-Z]/.test(val)) strength++;
        if (/[a-z]/.test(val)) strength++;
        if (/[0-9]/.test(val)) strength++;
        if (/[^a-zA-Z0-9]/.test(val)) strength++;
        var colors = ['#ef5350', '#ff7043', '#ffa726', '#66bb6a', '#26a69a'];
        var bar = document.getElementById('strengthBar');
        if (bar) {
            bar.style.width = (strength * 20) + '%';
            bar.style.background = colors[strength - 1] || '#ef5350';
        }
        var barContainer = document.getElementById('passwordStrengthBar');
        if (barContainer) barContainer.style.display = val ? '' : 'none';
    });

    function setError(inputId, errorId, msg) {
        $('#' + inputId).addClass('is-invalid');
        $('#' + errorId).text(msg).show();
    }

    function clearErrors() {
        $('.is-invalid').removeClass('is-invalid');
        $('.invalid-feedback').text('').hide();
    }

    $('.form-control').on('input change', function () { $(this).removeClass('is-invalid'); });

    $('#signUpForm').on('submit', function (e) {
        var valid = true;
        clearErrors();

        if (!$('#regName').val() || $('#regName').val().trim().length < 2)
            { setError('regName', 'nameError', msgs.name); valid = false; }

        if (!$('#regSurname').val() || $('#regSurname').val().trim().length < 2)
            { setError('regSurname', 'surnameError', msgs.surname); valid = false; }

        var username = $('#regUsername').val().trim();
        if (!username || username.length < 3 || !/^[a-zA-Z0-9._-]+$/.test(username))
            { setError('regUsername', 'usernameError', msgs.username); valid = false; }

        var email = $('#regEmail').val().trim();
        if (!email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email))
            { setError('regEmail', 'emailError', msgs.email); valid = false; }

        var phone = $('#fullPhoneNumber').val();
        if (!phone || !/^\+[1-9]\d{7,14}$/.test(phone))
            { $('#phoneNumber').addClass('is-invalid'); $('#phoneError').text(msgs.phone); valid = false; }

        var bd = $('#regBirthDate').val();
        if (!bd) {
            setError('regBirthDate', 'birthDateError', msgs.birthdate); valid = false;
        } else {
            var age = (new Date() - new Date(bd)) / (1000 * 60 * 60 * 24 * 365.25);
            if (age < 18) { setError('regBirthDate', 'birthDateError', msgs.birthdateAge); valid = false; }
        }

        var pwd = $('#regPassword').val();
        if (!pwd || pwd.length < 8 || !/[A-Z]/.test(pwd) || !/[a-z]/.test(pwd) || !/[0-9]/.test(pwd) || !/[^a-zA-Z0-9]/.test(pwd))
            { setError('regPassword', 'passwordError', msgs.password); valid = false; }

        if ($('#regConfirmPassword').val() !== pwd)
            { setError('regConfirmPassword', 'confirmPasswordError', msgs.confirm); valid = false; }

        if (!$('#natCode').val()) {
            $('#natSearch').addClass('is-invalid');
            $('#natValidation').text(msgs.nationality);
            valid = false;
        }

        if (!valid) e.preventDefault();
        else updateFullPhone();
    });
});
