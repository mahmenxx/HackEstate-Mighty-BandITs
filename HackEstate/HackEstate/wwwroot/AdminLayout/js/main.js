function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    const mainContent = document.getElementById('main-content');
    const topBar = document.getElementById('top-bar');
    sidebar.classList.toggle('collapsed');
    mainContent.classList.toggle('collapsed');
    topBar.classList.toggle('collapsed');
}

function filterUsers(role) {
    const table = document.getElementById('users-table');
    const rows = table.getElementsByTagName('tr');

    for (let i = 0; i < rows.length; i++) {
        const rowRole = rows[i].getAttribute('data-role');
        if (role === 'All' || rowRole === role) {
            rows[i].style.display = '';
        } else {
            rows[i].style.display = 'none';
        }
    }

    // Update the dropdown button text
    const dropdownButton = document.querySelector('.filter-dropdown .btn');
    dropdownButton.textContent = role === 'All' ? 'Filter by Role' : `Role: ${role}`;
}