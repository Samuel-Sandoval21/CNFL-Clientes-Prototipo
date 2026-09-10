// ============================================================
// APP.JS - FUNCIONES GLOBALES PARA APP MÓVIL + i18n + Ajustes
// ============================================================

// ------------------------------------------------------------
// 1. DICCIONARIO DE TRADUCCIONES
// ------------------------------------------------------------
const CNFL_I18N = {
    es: {
        // Header / Tabbar
        "nav.home": "Inicio",
        "nav.bills": "Facturas",
        "nav.reports": "Reportes",
        "nav.shop": "Tienda",
        "nav.profile": "Perfil",
        "nav.dashboard": "Dashboard",
        "nav.clients": "Clientes",
        "nav.faults": "Averías",
        "nav.logout": "Salir",

        // Menú de ajustes
        "settings.title": "Ajustes",
        "settings.language": "Idioma",
        "settings.account": "Cuenta",
        "settings.myProfile": "Mi Perfil",
        "settings.myNotifications": "Mis Notificaciones",
        "settings.help": "Ayuda",
        "settings.logout": "Cerrar sesión",
        "settings.logoutConfirm": "¿Seguro que quieres cerrar sesión?",

        // Dashboard
        "dashboard.welcome": "Bienvenido,",
        "dashboard.subtitle": "Gestiona tu servicio eléctrico desde tu celular.",
        "dashboard.bills": "Facturas",
        "dashboard.billsDesc": "Consulta y paga tus facturas",
        "dashboard.reports": "Reportes",
        "dashboard.reportsDesc": "Reporta averías y más",
        "dashboard.shop": "Tienda",
        "dashboard.shopDesc": "Productos y servicios",
        "dashboard.profile": "Mi Perfil",
        "dashboard.profileDesc": "Actualiza tus datos",
        "dashboard.currentBill": "Factura Actual",
        "dashboard.due": "Vence:",
        "dashboard.payNow": "Pagar ahora",

        // Mis Facturas
        "bills.title": "Mis Facturas",
        "bills.nise": "NISE",
        "bills.due": "Vence:",
        "bills.paidOn": "Pagada el",
        "bills.paid": "Pagada",
        "bills.pay": "Pagar",
        "bills.pending": "Pendiente",

        // Reportes
        "reports.title": "Reportes",
        "reports.faultTitle": "Reportar avería eléctrica",
        "reports.faultDesc": "Problemas con tu servicio",
        "reports.lightingTitle": "Reportar alumbrado público",
        "reports.lightingDesc": "Fallas en tu comunidad",
        "reports.historyTitle": "Historial de reportes",
        "reports.historyDesc": "Seguimiento de tus reportes",

        // Reportar Avería
        "fault.title": "Reportar Avería",
        "fault.subtitle": "Complete el formulario para reportar una avería.",
        "fault.nise": "NISE afectado",
        "fault.type": "Tipo de avería",
        "fault.description": "Descripción",
        "fault.address": "Dirección",
        "fault.photo": "Adjuntar foto (opcional)",
        "fault.submit": "Enviar Reporte",
        "fault.selectNise": "Seleccione un NISE",
        "fault.selectType": "Seleccione un tipo",

        // Reportar Alumbrado
        "lighting.title": "Reportar Alumbrado Público",
        "lighting.subtitle": "Complete el formulario para reportar problemas de alumbrado en su comunidad.",
        "lighting.type": "Tipo de falla",
        "lighting.description": "Descripción",
        "lighting.address": "Dirección",
        "lighting.photo": "Adjuntar foto (opcional)",
        "lighting.submit": "Enviar Reporte",
        "lighting.selectType": "Seleccione un tipo",

        // Tienda
        "shop.title": "Tienda CNFL",
        "shop.add": "Agregar",
        "shop.products": "Productos",

        // Notificaciones
        "notif.title": "Notificaciones",
        "notif.new": "Nueva",

        // Perfil
        "profile.title": "Mi Perfil",
        "profile.name": "Nombre",
        "profile.id": "Cédula",
        "profile.email": "Correo",
        "profile.phone": "Teléfono",
        "profile.registeredOn": "Fecha Registro",
        "profile.edit": "Editar Perfil",

        // Editar
        "edit.title": "Editar Perfil",
        "edit.name": "Nombre",
        "edit.lastname": "Apellidos",
        "edit.email": "Correo",
        "edit.phone": "Teléfono",
        "edit.save": "Guardar Cambios",
        "edit.cancel": "Cancelar",

        // Pagos
        "pay.title": "Mis Pagos",
        "pay.subtitle": "Seleccione su método de pago",
        "pay.methodCard": "Tarjeta",
        "pay.methodTokens": "Tokens",
        "pay.methodIban": "IBAN",
        "pay.methodSinpe": "SINPE Móvil",
        "pay.history": "Historial de pagos",
        "pay.invoice": "Factura",
        "pay.nise": "NISE",
        "pay.amount": "Monto",
        "pay.date": "Fecha",
        "pay.status": "Estado",
        "pay.paid": "Pagada",
        "pay.pending": "Pendiente",

        // Historial
        "history.title": "Historial de Compras",
        "history.subtitle": "Productos y servicios adquiridos.",
        "history.receipt": "Ver recibo",
        "history.completed": "Completado"
    },
    en: {
        "nav.home": "Home", "nav.bills": "Bills", "nav.reports": "Reports", "nav.shop": "Shop", "nav.profile": "Profile",
        "nav.dashboard": "Dashboard", "nav.clients": "Clients", "nav.faults": "Faults", "nav.logout": "Log out",
        "settings.title": "Settings", "settings.language": "Language", "settings.account": "Account",
        "settings.myProfile": "My Profile", "settings.myNotifications": "My Notifications", "settings.help": "Help",
        "settings.logout": "Sign out", "settings.logoutConfirm": "Are you sure you want to sign out?",
        "dashboard.welcome": "Welcome,", "dashboard.subtitle": "Manage your electric service from your phone.",
        "dashboard.bills": "Bills", "dashboard.billsDesc": "View and pay your bills",
        "dashboard.reports": "Reports", "dashboard.reportsDesc": "Report faults and more",
        "dashboard.shop": "Shop", "dashboard.shopDesc": "Products and services",
        "dashboard.profile": "My Profile", "dashboard.profileDesc": "Update your data",
        "dashboard.currentBill": "Current Bill", "dashboard.due": "Due:", "dashboard.payNow": "Pay now",
        "bills.title": "My Bills", "bills.nise": "NISE", "bills.due": "Due:", "bills.paidOn": "Paid on",
        "bills.paid": "Paid", "bills.pay": "Pay", "bills.pending": "Pending",
        "reports.title": "Reports", "reports.faultTitle": "Report power outage", "reports.faultDesc": "Issues with your service",
        "reports.lightingTitle": "Report street lighting", "reports.lightingDesc": "Issues in your community",
        "reports.historyTitle": "Report history", "reports.historyDesc": "Track your reports",
        "fault.title": "Report Fault", "fault.subtitle": "Fill the form to report a fault.",
        "fault.nise": "Affected NISE", "fault.type": "Fault type", "fault.description": "Description",
        "fault.address": "Address", "fault.photo": "Attach photo (optional)", "fault.submit": "Submit Report",
        "fault.selectNise": "Select a NISE", "fault.selectType": "Select a type",
        "lighting.title": "Report Street Lighting", "lighting.subtitle": "Fill the form to report lighting issues in your community.",
        "lighting.type": "Issue type", "lighting.description": "Description", "lighting.address": "Address",
        "lighting.photo": "Attach photo (optional)", "lighting.submit": "Submit Report", "lighting.selectType": "Select a type",
        "shop.title": "CNFL Store", "shop.add": "Add", "shop.products": "Products",
        "notif.title": "Notifications", "notif.new": "New",
        "profile.title": "My Profile", "profile.name": "Name", "profile.id": "ID", "profile.email": "Email",
        "profile.phone": "Phone", "profile.registeredOn": "Registered", "profile.edit": "Edit Profile",
        "edit.title": "Edit Profile", "edit.name": "Name", "edit.lastname": "Last name", "edit.email": "Email",
        "edit.phone": "Phone", "edit.save": "Save Changes", "edit.cancel": "Cancel",
        "pay.title": "My Payments", "pay.subtitle": "Select your payment method",
        "pay.methodCard": "Card", "pay.methodTokens": "Tokens", "pay.methodIban": "IBAN", "pay.methodSinpe": "SINPE Mobile",
        "pay.history": "Payment history", "pay.invoice": "Invoice", "pay.nise": "NISE", "pay.amount": "Amount",
        "pay.date": "Date", "pay.status": "Status", "pay.paid": "Paid", "pay.pending": "Pending",
        "history.title": "Purchase History", "history.subtitle": "Products and services acquired.",
        "history.receipt": "View receipt", "history.completed": "Completed"
    },
    pt: {
        "nav.home": "Início", "nav.bills": "Faturas", "nav.reports": "Relatos", "nav.shop": "Loja", "nav.profile": "Perfil",
        "nav.dashboard": "Painel", "nav.clients": "Clientes", "nav.faults": "Falhas", "nav.logout": "Sair",
        "settings.title": "Configurações", "settings.language": "Idioma", "settings.account": "Conta",
        "settings.myProfile": "Meu Perfil", "settings.myNotifications": "Minhas Notificações", "settings.help": "Ajuda",
        "settings.logout": "Sair", "settings.logoutConfirm": "Tem certeza que deseja sair?",
        "dashboard.welcome": "Bem-vindo,", "dashboard.subtitle": "Gerencie seu serviço elétrico pelo celular.",
        "dashboard.bills": "Faturas", "dashboard.billsDesc": "Veja e pague suas faturas",
        "dashboard.reports": "Relatos", "dashboard.reportsDesc": "Reporte falhas e mais",
        "dashboard.shop": "Loja", "dashboard.shopDesc": "Produtos e serviços",
        "dashboard.profile": "Meu Perfil", "dashboard.profileDesc": "Atualize seus dados",
        "dashboard.currentBill": "Fatura Atual", "dashboard.due": "Vence:", "dashboard.payNow": "Pagar agora",
        "bills.title": "Minhas Faturas", "bills.nise": "NISE", "bills.due": "Vence:", "bills.paidOn": "Paga em",
        "bills.paid": "Paga", "bills.pay": "Pagar", "bills.pending": "Pendente",
        "reports.title": "Relatos", "reports.faultTitle": "Reportar falha elétrica", "reports.faultDesc": "Problemas com seu serviço",
        "reports.lightingTitle": "Reportar iluminação pública", "reports.lightingDesc": "Falhas na sua comunidade",
        "reports.historyTitle": "Histórico de relatos", "reports.historyDesc": "Acompanhe seus relatos",
        "fault.title": "Reportar Falha", "fault.subtitle": "Preencha o formulário para reportar uma falha.",
        "fault.nise": "NISE afetado", "fault.type": "Tipo de falha", "fault.description": "Descrição",
        "fault.address": "Endereço", "fault.photo": "Anexar foto (opcional)", "fault.submit": "Enviar Relato",
        "fault.selectNise": "Selecione um NISE", "fault.selectType": "Selecione um tipo",
        "lighting.title": "Reportar Iluminação Pública", "lighting.subtitle": "Preencha o formulário para reportar problemas de iluminação.",
        "lighting.type": "Tipo de problema", "lighting.description": "Descrição", "lighting.address": "Endereço",
        "lighting.photo": "Anexar foto (opcional)", "lighting.submit": "Enviar Relato", "lighting.selectType": "Selecione um tipo",
        "shop.title": "Loja CNFL", "shop.add": "Adicionar", "shop.products": "Produtos",
        "notif.title": "Notificações", "notif.new": "Nova",
        "profile.title": "Meu Perfil", "profile.name": "Nome", "profile.id": "Documento", "profile.email": "E-mail",
        "profile.phone": "Telefone", "profile.registeredOn": "Registro", "profile.edit": "Editar Perfil",
        "edit.title": "Editar Perfil", "edit.name": "Nome", "edit.lastname": "Sobrenome", "edit.email": "E-mail",
        "edit.phone": "Telefone", "edit.save": "Salvar Alterações", "edit.cancel": "Cancelar",
        "pay.title": "Meus Pagamentos", "pay.subtitle": "Selecione seu método de pagamento",
        "pay.methodCard": "Cartão", "pay.methodTokens": "Tokens", "pay.methodIban": "IBAN", "pay.methodSinpe": "SINPE Móvel",
        "pay.history": "Histórico de pagamentos", "pay.invoice": "Fatura", "pay.nise": "NISE", "pay.amount": "Valor",
        "pay.date": "Data", "pay.status": "Status", "pay.paid": "Paga", "pay.pending": "Pendente",
        "history.title": "Histórico de Compras", "history.subtitle": "Produtos e serviços adquiridos.",
        "history.receipt": "Ver recibo", "history.completed": "Concluído"
    },
    fr: {
        "nav.home": "Accueil", "nav.bills": "Factures", "nav.reports": "Signalements", "nav.shop": "Boutique", "nav.profile": "Profil",
        "nav.dashboard": "Tableau", "nav.clients": "Clients", "nav.faults": "Pannes", "nav.logout": "Quitter",
        "settings.title": "Paramètres", "settings.language": "Langue", "settings.account": "Compte",
        "settings.myProfile": "Mon Profil", "settings.myNotifications": "Mes Notifications", "settings.help": "Aide",
        "settings.logout": "Se déconnecter", "settings.logoutConfirm": "Voulez-vous vraiment vous déconnecter ?",
        "dashboard.welcome": "Bienvenue,", "dashboard.subtitle": "Gérez votre service électrique depuis votre téléphone.",
        "dashboard.bills": "Factures", "dashboard.billsDesc": "Consultez et payez vos factures",
        "dashboard.reports": "Signalements", "dashboard.reportsDesc": "Signalez des pannes et plus",
        "dashboard.shop": "Boutique", "dashboard.shopDesc": "Produits et services",
        "dashboard.profile": "Mon Profil", "dashboard.profileDesc": "Mettez à jour vos données",
        "dashboard.currentBill": "Facture Actuelle", "dashboard.due": "Échéance :", "dashboard.payNow": "Payer maintenant",
        "bills.title": "Mes Factures", "bills.nise": "NISE", "bills.due": "Échéance :", "bills.paidOn": "Payée le",
        "bills.paid": "Payée", "bills.pay": "Payer", "bills.pending": "En attente",
        "reports.title": "Signalements", "reports.faultTitle": "Signaler une panne électrique", "reports.faultDesc": "Problèmes avec votre service",
        "reports.lightingTitle": "Signaler l'éclairage public", "reports.lightingDesc": "Pannes dans votre quartier",
        "reports.historyTitle": "Historique des signalements", "reports.historyDesc": "Suivi de vos signalements",
        "fault.title": "Signaler une Panne", "fault.subtitle": "Remplissez le formulaire pour signaler une panne.",
        "fault.nise": "NISE affecté", "fault.type": "Type de panne", "fault.description": "Description",
        "fault.address": "Adresse", "fault.photo": "Joindre une photo (facultatif)", "fault.submit": "Envoyer le signalement",
        "fault.selectNise": "Sélectionnez un NISE", "fault.selectType": "Sélectionnez un type",
        "lighting.title": "Signaler l'Éclairage Public", "lighting.subtitle": "Remplissez le formulaire pour signaler des problèmes d'éclairage.",
        "lighting.type": "Type de problème", "lighting.description": "Description", "lighting.address": "Adresse",
        "lighting.photo": "Joindre une photo (facultatif)", "lighting.submit": "Envoyer", "lighting.selectType": "Sélectionnez un type",
        "shop.title": "Boutique CNFL", "shop.add": "Ajouter", "shop.products": "Produits",
        "notif.title": "Notifications", "notif.new": "Nouveau",
        "profile.title": "Mon Profil", "profile.name": "Nom", "profile.id": "Pièce d'identité", "profile.email": "E-mail",
        "profile.phone": "Téléphone", "profile.registeredOn": "Inscription", "profile.edit": "Modifier le Profil",
        "edit.title": "Modifier le Profil", "edit.name": "Nom", "edit.lastname": "Nom de famille", "edit.email": "E-mail",
        "edit.phone": "Téléphone", "edit.save": "Enregistrer", "edit.cancel": "Annuler",
        "pay.title": "Mes Paiements", "pay.subtitle": "Sélectionnez votre mode de paiement",
        "pay.methodCard": "Carte", "pay.methodTokens": "Jetons", "pay.methodIban": "IBAN", "pay.methodSinpe": "SINPE Mobile",
        "pay.history": "Historique des paiements", "pay.invoice": "Facture", "pay.nise": "NISE", "pay.amount": "Montant",
        "pay.date": "Date", "pay.status": "Statut", "pay.paid": "Payée", "pay.pending": "En attente",
        "history.title": "Historique des Achats", "history.subtitle": "Produits et services acquis.",
        "history.receipt": "Voir le reçu", "history.completed": "Terminé"
    },
    zh: {
        "nav.home": "首页", "nav.bills": "账单", "nav.reports": "报告", "nav.shop": "商店", "nav.profile": "我的",
        "nav.dashboard": "仪表板", "nav.clients": "客户", "nav.faults": "故障", "nav.logout": "退出",
        "settings.title": "设置", "settings.language": "语言", "settings.account": "账户",
        "settings.myProfile": "我的资料", "settings.myNotifications": "我的通知", "settings.help": "帮助",
        "settings.logout": "退出登录", "settings.logoutConfirm": "确定要退出登录吗？",
        "dashboard.welcome": "欢迎，", "dashboard.subtitle": "在手机上管理您的电力服务。",
        "dashboard.bills": "账单", "dashboard.billsDesc": "查看并支付账单",
        "dashboard.reports": "报告", "dashboard.reportsDesc": "报告故障等",
        "dashboard.shop": "商店", "dashboard.shopDesc": "产品和服务",
        "dashboard.profile": "我的资料", "dashboard.profileDesc": "更新您的信息",
        "dashboard.currentBill": "当前账单", "dashboard.due": "到期：", "dashboard.payNow": "立即支付",
        "bills.title": "我的账单", "bills.nise": "NISE", "bills.due": "到期：", "bills.paidOn": "付款日期",
        "bills.paid": "已支付", "bills.pay": "支付", "bills.pending": "待处理",
        "reports.title": "报告", "reports.faultTitle": "报告电力故障", "reports.faultDesc": "您的服务问题",
        "reports.lightingTitle": "报告路灯故障", "reports.lightingDesc": "社区故障",
        "reports.historyTitle": "报告历史", "reports.historyDesc": "跟踪您的报告",
        "fault.title": "报告故障", "fault.subtitle": "填写表格报告故障。",
        "fault.nise": "受影响的NISE", "fault.type": "故障类型", "fault.description": "描述",
        "fault.address": "地址", "fault.photo": "附加照片（可选）", "fault.submit": "提交报告",
        "fault.selectNise": "选择NISE", "fault.selectType": "选择类型",
        "lighting.title": "报告路灯故障", "lighting.subtitle": "填写表格报告您社区的路灯问题。",
        "lighting.type": "问题类型", "lighting.description": "描述", "lighting.address": "地址",
        "lighting.photo": "附加照片（可选）", "lighting.submit": "提交", "lighting.selectType": "选择类型",
        "shop.title": "CNFL商店", "shop.add": "添加", "shop.products": "产品",
        "notif.title": "通知", "notif.new": "新",
        "profile.title": "我的资料", "profile.name": "姓名", "profile.id": "身份证", "profile.email": "邮箱",
        "profile.phone": "电话", "profile.registeredOn": "注册日期", "profile.edit": "编辑资料",
        "edit.title": "编辑资料", "edit.name": "名字", "edit.lastname": "姓氏", "edit.email": "邮箱",
        "edit.phone": "电话", "edit.save": "保存更改", "edit.cancel": "取消",
        "pay.title": "我的付款", "pay.subtitle": "选择您的付款方式",
        "pay.methodCard": "银行卡", "pay.methodTokens": "代币", "pay.methodIban": "IBAN", "pay.methodSinpe": "SINPE移动",
        "pay.history": "付款历史", "pay.invoice": "发票", "pay.nise": "NISE", "pay.amount": "金额",
        "pay.date": "日期", "pay.status": "状态", "pay.paid": "已支付", "pay.pending": "待处理",
        "history.title": "购买历史", "history.subtitle": "购买的产品和服务。",
        "history.receipt": "查看收据", "history.completed": "已完成"
    }
};

