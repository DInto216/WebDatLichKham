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
    { icon: '🤕', label: 'Đau nửa đầu, hoa mắt chóng mặt', query: 'Tôi bị đau nửa đầu kèm hoa mắt chóng mặt 2 ngày nay, nên khám chuyên khoa nào?' },
    { icon: '🫁', label: 'Ho khan, rát họng, sốt nhẹ', query: 'Tôi bị ho rát họng và sốt nhẹ 38 độ, tư vấn giúp tôi chuyên khoa và bác sĩ phù hợp' },
    { icon: '👶', label: 'Bé sốt cao và biếng ăn', query: 'Bé nhà tôi 3 tuổi bị sốt và biếng ăn quấy khóc, tôi cần đặt lịch khám bác sĩ nào?' },
    { icon: '💔', label: 'Đau tức ngực, hồi hộp', query: 'Tôi hay bị đau tức ngực và hồi hộp khi gắng sức, tôi nên khám gì?' },
    { icon: '🩺', label: 'Tư vấn gói khám tổng quát', query: 'Tôi muốn kiểm tra sức khỏe tổng quát định kỳ, phòng khám có những bác sĩ chuyên môn nào?' }
  ];

  const init = () => {
    // Chỉ tạo nếu chưa tồn tại trên trang
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
          <span>🩺 <strong>MediBot AI</strong>: Cần tư vấn triệu chứng?</span>
        </div>
        <button class="ai-fab-btn" id="aiFabBtn" aria-label="Mở Trợ lý AI Bác sĩ">
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
              <h6>MediBot AI <span class="ai-gemini-pill" id="aiModelBadge">AI Y Tế</span></h6>
              <p>Trợ lý Y tế & Gợi ý Đặt Lịch</p>
            </div>
          </div>
          <div class="ai-header-actions">
            <button class="ai-btn-icon" id="aiBtnReset" title="Bắt đầu hội thoại mới">
              <i class="bi bi-arrow-clockwise"></i>
            </button>
            <button class="ai-btn-icon" id="aiBtnClose" title="Đóng cửa sổ">
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
              <span>Chào bạn, tôi là Bác sĩ Trợ lý MediBot!</span>
            </div>
            <div class="ai-welcome-desc">
              Tôi là trợ lý ảo y tế thông minh, sẵn sàng phân tích các triệu chứng ban đầu và gợi ý chuyên khoa cũng như bác sĩ phù hợp nhất trong hệ thống để bạn đặt lịch khám.
            </div>
            <div style="font-size:11.5px;font-weight:600;color:#64748b;margin-bottom:8px;text-transform:uppercase;letter-spacing:0.04em;">
              Chọn câu hỏi nhanh:
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
            <input type="text" class="ai-chat-input" id="aiChatInput" placeholder="Mô tả triệu chứng hoặc vấn đề sức khỏe..." autocomplete="off">
            <button class="ai-send-btn" id="aiBtnSend" title="Gửi tin nhắn">
              <i class="bi bi-send-fill"></i>
            </button>
          </div>
          <p class="ai-disclaimer">
            <i class="bi bi-shield-check"></i> Thông tin tham khảo y khoa, không thay thế chẩn đoán bác sĩ trực tiếp.
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
    if (dom.tooltip) dom.tooltip.style.display = 'none';
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
    // Giữ lại welcome card, xóa các message khác
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
        history: history.slice(0, -1) // lịch sử trước tin nhắn này
      };

      const response = await API.consultAi(payload);

      hideTypingIndicator();
      setWaitingState(false);

      if (response && response.isSuccess) {
        history.push({ role: 'model', content: response.reply });
        appendBotMessage(response);
      } else {
        appendBotError(response?.errorMessage || 'Rất tiếc, đã có lỗi xảy ra khi kết nối tới trợ lý AI. Bạn vui lòng thử lại nhé.');
      }
    } catch (error) {
      console.error('AI Consult Error:', error);
      hideTypingIndicator();
      setWaitingState(false);
      appendBotError('Không thể kết nối đến máy chủ trợ lý AI. Vui lòng kiểm tra kết nối mạng và thử lại sau.');
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
            <strong>Dấu hiệu cấp cứu:</strong> Tình trạng này có thể cần can thiệp y tế khẩn cấp. Vui lòng liên hệ <a href="tel:115" style="color:#b91c1c;font-weight:700;">Đường dây nóng 115</a> hoặc đến ngay bệnh viện gần nhất!
          </div>
        </div>
      `;
    } else if (res.urgencyLevel === 'warning') {
      alertHtml = `
        <div class="ai-alert-card warning">
          <i class="bi bi-exclamation-circle-fill ai-alert-icon"></i>
          <div>
            <strong>Cần theo dõi sát:</strong> Bạn nên sắp xếp đi khám trong vòng 24 giờ tới để tránh tiến triển xấu.
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
            <i class="bi bi-person-check-fill me-1"></i> Bác sĩ đề xuất tại phòng khám:
          </div>
          ${res.suggestedDoctors.map(doc => {
            const title = [doc.academicTitle, doc.degree, doc.name].filter(Boolean).join(' ');
            const avatar = doc.avatar ? `<img src="${doc.avatar}" class="ai-doc-avatar" alt="${doc.name}">` : `<div class="ai-doc-avatar d-flex align-items-center justify-content-center" style="font-size:18px;">👨‍⚕️</div>`;
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
                  Đặt lịch <i class="bi bi-chevron-right"></i>
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
              💬 ${escapeHtml(q)}
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

    // Bullet points: - item hoặc * item
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

