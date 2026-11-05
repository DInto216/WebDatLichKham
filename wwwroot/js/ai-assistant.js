/* ============================================================
   MEDIBOT AI CHATBOT - CLIENT LOGIC (Google Gemini Powered)
   ============================================================ */

const AiAssistant = (() => {
  'use strict';

  let isOpen = false;
  let isWaiting = false;
  let history = []; // [{ role: 'user' | 'model', content: '...' }]

  // DOM Elements cache
  let dom = {};

  const QUICK_QUESTIONS = [
    { icon: '??', label: 'Ðau n?a d?u, hoa m?t chóng m?t', query: 'Tôi b? dau n?a d?u kèm hoa m?t chóng m?t 2 ngày nay, nên khám chuyên khoa nào?' },
    { icon: '??', label: 'Ho khan, rát h?ng, s?t nh?', query: 'Tôi b? ho rát h?ng và s?t nh? 38 d?, tu v?n giúp tôi chuyên khoa và bác si phù h?p' },
    { icon: '??', label: 'Bé s?t cao và bi?ng an', query: 'Bé nhà tôi 3 tu?i b? s?t và bi?ng an qu?y khóc, tôi c?n d?t l?ch khám bác si nào?' },
    { icon: '??', label: 'Ðau t?c ng?c, h?i h?p', query: 'Tôi hay b? dau t?c ng?c và h?i h?p khi g?ng s?c, tôi nên khám gì?' },
    { icon: '??', label: 'Tu v?n gói khám t?ng quát', query: 'Tôi mu?n ki?m tra s?c kh?e t?ng quát d?nh k?, phòng khám có nh?ng bác si chuyên môn nào?' }
  ];

  const init = () => {
    // Ch? t?o n?u chua t?n t?i trên trang
    if (document.getElementById('aiAssistantRoot')) return;

    createDomStructure();
    bindEvents();
  };

  const createDomStructure = () => {
    const root = document.createElement('div');
    root.id = 'aiAssistantRoot';
    root.innerHTML = `
      <!-- Floating Action Button -->
      <div class="ai-fab-container" id="aiFabContainer">
        <div class="ai-fab-tooltip" id="aiFabTooltip">
          <span>?? <strong>MediBot AI</strong>: C?n tu v?n tri?u ch?ng?</span>
        </div>
        <button class="ai-fab-btn" id="aiFabBtn" aria-label="M? Tr? lý AI Bác si">
          <div class="ai-fab-pulse"></div>
          <div class="ai-fab-icon"><i class="bi bi-robot"></i></div>
          <div class="ai-fab-badge"><i class="bi bi-stars"></i> AI</div>
        </button>
      </div>

      <!-- Chatbot Popup Window -->
      <div class="ai-chat-window" id="aiChatWindow" role="dialog" aria-hidden="true">
        <!-- Header -->
        <div class="ai-chat-header">
          <div class="ai-header-info">
            <div class="ai-avatar-wrapper">
              <i class="bi bi-heart-pulse-fill"></i>
              <span class="ai-online-dot"></span>
            </div>
            <div class="ai-header-text">
              <h6>MediBot AI <span class="ai-gemini-pill" id="aiModelBadge">AI Y T?</span></h6>
              <p>Tr? lý Y t? & G?i ý Ð?t L?ch</p>
            </div>
          </div>
          <div class="ai-header-actions">
            <button class="ai-btn-icon" id="aiBtnReset" title="B?t d?u h?i tho?i m?i">
              <i class="bi bi-arrow-clockwise"></i>
            </button>
            <button class="ai-btn-icon" id="aiBtnClose" title="Ðóng c?a s?">
              <i class="bi bi-x-lg"></i>
            </button>
          </div>
        </div>

        <!-- Chat Body -->
        <div class="ai-chat-body" id="aiChatBody">
          <!-- Welcome message card -->
          <div class="ai-welcome-card" id="aiWelcomeCard">
            <div class="ai-welcome-title">
              <i class="bi bi-stars text-primary"></i>
              <span>Chào b?n, tôi là Bác si Tr? lý MediBot!</span>
            </div>
            <div class="ai-welcome-desc">
              Tôi là tr? lý ?o y t? thông minh, s?n sàng phân tích các tri?u ch?ng ban d?u và g?i ý chuyên khoa cung nhu bác si phù h?p nh?t trong h? th?ng d? b?n d?t l?ch khám.
            </div>
            <div style="font-size:11.5px;font-weight:600;color:#64748b;margin-bottom:8px;text-transform:uppercase;letter-spacing:0.04em;">
              Ch?n câu h?i nhanh:
            </div>
            <div class="ai-suggestion-pills" id="aiQuickPills">
              ${QUICK_QUESTIONS.map(q => `
                <div class="ai-pill" data-query="${q.query}">
                  ${q.icon} ${q.label}
                </div>
              `).join('')}
            </div>
          </div>
        </div>

        <!-- Chat Footer -->
        <div class="ai-chat-footer">
          <div class="ai-input-row">
            <input type="text" class="ai-chat-input" id="aiChatInput" placeholder="Mô t? tri?u ch?ng ho?c v?n d? s?c kh?e..." autocomplete="off">
            <button class="ai-send-btn" id="aiBtnSend" title="G?i tin nh?n">
              <i class="bi bi-send-fill"></i>
            </button>
          </div>
          <p class="ai-disclaimer">
            <i class="bi bi-shield-check"></i> Thông tin tham kh?o y khoa, không thay th? ch?n doán bác si tr?c ti?p.
          </p>
        </div>
      </div>
    `;

    document.body.appendChild(root);

    // Cache DOM refs
    dom.window = document.getElementById('aiChatWindow');
    dom.fabContainer = document.getElementById('aiFabContainer');
    dom.fabBtn = document.getElementById('aiFabBtn');
    dom.tooltip = document.getElementById('aiFabTooltip');
    dom.btnClose = document.getElementById('aiBtnClose');
    dom.btnReset = document.getElementById('aiBtnReset');
    dom.body = document.getElementById('aiChatBody');
    dom.input = document.getElementById('aiChatInput');
    dom.btnSend = document.getElementById('aiBtnSend');
    dom.quickPills = document.getElementById('aiQuickPills');
  };

  const bindEvents = () => {
    dom.fabBtn.addEventListener('click', toggleChat);
    if (dom.tooltip) {
      dom.tooltip.addEventListener('click', openChat);
    }
    dom.btnClose.addEventListener('click', closeChat);
    dom.btnReset.addEventListener('click', resetChat);

    dom.btnSend.addEventListener('click', handleSend);
    dom.input.addEventListener('keydown', (e) => {
      if (e.key === 'Enter' && !e.shiftKey) {
        e.preventDefault();
        handleSend();
      }
    });

    // Delegate quick pill click
    dom.quickPills.addEventListener('click', (e) => {
      const pill = e.target.closest('.ai-pill');
      if (pill) {
        const query = pill.getAttribute('data-query');
        if (query) {
          sendMessage(query);
        }
      }
    });
  };

  const toggleChat = () => {
    if (isOpen) closeChat();
    else openChat();
  };

  const openChat = () => {
    isOpen = true;
    dom.window.classList.add('open');
    dom.window.setAttribute('aria-hidden', 'false');
    if (dom.fabContainer) dom.fabContainer.classList.add('chat-open');
    setTimeout(() => {
      dom.input.focus();
    }, 200);
  };

  const closeChat = () => {
    isOpen = false;
    dom.window.classList.remove('open');
    dom.window.setAttribute('aria-hidden', 'true');
    if (dom.fabContainer) dom.fabContainer.classList.remove('chat-open');
  };

  const resetChat = () => {
    history = [];
    // Gi? l?i welcome card, xóa các message khác
    const welcome = document.getElementById('aiWelcomeCard');
    dom.body.innerHTML = '';
    if (welcome) dom.body.appendChild(welcome);
    dom.input.value = '';
    dom.input.focus();
  };

  const handleSend = () => {
    const text = dom.input.value.trim();
    if (!text || isWaiting) return;
    dom.input.value = '';
    sendMessage(text);
  };

  const sendMessage = async (text) => {
    appendUserMessage(text);
    history.push({ role: 'user', content: text });

    setWaitingState(true);
    showTypingIndicator();

    try {
      const payload = {
        message: text,
        history: history.slice(0, -1) // l?ch s? tru?c tin nh?n này
      };

      const response = await API.consultAi(payload);

      hideTypingIndicator();
      setWaitingState(false);

      if (response && response.isSuccess) {
        history.push({ role: 'model', content: response.reply });
        appendBotMessage(response);
      } else {
        appendBotError(response?.errorMessage || 'R?t ti?c, dã có l?i x?y ra khi k?t n?i t?i tr? lý AI. B?n vui lòng th? l?i nhé.');
      }
    } catch (error) {
      console.error('AI Consult Error:', error);
      hideTypingIndicator();
      setWaitingState(false);
      appendBotError('Không th? k?t n?i d?n máy ch? tr? lý AI. Vui lòng ki?m tra k?t n?i m?ng và th? l?i sau.');
    }
  };

  const setWaitingState = (waiting) => {
    isWaiting = waiting;
    dom.btnSend.disabled = waiting;
    dom.input.disabled = waiting;
    if (!waiting) {
      dom.input.focus();
    }
  };

  const appendUserMessage = (text) => {
    const row = document.createElement('div');
    row.className = 'ai-msg-row user';
    row.innerHTML = `
      <div class="ai-bubble">${escapeHtml(text)}</div>
    `;
    dom.body.appendChild(row);
    scrollToBottom();
  };

  const appendBotMessage = (res) => {
    const row = document.createElement('div');
    row.className = 'ai-msg-row bot';

    const formattedReply = formatMarkdown(res.reply || '');

    // Urgency alert banner
    let alertHtml = '';
    if (res.urgencyLevel === 'emergency') {
      alertHtml = `
        <div class="ai-alert-card emergency">
          <i class="bi bi-exclamation-triangle-fill ai-alert-icon"></i>
          <div>
            <strong>D?u hi?u c?p c?u:</strong> Tình tr?ng này có th? c?n can thi?p y t? kh?n c?p. Vui lòng liên h? <a href="tel:115" style="color:#b91c1c;font-weight:700;">Ðu?ng dây nóng 115</a> ho?c d?n ngay b?nh vi?n g?n nh?t!
          </div>
        </div>
      `;
    } else if (res.urgencyLevel === 'warning') {
      alertHtml = `
        <div class="ai-alert-card warning">
          <i class="bi bi-exclamation-circle-fill ai-alert-icon"></i>
          <div>
            <strong>C?n theo dõi sát:</strong> B?n nên s?p x?p di khám trong vòng 24 gi? t?i d? tránh ti?n tri?n x?u.
          </div>
        </div>
      `;
    }

    // Recommended doctors cards
    let doctorsHtml = '';
    if (res.suggestedDoctors && res.suggestedDoctors.length > 0) {
      doctorsHtml = `
        <div class="ai-doctor-recommendations">
          <div style="font-size:11.5px;font-weight:700;color:#0f766e;text-transform:uppercase;letter-spacing:0.04em;">
            <i class="bi bi-person-check-fill me-1"></i> Bác si d? xu?t t?i phòng khám:
          </div>
          ${res.suggestedDoctors.map(doc => {
            const title = [doc.academicTitle, doc.degree, doc.name].filter(Boolean).join(' ');
            const avatar = doc.avatar ? `<img src="${doc.avatar}" class="ai-doc-avatar" alt="${doc.name}">` : `<div class="ai-doc-avatar d-flex align-items-center justify-content-center" style="font-size:18px;">?????</div>`;
            return `
              <div class="ai-doc-card">
                <div class="ai-doc-info">
                  ${avatar}
                  <div class="ai-doc-text">
                    <div class="ai-doc-name" title="${title}">${title}</div>
                    <div class="ai-doc-specialty"><i class="bi bi-hospital me-1"></i>${doc.specialtyName}</div>
                  </div>
                </div>
                <a href="/Home/Booking?doctor=${doc.id}" class="ai-doc-book-btn">
                  Ð?t l?ch <i class="bi bi-chevron-right"></i>
                </a>
              </div>
            `;
          }).join('')}
        </div>
      `;
    }

    // Follow-up question chips
    let followUpsHtml = '';
    if (res.followUpQuestions && res.followUpQuestions.length > 0) {
      followUpsHtml = `
        <div class="ai-followups-container">
          ${res.followUpQuestions.map(q => `
            <div class="ai-followup-chip" onclick="AiAssistant.askQuestion('${escapeQuotes(q)}')">
              ?? ${escapeHtml(q)}
            </div>
          `).join('')}
        </div>
      `;
    }

    row.innerHTML = `
      <div class="ai-msg-avatar">
        <i class="bi bi-robot"></i>
      </div>
      <div class="ai-bubble">
        ${formattedReply}
        ${alertHtml}
        ${doctorsHtml}
        ${followUpsHtml}
      </div>
    `;

    dom.body.appendChild(row);
    scrollToBottom();
  };

  const appendBotError = (errorMsg) => {
    const row = document.createElement('div');
    row.className = 'ai-msg-row bot';
    row.innerHTML = `
      <div class="ai-msg-avatar" style="background:#ef4444;"><i class="bi bi-exclamation-triangle"></i></div>
      <div class="ai-bubble" style="border-color:#fca5a5;background:#fff5f5;color:#991b1b;">
        ${escapeHtml(errorMsg)}
      </div>
    `;
    dom.body.appendChild(row);
    scrollToBottom();
  };

  const showTypingIndicator = () => {
    hideTypingIndicator();
    const row = document.createElement('div');
    row.className = 'ai-msg-row bot';
    row.id = 'aiTypingRow';
    row.innerHTML = `
      <div class="ai-msg-avatar"><i class="bi bi-robot"></i></div>
      <div class="ai-typing-indicator">
        <div class="ai-dot"></div>
        <div class="ai-dot"></div>
        <div class="ai-dot"></div>
      </div>
    `;
    dom.body.appendChild(row);
    scrollToBottom();
  };

  const hideTypingIndicator = () => {
    const existing = document.getElementById('aiTypingRow');
    if (existing) existing.remove();
  };

  const scrollToBottom = () => {
    setTimeout(() => {
      dom.body.scrollTop = dom.body.scrollHeight;
    }, 50);
  };

  const escapeHtml = (text) => {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
  };

  const escapeQuotes = (str) => {
    return str.replace(/'/g, "\\'").replace(/"/g, '&quot;');
  };

  const formatMarkdown = (text) => {
    if (!text) return '';
    let html = escapeHtml(text);

    // Bold **text**
    html = html.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');

    // Bullet points: - item ho?c * item
    const lines = html.split('\n');
    let inList = false;
    let result = [];

    for (let line of lines) {
      const trimmed = line.trim();
      if (trimmed.startsWith('- ') || trimmed.startsWith('* ')) {
        if (!inList) {
          result.push('<ul>');
          inList = true;
        }
        result.push(`<li>${trimmed.substring(2)}</li>`);
      } else {
        if (inList) {
          result.push('</ul>');
          inList = false;
        }
        if (trimmed) {
          result.push(`<p>${trimmed}</p>`);
        }
      }
    }
    if (inList) result.push('</ul>');

    return result.join('');
  };

  const askQuestion = (query) => {
    sendMessage(query);
  };

  return {
    init,
    openChat,
    closeChat,
    toggleChat,
    sendMessage,
    askQuestion
  };
})();

document.addEventListener('DOMContentLoaded', AiAssistant.init);

