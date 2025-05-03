// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

async function fetchAlerts() {
    try {
        // Show loading state
        const container = document.getElementById('alertsContainer');
        container.innerHTML = `
                    <h6 class="dropdown-header">Alerts Center</h6>
                    <div class="text-center p-3">
                        <div class="spinner-border text-primary" role="status">
                            <span class="sr-only">Loading...</span>
                        </div>
                    </div>
                `;

        // Fetch data from your API endpoint
        const response = await fetch('/notes');
        if (!response.ok) throw new Error('Network response was not ok');

        const alerts = await response.json();
        console.log(alerts);
        // Build HTML content
        let alertsHtml = `
            <h6 class="dropdown-header">Notifications Center</h6>
            ${alerts.length > 0 ?
                alerts.map(alert => `
                    <a class="dropdown-item d-flex align-items-center">
                        <div>
                            <div class="small text-gray-500">${new Date(alert.createdAt).toLocaleDateString()}</div>
                            <span>${alert.message}</span>
                        </div>
                    </a>
                `).join('')
                :
                `<div class="dropdown-item text-muted text-center py-3">
                    <i class="far fa-bell-slash fa-2x mb-2"></i>
                    <div>There are no new notifications</div>
                </div>`
            }
            <!-- ALWAYS SHOW "Show All Notifications" LINK -->
            <a class="dropdown-item text-center small text-gray-500" href="/Notifications">Show All Notifications</a>
        `;

        container.innerHTML = alertsHtml;
    } catch (error) {
        console.error('Error fetching alerts:', error);
        document.getElementById('alertsContainer').innerHTML = `
                            <h6 class="dropdown-header">Notifications Center</h6>
                    <div class="dropdown-item text-danger">Failed to load notifications</div>
                `;
    }
document.getElementById('alertsDropdown').addEventListener('show.bs.dropdown', fetchAlerts);
}

function showToastr(message) {
    toastr.success(message);
}

function hundleAddMemberToTeam(hasUsers) {
    console.log(hasUsers);
    if (hasUsers=='True') {
        $('#addMemberModal').modal('show');
    } else {
        toastr.info('There is user left to be added to this team');
    }
}
function GetStatusBadgeClass(status)
{
    switch (status) {
        case "1":
            return "badge-secondary";
        case "2":
            return "badge-primary";
        case "3":
            return "badge-success";
        default:
            return "badge-light";
    }
    }