const CNFL_LANGS = [
    { code: "es", label: "ES", name: "Español", flag: "🇨🇷" },
    { code: "en", label: "EN", name: "English", flag: "🇺🇸" },
    { code: "pt", label: "PT", name: "Português", flag: "🇧🇷" },
    { code: "fr", label: "FR", name: "Français", flag: "🇫🇷" },
    { code: "zh", label: "ZH", name: "中文", flag: "🇨🇳" }
];

// ------------------------------------------------------------
// 2. UTILIDADES i18n
// ------------------------------------------------------------
function cnflGetLang() {
    return localStorage.getItem('cnfl_lang') || 'es';
}

function cnflSetLang(lang) {
    if (!CNFL_I18N[lang]) return;
    localStorage.setItem('cnfl_lang', lang);
    cnflApplyLang(lang);
    document.documentElement.setAttribute('lang', lang);
}

function cnflT(key) {
    const lang = cnflGetLang();
    const dict = CNFL_I18N[lang] || CNFL_I18N.es;
    return dict[key] || (CNFL_I18N.es[key] || key);
}

function cnflApplyLang(lang) {
    const dict = CNFL_I18N[lang] || CNFL_I18N.es;

    // Texto: <span data-i18n="nav.home">Inicio</span>
    document.querySelectorAll('[data-i18n]').forEach(function (el) {
        const key = el.getAttribute('data-i18n');
        if (dict[key]) el.textContent = dict[key];
    });

    // Placeholder: <input data-i18n-placeholder="...">
    document.querySelectorAll('[data-i18n-placeholder]').forEach(function (el) {
        const key = el.getAttribute('data-i18n-placeholder');
        if (dict[key]) el.setAttribute('placeholder', dict[key]);
    });

    // Value para botones submit: <input type="submit" data-i18n-value="...">
    document.querySelectorAll('[data-i18n-value]').forEach(function (el) {
        const key = el.getAttribute('data-i18n-value');
        if (dict[key]) el.value = dict[key];
    });

    // Marcar el idioma activo en el menú
    document.querySelectorAll('.cnfl-lang-chip').forEach(function (chip) {
        chip.classList.toggle('active', chip.dataset.lang === lang);
    });
}

