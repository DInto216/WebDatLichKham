/* ============================================
   MEDICAL BOOKING - API MODULE (Real Database Backend)
   Communicates with ASP.NET Core MVC controllers
   ============================================ */

const API = (() => {
  'use strict';

  // ---- Helper for Fetch API ----
  const request = async (url, options = {}) => {
    try {
      const response = await fetch(url, {
        headers: {
          'Content-Type': 'application/json',
          ...options.headers
        },
        ...options
      });
      if (response.status === 401 || response.status === 403) {
        localStorage.removeItem('currentUser');
        window.location.href = '/Account/Login';
        throw new Error('Unauthorized');
      }
      if (!response.ok) {
        const errData = await response.json().catch(() => ({}));
        throw new Error(errData.message || `API Error: ${response.statusText}`);
      }
      const text = await response.text();
      return text ? JSON.parse(text) : {};
    } catch (error) {
      console.error(`Request to ${url} failed:`, error);
      throw error;
    }
  };

  // ---- Specialties ----
  const getSpecialties = async () => {
    return await request('/api/specialties');
  };

  const addSpecialty = async (data) => {
    return await request('/api/specialties', {
      method: 'POST',
      body: JSON.stringify(data)
    });
  };

  const updateSpecialty = async (id, data) => {
    return await request(`/api/specialties/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  };

  const deleteSpecialty = async (id) => {
    return await request(`/api/specialties/${id}`, {
      method: 'DELETE'
    });
  };

  // ---- Doctors ----
  const getDoctors = async (filters = {}) => {
    const params = new URLSearchParams();
    if (filters.specialtyId) params.append('specialtyId', filters.specialtyId);
    if (filters.search) params.append('search', filters.search);
    return await request(`/api/doctors?${params.toString()}`);
  };

  const getDoctorById = async (id) => {
    return await request(`/api/doctors/${id}`);
  };

  const updateDoctor = async (id, data) => {
    return await request(`/api/doctors/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  };

  const addDoctor = async (data) => {
    return await request('/api/doctors', {
      method: 'POST',
      body: JSON.stringify(data)
    });
  };

  const deleteDoctor = async (id) => {
    return await request(`/api/doctors/${id}`, {
      method: 'DELETE'
    });
  };

  // ---- Get available time slots ----
  const getAvailableSlots = async (doctorId, date) => {
    return await request(`/api/doctors/${doctorId}/slots?date=${date}`);
  };

  // ---- Appointments ----
  const getAppointments = async (filters = {}) => {
    const params = new URLSearchParams();
    if (filters.status) params.append('status', filters.status);
    if (filters.doctorId) params.append('doctorId', filters.doctorId);
    if (filters.date) params.append('date', filters.date);
    return await request(`/api/appointments?${params.toString()}`);
  };

  const createAppointment = async (data) => {
    return await request('/api/appointments', {
      method: 'POST',
      body: JSON.stringify(data)
    });
  };

  const updateAppointmentStatus = async (id, status) => {
    return await request(`/api/appointments/${id}/status`, {
      method: 'PUT',
      body: JSON.stringify({ status })
    });
  };

  // ---- Lookup ----
  const lookupAppointment = async (phone, bookingId) => {
    const params = new URLSearchParams({ phone, bookingId });
    return await request(`/api/appointments/lookup?${params.toString()}`);
  };

  // ---- Accounts ----
  const getAccounts = async () => {
    return await request('/api/accounts');
  };

  const createAccount = async (data) => {
    return await request('/api/accounts', {
      method: 'POST',
      body: JSON.stringify(data)
    });
  };

  const deleteAccount = async (id) => {
    return await request(`/api/accounts/${id}`, {
      method: 'DELETE'
    });
  };

  const updateProfile = async (accountId, data) => {
    const updated = await request(`/api/accounts/${accountId}/profile`, {
      method: 'PUT',
      body: JSON.stringify(data)
    });
    const current = getCurrentUser();
    if (current && current.id === accountId) {
      const merged = { ...current, name: updated.name, avatar: updated.avatar };
      localStorage.setItem('currentUser', JSON.stringify(merged));
    }
    return updated;
  };

  // ---- Auth ----
  const login = async (username, password) => {
    const session = await request('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({ username, password })
    });
    localStorage.setItem('currentUser', JSON.stringify(session));
    return session;
  };

  const logout = async () => {
    // Fix #5: Gọi API trước, xóa localStorage sau
    try {
      await fetch('/api/auth/logout', { method: 'POST' });
    } catch (e) {}
    localStorage.removeItem('currentUser');
  };

  const getCurrentUser = () => {
    try {
      return JSON.parse(localStorage.getItem('currentUser')) || null;
    } catch {
      return null;
    }
  };

  // ---- Medicines ----
  const getMedicines = async () => {
    return await request('/api/medicines');
  };

  const addMedicine = async (data) => {
    return await request('/api/medicines', {
      method: 'POST',
      body: JSON.stringify(data)
    });
  };

  const updateMedicine = async (id, data) => {
    return await request(`/api/medicines/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  };

  const deleteMedicine = async (id) => {
    return await request(`/api/medicines/${id}`, {
      method: 'DELETE'
    });
  };

  // ---- Medical History ----
  const searchMedicalHistory = async (phone) => {
    const params = new URLSearchParams();
    if (phone) params.append('phone', phone);
    return await request(`/api/medical-history?${params.toString()}`);
  };

  const addMedicalHistory = async (data) => {
    return await request('/api/medical-history', {
      method: 'POST',
      body: JSON.stringify(data)
    });
  };

  const getExamResult = async (bookingId) => {
    return await request(`/api/medical-history/result?bookingId=${bookingId}`);
  };

  // ---- Dashboard Stats ----
  const getDashboardStats = async () => {
    return await request('/api/dashboard/stats');
  };

  // ---- Utilities ----
  const maskName = (name) => {
    if (!name) return '';
    const parts = name.trim().split(' ');
    if (parts.length <= 1) return name.charAt(0) + '***';
    return parts[0] + ' ' + parts.slice(1, -1).map(() => '*').join(' ') + (parts.length > 2 ? ' ' : '') + parts[parts.length - 1].charAt(0) + '***';
  };

  const maskPhone = (phone) => {
    if (!phone || phone.length < 6) return phone;
    return phone.substring(0, 3) + '****' + phone.substring(phone.length - 2);
  };

  const generateBookingId = () => {
    const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
    let result = 'BK-';
    for (let i = 0; i < 8; i++) {
      result += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    return result;
  };

  // ---- Public API ----
  return {
    getSpecialties,
    addSpecialty,
    updateSpecialty,
    deleteSpecialty,
    getDoctors,
    getDoctorById,
    updateDoctor,
    addDoctor,
    deleteDoctor,
    getAvailableSlots,
    getAppointments,
    createAppointment,
    updateAppointmentStatus,
    lookupAppointment,
    getAccounts,
    createAccount,
    deleteAccount,
    login,
    logout,
    getCurrentUser,
    updateProfile,
    getMedicines,
    addMedicine,
    updateMedicine,
    deleteMedicine,
    searchMedicalHistory,
    addMedicalHistory,
    getExamResult,
    getDashboardStats,
    maskName,
    maskPhone,
    generateBookingId
  };
})();
