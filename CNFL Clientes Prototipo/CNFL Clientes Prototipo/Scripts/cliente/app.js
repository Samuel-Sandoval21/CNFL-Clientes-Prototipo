/* ============================================================
   APP.JS — Comportamiento global de la app móvil CNFL
   ============================================================ */

(function () {
    'use strict';

    // ══════════════════════════════════════════════════════════
    // 1. TOGGLE DEL MENÚ DE AJUSTES
    // ══════════════════════════════════════════════════════════
    window.cnflToggleSettings = function (e) {
        if (e) e.stopPropagation();
        var menu = document.getElementById('cnflSettingsMenu');
        var overlay = document.getElementById('cnflSettingsOverlay');
        if (!menu) return;
        menu.classList.toggle('open');
        if (overlay) overlay.classList.toggle('open');
    };

    window.cnflCloseSettings = function () {
        var menu = document.getElementById('cnflSettingsMenu');
        var overlay = document.getElementById('cnflSettingsOverlay');
        if (menu) menu.classList.remove('open');
        if (overlay) overlay.classList.remove('open');
    };

    document.addEventListener('click', function (e) {
        var menu = document.getElementById('cnflSettingsMenu');
        if (!menu || !menu.classList.contains('open')) return;
        if (menu.contains(e.target)) return;
        if (e.target.closest('.icon-btn')) return;
        window.cnflCloseSettings();
    });

    // ══════════════════════════════════════════════════════════
    // 2. CERRAR SESIÓN
    // ══════════════════════════════════════════════════════════
    window.cnflLogout = function () {
        if (confirm('¿Seguro que querés cerrar sesión?')) {
            var url = window.CNFL_LOGOUT_URL || '/Cuenta/CerrarSesion';
            window.location.href = url;
        }
    };

    // ══════════════════════════════════════════════════════════
    // 3. ZOOM GLOBAL (TAMAÑO DE TEXTO)
    // ══════════════════════════════════════════════════════════
    var ZOOM_KEY = 'cnfl_zoom';
    var ZOOM_NIVELES = ['small', 'normal', 'large'];
    var ZOOM_LABELS = {
        small: { es: 'Pequeño', en: 'Small', pt: 'Pequeno', fr: 'Petit', cn: '小' },
        normal: { es: 'Normal', en: 'Normal', pt: 'Normal', fr: 'Normal', cn: '正常' },
        large: { es: 'Grande', en: 'Large', pt: 'Grande', fr: 'Grand', cn: '大' }
    };

    function aplicarZoom(nivel) {
        if (ZOOM_NIVELES.indexOf(nivel) === -1) nivel = 'normal';
        document.documentElement.setAttribute('data-zoom', nivel);

        var idx = ZOOM_NIVELES.indexOf(nivel);
        var fillPct = ((idx + 1) / 3) * 100;

        var fill = document.getElementById('cnflZoomFill');
        var lbl = document.getElementById('cnflZoomLbl');
        if (fill) fill.style.width = fillPct + '%';

        if (lbl) {
            var lang = 'CR';
            try { lang = localStorage.getItem('cnfl_lang') || 'CR'; } catch (e) { }
            var langKey = { CR: 'es', US: 'en', BR: 'pt', FR: 'fr', CN: 'cn' }[lang] || 'es';
            lbl.textContent = ZOOM_LABELS[nivel][langKey];
        }

        document.querySelectorAll('.cnfl-zoom-btn').forEach(function (btn) {
            var dir = btn.getAttribute('data-zoom');
            if (dir === 'small') btn.disabled = (nivel === 'small');
            if (dir === 'large') btn.disabled = (nivel === 'large');
        });

        try { localStorage.setItem(ZOOM_KEY, nivel); } catch (e) { }
    }

    function initZoom() {
        var guardado = 'normal';
        try { guardado = localStorage.getItem(ZOOM_KEY) || 'normal'; } catch (e) { }
        aplicarZoom(guardado);

        document.querySelectorAll('.cnfl-zoom-btn').forEach(function (btn) {
            btn.addEventListener('click', function () {
                var current = document.documentElement.getAttribute('data-zoom') || 'normal';
                var idx = ZOOM_NIVELES.indexOf(current);
                var dir = this.getAttribute('data-zoom');

                if (dir === 'small' && idx > 0) {
                    aplicarZoom(ZOOM_NIVELES[idx - 1]);
                } else if (dir === 'large' && idx < ZOOM_NIVELES.length - 1) {
                    aplicarZoom(ZOOM_NIVELES[idx + 1]);
                }
            });
        });
    }

    // ══════════════════════════════════════════════════════════
    // 4. IDIOMA GLOBAL (i18n)
    // ══════════════════════════════════════════════════════════
    var LANG_KEY = 'cnfl_lang';

    var IDIOMAS = [
        { code: 'CR', flag: '🇨🇷', label: 'ES' },
        { code: 'US', flag: '🇺🇸', label: 'EN' },
        { code: 'BR', flag: '🇧🇷', label: 'PT' },
        { code: 'FR', flag: '🇫🇷', label: 'FR' },
        { code: 'CN', flag: '🇨🇳', label: 'ZH' }
    ];

    var TRADUCCIONES = {
        CR: {
            'nav.home': 'Inicio', 'nav.tramites': 'Trámites', 'nav.bills': 'Facturas',
            'nav.reports': 'Reportes', 'nav.products': 'Productos y Servicios', 'nav.profile': 'Perfil',
            'nav.dashboard': 'Dashboard', 'nav.clients': 'Clientes', 'nav.faults': 'Averías', 'nav.logout': 'Salir',

            'settings.language': 'Idioma', 'settings.textSize': 'Tamaño de texto',
            'settings.myProfile': 'Mi Perfil', 'settings.myNotifications': 'Mis Notificaciones', 'settings.logout': 'Cerrar sesión',

            'tramites.hero.title': '📋 Trámites CNFL',
            'tramites.hero.subtitle': 'Realizá tus solicitudes en línea de forma rápida y segura.',
            'tramites.kpi.total': 'MIS TRÁMITES', 'tramites.kpi.pending': 'EN PROCESO',
            'tramites.cat.cambios': 'Cambios y Modificaciones', 'tramites.cat.diseno': 'Diseño e Ingeniería',
            'tramites.cat.tarifas': 'Tarifas y Reclamos', 'tramites.cat.nuevos': 'Servicios Nuevos',
            'tramites.cat.traspasos': 'Traspasos y Suministros',
            'tramites.recent': '🕒 Mis trámites recientes', 'tramites.seeAll': 'Ver todos mis trámites →',
            'tramites.empty.title': 'Sin trámites registrados',
            'tramites.empty.sub': 'Cuando solicites un trámite, aparecerá acá con su número de referencia.',

            'detalle.back': 'Volver a trámites', 'detalle.nise': 'NISE ASOCIADO',
            'detalle.desc': 'DESCRIPCIÓN DEL TRÁMITE',
            'detalle.desc.placeholder': 'Describí brevemente el motivo de tu trámite...',
            'detalle.submit': 'Enviar solicitud', 'detalle.cancel': 'Cancelar', 'detalle.sending': 'Enviando...',
            'detalle.success.title': 'Trámite enviado correctamente', 'detalle.success.ref': 'Número de referencia:',
            'detalle.success.msg': 'Podés consultar el estado en "Mis Trámites".', 'detalle.back.to.list': 'Volver a trámites',

            'inicio.hola': '¡Hola!', 'inicio.resumen': 'Resumen de tu cuenta',
            'inicio.proximoPago': 'PRÓXIMO PAGO', 'inicio.verFacturas': 'Ver facturas',
            'inicio.misNises': 'Mis NISEs', 'inicio.ultimosReportes': 'Últimos reportes',
            'inicio.accesos': 'Accesos rápidos',

            'facturas.title': '💡 Mis Facturas',
            'facturas.sub': 'Gestioná tus pagos pendientes e historial',
            'facturas.totalPendiente': 'TOTAL PENDIENTE', 'facturas.porPagar': 'por pagar',
            'facturas.kpi.pendientes': 'PENDIENTES', 'facturas.kpi.pagadas': 'PAGADAS', 'facturas.kpi.total': 'TOTAL',
            'facturas.pendientes.title': 'Pendientes de pago', 'facturas.historial.title': 'Historial de pagos',
            'facturas.porPagar.badge': 'Por pagar', 'facturas.vencida.badge': 'Vencida', 'facturas.pagada.badge': 'Pagada',
            'facturas.pagar': 'Pagar ahora', 'facturas.vence': 'Vence el', 'facturas.vencio': 'Venció el',
            'facturas.pagadaEl': 'Pagada el', 'facturas.todoAlDia': '¡Todo al día!',
            'facturas.todoAlDia.sub': 'No tenés facturas pendientes de pago. Gracias por tu puntualidad.',

            'reportes.title': '📋 Reportes',
            'reportes.sub': 'Reportá averías y consultá su estado',
            'reportes.reportar': 'Reportar avería', 'reportes.consultar': 'Consultar avería',
            'reportes.mapa': 'Mapa de averías', 'reportes.historial': 'Historial de reportes',
            'reportes.alumbrado': 'Reportar alumbrado público', 'reportes.generar': 'Generar comprobante',

            'productos.title': '🛒 Productos y Servicios',
            'productos.sub': 'Explorá nuestra tienda y servicios',

            'perfil.title': 'Mi Perfil',
            'perfil.resumen': 'Resumen de tu cuenta',
            'perfil.kpi.nises': 'NISEs ACTIVOS', 'perfil.kpi.facturas': 'FACTURAS PENDIENTES',
            'perfil.kpi.suscripciones': 'SUSCRIPCIONES', 'perfil.kpi.averias': 'AVERÍAS ACTIVAS',
            'perfil.misNises': 'Mis NISEs', 'perfil.accesos': 'Accesos rápidos',
            'perfil.ultimasFacturas': 'Últimas facturas', 'perfil.averiasSeg': 'Averías en seguimiento',
            'perfil.editar': 'Editar Perfil', 'perfil.cerrar': 'Cerrar sesión',
            'perfil.calculadora': 'Calculadora', 'perfil.suscripciones': 'Suscripciones',
            'perfil.historialCompras': 'Historial compras', 'perfil.chatbot': 'Chatbot CNFL',
            'perfil.reportes': 'Reportes', 'perfil.tienda': 'Tienda CNFL',

            'notif.title': 'Notificaciones',
            'notif.empty': 'Sin notificaciones por ahora',

            'subs.title': '📦 Mis Suscripciones',
            'subs.sub': 'Servicios contratados y suscripciones activas',
            'subs.activas': 'ACTIVAS', 'subs.mensual': 'MONTO MENSUAL',
            'subs.todas': 'Todas mis suscripciones',
            'subs.empty.title': 'Sin suscripciones registradas',
            'subs.empty.sub': 'Cuando contrates un servicio aparecerá acá.',

            'pagos.title': '🧾 Historial de pagos',
            'pagos.sub': 'Todos tus pagos registrados en la app',
            'pagos.totalPagado': 'TOTAL PAGADO', 'pagos.confirmados': 'CONFIRMADOS', 'pagos.pendientes': 'PENDIENTES',
            'pagos.todos': 'Todos los pagos',
            'pagos.empty.title': 'Sin pagos registrados',
            'pagos.empty.sub': 'Cuando realices tu primer pago aparecerá acá.',

            'chat.title': 'Chatbot CNFL', 'chat.sub': 'Asistente virtual · En línea',
            'chat.placeholder': 'Escribí tu mensaje...',

            'calc.title': '🧮 Calculadora de consumo',
            'calc.sub': 'Estimá el monto aproximado de tu factura',

            'mapa.title': 'Mapa de averías', 'mapa.sub': 'Reportes en tiempo real sobre la red eléctrica',
            'mapa.enRevision': 'En revisión', 'mapa.enCamino': 'Operador en camino', 'mapa.resuelto': 'Resuelto'
        },

        US: {
            'nav.home': 'Home', 'nav.tramites': 'Procedures', 'nav.bills': 'Bills',
            'nav.reports': 'Reports', 'nav.products': 'Products & Services', 'nav.profile': 'Profile',
            'nav.dashboard': 'Dashboard', 'nav.clients': 'Clients', 'nav.faults': 'Faults', 'nav.logout': 'Exit',

            'settings.language': 'Language', 'settings.textSize': 'Text size',
            'settings.myProfile': 'My Profile', 'settings.myNotifications': 'My Notifications', 'settings.logout': 'Log out',

            'tramites.hero.title': '📋 CNFL Procedures',
            'tramites.hero.subtitle': 'Submit your requests online, quickly and securely.',
            'tramites.kpi.total': 'MY PROCEDURES', 'tramites.kpi.pending': 'IN PROGRESS',
            'tramites.cat.cambios': 'Changes & Modifications', 'tramites.cat.diseno': 'Design & Engineering',
            'tramites.cat.tarifas': 'Rates & Claims', 'tramites.cat.nuevos': 'New Services',
            'tramites.cat.traspasos': 'Transfers & Supplies',
            'tramites.recent': '🕒 My recent procedures', 'tramites.seeAll': 'See all my procedures →',
            'tramites.empty.title': 'No procedures registered',
            'tramites.empty.sub': 'When you submit a procedure, it will appear here with its reference number.',

            'detalle.back': 'Back to procedures', 'detalle.nise': 'ASSOCIATED NISE',
            'detalle.desc': 'PROCEDURE DESCRIPTION',
            'detalle.desc.placeholder': 'Briefly describe the reason for your procedure...',
            'detalle.submit': 'Submit request', 'detalle.cancel': 'Cancel', 'detalle.sending': 'Sending...',
            'detalle.success.title': 'Procedure submitted successfully', 'detalle.success.ref': 'Reference number:',
            'detalle.success.msg': 'You can check the status in "My Procedures".', 'detalle.back.to.list': 'Back to procedures',

            'inicio.hola': 'Hello!', 'inicio.resumen': 'Account summary',
            'inicio.proximoPago': 'NEXT PAYMENT', 'inicio.verFacturas': 'View bills',
            'inicio.misNises': 'My NISEs', 'inicio.ultimosReportes': 'Latest reports',
            'inicio.accesos': 'Quick access',

            'facturas.title': '💡 My Bills',
            'facturas.sub': 'Manage your pending payments and history',
            'facturas.totalPendiente': 'TOTAL PENDING', 'facturas.porPagar': 'to pay',
            'facturas.kpi.pendientes': 'PENDING', 'facturas.kpi.pagadas': 'PAID', 'facturas.kpi.total': 'TOTAL',
            'facturas.pendientes.title': 'Pending payments', 'facturas.historial.title': 'Payment history',
            'facturas.porPagar.badge': 'To pay', 'facturas.vencida.badge': 'Overdue', 'facturas.pagada.badge': 'Paid',
            'facturas.pagar': 'Pay now', 'facturas.vence': 'Due', 'facturas.vencio': 'Overdue since',
            'facturas.pagadaEl': 'Paid on', 'facturas.todoAlDia': 'All caught up!',
            'facturas.todoAlDia.sub': 'You have no pending bills. Thanks for being on time.',

            'reportes.title': '📋 Reports',
            'reportes.sub': 'Report outages and check their status',
            'reportes.reportar': 'Report outage', 'reportes.consultar': 'Check outage',
            'reportes.mapa': 'Outage map', 'reportes.historial': 'Report history',
            'reportes.alumbrado': 'Report street lighting', 'reportes.generar': 'Generate receipt',

            'productos.title': '🛒 Products & Services',
            'productos.sub': 'Explore our store and services',

            'perfil.title': 'My Profile',
            'perfil.resumen': 'Account summary',
            'perfil.kpi.nises': 'ACTIVE NISEs', 'perfil.kpi.facturas': 'PENDING BILLS',
            'perfil.kpi.suscripciones': 'SUBSCRIPTIONS', 'perfil.kpi.averias': 'ACTIVE OUTAGES',
            'perfil.misNises': 'My NISEs', 'perfil.accesos': 'Quick access',
            'perfil.ultimasFacturas': 'Latest bills', 'perfil.averiasSeg': 'Outages being tracked',
            'perfil.editar': 'Edit Profile', 'perfil.cerrar': 'Log out',
            'perfil.calculadora': 'Calculator', 'perfil.suscripciones': 'Subscriptions',
            'perfil.historialCompras': 'Purchase history', 'perfil.chatbot': 'CNFL Chatbot',
            'perfil.reportes': 'Reports', 'perfil.tienda': 'CNFL Store',

            'notif.title': 'Notifications',
            'notif.empty': 'No notifications yet',

            'subs.title': '📦 My Subscriptions',
            'subs.sub': 'Contracted services and active subscriptions',
            'subs.activas': 'ACTIVE', 'subs.mensual': 'MONTHLY AMOUNT',
            'subs.todas': 'All my subscriptions',
            'subs.empty.title': 'No subscriptions registered',
            'subs.empty.sub': 'When you contract a service it will appear here.',

            'pagos.title': '🧾 Payment history',
            'pagos.sub': 'All your payments registered in the app',
            'pagos.totalPagado': 'TOTAL PAID', 'pagos.confirmados': 'CONFIRMED', 'pagos.pendientes': 'PENDING',
            'pagos.todos': 'All payments',
            'pagos.empty.title': 'No payments registered',
            'pagos.empty.sub': 'When you make your first payment it will appear here.',

            'chat.title': 'CNFL Chatbot', 'chat.sub': 'Virtual assistant · Online',
            'chat.placeholder': 'Type your message...',

            'calc.title': '🧮 Consumption calculator',
            'calc.sub': 'Estimate the approximate amount of your bill',

            'mapa.title': 'Outage map', 'mapa.sub': 'Real-time reports on the electrical grid',
            'mapa.enRevision': 'Under review', 'mapa.enCamino': 'Operator on the way', 'mapa.resuelto': 'Resolved'
        },

        BR: {
            'nav.home': 'Início', 'nav.tramites': 'Trâmites', 'nav.bills': 'Faturas',
            'nav.reports': 'Relatórios', 'nav.products': 'Produtos e Serviços', 'nav.profile': 'Perfil',
            'nav.dashboard': 'Painel', 'nav.clients': 'Clientes', 'nav.faults': 'Falhas', 'nav.logout': 'Sair',

            'settings.language': 'Idioma', 'settings.textSize': 'Tamanho do texto',
            'settings.myProfile': 'Meu Perfil', 'settings.myNotifications': 'Minhas Notificações', 'settings.logout': 'Sair',

            'tramites.hero.title': '📋 Trâmites CNFL',
            'tramites.hero.subtitle': 'Faça suas solicitações online de forma rápida e segura.',
            'tramites.kpi.total': 'MEUS TRÂMITES', 'tramites.kpi.pending': 'EM ANDAMENTO',
            'tramites.cat.cambios': 'Mudanças e Modificações', 'tramites.cat.diseno': 'Design e Engenharia',
            'tramites.cat.tarifas': 'Tarifas e Reclamações', 'tramites.cat.nuevos': 'Novos Serviços',
            'tramites.cat.traspasos': 'Transferências e Suprimentos',
            'tramites.recent': '🕒 Meus trâmites recentes', 'tramites.seeAll': 'Ver todos os meus trâmites →',
            'tramites.empty.title': 'Nenhum trâmite registrado',
            'tramites.empty.sub': 'Quando você solicitar um trâmite, ele aparecerá aqui.',

            'detalle.back': 'Voltar aos trâmites', 'detalle.nise': 'NISE ASSOCIADO',
            'detalle.desc': 'DESCRIÇÃO DO TRÂMITE',
            'detalle.desc.placeholder': 'Descreva brevemente o motivo do seu trâmite...',
            'detalle.submit': 'Enviar solicitação', 'detalle.cancel': 'Cancelar', 'detalle.sending': 'Enviando...',
            'detalle.success.title': 'Trâmite enviado com sucesso', 'detalle.success.ref': 'Número de referência:',
            'detalle.success.msg': 'Você pode verificar o status em "Meus Trâmites".', 'detalle.back.to.list': 'Voltar aos trâmites',

            'inicio.hola': 'Olá!', 'inicio.resumen': 'Resumo da sua conta',
            'inicio.proximoPago': 'PRÓXIMO PAGAMENTO', 'inicio.verFacturas': 'Ver faturas',
            'inicio.misNises': 'Meus NISEs', 'inicio.ultimosReportes': 'Últimos relatórios',
            'inicio.accesos': 'Acesso rápido',

            'facturas.title': '💡 Minhas Faturas',
            'facturas.sub': 'Gerencie seus pagamentos pendentes e histórico',
            'facturas.totalPendiente': 'TOTAL PENDENTE', 'facturas.porPagar': 'a pagar',
            'facturas.kpi.pendientes': 'PENDENTES', 'facturas.kpi.pagadas': 'PAGAS', 'facturas.kpi.total': 'TOTAL',
            'facturas.pendientes.title': 'Pagamentos pendentes', 'facturas.historial.title': 'Histórico de pagamentos',
            'facturas.porPagar.badge': 'A pagar', 'facturas.vencida.badge': 'Vencida', 'facturas.pagada.badge': 'Paga',
            'facturas.pagar': 'Pagar agora', 'facturas.vence': 'Vence em', 'facturas.vencio': 'Venceu em',
            'facturas.pagadaEl': 'Paga em', 'facturas.todoAlDia': 'Tudo em dia!',
            'facturas.todoAlDia.sub': 'Você não tem faturas pendentes. Obrigado pela pontualidade.',

            'reportes.title': '📋 Relatórios',
            'reportes.sub': 'Reporte falhas e consulte seu status',
            'reportes.reportar': 'Reportar falha', 'reportes.consultar': 'Consultar falha',
            'reportes.mapa': 'Mapa de falhas', 'reportes.historial': 'Histórico de relatórios',
            'reportes.alumbrado': 'Reportar iluminação pública', 'reportes.generar': 'Gerar comprovante',

            'productos.title': '🛒 Produtos e Serviços',
            'productos.sub': 'Explore nossa loja e serviços',

            'perfil.title': 'Meu Perfil', 'perfil.resumen': 'Resumo da sua conta',
            'perfil.kpi.nises': 'NISEs ATIVOS', 'perfil.kpi.facturas': 'FATURAS PENDENTES',
            'perfil.kpi.suscripciones': 'ASSINATURAS', 'perfil.kpi.averias': 'FALHAS ATIVAS',
            'perfil.misNises': 'Meus NISEs', 'perfil.accesos': 'Acesso rápido',
            'perfil.ultimasFacturas': 'Últimas faturas', 'perfil.averiasSeg': 'Falhas em andamento',
            'perfil.editar': 'Editar Perfil', 'perfil.cerrar': 'Sair',
            'perfil.calculadora': 'Calculadora', 'perfil.suscripciones': 'Assinaturas',
            'perfil.historialCompras': 'Histórico de compras', 'perfil.chatbot': 'Chatbot CNFL',
            'perfil.reportes': 'Relatórios', 'perfil.tienda': 'Loja CNFL',

            'notif.title': 'Notificações', 'notif.empty': 'Sem notificações por enquanto',

            'subs.title': '📦 Minhas Assinaturas', 'subs.sub': 'Serviços contratados e assinaturas ativas',
            'subs.activas': 'ATIVAS', 'subs.mensual': 'VALOR MENSAL', 'subs.todas': 'Todas as minhas assinaturas',
            'subs.empty.title': 'Sem assinaturas registradas', 'subs.empty.sub': 'Quando contratar um serviço aparecerá aqui.',

            'pagos.title': '🧾 Histórico de pagamentos', 'pagos.sub': 'Todos os seus pagamentos registrados no app',
            'pagos.totalPagado': 'TOTAL PAGO', 'pagos.confirmados': 'CONFIRMADOS', 'pagos.pendientes': 'PENDENTES',
            'pagos.todos': 'Todos os pagamentos', 'pagos.empty.title': 'Sem pagamentos registrados',
            'pagos.empty.sub': 'Quando fizer seu primeiro pagamento aparecerá aqui.',

            'chat.title': 'Chatbot CNFL', 'chat.sub': 'Assistente virtual · Online', 'chat.placeholder': 'Digite sua mensagem...',
            'calc.title': '🧮 Calculadora de consumo', 'calc.sub': 'Estime o valor aproximado da sua fatura',

            'mapa.title': 'Mapa de falhas', 'mapa.sub': 'Relatórios em tempo real sobre a rede elétrica',
            'mapa.enRevision': 'Em análise', 'mapa.enCamino': 'Operador a caminho', 'mapa.resuelto': 'Resolvido'
        },

        FR: {
            'nav.home': 'Accueil', 'nav.tramites': 'Démarches', 'nav.bills': 'Factures',
            'nav.reports': 'Rapports', 'nav.products': 'Produits et Services', 'nav.profile': 'Profil',
            'nav.dashboard': 'Tableau', 'nav.clients': 'Clients', 'nav.faults': 'Pannes', 'nav.logout': 'Quitter',

            'settings.language': 'Langue', 'settings.textSize': 'Taille du texte',
            'settings.myProfile': 'Mon Profil', 'settings.myNotifications': 'Mes Notifications', 'settings.logout': 'Déconnexion',

            'tramites.hero.title': '📋 Démarches CNFL',
            'tramites.hero.subtitle': 'Soumettez vos demandes en ligne, rapidement et en toute sécurité.',
            'tramites.kpi.total': 'MES DÉMARCHES', 'tramites.kpi.pending': 'EN COURS',
            'tramites.cat.cambios': 'Changements et Modifications', 'tramites.cat.diseno': 'Conception et Ingénierie',
            'tramites.cat.tarifas': 'Tarifs et Réclamations', 'tramites.cat.nuevos': 'Nouveaux Services',
            'tramites.cat.traspasos': 'Transferts et Fournitures',
            'tramites.recent': '🕒 Mes démarches récentes', 'tramites.seeAll': 'Voir toutes mes démarches →',
            'tramites.empty.title': 'Aucune démarche enregistrée',
            'tramites.empty.sub': 'Lorsque vous soumettrez une démarche, elle apparaîtra ici.',

            'detalle.back': 'Retour aux démarches', 'detalle.nise': 'NISE ASSOCIÉ',
            'detalle.desc': 'DESCRIPTION DE LA DÉMARCHE',
            'detalle.desc.placeholder': 'Décrivez brièvement le motif de votre démarche...',
            'detalle.submit': 'Envoyer la demande', 'detalle.cancel': 'Annuler', 'detalle.sending': 'Envoi...',
            'detalle.success.title': 'Démarche envoyée avec succès', 'detalle.success.ref': 'Numéro de référence:',
            'detalle.success.msg': 'Vous pouvez consulter l\'état dans « Mes Démarches ».', 'detalle.back.to.list': 'Retour aux démarches',

            'inicio.hola': 'Bonjour!', 'inicio.resumen': 'Résumé de votre compte',
            'inicio.proximoPago': 'PROCHAIN PAIEMENT', 'inicio.verFacturas': 'Voir les factures',
            'inicio.misNises': 'Mes NISEs', 'inicio.ultimosReportes': 'Derniers rapports',
            'inicio.accesos': 'Accès rapide',

            'facturas.title': '💡 Mes Factures', 'facturas.sub': 'Gérez vos paiements et votre historique',
            'facturas.totalPendiente': 'TOTAL EN ATTENTE', 'facturas.porPagar': 'à payer',
            'facturas.kpi.pendientes': 'EN ATTENTE', 'facturas.kpi.pagadas': 'PAYÉES', 'facturas.kpi.total': 'TOTAL',
            'facturas.pendientes.title': 'Paiements en attente', 'facturas.historial.title': 'Historique des paiements',
            'facturas.porPagar.badge': 'À payer', 'facturas.vencida.badge': 'En retard', 'facturas.pagada.badge': 'Payée',
            'facturas.pagar': 'Payer maintenant', 'facturas.vence': 'Échéance', 'facturas.vencio': 'Échue le',
            'facturas.pagadaEl': 'Payée le', 'facturas.todoAlDia': 'Tout est à jour!',
            'facturas.todoAlDia.sub': 'Vous n\'avez aucune facture en attente. Merci pour votre ponctualité.',

            'reportes.title': '📋 Rapports', 'reportes.sub': 'Signalez les pannes et consultez leur statut',
            'reportes.reportar': 'Signaler une panne', 'reportes.consultar': 'Consulter une panne',
            'reportes.mapa': 'Carte des pannes', 'reportes.historial': 'Historique des rapports',
            'reportes.alumbrado': 'Signaler l\'éclairage public', 'reportes.generar': 'Générer un reçu',

            'productos.title': '🛒 Produits et Services', 'productos.sub': 'Découvrez notre boutique et nos services',

            'perfil.title': 'Mon Profil', 'perfil.resumen': 'Résumé de votre compte',
            'perfil.kpi.nises': 'NISEs ACTIFS', 'perfil.kpi.facturas': 'FACTURES EN ATTENTE',
            'perfil.kpi.suscripciones': 'ABONNEMENTS', 'perfil.kpi.averias': 'PANNES ACTIVES',
            'perfil.misNises': 'Mes NISEs', 'perfil.accesos': 'Accès rapide',
            'perfil.ultimasFacturas': 'Dernières factures', 'perfil.averiasSeg': 'Pannes en cours',
            'perfil.editar': 'Modifier le profil', 'perfil.cerrar': 'Déconnexion',
            'perfil.calculadora': 'Calculatrice', 'perfil.suscripciones': 'Abonnements',
            'perfil.historialCompras': 'Historique d\'achats', 'perfil.chatbot': 'Chatbot CNFL',
            'perfil.reportes': 'Rapports', 'perfil.tienda': 'Boutique CNFL',

            'notif.title': 'Notifications', 'notif.empty': 'Aucune notification pour le moment',

            'subs.title': '📦 Mes Abonnements', 'subs.sub': 'Services contractés et abonnements actifs',
            'subs.activas': 'ACTIFS', 'subs.mensual': 'MONTANT MENSUEL', 'subs.todas': 'Tous mes abonnements',
            'subs.empty.title': 'Aucun abonnement enregistré', 'subs.empty.sub': 'Lorsque vous souscrirez un service, il apparaîtra ici.',

            'pagos.title': '🧾 Historique des paiements', 'pagos.sub': 'Tous vos paiements enregistrés dans l\'app',
            'pagos.totalPagado': 'TOTAL PAYÉ', 'pagos.confirmados': 'CONFIRMÉS', 'pagos.pendientes': 'EN ATTENTE',
            'pagos.todos': 'Tous les paiements', 'pagos.empty.title': 'Aucun paiement enregistré',
            'pagos.empty.sub': 'Lorsque vous effectuerez votre premier paiement, il apparaîtra ici.',

            'chat.title': 'Chatbot CNFL', 'chat.sub': 'Assistant virtuel · En ligne', 'chat.placeholder': 'Tapez votre message...',
            'calc.title': '🧮 Calculateur de consommation', 'calc.sub': 'Estimez le montant approximatif de votre facture',

            'mapa.title': 'Carte des pannes', 'mapa.sub': 'Signalements en temps réel sur le réseau électrique',
            'mapa.enRevision': 'En révision', 'mapa.enCamino': 'Opérateur en route', 'mapa.resuelto': 'Résolu'
        },

        CN: {
            'nav.home': '首页', 'nav.tramites': '手续', 'nav.bills': '账单',
            'nav.reports': '报告', 'nav.products': '产品与服务', 'nav.profile': '个人',
            'nav.dashboard': '仪表板', 'nav.clients': '客户', 'nav.faults': '故障', 'nav.logout': '退出',

            'settings.language': '语言', 'settings.textSize': '文字大小',
            'settings.myProfile': '我的资料', 'settings.myNotifications': '我的通知', 'settings.logout': '退出登录',

            'tramites.hero.title': '📋 CNFL 手续',
            'tramites.hero.subtitle': '在线快速、安全地提交您的申请。',
            'tramites.kpi.total': '我的手续', 'tramites.kpi.pending': '处理中',
            'tramites.cat.cambios': '更改与修改', 'tramites.cat.diseno': '设计与工程',
            'tramites.cat.tarifas': '费率与索赔', 'tramites.cat.nuevos': '新服务',
            'tramites.cat.traspasos': '转让与供应',
            'tramites.recent': '🕒 我最近的手续', 'tramites.seeAll': '查看所有手续 →',
            'tramites.empty.title': '没有已登记的手续',
            'tramites.empty.sub': '当您提交手续时，它将连同参考编号显示在此处。',

            'detalle.back': '返回手续', 'detalle.nise': '关联的 NISE',
            'detalle.desc': '手续描述',
            'detalle.desc.placeholder': '简要描述您办理手续的原因...',
            'detalle.submit': '提交申请', 'detalle.cancel': '取消', 'detalle.sending': '发送中...',
            'detalle.success.title': '手续已成功提交', 'detalle.success.ref': '参考编号：',
            'detalle.success.msg': '您可以在"我的手续"中查看状态。', 'detalle.back.to.list': '返回手续',

            'inicio.hola': '你好！', 'inicio.resumen': '账户摘要',
            'inicio.proximoPago': '下次付款', 'inicio.verFacturas': '查看账单',
            'inicio.misNises': '我的 NISEs', 'inicio.ultimosReportes': '最新报告',
            'inicio.accesos': '快速访问',

            'facturas.title': '💡 我的账单', 'facturas.sub': '管理您的待付款和历史记录',
            'facturas.totalPendiente': '待付总额', 'facturas.porPagar': '待付',
            'facturas.kpi.pendientes': '待付', 'facturas.kpi.pagadas': '已付', 'facturas.kpi.total': '总计',
            'facturas.pendientes.title': '待付款', 'facturas.historial.title': '付款历史',
            'facturas.porPagar.badge': '待付', 'facturas.vencida.badge': '逾期', 'facturas.pagada.badge': '已付',
            'facturas.pagar': '立即付款', 'facturas.vence': '到期日', 'facturas.vencio': '已逾期',
            'facturas.pagadaEl': '付款日期', 'facturas.todoAlDia': '全部结清！',
            'facturas.todoAlDia.sub': '您没有待付账单。感谢您的准时。',

            'reportes.title': '📋 报告', 'reportes.sub': '报告故障并查询状态',
            'reportes.reportar': '报告故障', 'reportes.consultar': '查询故障',
            'reportes.mapa': '故障地图', 'reportes.historial': '报告历史',
            'reportes.alumbrado': '报告路灯问题', 'reportes.generar': '生成收据',

            'productos.title': '🛒 产品与服务', 'productos.sub': '探索我们的商店和服务',

            'perfil.title': '我的资料', 'perfil.resumen': '账户摘要',
            'perfil.kpi.nises': '有效 NISEs', 'perfil.kpi.facturas': '待付账单',
            'perfil.kpi.suscripciones': '订阅', 'perfil.kpi.averias': '进行中的故障',
            'perfil.misNises': '我的 NISEs', 'perfil.accesos': '快速访问',
            'perfil.ultimasFacturas': '最新账单', 'perfil.averiasSeg': '追踪中的故障',
            'perfil.editar': '编辑资料', 'perfil.cerrar': '退出登录',
            'perfil.calculadora': '计算器', 'perfil.suscripciones': '订阅',
            'perfil.historialCompras': '购买历史', 'perfil.chatbot': 'CNFL 客服',
            'perfil.reportes': '报告', 'perfil.tienda': 'CNFL 商店',

            'notif.title': '通知', 'notif.empty': '暂无通知',

            'subs.title': '📦 我的订阅', 'subs.sub': '已签约的服务和有效订阅',
            'subs.activas': '有效', 'subs.mensual': '每月金额', 'subs.todas': '我的所有订阅',
            'subs.empty.title': '没有已登记的订阅', 'subs.empty.sub': '当您签约服务后，它会显示在此处。',

            'pagos.title': '🧾 付款历史', 'pagos.sub': '您在应用中登记的所有付款',
            'pagos.totalPagado': '已付总额', 'pagos.confirmados': '已确认', 'pagos.pendientes': '待处理',
            'pagos.todos': '所有付款', 'pagos.empty.title': '没有已登记的付款',
            'pagos.empty.sub': '当您进行首次付款后，它会显示在此处。',

            'chat.title': 'CNFL 客服', 'chat.sub': '虚拟助手 · 在线', 'chat.placeholder': '输入您的消息...',
            'calc.title': '🧮 用电计算器', 'calc.sub': '估算您的账单金额',

            'mapa.title': '故障地图', 'mapa.sub': '电网实时报告',
            'mapa.enRevision': '审核中', 'mapa.enCamino': '操作员正在赶来', 'mapa.resuelto': '已解决'
        }
    };

    var CATEGORIAS_MAP = {
        'Cambios y Modificaciones': 'tramites.cat.cambios',
        'Diseño e Ingeniería': 'tramites.cat.diseno',
        'Tarifas y Reclamos': 'tramites.cat.tarifas',
        'Servicios Nuevos': 'tramites.cat.nuevos',
        'Traspasos y Suministros': 'tramites.cat.traspasos'
    };

    // Función pública para traducir desde las vistas
    window.CNFL_T = function (key) {
        var codigo = 'CR';
        try { codigo = localStorage.getItem(LANG_KEY) || 'CR'; } catch (e) { }
        var dict = TRADUCCIONES[codigo] || TRADUCCIONES.CR;
        return dict[key] || key;
    };

    function aplicarIdioma(codigo) {
        var dict = TRADUCCIONES[codigo] || TRADUCCIONES.CR;

        // data-i18n: contenido de texto
        document.querySelectorAll('[data-i18n]').forEach(function (el) {
            var key = el.getAttribute('data-i18n');
            if (dict[key]) {
                if (el.tagName === 'INPUT' && el.hasAttribute('placeholder')) {
                    el.setAttribute('placeholder', dict[key]);
                } else {
                    el.textContent = dict[key];
                }
            }
        });

        // data-i18n-placeholder
        document.querySelectorAll('[data-i18n-placeholder]').forEach(function (el) {
            var key = el.getAttribute('data-i18n-placeholder');
            if (dict[key]) el.setAttribute('placeholder', dict[key]);
        });

        // Traducir categorías del catálogo de trámites
        document.querySelectorAll('.tr-cat').forEach(function (el) {
            var span = el.querySelector('.cnt');
            var cntHtml = span ? span.outerHTML : '';
            var textoActual = el.textContent.replace(/\s*\d+\s*$/, '').trim();

            for (var cat in CATEGORIAS_MAP) {
                if (textoActual === cat) {
                    var key = CATEGORIAS_MAP[cat];
                    el.innerHTML = (dict[key] || cat) + ' ' + cntHtml;
                    break;
                }
            }
        });

        // lang del html
        var langMap = { CR: 'es-CR', US: 'en-US', BR: 'pt-BR', FR: 'fr-FR', CN: 'zh-CN' };
        document.documentElement.setAttribute('lang', langMap[codigo] || 'es-CR');

        // Chip activo
        document.querySelectorAll('.cnfl-lang-chip').forEach(function (chip) {
            chip.classList.toggle('active', chip.getAttribute('data-lang') === codigo);
        });

        // Refrescar label del zoom
        var zoomActual = document.documentElement.getAttribute('data-zoom') || 'normal';
        var lbl = document.getElementById('cnflZoomLbl');
        if (lbl) {
            var langKey = { CR: 'es', US: 'en', BR: 'pt', FR: 'fr', CN: 'cn' }[codigo] || 'es';
            lbl.textContent = ZOOM_LABELS[zoomActual][langKey];
        }

        try { localStorage.setItem(LANG_KEY, codigo); } catch (e) { }
    }

    function renderLangChips() {
        var grid = document.getElementById('cnflLangGrid');
        if (!grid) return;
        grid.innerHTML = '';
        IDIOMAS.forEach(function (idioma) {
            var btn = document.createElement('button');
            btn.type = 'button';
            btn.className = 'cnfl-lang-chip';
            btn.setAttribute('data-lang', idioma.code);
            btn.innerHTML = '<span class="flag">' + idioma.flag + '</span><span class="label">' + idioma.label + '</span>';
            btn.addEventListener('click', function () { aplicarIdioma(idioma.code); });
            grid.appendChild(btn);
        });
    }

    function initIdioma() {
        var guardado = 'CR';
        try { guardado = localStorage.getItem(LANG_KEY) || 'CR'; } catch (e) { }
        renderLangChips();
        aplicarIdioma(guardado);
    }

    // ══════════════════════════════════════════════════════════
    // INIT
    // ══════════════════════════════════════════════════════════
    document.addEventListener('DOMContentLoaded', function () {
        initZoom();
        initIdioma();
    });

})();