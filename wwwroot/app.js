// Inicialização do AOS e outros componentes após o carregamento do DOM
document.addEventListener('DOMContentLoaded', () => {
    AOS.init();
    initializeCharts();
    fetchProfile('seu_screen_name');
    fetchTimeline('seu_screen_name');
    fetchStats('seu_screen_name');

    // Event Listeners
    document.getElementById('refreshButton').addEventListener('click', () => {
        fetchProfile('seu_screen_name');
        fetchTimeline('seu_screen_name');
        fetchStats('seu_screen_name');
    });

    document.getElementById('themeToggle').addEventListener('click', toggleTheme);
});

// Função para alternar tema (opcional)
function toggleTheme() {
    document.body.classList.toggle('dark-theme');
    // Adicione lógica adicional para alternar ícones ou outros elementos se necessário
}

// Configuração dos Gráficos
function initializeCharts() {
    const engagementCtx = document.getElementById('engagementChart').getContext('2d');
    new Chart(engagementCtx, {
        type: 'line',
        data: {
            labels: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun'],
            datasets: [{
                label: 'Engajamento',
                data: [12, 19, 3, 5, 2, 3],
                borderColor: '#3b82f6',
                tension: 0.4
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    labels: {
                        color: '#fff'
                    }
                }
            },
            scales: {
                y: {
                    ticks: {
                        color: '#fff'
                    }
                },
                x: {
                    ticks: {
                        color: '#fff'
                    }
                }
            }
        }
    });

    const growthCtx = document.getElementById('growthChart').getContext('2d');
    new Chart(growthCtx, {
        type: 'bar',
        data: {
            labels: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun'],
            datasets: [{
                label: 'Crescimento',
                data: [30, 25, 35, 40, 45, 50],
                backgroundColor: '#10b981'
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    labels: {
                        color: '#fff'
                    }
                }
            },
            scales: {
                y: {
                    ticks: {
                        color: '#fff'
                    }
                },
                x: {
                    ticks: {
                        color: '#fff'
                    }
                }
            }
        }
    });
}

// Funções de Fetch para API
async function fetchStats(screenName) {
    try {
        const response = await fetch(`/api/twitter/stats/${screenName}`);
        if (!response.ok) throw new Error('Erro na requisição');

        const stats = await response.json();
        document.getElementById('followersCount').textContent = stats.followers;
        document.getElementById('followingCount').textContent = stats.following;
        document.getElementById('tweetsCount').textContent = stats.tweets;
        document.getElementById('likesCount').textContent = stats.likes;
    } catch (error) {
        console.error('Erro ao carregar estatísticas:', error);
        document.getElementById('followersCount').textContent = 'Erro';
        document.getElementById('followingCount').textContent = 'Erro';
        document.getElementById('tweetsCount').textContent = 'Erro';
        document.getElementById('likesCount').textContent = 'Erro';
    }
}

async function fetchProfile(screenName) {
    try {
        const response = await fetch(`/api/twitter/profile/${screenName}`);
        if (!response.ok) throw new Error('Erro na requisição');

        const profile = await response.json();
        document.getElementById('profile').innerHTML = `
            <div class="flex items-center">
                <img src="${profile.ProfileImageUrl}" alt="Profile" class="w-24 h-24 rounded-full">
                <div class="ml-4">
                    <h2 class="text-xl font-bold text-white">${profile.Name}</h2>
                    <p class="text-gray-400">@${profile.ScreenName}</p>
                    <p class="text-gray-300 mt-2">${profile.Description}</p>
                </div>
            </div>
        `;
    } catch (error) {
        console.error('Erro ao carregar perfil:', error);
        document.getElementById('profile').innerHTML = `
            <div class="text-red-500">Erro ao carregar perfil</div>
        `;
    }
}

async function fetchTimeline(screenName) {
    try {
        const response = await fetch(`/api/twitter/timeline/${screenName}`);
        if (!response.ok) throw new Error('Erro na requisição');

        const tweets = await response.json();
        const timelineHtml = tweets.map(tweet => `
            <div class="bg-zinc-900 p-4 rounded-xl border border-zinc-800">
                <p class="text-white">${tweet.FullText}</p>
                <p class="text-gray-400 text-sm mt-2">${new Date(tweet.CreatedAt).toLocaleString('pt-BR')}</p>
            </div>
        `).join('');
        document.getElementById('timeline').innerHTML = timelineHtml;
    } catch (error) {
        console.error('Erro ao carregar timeline:', error);
        document.getElementById('timeline').innerHTML = `
            <div class="text-red-500">Erro ao carregar tweets</div>
        `;
    }
}