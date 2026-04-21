(function () {
    // Inject overlay HTML once DOM is ready
    function injectOverlay() {
        if (document.getElementById('login-loading-overlay')) return;
        var overlay = document.createElement('div');
        overlay.id = 'login-loading-overlay';
        overlay.innerHTML =
            '<div class="login-loading-spinner"></div>' +
            '<span class="login-loading-text">Signing in, please wait...</span>';
        document.body.appendChild(overlay);
    }

    function showOverlay() {
        var overlay = document.getElementById('login-loading-overlay');
        if (overlay) overlay.classList.add('active');
    }

    function hideOverlay() {
        var overlay = document.getElementById('login-loading-overlay');
        if (overlay) overlay.classList.remove('active');
    }

    function attachFormListeners() {
        // ABP login form id is "LoginForm"; also catch any form with a submit button on the page
        var forms = document.querySelectorAll('form');
        forms.forEach(function (form) {
            form.addEventListener('submit', function () {
                showOverlay();
            });
        });

        // Catch explicit button clicks that may trigger navigation (e.g. external login buttons)
        var loginButtons = document.querySelectorAll(
            'button[type="submit"], input[type="submit"], .login-btn, .external-login button'
        );
        loginButtons.forEach(function (btn) {
            btn.addEventListener('click', function () {
                // Small delay to let validation run first — only show if form is valid
                setTimeout(function () {
                    var invalid = document.querySelector('.input-validation-error, [aria-invalid="true"]');
                    if (!invalid) showOverlay();
                }, 80);
            });
        });
    }

    // Hide overlay when navigating back (browser back button restores cached page)
    window.addEventListener('pageshow', function (e) {
        if (e.persisted) hideOverlay();
    });

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function () {
            injectOverlay();
            attachFormListeners();
        });
    } else {
        injectOverlay();
        attachFormListeners();
    }
})();
