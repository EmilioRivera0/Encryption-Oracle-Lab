(function () {
    if (typeof Chart === 'undefined') {
        return;
    }

    const ink = '#212529';
    const muted = '#adb5bd';
    const accent = '#198754';
    const soft = 'rgba(33, 37, 41, 0.08)';

    Chart.defaults.font.family = getComputedStyle(document.body).fontFamily;
    Chart.defaults.color = '#6c757d';
    Chart.defaults.plugins.legend.labels.usePointStyle = true;
    Chart.defaults.plugins.legend.labels.boxWidth = 8;
    Chart.defaults.maintainAspectRatio = false;

    const gridless = {
        x: { grid: { display: false }, border: { display: false } },
        y: { grid: { color: soft }, border: { display: false }, ticks: { maxTicksLimit: 4 } }
    };

    function gradient(ctx, color) {
        const g = ctx.createLinearGradient(0, 0, 0, ctx.canvas.clientHeight || 200);
        g.addColorStop(0, color.replace('1)', '0.25)'));
        g.addColorStop(1, color.replace('1)', '0)'));
        return g;
    }

    function create(id, config) {
        const el = document.getElementById(id);
        if (el) {
            new Chart(el, typeof config === 'function' ? config(el.getContext('2d')) : config);
        }
    }

    create('staffSalesChart', ctx => ({
        type: 'line',
        data: {
            labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
            datasets: [{
                label: 'Sales',
                data: [5200, 6100, 5800, 7400, 8900, 6300, 4800],
                borderColor: ink,
                backgroundColor: gradient(ctx, 'rgba(33, 37, 41, 1)'),
                fill: true,
                tension: 0.4,
                pointRadius: 0,
                borderWidth: 2
            }]
        },
        options: { plugins: { legend: { display: false } }, scales: gridless }
    }));

    create('staffBudgetChart', {
        type: 'doughnut',
        data: {
            labels: ['Engineering', 'Sales', 'Operations', 'Marketing', 'HR'],
            datasets: [{
                data: [98000, 41200, 58900, 32500, 12750],
                backgroundColor: ['#212529', '#495057', '#868e96', '#ced4da', '#e9ecef'],
                borderWidth: 0
            }]
        },
        options: { cutout: '72%', plugins: { legend: { position: 'bottom' } } }
    });

    create('adminServerChart', ctx => ({
        type: 'line',
        data: {
            labels: Array.from({ length: 12 }, (_, i) => `${String(i * 2).padStart(2, '0')}:00`),
            datasets: [
                {
                    label: 'CPU %',
                    data: [22, 25, 19, 31, 44, 58, 63, 55, 49, 41, 36, 28],
                    borderColor: ink,
                    backgroundColor: gradient(ctx, 'rgba(33, 37, 41, 1)'),
                    fill: true,
                    tension: 0.4,
                    pointRadius: 0,
                    borderWidth: 2
                },
                {
                    label: 'Memory %',
                    data: [48, 49, 47, 52, 57, 61, 66, 64, 60, 58, 55, 51],
                    borderColor: accent,
                    backgroundColor: gradient(ctx, 'rgba(25, 135, 84, 1)'),
                    fill: true,
                    tension: 0.4,
                    pointRadius: 0,
                    borderWidth: 2
                }
            ]
        },
        options: {
            interaction: { mode: 'index', intersect: false },
            plugins: { legend: { position: 'top', align: 'end' } },
            scales: { ...gridless, y: { ...gridless.y, max: 100 } }
        }
    }));

    create('adminOperationsChart', {
        type: 'bar',
        data: {
            labels: ['Deposits', 'Withdrawals', 'Transfers', 'Payments', 'Loans'],
            datasets: [{
                label: 'Operations',
                data: [4820, 3910, 6230, 5140, 860],
                backgroundColor: ink,
                borderRadius: 6,
                barThickness: 18
            }]
        },
        options: { plugins: { legend: { display: false } }, scales: gridless }
    });

    create('adminStatusChart', {
        type: 'doughnut',
        data: {
            labels: ['Completed', 'Pending', 'Failed'],
            datasets: [{
                data: [94.1, 5.5, 0.4],
                backgroundColor: [ink, muted, '#dc3545'],
                borderWidth: 0
            }]
        },
        options: { cutout: '72%', plugins: { legend: { position: 'bottom' } } }
    });
})();
