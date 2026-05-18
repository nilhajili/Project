import axios from "axios";

const api = axios.create({ baseURL: "http://52.7.95.217:5062/api" });

api.interceptors.request.use(config => {
  const token = localStorage.getItem("token");
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

api.interceptors.response.use(
  response => response,
  async error => {
    if (error.response?.status === 401) {
      const refreshToken = localStorage.getItem("refreshToken");
      if (!refreshToken) {
        window.location.href = "/login";
        return Promise.reject(error);
      }
      try {
        const res = await api.post("/auth/refresh", { token: refreshToken });
        localStorage.setItem("token", res.data.accessToken);
        error.config.headers.Authorization = `Bearer ${res.data.accessToken}`;
        return api.request(error.config);
      } catch {
        window.location.href = "/login";
        return Promise.reject(error);
      }
    }
    return Promise.reject(error);
  }
);

export default api;