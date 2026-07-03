/* ============================================
   MEDICAL BOOKING - MAIN JS
   Global utilities, navigation, toast, scroll
   ============================================ */

const App = (() => {
  'use strict';

  // ============================================
  // TOAST NOTIFICATION SYSTEM
  // ============================================
  const toast = {
    container: null,

    init() {
      if (document.getElementById('toast-container')) {
        this.container = document.getElementById('toast-container');
        return;
      }
      this.container = document.createElement('div');
      this.container.id = 'toast-container';
      this.container.className = 'toast-container';
      this.container.setAttribute('aria-live', 'polite');
      this.container.setAttribute('aria-atomic', 'true');
      document.body.appendChild(this.container);
    },

    show(title, message, type = 'info', duration = 4000) {
      if (!this.container) this.init();

      const icons = {
        success: 'bi-check-circle-fill',
        error: 'bi-x-circle-fill',
        warning: 'bi-exclamation-triangle-fill',
        info: 'bi-info-circle-fill'
      };

      const bgColors = {
        success: 'var(--success-light)',
        error: 'var(--danger-light)',
        warning: 'var(--warning-light)',
        info: 'var(--info-light)'
      };

      const iconColors = {
        success: 'var(--success)',
        error: 'var(--danger)',
        warning: 'var(--warning)',
        info: 'var(--info)'
      };

      const toastEl = document.createElement('div');
      toastEl.className = `toast-medical ${type} toast-enter`;
      toastEl.innerHTML = `
        <div class="toast-icon" style="background:${bgColors[type]};color:${iconColors[type]}">
          <i class="bi ${icons[type]}"></i>
        </div>
        <div class="toast-body">
          <div class="toast-title">${title}</div>
          <div class="toast-message">${message}</div>
        </div>
        <button class="toast-close" aria-label="Đóng thông báo"><i class="bi bi-x-lg"></i></button>
      `;

      this.container.appendChild(toastEl);

      const closeBtn = toastEl.querySelector('.toast-close');
      const removeToast = () => {
        toastEl.classList.remove('toast-enter');
        toastEl.classList.add('toast-exit');
        setTimeout(() => toastEl.remove(), 300);
      };

      closeBtn.addEventListener('click', removeToast);
      setTimeout(removeToast, duration);
    },

    success(title, message) { this.show(title, message, 'success'); },
    error(title, message) { this.show(title, message, 'error'); },
    warning(title, message) { this.show(title, message, 'warning'); },
    info(title, message) { this.show(title, message, 'info'); }
  };

  // ============================================
  // NAVBAR SCROLL EFFECT
  // ============================================
  const initNavbar = () => {
    const navbar = document.querySelector('.navbar-medical');
    if (!navbar || navbar.dataset.init) return;
    navbar.dataset.init = 'true';

    let lastScrollY = 0;
    const handleScroll = () => {
      const scrollY = window.scrollY;
      if (scrollY > 50) {
        navbar.classList.add('scrolled');
      } else {
        navbar.classList.remove('scrolled');
      }
      lastScrollY = scrollY;
    };

    window.addEventListener('scroll', handleScroll, { passive: true });
    handleScroll();
  };

  // ============================================
  // SCROLL REVEAL ANIMATION
  // ============================================
  const initScrollReveal = () => {
    const reveals = document.querySelectorAll('.reveal:not(.observed)');
    if (reveals.length === 0) return;

    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('active');
          observer.unobserve(entry.target);
        }
      });
    }, {
      threshold: 0.15,
      rootMargin: '0px 0px -50px 0px'
    });

    reveals.forEach(el => {
      el.classList.add('observed');
      observer.observe(el);
    });
  };

  // ============================================
  // SIDEBAR TOGGLE (Admin/Doctor pages)
  // ============================================
  const initSidebar = () => {
    const sidebar = document.querySelector('.sidebar');
    const toggleBtn = document.querySelector('.sidebar-toggle');
    const overlay = document.querySelector('.sidebar-overlay');

    if (!sidebar || !toggleBtn) return;

    const toggleSidebar = () => {
      sidebar.classList.toggle('active');
      if (overlay) overlay.classList.toggle('active');
      document.body.style.overflow = sidebar.classList.contains('active') ? 'hidden' : '';
    };

    toggleBtn.addEventListener('click', toggleSidebar);
    if (overlay) overlay.addEventListener('click', toggleSidebar);

    // Close on ESC
    document.addEventListener('keydown', (e) => {
      if (e.key === 'Escape' && sidebar.classList.contains('active')) {
        toggleSidebar();
      }
    });
  };

  // ============================================
  // FORM VALIDATION
  // ============================================
  const validateForm = (formEl) => {
    const inputs = formEl.querySelectorAll('input, select, textarea');
    let isValid = true;

    inputs.forEach(input => {
      const isRequired = input.hasAttribute('required');
      const value = input.value.trim();
      let feedback = input.nextElementSibling;
      if (feedback && !feedback.classList.contains('invalid-feedback')) {
        feedback = null;
      }

      if (isRequired && !value) {
        input.classList.add('is-invalid');
        isValid = false;
      } else {
        input.classList.remove('is-invalid');
      }

      // Email validation
      if (input.type === 'email' && value) {
        const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        if (!emailRegex.test(value)) {
          input.classList.add('is-invalid');
          if (feedback) feedback.textContent = 'Email không hợp lệ (VD: user@gmail.com)';
          isValid = false;
        }
      }

      // Phone validation
      if (input.type === 'tel' && value) {
        const phoneRegex = /^[0-9]{10,11}$/;
        if (!phoneRegex.test(value)) {
          input.classList.add('is-invalid');
          if (feedback) feedback.textContent = 'Số điện thoại phải từ 10 đến 11 số';
          isValid = false;
        }
      }
    });

    return isValid;
  };

  // ============================================
  // FORMAT UTILITIES
  // ============================================
  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
  };

  const formatDate = (dateStr) => {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    return date.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' });
  };

  const formatDateTime = (dateStr) => {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    return date.toLocaleDateString('vi-VN', {
      day: '2-digit', month: '2-digit', year: 'numeric',
      hour: '2-digit', minute: '2-digit'
    });
  };

  const getDayName = (dateStr) => {
    const days = ['Chủ nhật', 'Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7'];
    return days[new Date(dateStr).getDay()];
  };

  const getStatusLabel = (status) => {
    const map = {
      pending: { text: 'Chờ duyệt', class: 'badge-pending' },
      approved: { text: 'Đã duyệt', class: 'badge-approved' },
      cancelled: { text: 'Đã hủy', class: 'badge-cancelled' },
      completed: { text: 'Hoàn thành', class: 'badge-completed' }
    };
    return map[status] || { text: status, class: '' };
  };

  const renderStatusBadge = (status) => {
    const info = getStatusLabel(status);
    return `<span class="badge-medical ${info.class}"><span class="status-dot"></span>${info.text}</span>`;
  };

  // ============================================
  // DEBOUNCE
  // ============================================
  const debounce = (func, wait = 300) => {
    let timeout;
    return function executedFunction(...args) {
      clearTimeout(timeout);
      timeout = setTimeout(() => func.apply(this, args), wait);
    };
  };

  // ============================================
  // LOADING STATE
  // ============================================
  const showLoading = (container, rows = 3) => {
    let skeletonHTML = '';
    for (let i = 0; i < rows; i++) {
      skeletonHTML += `
        <div class="skeleton-card mb-3">
          <div class="skeleton-row">
            <div class="skeleton skeleton-avatar"></div>
            <div style="flex:1">
              <div class="skeleton skeleton-text w-75"></div>
              <div class="skeleton skeleton-text w-50"></div>
            </div>
            <div class="skeleton skeleton-badge"></div>
          </div>
        </div>
      `;
    }
    container.innerHTML = skeletonHTML;
  };

  const showTableLoading = (tbody, cols = 5, rows = 4) => {
    let html = '';
    for (let i = 0; i < rows; i++) {
      html += '<tr>';
      for (let j = 0; j < cols; j++) {
        html += `<td><div class="skeleton skeleton-text" style="width:${60 + Math.random() * 40}%"></div></td>`;
      }
      html += '</tr>';
    }
    tbody.innerHTML = html;
  };

  // ============================================
  // AUTH GUARD
  // ============================================
  const updateSidebarUser = (user) => {
    if (!user) return;
    const sidebarUser = document.querySelector('.sidebar-user');
    if (!sidebarUser) return;

    const nameEl = sidebarUser.querySelector('.user-name');
    if (nameEl) nameEl.textContent = user.name;

    const avatarEl = sidebarUser.firstElementChild;
    if (avatarEl) {
      if (user.avatar) {
        avatarEl.innerHTML = `<img src="${user.avatar}" style="width:100%;height:100%;object-fit:cover;border-radius:var(--radius-full);" alt="Avatar">`;
        avatarEl.style.background = 'transparent';
      } else {
        if (user.role === 'admin') {
          avatarEl.textContent = user.name.charAt(0).toUpperCase();
          avatarEl.style.background = 'var(--gradient-primary)';
        } else {
          avatarEl.innerHTML = '👨‍⚕️';
          avatarEl.style.background = 'var(--gradient-secondary)';
        }
      }
    }
  };

  const requireAuth = (allowedRoles = []) => {
    const user = API.getCurrentUser();
    if (!user) {
      window.location.href = '/Account/Login';
      return null;
    }
    if (allowedRoles.length > 0 && !allowedRoles.includes(user.role)) {
      window.location.href = '/Account/Login';
      return null;
    }
    updateSidebarUser(user);
    return user;
  };

  const getBasePath = () => {
    const path = window.location.pathname;
    if (path.includes('/pages/admin/') || path.includes('/pages/doctor/')) {
      return '../../';
    } else if (path.includes('/pages/')) {
      return '../';
    }
    return './';
  };

  // ============================================
  // COUNTER ANIMATION
  // ============================================
  const animateCounter = (element, target, duration = 1500) => {
    const originalText = element.textContent.trim();
    let suffix = originalText.replace(/[0-9,.]/g, ''); // Extract '+' or '%'
    if (suffix === '-') suffix = '';
    
    let start = 0;
    const startTime = performance.now();

    const update = (currentTime) => {
      const elapsed = currentTime - startTime;
      const progress = Math.min(elapsed / duration, 1);
      const easeOut = 1 - Math.pow(1 - progress, 3);
      const current = Math.floor(easeOut * target);

      element.textContent = new Intl.NumberFormat('vi-VN').format(current) + suffix;

      if (progress < 1) {
        requestAnimationFrame(update);
      } else {
        element.textContent = new Intl.NumberFormat('vi-VN').format(target) + suffix;
      }
    };

    requestAnimationFrame(update);
  };

  const initCounters = () => {
    const counters = document.querySelectorAll('.stat-number:not(.observed)');
    if (counters.length === 0) return;

    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          const target = parseInt(entry.target.getAttribute('data-count')) || 0;
          animateCounter(entry.target, target);
          observer.unobserve(entry.target);
        }
      });
    }, { threshold: 0.5 });

    counters.forEach(el => {
      el.classList.add('observed');
      observer.observe(el);
    });
  };

  // ============================================
  // MODAL CONFIRM
  // ============================================
  const confirm = (title, message, onConfirm, confirmText = 'Xác nhận') => {
    const modalHTML = `
      <div class="modal fade" id="confirmModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
          <div class="modal-content modal-medical" style="border:none;border-radius:var(--radius-xl)">
            <div class="modal-header" style="border-bottom:1px solid var(--bg-tertiary);padding:var(--space-5) var(--space-6)">
              <h5 class="modal-title" style="font-weight:700">${title}</h5>
              <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Đóng"></button>
            </div>
            <div class="modal-body" style="padding:var(--space-6)">
              <p style="color:var(--text-secondary);margin:0">${message}</p>
            </div>
            <div class="modal-footer" style="border-top:1px solid var(--bg-tertiary);padding:var(--space-4) var(--space-6)">
              <button type="button" class="btn-secondary-medical" data-bs-dismiss="modal">Hủy bỏ</button>
              <button type="button" class="btn-primary-medical" id="confirmBtn">${confirmText}</button>
            </div>
          </div>
        </div>
      </div>
    `;

    // Remove existing
    const existing = document.getElementById('confirmModal');
    if (existing) existing.remove();

    document.body.insertAdjacentHTML('beforeend', modalHTML);
    const modalEl = document.getElementById('confirmModal');
    const modal = new bootstrap.Modal(modalEl);

    document.getElementById('confirmBtn').addEventListener('click', () => {
      modal.hide();
      onConfirm();
    });

    modalEl.addEventListener('hidden.bs.modal', () => {
      modalEl.remove();
    });

    modal.show();
  };

  const handleLogout = () => {
    confirm(
      'Xác nhận đăng xuất',
      'Bạn có chắc chắn muốn đăng xuất khỏi hệ thống không?',
      () => {
        API.logout();
        window.location.href = '/Account/Login';
      },
      'Đăng xuất'
    );
  };

  const initLogoutConfirm = () => {
    const logoutBtn = document.querySelector('.sidebar-user button');
    if (!logoutBtn) return;

    logoutBtn.removeAttribute('onclick');
    logoutBtn.addEventListener('click', (e) => {
      e.preventDefault();
      handleLogout();
    });
  };

  // ============================================
  // INITIALIZE
  // ============================================
  const init = () => {
    toast.init();
    initNavbar();
    initScrollReveal();
    initSidebar();
    initCounters();
    initLogoutConfirm();

    // Remove loading screen
    const loader = document.getElementById('page-loader');
    if (loader) {
      setTimeout(() => {
        loader.style.opacity = '0';
        setTimeout(() => loader.remove(), 300);
      }, 300);
    }

    // Real-time input validation
    document.addEventListener('input', (e) => {
      if (e.target.classList.contains('is-invalid')) {
        if (e.target.value.trim()) {
          e.target.classList.remove('is-invalid');
        }
      }
    });
  };

  // Auto-init when DOM ready
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else {
    init();
  }

  // ---- Public API ----
  return {
    toast,
    validateForm,
    formatCurrency,
    formatDate,
    formatDateTime,
    getDayName,
    getStatusLabel,
    renderStatusBadge,
    debounce,
    showLoading,
    showTableLoading,
    requireAuth,
    updateSidebarUser,
    handleLogout,
    getBasePath,
    animateCounter,
    confirm,
    init
  };
})();