// ------------------------------------------------------------
// 3. MENÚ DE AJUSTES (dropdown desde el engranaje)
// ------------------------------------------------------------
function cnflOpenSettings() {
    const menu = document.getElementById('cnflSettingsMenu');
    const overlay = document.getElementById('cnflSettingsOverlay');
    if (!menu || !overlay) return;
    menu.classList.add('open');
    overlay.classList.add('open');
}

function cnflCloseSettings() {
    const menu = document.getElementById('cnflSettingsMenu');
    const overlay = document.getElementById('cnflSettingsOverlay');
    if (!menu || !overlay) return;
    menu.classList.remove('open');
    overlay.classList.remove('open');
}

function cnflToggleSettings(e) {
    if (e) e.stopPropagation();
    const menu = document.getElementById('cnflSettingsMenu');
    if (!menu) return;
    if (menu.classList.contains('open')) cnflCloseSettings();
    else cnflOpenSettings();
}

function cnflLogout() {
    if (confirm(cnflT('settings.logoutConfirm'))) {
        // El href real lo pone el layout con @Url.Action("CerrarSesion","Cuenta")
        window.location.href = window.CNFL_LOGOUT_URL || '/Cuenta/CerrarSesion';
    }
}

function cnflInitSettings() {
    // Construir los chips de idioma
    const langContainer = document.getElementById('cnflLangGrid');
    if (langContainer && !langContainer.dataset.built) {
        const currentLang = cnflGetLang();
        CNFL_LANGS.forEach(function (l) {
            const chip = document.createElement('button');
            chip.type = 'button';
            chip.className = 'cnfl-lang-chip' + (l.code === currentLang ? ' active' : '');
            chip.dataset.lang = l.code;
            chip.innerHTML = '<span class="flag">' + l.flag + '</span><span class="label">' + l.label + '</span>';
            chip.addEventListener('click', function () {
                cnflSetLang(l.code);
            });
            langContainer.appendChild(chip);
        });
        langContainer.dataset.built = '1';
    }

    // Click en overlay cierra el menú
    const overlay = document.getElementById('cnflSettingsOverlay');
    if (overlay) overlay.addEventListener('click', cnflCloseSettings);

    // Escape cierra el menú
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') cnflCloseSettings();
    });
}

