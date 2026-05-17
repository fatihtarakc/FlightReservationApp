var countries = [
    { code: '+90',  flag: '🇹🇷', nameTr: 'Türkiye',                      nameEn: 'Turkey' },
    { code: '+1',   flag: '🇺🇸', nameTr: 'ABD',                           nameEn: 'United States' },
    { code: '+1',   flag: '🇨🇦', nameTr: 'Kanada',                        nameEn: 'Canada' },
    { code: '+44',  flag: '🇬🇧', nameTr: 'Birleşik Krallık',              nameEn: 'United Kingdom' },
    { code: '+49',  flag: '🇩🇪', nameTr: 'Almanya',                       nameEn: 'Germany' },
    { code: '+33',  flag: '🇫🇷', nameTr: 'Fransa',                        nameEn: 'France' },
    { code: '+39',  flag: '🇮🇹', nameTr: 'İtalya',                        nameEn: 'Italy' },
    { code: '+34',  flag: '🇪🇸', nameTr: 'İspanya',                       nameEn: 'Spain' },
    { code: '+31',  flag: '🇳🇱', nameTr: 'Hollanda',                      nameEn: 'Netherlands' },
    { code: '+32',  flag: '🇧🇪', nameTr: 'Belçika',                       nameEn: 'Belgium' },
    { code: '+41',  flag: '🇨🇭', nameTr: 'İsviçre',                       nameEn: 'Switzerland' },
    { code: '+43',  flag: '🇦🇹', nameTr: 'Avusturya',                     nameEn: 'Austria' },
    { code: '+46',  flag: '🇸🇪', nameTr: 'İsveç',                         nameEn: 'Sweden' },
    { code: '+47',  flag: '🇳🇴', nameTr: 'Norveç',                        nameEn: 'Norway' },
    { code: '+45',  flag: '🇩🇰', nameTr: 'Danimarka',                     nameEn: 'Denmark' },
    { code: '+358', flag: '🇫🇮', nameTr: 'Finlandiya',                    nameEn: 'Finland' },
    { code: '+30',  flag: '🇬🇷', nameTr: 'Yunanistan',                    nameEn: 'Greece' },
    { code: '+351', flag: '🇵🇹', nameTr: 'Portekiz',                      nameEn: 'Portugal' },
    { code: '+353', flag: '🇮🇪', nameTr: 'İrlanda',                       nameEn: 'Ireland' },
    { code: '+48',  flag: '🇵🇱', nameTr: 'Polonya',                       nameEn: 'Poland' },
    { code: '+36',  flag: '🇭🇺', nameTr: 'Macaristan',                    nameEn: 'Hungary' },
    { code: '+40',  flag: '🇷🇴', nameTr: 'Romanya',                       nameEn: 'Romania' },
    { code: '+420', flag: '🇨🇿', nameTr: 'Çekya',                         nameEn: 'Czech Republic' },
    { code: '+421', flag: '🇸🇰', nameTr: 'Slovakya',                      nameEn: 'Slovakia' },
    { code: '+386', flag: '🇸🇮', nameTr: 'Slovenya',                      nameEn: 'Slovenia' },
    { code: '+385', flag: '🇭🇷', nameTr: 'Hırvatistan',                   nameEn: 'Croatia' },
    { code: '+381', flag: '🇷🇸', nameTr: 'Sırbistan',                     nameEn: 'Serbia' },
    { code: '+387', flag: '🇧🇦', nameTr: 'Bosna-Hersek',                  nameEn: 'Bosnia and Herzegovina' },
    { code: '+389', flag: '🇲🇰', nameTr: 'Kuzey Makedonya',               nameEn: 'North Macedonia' },
    { code: '+355', flag: '🇦🇱', nameTr: 'Arnavutluk',                    nameEn: 'Albania' },
    { code: '+382', flag: '🇲🇪', nameTr: 'Karadağ',                       nameEn: 'Montenegro' },
    { code: '+7',   flag: '🇷🇺', nameTr: 'Rusya',                         nameEn: 'Russia' },
    { code: '+380', flag: '🇺🇦', nameTr: 'Ukrayna',                       nameEn: 'Ukraine' },
    { code: '+375', flag: '🇧🇾', nameTr: 'Belarus',                       nameEn: 'Belarus' },
    { code: '+373', flag: '🇲🇩', nameTr: 'Moldova',                       nameEn: 'Moldova' },
    { code: '+995', flag: '🇬🇪', nameTr: 'Gürcistan',                     nameEn: 'Georgia' },
    { code: '+374', flag: '🇦🇲', nameTr: 'Ermenistan',                    nameEn: 'Armenia' },
    { code: '+994', flag: '🇦🇿', nameTr: 'Azerbaycan',                    nameEn: 'Azerbaijan' },
    { code: '+7',   flag: '🇰🇿', nameTr: 'Kazakistan',                    nameEn: 'Kazakhstan' },
    { code: '+998', flag: '🇺🇿', nameTr: 'Özbekistan',                    nameEn: 'Uzbekistan' },
    { code: '+993', flag: '🇹🇲', nameTr: 'Türkmenistan',                  nameEn: 'Turkmenistan' },
    { code: '+996', flag: '🇰🇬', nameTr: 'Kırgızistan',                   nameEn: 'Kyrgyzstan' },
    { code: '+992', flag: '🇹🇯', nameTr: 'Tacikistan',                    nameEn: 'Tajikistan' },
    { code: '+971', flag: '🇦🇪', nameTr: 'Birleşik Arap Emirlikleri',     nameEn: 'United Arab Emirates' },
    { code: '+966', flag: '🇸🇦', nameTr: 'Suudi Arabistan',               nameEn: 'Saudi Arabia' },
    { code: '+974', flag: '🇶🇦', nameTr: 'Katar',                         nameEn: 'Qatar' },
    { code: '+965', flag: '🇰🇼', nameTr: 'Kuveyt',                        nameEn: 'Kuwait' },
    { code: '+973', flag: '🇧🇭', nameTr: 'Bahreyn',                       nameEn: 'Bahrain' },
    { code: '+968', flag: '🇴🇲', nameTr: 'Umman',                         nameEn: 'Oman' },
    { code: '+962', flag: '🇯🇴', nameTr: 'Ürdün',                         nameEn: 'Jordan' },
    { code: '+961', flag: '🇱🇧', nameTr: 'Lübnan',                        nameEn: 'Lebanon' },
    { code: '+964', flag: '🇮🇶', nameTr: 'Irak',                          nameEn: 'Iraq' },
    { code: '+98',  flag: '🇮🇷', nameTr: 'İran',                          nameEn: 'Iran' },
    { code: '+972', flag: '🇮🇱', nameTr: 'İsrail',                        nameEn: 'Israel' },
    { code: '+90',  flag: '🇨🇾', nameTr: 'Kıbrıs',                        nameEn: 'Cyprus' },
    { code: '+20',  flag: '🇪🇬', nameTr: 'Mısır',                         nameEn: 'Egypt' },
    { code: '+212', flag: '🇲🇦', nameTr: 'Fas',                           nameEn: 'Morocco' },
    { code: '+213', flag: '🇩🇿', nameTr: 'Cezayir',                       nameEn: 'Algeria' },
    { code: '+216', flag: '🇹🇳', nameTr: 'Tunus',                         nameEn: 'Tunisia' },
    { code: '+218', flag: '🇱🇾', nameTr: 'Libya',                         nameEn: 'Libya' },
    { code: '+249', flag: '🇸🇩', nameTr: 'Sudan',                         nameEn: 'Sudan' },
    { code: '+251', flag: '🇪🇹', nameTr: 'Etiyopya',                      nameEn: 'Ethiopia' },
    { code: '+254', flag: '🇰🇪', nameTr: 'Kenya',                         nameEn: 'Kenya' },
    { code: '+255', flag: '🇹🇿', nameTr: 'Tanzanya',                      nameEn: 'Tanzania' },
    { code: '+256', flag: '🇺🇬', nameTr: 'Uganda',                        nameEn: 'Uganda' },
    { code: '+234', flag: '🇳🇬', nameTr: 'Nijerya',                       nameEn: 'Nigeria' },
    { code: '+233', flag: '🇬🇭', nameTr: 'Gana',                          nameEn: 'Ghana' },
    { code: '+221', flag: '🇸🇳', nameTr: 'Senegal',                       nameEn: 'Senegal' },
    { code: '+27',  flag: '🇿🇦', nameTr: 'Güney Afrika',                  nameEn: 'South Africa' },
    { code: '+86',  flag: '🇨🇳', nameTr: 'Çin',                           nameEn: 'China' },
    { code: '+81',  flag: '🇯🇵', nameTr: 'Japonya',                       nameEn: 'Japan' },
    { code: '+82',  flag: '🇰🇷', nameTr: 'Güney Kore',                    nameEn: 'South Korea' },
    { code: '+91',  flag: '🇮🇳', nameTr: 'Hindistan',                     nameEn: 'India' },
    { code: '+92',  flag: '🇵🇰', nameTr: 'Pakistan',                      nameEn: 'Pakistan' },
    { code: '+880', flag: '🇧🇩', nameTr: 'Bangladeş',                     nameEn: 'Bangladesh' },
    { code: '+94',  flag: '🇱🇰', nameTr: 'Sri Lanka',                     nameEn: 'Sri Lanka' },
    { code: '+977', flag: '🇳🇵', nameTr: 'Nepal',                         nameEn: 'Nepal' },
    { code: '+66',  flag: '🇹🇭', nameTr: 'Tayland',                       nameEn: 'Thailand' },
    { code: '+84',  flag: '🇻🇳', nameTr: 'Vietnam',                       nameEn: 'Vietnam' },
    { code: '+60',  flag: '🇲🇾', nameTr: 'Malezya',                       nameEn: 'Malaysia' },
    { code: '+65',  flag: '🇸🇬', nameTr: 'Singapur',                      nameEn: 'Singapore' },
    { code: '+62',  flag: '🇮🇩', nameTr: 'Endonezya',                     nameEn: 'Indonesia' },
    { code: '+63',  flag: '🇵🇭', nameTr: 'Filipinler',                    nameEn: 'Philippines' },
    { code: '+93',  flag: '🇦🇫', nameTr: 'Afganistan',                    nameEn: 'Afghanistan' },
    { code: '+55',  flag: '🇧🇷', nameTr: 'Brezilya',                      nameEn: 'Brazil' },
    { code: '+54',  flag: '🇦🇷', nameTr: 'Arjantin',                      nameEn: 'Argentina' },
    { code: '+52',  flag: '🇲🇽', nameTr: 'Meksika',                       nameEn: 'Mexico' },
    { code: '+57',  flag: '🇨🇴', nameTr: 'Kolombiya',                     nameEn: 'Colombia' },
    { code: '+56',  flag: '🇨🇱', nameTr: 'Şili',                          nameEn: 'Chile' },
    { code: '+51',  flag: '🇵🇪', nameTr: 'Peru',                          nameEn: 'Peru' },
    { code: '+58',  flag: '🇻🇪', nameTr: 'Venezuela',                     nameEn: 'Venezuela' },
    { code: '+61',  flag: '🇦🇺', nameTr: 'Avustralya',                    nameEn: 'Australia' },
    { code: '+64',  flag: '🇳🇿', nameTr: 'Yeni Zelanda',                  nameEn: 'New Zealand' },
];

