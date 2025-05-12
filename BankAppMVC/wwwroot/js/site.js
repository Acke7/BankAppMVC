// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

    $('#loadMoreBtn').on('click', function () {
          var button = $(this);
    var skip = button.data('skip');
    var accountNumber = button.data('accountnumber'); // lowercase key used in HTML

    $.get('/Account/LoadMoreTransactions', {AccountNumber: accountNumber, skip: skip }, function (data) {
              if (data.trim() !== '') {
        $('#transaction-body').append(data);
    button.data('skip', skip + 20);
              } else {
        button.hide(); // No more transactions
              }
          });
      });

function applyThemeBackgroundClass() {
    const theme = document.documentElement.getAttribute("data-bs-theme");
    const body = document.getElementById("app-body");
    const secondaryButtons = document.querySelectorAll(".btn-secondary");

    body.classList.remove("bg-app-wrapper", "bg-app-wrapper-light");

    if (theme === "dark") {
        body.classList.add("bg-app-wrapper");
        secondaryButtons.forEach(btn => btn.classList.remove("light-theme"));
    } else if (theme === "light") {
        body.classList.add("bg-app-wrapper-light");
        secondaryButtons.forEach(btn => btn.classList.add("light-theme"));
    }
}
    // Run on page load
    applyThemeBackgroundClass();

    // Watch for theme changes
    const observer = new MutationObserver(applyThemeBackgroundClass);
    observer.observe(document.documentElement, {
        attributes: true,
    attributeFilter: ["data-bs-theme"]
        });