// ------------------------------------------------------------
// 4. INICIALIZACIÓN GLOBAL
// ------------------------------------------------------------
document.addEventListener('DOMContentLoaded', function () {
    console.log('CNFL App cargada');

    // Reloj
    function actualizarReloj() {
        var ahora = new Date();
        var horas = String(ahora.getHours()).padStart(2, '0');
        var minutos = String(ahora.getMinutes()).padStart(2, '0');
        var reloj = document.getElementById('clock');
        if (reloj) reloj.textContent = horas + ':' + minutos;
    }
    actualizarReloj();
    setInterval(actualizarReloj, 10000);

    // Aplicar idioma guardado (o español por defecto)
    cnflSetLang(cnflGetLang());

    // Construir menú de ajustes
    cnflInitSettings();

    // Cerrar alertas
    document.querySelectorAll('.cerrar-alerta').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var alerta = this.closest('.alerta');
            if (alerta) alerta.style.display = 'none';
        });
    });

    // Toast global
    window.mostrarToast = function (mensaje, tipo) {
        tipo = tipo || 'info';
        var toast = document.getElementById('toastGlobal');
        if (!toast) {
            toast = document.createElement('div');
            toast.id = 'toastGlobal';
            toast.style.cssText = 'position:fixed;bottom:90px;left:50%;transform:translateX(-50%);padding:12px 24px;border-radius:12px;font-weight:700;z-index:9999;background:#0E1116;color:white;box-shadow:0 8px 24px rgba(0,0,0,0.2);opacity:0;transition:opacity 0.3s;max-width:90%;text-align:center;';
            document.body.appendChild(toast);
        }
        toast.textContent = mensaje;
        toast.style.opacity = '1';
        var colores = { success: '#2E7D32', error: '#D32F2F', warning: '#F5A623', info: '#0E1116' };
        toast.style.background = colores[tipo] || colores.info;
        clearTimeout(toast._timeout);
        toast._timeout = setTimeout(function () { toast.style.opacity = '0'; }, 3000);
    };

    // Badge de notificaciones
    window.actualizarBadge = function (cantidad) {
        document.querySelectorAll('.badge').forEach(function (badge) {
            if (cantidad > 0) {
                badge.textContent = cantidad;
                badge.style.display = 'grid';
            } else {
                badge.style.display = 'none';
            }
        });
    };
});

// Exponer helpers globales
window.cnflT = cnflT;
window.cnflSetLang = cnflSetLang;
window.cnflGetLang = cnflGetLang;
window.cnflToggleSettings = cnflToggleSettings;
window.cnflCloseSettings = cnflCloseSettings;
window.cnflLogout = cnflLogout;