var selectedCountryCode = '+90';
var pageLang = document.documentElement.lang || 'tr';

function countryName(c) {
    return pageLang === 'en' ? (c.nameEn || c.name) : (c.nameTr || c.name);
}

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
        var noResult = pageLang === 'en' ? 'No results found' : 'Sonuç bulunamadı';
        $ul.append('<li style="padding:10px 14px;color:#6c757d;font-size:.85rem">' + noResult + '</li>');
    } else {
        list.forEach(function (c) {
            var flag = isoFlag(c.code);
            var name = countryName(c);
            $('<li></li>')
                .html('<span style="margin-right:8px;font-size:1.15rem">' + flag + '</span>' +
                      '<span>' + name + '</span>' +
                      '<span style="float:right;color:#6c757d;font-size:.8rem">' + c.code + '</span>')
                .css({ padding: '9px 14px', cursor: 'pointer', fontSize: '.875rem' })
                .on('mouseenter', function () { $(this).css('background', '#f0f4ff'); })
                .on('mouseleave', function () { $(this).css('background', ''); })
                .on('mousedown', function (e) {
                    e.preventDefault();
                    var name = countryName(c);
                    $('#natCode').val(c.code);
                    $('#natSearch').val(name).removeClass('is-invalid');
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
                if (found) { $('#natSearch').val(countryName(found)); $('#natFlag').text(isoFlag(found.code)); }
            }
        });

    $('#natSearch')
        .on('focus', function () {
            var q = $(this).val().toLowerCase();
            renderNat(q ? _natAll.filter(function (c) { return countryName(c).toLowerCase().includes(q) || c.code.toLowerCase().includes(q); }) : _natAll);
        })
        .on('input', function () {
            var q = $(this).val().toLowerCase();
            if (!q) { renderNat(_natAll); return; }
            renderNat(_natAll.filter(function (c) { return countryName(c).toLowerCase().includes(q) || c.code.toLowerCase().includes(q); }));
        })
        .on('blur', function () { setTimeout(function () { $('#natDropdown').hide(); }, 150); });
}
// ─────────────────────────────────────────────────────────────────────────────

function renderCountries(list) {
    var $ul = $('#countryList');
    $ul.find('li:not(:first-child)').remove();
    list.forEach(function (c) {
        var name = countryName(c);
        var li = $('<li></li>');
        var a = $('<a class="dropdown-item" href="#"></a>')
            .html('<span class="me-2">' + c.flag + '</span><span>' + name + '</span><span class="float-end text-muted">' + c.code + '</span>')
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
            return countryName(c).toLowerCase().includes(q) || c.code.includes(q);
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
